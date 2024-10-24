using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meta.WitAi.Data;
using OpenAI;
using OpenAI.Audio;
using OpenAI.Chat;


public class MultiConversationAI
{
    private List<Dictionary<string, string>> conversation1;
    private List<Dictionary<string, string>> conversation2;
    private string apiKey;

    public MultiConversationAI()
    {
        conversation1 = new List<Dictionary<string, string>>();
        conversation2 = new List<Dictionary<string, string>>();
        apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "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";
        

        LoadPrompts();
        STT();
    }

    // Main Speech-To-Text Daemon -- will take in the already inputted user microphone recording
    private string STT()
    {
        OpenAIClient client = new OpenAIClient(apiKey);
        AudioClient audioclient = client.GetAudioClient("whisper-1");
        string audioFilePath = $"Assets/user_input.wav";
        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.Verbose,
            TimestampGranularities = AudioTimestampGranularities.Word | AudioTimestampGranularities.Segment,
        };

        AudioTranscription transcription = audioclient.TranscribeAudio(audioFilePath, options);
        return transcription.Text;
    }

    private void LoadPrompts()
    {
        try
        {
            if (!File.Exists("Chat1.txt"))
                Console.WriteLine("Error: Chat1.txt not found!");

            if (!File.Exists("Chat2.txt"))
                Console.WriteLine("Error: Chat2.txt not found!");

            conversation1.Add(new Dictionary<string, string> { { "role", "system" }, { "content", File.ReadAllText("Chat1.txt") } });
            conversation2.Add(new Dictionary<string, string> { { "role", "system" }, { "content", File.ReadAllText("Chat2.txt") } });
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error loading prompts: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private string GPT3Agent(List<Dictionary<string, string>> messages, string model = "gpt-3.5-turbo", float temperature = 0.9f, int maxTokens = 100, float frequencyPenalty = 2.0f, float presencePenalty = 2.0f)
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
            Console.WriteLine($"An error occurred: {ex.Message}");
            return "";
        }
    }


    private void SaveResponse(string response, string fileName)
    {
        Console.WriteLine($"Saving response to: {Path.GetFullPath(fileName)}\nResponse: {response}");
        File.WriteAllText(fileName, response);
        Console.WriteLine("Test to See File Content is Being Saved: " + File.ReadAllText(fileName));
    }

    private void WriteSyncFile()
    {
        Console.WriteLine($"Writing to sync.txt: {Path.GetFullPath("sync.txt")}");
        File.WriteAllText("sync.txt", "a");
    }

    public void Start()
    {
        int ezioCount = 0;
        int kitanaCount = 0;

        int agentSelected = new Random().Next(1, 2);
        Console.WriteLine($"{(agentSelected == 1 ? "Kitana" : "Ezio")}: Welcome to the Multi-Conversation AI! Just start talking when you're ready. Say 'stop' to end the conversation.");

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

            string userInput = GetAudio();
            if (userInput.ToLower() == "stop")
            {
                if (ConfirmExit())
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Continue Speaking!");
                    continue;
                }
            }

            Console.WriteLine("User: " + userInput);
            conversation1.Add(new Dictionary<string, string> { { "role", "user" }, { "content", userInput } });
            conversation2.Add(new Dictionary<string, string> { { "role", "user" }, { "content", userInput } });

            string response = GPT3Agent(agentSelected == 1 ? conversation1 : conversation2);
            Console.WriteLine($"{(agentSelected == 1 ? "Kitana" : "Ezio")}: {response}");

            SaveResponse(response, agentSelected == 1 ? "speaker1.txt" : "speaker2.txt");
            WriteSyncFile();
        }
    }

    private bool ConfirmExit()
    {
        Console.WriteLine("Are you sure you want to exit? (yes/no)");
        string response = GetAudio();
        return response.ToLower() == "yes";
    }

    public static void Main(string[] args)
    {
        var ai = new MultiConversationAI();
        ai.Start();
    }
}
