using UnityEngine;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.IO;

public class ChatbotManager : MonoBehaviour
{
    private Process pythonProcess;

    // Start is called before the first frame update
    void Start()
    {
        string pythonInterpreterPath = @"C:\Users\Student\AppData\Local\Programs\Python\Python310\python.exe";
        string pythonScriptPath = System.IO.Path.Combine(Application.dataPath, "SpeechRec.py");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UnityEngine.Debug.Log("Q Pressed");
            StartPythonScript();

            //animator.SetBool("IsIdle", true);
            File.WriteAllText("Assets/speaker.txt", "");
            File.WriteAllText("Assets/user.txt", "");
        }
    }

    void StartPythonScript()
    {
        UnityEngine.Debug.Log("Starting Python Script...");

        string pythonInterpreterPath = @"C:\Users\Student\AppData\Local\Programs\Python\Python310\python.exe";
        string pythonScriptPath = @$"{Application.dataPath}/SpeechRec.py";

        UnityEngine.Debug.Log("Python Interpreter Path: " + pythonInterpreterPath);
        UnityEngine.Debug.Log("Python Script Path: " + pythonScriptPath);

        pythonProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = pythonInterpreterPath,
                Arguments = $"\"{pythonScriptPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = false  // Set to true if you don't want to see the window
            },
            EnableRaisingEvents = true
        };

        pythonProcess.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                UnityEngine.Debug.Log("Output: " + e.Data);
            }
        };

        pythonProcess.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                UnityEngine.Debug.LogError("Error: " + e.Data);
            }
        };

        pythonProcess.Exited += (sender, e) =>
        {
            UnityEngine.Debug.Log("Python process exited.");
            pythonProcess.Dispose();
        };

        try
        {
            pythonProcess.Start();
            pythonProcess.BeginOutputReadLine();
            pythonProcess.BeginErrorReadLine();
            UnityEngine.Debug.Log("Python process started successfully.");
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("Failed to start Python script: " + ex.Message);
        }
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
}