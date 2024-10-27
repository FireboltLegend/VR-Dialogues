using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenAI;
using OpenAI.Audio;
using OpenAI.Chat;
using UnityEngine;
using System.Collections;

public class MultiConversationAI : MonoBehaviour
{
    private List<ChatMessage> conversation1;
    private List<ChatMessage> conversation2;
    private string apiKey;

    // Speech-To-Text Daemon (overload #1) -- given you want to use MicrophoneFactory or other custom Microphone Invocation
    private string STT(string audioFilePath)
    {
        OpenAIClient client = new OpenAIClient(apiKey); 
        AudioClient audioclient = client.GetAudioClient("whisper-1");
        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.Verbose,
            TimestampGranularities = AudioTimestampGranularities.Word | AudioTimestampGranularities.Segment,
        };


        AudioTranscription transcription = audioclient.TranscribeAudio(audioFilePath, options);
        conversation1.Add(ChatMessage.CreateUserMessage(transcription.Text));
        conversation2.Add(ChatMessage.CreateUserMessage(transcription.Text));

        return transcription.Text;
    }

    private void LoadPrompts()
    {
        try
        {
            if (!File.Exists("Assets/Chat1.txt"))
                UnityEngine.Debug.Log("Error: Chat1.txt not found!");

            if (!File.Exists("Assets/Chat2.txt"))
                UnityEngine.Debug.Log("Error: Chat2.txt not found!");

            conversation1.Add(ChatMessage.CreateSystemMessage(File.ReadAllText("Assets/Chat1.txt")));
            conversation2.Add(ChatMessage.CreateSystemMessage(File.ReadAllText("Assets/Chat2.txt")));
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

    public void RunMCAI()
    {

        conversation1 = new List<ChatMessage>();
        conversation2 = new List<ChatMessage>();
        apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "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";


        LoadPrompts();
        StartCoroutine(ConversationLoop());
    }

    private IEnumerator ConversationLoop()
    {
        int agentSelected = new System.Random().Next(1, 2);
        UnityEngine.Debug.Log($"{(agentSelected == 1 ? "Kitana" : "Ezio")}: Welcome to the Multi-Conversation AI! Just start talking when you're ready. Say 'stop' to end the conversation.");

        SaveResponse("Welcome to the Multi-Conversation AI! Just start talking when you're ready. Say 'stop' to end the conversation.", agentSelected == 1 ? "speaker1.txt" : "speaker2.txt");

        while (true)
        {
            string userInput = STT($"{Application.dataPath}/user_input.wav");
            if (userInput.ToLower() == "stop")
            {
                if (ConfirmExit())
                {
                    UnityEngine.Debug.Log("Exiting conversation. Returning to normal state in Unity.");
                    yield break;
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

            yield return null;
        }
    }

    private bool ConfirmExit()
    {
        UnityEngine.Debug.Log("Are you sure you want to exit? (yes/no)");
        string response = STT($"{Application.dataPath}/user_input.wav");
        return response.ToLower() == "yes";
    }
}