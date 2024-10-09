using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TTS : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public enum PollyVoices { Aditi, Amy, Astrid, Bianca, Brian, Camila, Carla, Carmen, Celine, Chantal, Conchita, Cristiano, Dora, Emma, Enrique, Ewa, Filiz, Gabrielle, Geraint, Giorgio, Gwyneth, Hans, Ines, Ivy, Jacek, Jan, Joanna, Joey, Justin, Karl, Kendra, Kevin, Kimberly, Lea, Liv, Lotte, Lucia, Lupe, Mads, Maja, Marlene, Mathieu, Matthew, Maxim, Mia, Miguel, Mizuki, Naja, Nicole, Olivia, Penelope, Raveena, Ricardo, Ruben, Russell, Salli, Seoyeon, Takumi, Tatyana, Vicki, Vitoria, Zeina, Zhiyu, Aria, Ayanda, Arlet, Hannah, Arthur, Daniel, Liam, Pedro, Kajal, Hiujin, Laura, Elin, Ida, Suvi, Ola, Hala, Andres, Sergio, Remi, Adriano, Thiago, Ruth, Stephen, Kazuha, Tomoko, Niamh, Sofie, Lisa, Isabelle, Zayd, Danielle, Gregory, Burcu };
    public enum PollyLanguageCodes { None, arb, cmn_CN, cy_GB, da_DK, de_DE, en_AU, en_GB, en_GB_WLS, en_IN, en_US, es_ES, es_MX, es_US, fr_CA, fr_FR, is_IS, it_IT, ja_JP, hi_IN, ko_KR, nb_NO, nl_NL, pl_PL, pt_BR, pt_PT, ro_RO, ru_RU, sv_SE, tr_TR, en_NZ, en_ZA, ca_ES, de_AT, yue_CN, ar_AE, fi_FI, en_IE, nl_BE, fr_BE }

    [SerializeField] private PollyVoices voice;
    [SerializeField] private PollyLanguageCodes languagecode;

    private async void Start()
    {
        string filePath = $"{Application.dataPath}/dialogue.txt";
        string textToSynthesize = File.ReadAllText(filePath);

        if (string.IsNullOrEmpty(textToSynthesize))
        {
            Debug.LogError("Either the dialogue is empty or not found!");
            return;
        }

        var credentials = new BasicAWSCredentials("AKIA2NK3X4NVQCGYVBEE", "yF5jkrnJ2uEMcI/PkbsON4EYaPshsXo2NYPbbXSs");
        var client = new AmazonPollyClient(credentials, RegionEndpoint.USEast1);

        var request = new SynthesizeSpeechRequest()
        {
            Text = textToSynthesize,
            Engine = Engine.Neural,
            VoiceId = voice.ToString(),
            LanguageCode = languagecode == PollyLanguageCodes.None ? string.Empty : languagecode.ToString().Replace('_', '-'),
            OutputFormat = OutputFormat.Mp3  // Switch to WAV if MP3 fails
        };

        SynthesizeSpeechResponse response;
        try
        {
            response = await client.SynthesizeSpeechAsync(request);
        }
        catch (Exception e)
        {
            Debug.LogError("Polly failed: " + e.Message);
            return;
        }

        if (response == null || response.AudioStream == null)
        {
            Debug.LogError("Failed to get valid Polly response.");
            return;
        }

        WriteIntoFile(response.AudioStream);

        // Await the download process correctly
        var audioFilePath = $"{Application.persistentDataPath}/audio.mp3";
        using (var www = UnityWebRequestMultimedia.GetAudioClip(audioFilePath, AudioType.MPEG))
        {
            var webrequest = www.SendWebRequest();

            // Wait for request completion
            while (!webrequest.isDone)
            {
                await Task.Yield();
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Audio download failed: " + www.error);
            }
            else
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                if (audioClip == null)
                {
                    Debug.LogError("Failed to load AudioClip.");
                    return;
                }

                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }
    }

    private void WriteIntoFile(Stream stream)
    {
        var filePath = $"{Application.persistentDataPath}/audio.mp3";
        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            byte[] buffer = new byte[8 * 1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                fileStream.Write(buffer, 0, bytesRead);
            }
        }
    }
}