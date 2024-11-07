//using Amazon;
//using Amazon.Polly;
//using Amazon.Polly.Model;
//using Amazon.Runtime;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class TTS : MonoBehaviour
{
    public AudioSource girlAudioSource;
    public AudioSource boyAudioSource;
    [SerializeField] private TextAsset textFile;

	public enum PollyVoices { Amy, Brian, Camila, Emma, Gabrielle, Hannah, Isabella, Kendra, Kimberly, Lupe, Mia, Niamh, Olivia, Ruth, Stephen, Suvi, Takumi, Zayd, Arlet, Adriano, Laura, Seoyeon, Gregory, Hala, Joaquín, Inês, Thiago, Vicki, Daniel, Aria, Ayanda, Jitka, Kazuha, Lisa, Rémi, Andrés, Sergio, Burcu };
	public enum PollyLanguageCodes { None, arb, cmn_CN, cy_GB, da_DK, de_DE, en_AU, en_GB, en_GB_WLS, en_IN, en_US, es_ES, es_MX, es_US, fr_CA, fr_FR, is_IS, it_IT, ja_JP, hi_IN, ko_KR, nb_NO, nl_NL, pl_PL, pt_BR, pt_PT, ro_RO, ru_RU, sv_SE, tr_TR, en_NZ, en_ZA, ca_ES, de_AT, yue_CN, ar_AE, fi_FI, en_IE, nl_BE, fr_BE };

	//[SerializeField] private PollyVoices voice;
	//[SerializeField] private PollyLanguageCodes languagecode;

	//private FileSystemWatcher fileWatcher;

	/*private void Start() { 
		fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(Application.dataPath))
		{
			NotifyFilter = NotifyFilters.LastWrite,
			Filter = Path.GetFileName("sync.txt") 
		};

		fileWatcher.Changed += OnSpeakerFileChanged;
		fileWatcher.EnableRaisingEvents = true;
	}

	private void OnSpeakerFileChanged(object sender, FileSystemEventArgs e)
	{
		if (e.Name == "speaker1.txt" || e.Name == "speaker2.txt")
		{
			PlayTTS();
		}
	}*/

	// Functionality to simulate manual sync between command prompt + Unity
	public void Update()
	{
		string filePath = Path.Combine(Application.dataPath, "sync.txt");
		// Debug.Log(filePath);
		if (File.Exists(filePath))
		{
			string content = File.ReadAllText(filePath);
			// Debug.Log(content);

			if (content.Contains("a"))
			{
				if (textFile != null && textFile.text != "")
				{
					if (textFile.text.Contains("1"))
					{
                        AudioClip girlAudioClip = Resources.Load<AudioClip>("girlAudio.wav");
                        if (girlAudioSource != null && girlAudioClip != null)
                        {
                            girlAudioSource.clip = girlAudioClip;
                            girlAudioSource.Play();
							StartCoroutine(CheckAudioPlayback(girlAudioSource)); // concurrency for girl avatar
                        }

                    }

                    if (textFile.text.Contains("2"))
                    {
                        AudioClip boyAudioClip = Resources.Load<AudioClip>("boyAudio.wav");
                        if (boyAudioSource != null && boyAudioClip != null)
                        {
                            boyAudioSource.clip = boyAudioClip;
                            boyAudioSource.Play();
                            StartCoroutine(CheckAudioPlayback(boyAudioSource)); // concurrency for boy avatar
                        }

                    }
				}
			}
		}
	}

    public IEnumerator CheckAudioPlayback(AudioSource avatarAudioSource)
    {
        while (avatarAudioSource.isPlaying)
        {
            yield return null;
        }

        File.WriteAllText($"Assets/sync.txt", "b");
    }

    /*public void PlayTTS()
	{
		StartCoroutine(StartTTS());
	}

	private IEnumerator StartTTS()
	{
		Debug.Log(gameObject.name + " is going to speak.");
		string textToSynthesize = textFile != null ? textFile.text : string.Empty;
		File.WriteAllText("Assets/speaker1.txt", "");
		File.WriteAllText("Assets/speaker2.txt", "");

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

		var audioFilePath = $"{Application.dataPath}/audio.mp3";
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
				Debug.Log(gameObject.name);
				
				playingAudio = true;
			}
		}
		while(playingAudio && audioSource.isPlaying)
		{
			yield return null;
		}
		// Write the updated content back to the file
		string filePath = Path.Combine(Application.dataPath, "sync.txt");
		File.WriteAllText(filePath, "b");
		Debug.Log("sync.txt updated: 'a' replaced with 'b'.");
		playingAudio = false;
	}

	private async Task<SynthesizeSpeechResponse> SynthesizeSpeechAsync(IAmazonPolly client, SynthesizeSpeechRequest request)
	{
		return await client.SynthesizeSpeechAsync(request);
	}

	private void WriteIntoFile(Stream stream)
	{
		var filePath = $"{Application.dataPath}/audio.mp3";
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
	}*/
}