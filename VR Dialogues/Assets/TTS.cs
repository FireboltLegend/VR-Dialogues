using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class TTS : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextAsset textFile;

    public enum PollyVoices { Amy, Brian, Camila, Emma, Gabrielle, Hannah, Isabella, Kendra, Kimberly, Lupe, Mia, Niamh, Olivia, Ruth, Stephen, Suvi, Takumi, Zayd, Arlet, Adriano, Laura, Seoyeon, Gregory, Hala, Joaquín, Inês, Thiago, Vicki, Daniel, Aria, Ayanda, Jitka, Kazuha, Lisa, Rémi, Andrés, Sergio, Burcu };
    public enum PollyLanguageCodes { None, arb, cmn_CN, cy_GB, da_DK, de_DE, en_AU, en_GB, en_GB_WLS, en_IN, en_US, es_ES, es_MX, es_US, fr_CA, fr_FR, is_IS, it_IT, ja_JP, hi_IN, ko_KR, nb_NO, nl_NL, pl_PL, pt_BR, pt_PT, ro_RO, ru_RU, sv_SE, tr_TR, en_NZ, en_ZA, ca_ES, de_AT, yue_CN, ar_AE, fi_FI, en_IE, nl_BE, fr_BE };

    [SerializeField] private PollyVoices voice;
    [SerializeField] private PollyLanguageCodes languagecode;

    private void Update()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "sync.txt");

        if (File.Exists(filePath))
        {
            string content = File.ReadAllText(filePath);

            if (content.Contains("a"))
            {
                Debug.Log("Condition met: 'a' detected in sync.txt. Starting TTS playback.");
                File.WriteAllText(filePath, "");
                PlayTTS();



            }
        }
    }

    public void PlayTTS()
    {
        StartCoroutine(StartTTS());
    }

    private IEnumerator StartTTS()
    {
        string textToSynthesize = textFile != null ? textFile.text : string.Empty;

        if (string.IsNullOrEmpty(textToSynthesize))
        {
            Debug.LogError("Either the dialogue is empty or not found!");
            yield break;
        }
        
        Debug.Log("Loaded");
        var credentials = new BasicAWSCredentials("AKIA2NK3X4NVQCGYVBEE", "yF5jkrnJ2uEMcI/PkbsON4EYaPshsXo2NYPbbXSs");
        var client = new AmazonPollyClient(credentials, RegionEndpoint.USEast1);

        var request = new SynthesizeSpeechRequest()
        {
            Text = textToSynthesize,
            Engine = Engine.Neural,
            VoiceId = voice.ToString(),
            LanguageCode = languagecode == PollyLanguageCodes.None ? string.Empty : languagecode.ToString().Replace('_', '-'),
            OutputFormat = OutputFormat.Mp3
        };

        SynthesizeSpeechResponse response = null;
        Exception exception = null;

        Debug.Log("Sending request to Polly...");
        var task = SynthesizeSpeechAsync(client, request);
        while (!task.IsCompleted)
        {
            yield return null;
        }

        Debug.Log("Polly request completed.");

        if (task.IsFaulted)
        {
            exception = task.Exception;
        }
        else
        {
            response = task.Result;
        }

        if (exception != null)
        {
            Debug.LogError("Polly failed: " + exception.Message);
            yield break;
        }

        if (response == null || response.AudioStream == null)
        {
            Debug.LogError("Failed to get valid Polly response.");
            yield break;
        }

        WriteIntoFile(response.AudioStream);

        var audioFilePath = $"{Application.persistentDataPath}/audio.mp3";
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + audioFilePath, AudioType.MPEG))
        {
            var webrequest = www.SendWebRequest();

            while (!webrequest.isDone)
            {
                yield return null;
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
                    yield break;
                }

                Debug.Log("Audio Clip Loaded: " + (audioClip != null));
                audioSource.clip = audioClip;
                audioSource.Play();
            }

        }
        string filePath = Path.Combine(Application.persistentDataPath, "sync.txt");
        string content = File.ReadAllText(filePath);
        // Replace 'a' with 'b' in the content
        string updatedContent = content.Replace('a', 'b');

        // Write the updated content back to the file
        File.WriteAllText(filePath, updatedContent);
        Debug.Log("sync.txt updated: 'a' replaced with 'b'.");
    }

    private async Task<SynthesizeSpeechResponse> SynthesizeSpeechAsync(IAmazonPolly client, SynthesizeSpeechRequest request)
    {
        return await client.SynthesizeSpeechAsync(request);
    }

    private void WriteIntoFile(Stream stream)
    {
        var filePath = $"{Application.persistentDataPath}/audio.mp3";
        Debug.Log("Audio File Path: " + filePath);
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