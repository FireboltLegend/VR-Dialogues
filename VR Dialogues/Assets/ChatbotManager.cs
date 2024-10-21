using UnityEngine;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.IO;

public class ChatbotManager : MonoBehaviour
{
    private TTS ttsmanager;
    private Process pythonProcess;

    // Start is called before the first frame update
    void Start()
    {
        string pythonInterpreterPath = @"C:/Library/Frameworks/Python.framework/Versions/3.9/bin/python3";
        string pythonScriptPath = System.IO.Path.Combine(Application.dataPath, "SpeechRec.py");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UnityEngine.Debug.Log("Q Pressed");
            StartPythonScript();
            File.WriteAllText("Assets/speaker.txt", "");
            File.WriteAllText("Assets/user.txt", "");
        }

        // Check if transcription is ready
        if (IsTranscriptionReady())
        {
            string responseText = File.ReadAllText("Assets/speaker.txt");
            if (!string.IsNullOrEmpty(responseText))
            {
                UnityEngine.Debug.Log("Playing TTS: " + responseText);
                PlayTextAsSpeech(responseText);

                // Clear the sync file after reading
                File.WriteAllText("Assets/sync.txt", "");
            }
        }
    }

    void StartPythonScript()
    {
        UnityEngine.Debug.Log("Starting Python Script...");

        string pythonInterpreterPath = @"C:\Users\ayush\AppData\Local\Programs\Python\Python39\python.exe";
        string pythonScriptPath = @$"{Application.dataPath}/SpeechRec.py";  // Make sure the script path is correct

        // Log the paths to help debug
        UnityEngine.Debug.Log("Python interpreter path: " + pythonInterpreterPath);
        UnityEngine.Debug.Log("Python script path: " + pythonScriptPath);

        // Check if the Python interpreter exists
        if (!File.Exists(pythonInterpreterPath))
        {
            UnityEngine.Debug.LogError("Python interpreter not found: " + pythonInterpreterPath);
            return;
        }

        // Check if the Python script exists
        if (!File.Exists(pythonScriptPath))
        {
            UnityEngine.Debug.LogError("Python script not found: " + pythonScriptPath);
            return;
        }

        // Add double quotes around the script path
        string arguments = $"\"{pythonScriptPath}\"";

        pythonProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = arguments,  // Properly quote the Python script path
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = false,
                WorkingDirectory = Path.GetDirectoryName(pythonScriptPath) // Set the working directory
            },
            EnableRaisingEvents = true // Allow capturing process exit event
        };

        // Event handler for capturing the process exit event
        pythonProcess.Exited += (sender, e) =>
        {
            string output = pythonProcess.StandardOutput.ReadToEnd();
            string errorOutput = pythonProcess.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(output))
            {
                UnityEngine.Debug.Log("Python script output: " + output);
            }

            if (!string.IsNullOrEmpty(errorOutput))
            {
                UnityEngine.Debug.LogError("Python script error: " + errorOutput);
            }

            pythonProcess.Dispose(); // Dispose of the process to free resources
        };

        // Start the Python process
        try
        {
            pythonProcess.Start();
            UnityEngine.Debug.Log("Python process started successfully.");
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("Failed to start Python script: " + ex.Message);
        }

        UnityEngine.Debug.Log("Python time part2");
    }


    // Method to check if the transcription is ready by reading the sync file
    private bool IsTranscriptionReady()
    {
        string syncFilePath = "Assets/sync.txt";
        if (File.Exists(syncFilePath))
        {
            string status = File.ReadAllText(syncFilePath);
            return status.Trim() == "ready";
        }
        return false;
    }

    // Optionally, you may want to stop the Python process when the Unity application quits
    private void OnApplicationQuit()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.Kill();
            pythonProcess.Dispose();
        }
    }

    // Method to play the transcribed text as speech
    void PlayTextAsSpeech(string text)
    {
        // Assuming you have a text-to-speech method in Unity
        // Implement the logic here to convert text to speech using your chosen TTS service
        // For example, you could use Unity's Text-to-Speech integration or a third-party API.
        UnityEngine.Debug.Log("Text to Speech: " + text);
        // Add the TTS integration here
        ttsmanager.PlayTTS();
    }
}
