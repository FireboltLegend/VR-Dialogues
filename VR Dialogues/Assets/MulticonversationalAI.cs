using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Meta.WitAi.Data;
using OpenAI;
using OpenAI.Audio;
using OpenAI.Chat;
using NAudio.Wave;

public class MultiConversationAI
{
    private List<ChatMessage> conversation1;
    private List<ChatMessage> conversation2;
    private string apiKey;

    public MultiConversationAI()
    {
        conversation1 = new List<ChatMessage>();
        conversation2 = new List<ChatMessage>();
        apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "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";
        
        LoadPrompts();
        STT();
    }

    // Speech-To-Text Daemon (overload #1) -- given you want to use MicrophoneFactory or other custom Microphone Invocation
    private string STT(string audioFilePath="Assets/user_input.wav")
    {
        OpenAIClient client = new OpenAIClient(apiKey); 
        AudioClient audioclient = client.GetAudioClient("whisper-1");
        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.Verbose,
            TimestampGranularities = AudioTimestampGranularities.Word | AudioTimestampGranularities.Segment,
        };


        AudioTranscription transcription = audioclient.TranscribeAudio(audioFilePath, options);
        return transcription.Text;
    }


    // Speech-To-Text Daemon (overload #2) -- utilize in-house Microphone and Automated Speech Recognition
    private string STT()
    {
        OpenAIClient client = new OpenAIClient(apiKey); 
        AudioClient audioclient = client.GetAudioClient("whisper-1");
        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.Verbose,
            TimestampGranularities = AudioTimestampGranularities.Word | AudioTimestampGranularities.Segment,
        };

        WaveInEvent waveInput = new WaveInEvent();  // initiate the Micrphone device that is set at number 0

        waveInput.WaveFormat = new WaveFormat(160000, 1);

        using (var waveFileWriter = new WaveFileWriter("Assets/user_input.wav", new WaveFormat(16000, 1)))
        { 
            waveInput.DataAvailable += (s, e) => { waveFileWriter.Write(e.Buffer, 0, e.BytesRecorded); };

            waveInput.StartRecording();

            // Initialize the SilenceDetection -- longer timespan prevents false positives but also slower detection speed
            SilenceDetector speechSilenceDetect = new SilenceDetector(waveInput, 16000, TimeSpan.FromSeconds(4), () => 
            {
                UnityEngine.Debug.Log("Silence detected. Stopping recording...");
                waveInput.StopRecording();
            });
        }

        AudioTranscription transcription = audioclient.TranscribeAudio("Assets/user_input.wav", options);
        return transcription.Text;
    }

    private void LoadPrompts()
    {
        try
        {
            if (!File.Exists("Chat1.txt"))
                UnityEngine.Debug.Log("Error: Chat1.txt not found!");

            if (!File.Exists("Chat2.txt"))
                UnityEngine.Debug.Log("Error: Chat2.txt not found!");

            conversation1.Add(ChatMessage.CreateSystemMessage(File.ReadAllText("Chat1.txt")));
            conversation2.Add(ChatMessage.CreateSystemMessage(File.ReadAllText("Chat2.txt")));
        }
        catch (FileNotFoundException ex)
        {
            UnityEngine.Debug.Log($"Error loading prompts: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private string GPT3Agent(List<ChatMessage> messages, string model = "gpt-3.5-turbo", float temperature = 0.9f, int maxTokens = 100, float frequencyPenalty = 2.0f, float presencePenalty = 2.0f)
    {
        OpenAIClient client = new OpenAIClient(apiKey);
        try
        {
            ChatCompletionOptions opts = new ChatCompletionOptions()
            {
                Temperature = temperature,
                MaxOutputTokenCount = maxTokens,
                FrequencyPenalty = frequencyPenalty,
                PresencePenalty = presencePenalty
            };

            ChatClient cl = client.GetChatClient(apiKey);
            ChatCompletion chcmp = cl.CompleteChat(messages, opts);

            return chcmp.Content[0].Text;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log($"An error occurred: {ex.Message}");
            return "";
        }
    }


    private void SaveResponse(string response, string fileName)
    {
        UnityEngine.Debug.Log($"Saving response to: {Path.GetFullPath(fileName)}\nResponse: {response}");
        File.WriteAllText(fileName, response);
        UnityEngine.Debug.Log("Test to See File Content is Being Saved: " + File.ReadAllText(fileName));
    }

    private void WriteSyncFile()
    {
        UnityEngine.Debug.Log($"Writing to sync.txt: {Path.GetFullPath("sync.txt")}");
        File.WriteAllText("sync.txt", "a");
    }

    public void Start()
    {
        int agentSelected = new Random().Next(1, 2);
        UnityEngine.Debug.Log($"{(agentSelected == 1 ? "Kitana" : "Ezio")}: Welcome to the Multi-Conversation AI! Just start talking when you're ready. Say 'stop' to end the conversation.");

        SaveResponse("Welcome to the Multi-Conversation AI! Just start talking when you're ready. Say 'stop' to end the conversation.", agentSelected == 1 ? "speaker1.txt" : "speaker2.txt");
        WriteSyncFile();

        while (true)
        {
            var syncChar = File.ReadAllText("sync.txt");
            if (syncChar == "b")
            {
                File.WriteAllText("sync.txt", string.Empty);
                break;
            }

            string userInput = STT();
            if (userInput.ToLower() == "stop")
            {
                if (ConfirmExit())
                {
                    break;
                }
                else
                {
                    UnityEngine.Debug.Log("Continue Speaking!");
                    continue;
                }
            }

            conversation1.Add(ChatMessage.CreateUserMessage(userInput));
            conversation2.Add(ChatMessage.CreateUserMessage(userInput));

            string response = GPT3Agent(agentSelected == 1 ? conversation1 : conversation2);
            UnityEngine.Debug.Log($"{(agentSelected == 1 ? "Kitana" : "Ezio")}: {response}");

            SaveResponse(response, agentSelected == 1 ? "speaker1.txt" : "speaker2.txt");
            WriteSyncFile();
        }
    }

    private bool ConfirmExit()
    {
        UnityEngine.Debug.Log("Are you sure you want to exit? (yes/no)");
        string response = STT();
        return response.ToLower() == "yes";
    }
}

// Automates the user input removal of silence or background noise

/*
Inspired by: https://ieeexplore.ieee.org/document/9678476

In short, this method uses what the researchers have as mathematical models to compute silence areas, which utilizse the robustness of audio signal energies.

Implementation Steps (or general layout I had): 
1. Calculate the continuous average energy (this is computing the energy of audio samples over a sample moving window of 50ms -- an overlap by 98%)
2. Normalize Energy (compute normalized continous average energy to obtain signal's enveloping representation)
3. Identify Silence (zero-crossing method helps identify the silence based on normalized energy values)

*/
public class SilenceDetector
{
    private readonly WaveInEvent _waveIn;
    private readonly int _sampleRate;
    private readonly int _bufferSize;
    private readonly double _silenceThreshold;
    private DateTime _lastSoundTime;
    private readonly TimeSpan _silenceTimeout;
    private readonly Action _onSilenceDetected;
    private List<double> _energyLevels = new List<double>();

    public SilenceDetector(WaveInEvent waveIn, int sampleRate, TimeSpan silenceTimeout, Action onSilenceDetected, double silenceThreshold = 0.1)
    {
        _waveIn = waveIn;
        _sampleRate = sampleRate;
        _silenceTimeout = silenceTimeout;
        _onSilenceDetected = onSilenceDetected;
        _silenceThreshold = silenceThreshold;
        _bufferSize = (int)(sampleRate * 0.05); // 50 ms window

        _waveIn.DataAvailable += OnDataAvailable;
    }

    private void OnDataAvailable(object sender, WaveInEventArgs e)
    {
        double[] samples = new double[e.Buffer.Length / 2];
        Buffer.BlockCopy(e.Buffer, 0, samples, 0, e.Buffer.Length);
        
        double energy = CalculateEnergy(samples);
        _energyLevels.Add(energy);

        if (_energyLevels.Count >= _bufferSize)
        {
            double normalizedEnergy = NormalizeEnergy(_energyLevels.TakeLast(_bufferSize).ToArray());
            DetectSilence(normalizedEnergy);
        }
    }

    private double CalculateEnergy(double[] samples)
    {
        return samples.Sum(s => s * s); // E = s^2 (equation 1 in paper)
    }

    private double NormalizeEnergy(double[] energyLevels)
    {
        double average = energyLevels.Average();
        double variance = energyLevels.Select(e => Math.Pow(e - average, 2)).Average();
        double stdDev = Math.Sqrt(variance);

        return (energyLevels.Last() - average) / stdDev; // normalization
    }

    private void DetectSilence(double normalizedEnergy)
    {
        if (normalizedEnergy < _silenceThreshold)
        {
            if (DateTime.Now - _lastSoundTime > _silenceTimeout)
            {
                _onSilenceDetected.Invoke();
            }
        }
        else
        {
            _lastSoundTime = DateTime.Now;
        }
    }
}