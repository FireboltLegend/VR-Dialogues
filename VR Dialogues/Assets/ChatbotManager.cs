using UnityEngine;
using System.Diagnostics;
using System.IO;
using TMPro;

public class ChatbotManager : MonoBehaviour
{
    [SerializeField] private TMP_Text response; 
    private string pythonScriptPath;

    void Start()
    {
        // Set the path to the Python script
        pythonScriptPath = Path.Combine(Application.dataPath, "SpeechRec.py");
        UnityEngine.Debug.Log("Python Script Path: " + pythonScriptPath);

        // Clear text files initially
        ClearTextFiles();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartPythonScript();
            UpdateResponseText();
        }
    }

    void StartPythonScript()
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                //FileName = "python", 
                FileName = "py", // use this if the above line doesn't work; seems to be how Python sets the default launcher
                Arguments = $"\"{pythonScriptPath}\"",
                UseShellExecute = true,
                CreateNoWindow = false
            };

            Process process = Process.Start(startInfo);
            process.WaitForExit();

            UnityEngine.Debug.Log("Python script started successfully.");
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("Failed to start Python script: " + ex.Message);
        }
    }

    void UpdateResponseText()
    {
        string speakerFilePath = Path.Combine(Application.dataPath, "speaker.txt");

        if (File.Exists(speakerFilePath))
        {
            response.text = File.ReadAllText(speakerFilePath);
        }
        else
        {
            response.text = "No response available.";
        }
    }

    void ClearTextFiles()
    {
        File.WriteAllText(Path.Combine(Application.dataPath, "speaker.txt"), "");
        File.WriteAllText(Path.Combine(Application.dataPath, "user.txt"), "");
    }
}
