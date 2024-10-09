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
    [SerializeField] private Dropdown voiceDropdown;

    private async void Start()
    {
        string filePath = $"{Application.dataPath}/dialogue.txt";
        string textToSynthesize = File.ReadAllText(filePath);
        //List<string> voiceOptions = Enum.GetNames(typeof(VoiceId)).ToList();
        //PopulateVoiceDropdown(voiceOptions);
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
            VoiceId = VoiceId.Aria,
            OutputFormat = OutputFormat.Mp3
        };

        var response = await client.SynthesizeSpeechAsync(request);
        WriteIntoFile(response.AudioStream);

        using (var uwrq = UnityWebRequestMultimedia.GetAudioClip($"{Application.persistentDataPath}/audio.mp3", AudioType.MPEG))
        {
            var sentWebRqHandler = uwrq.SendWebRequest();

            while (!sentWebRqHandler.isDone) await Task.Yield();
            {
                var audioClip = DownloadHandlerAudioClip.GetContent(uwrq);

                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }
    }

    private void PopulateVoiceDropdown(List<string> voiceOptions)
    {
        voiceDropdown.ClearOptions();
        voiceDropdown.AddOptions(voiceOptions);
    }

    private void WriteIntoFile(Stream stream)
    {
        using (var fileStream = new FileStream($"{Application.persistentDataPath}/audio.mp3", FileMode.Create))
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