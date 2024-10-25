using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MicrophoneFactory : MonoBehaviour
{
    // GUI-Friendly Elements
    public Dropdown microphoneDropdown;  // display all system-wide microphones (mainly for debugging purposes)
    public Button startButton;
    public Button stopButton;
    //public Image microphoneVisualizer;
    //public RawImage spectrogramDisplay;

    // Internal Properties
    private AudioClip audioClip;
    private string selectedMicrophone;
    private bool isRecording = false;
    private float[] samples = new float[256];
    //private float[] spectrumData = new float[256];
    private Texture2D spectrogramTexture;
    //private int spectrogramWidth = 512;
    //private int spectrogramHeight = 256;
    //private int spectrogramXPosition = 0;

    // Initialize GUI elements
    void Awake()
    {
        microphoneDropdown = GameObject.Find("MicrophoneDropdown").GetComponent<Dropdown>();
        startButton = GameObject.Find("Record").GetComponent<Button>();
        stopButton = GameObject.Find("Stop").GetComponent<Button>();
        //microphoneVisualizer = GameObject.Find("MicrophoneVisualizer").GetComponent<Image>();
        //spectrogramDisplay = GameObject.Find("SpectrogramDisplay").GetComponent<RawImage>();
    }
    void Start()
    {
        //spectrogramTexture = new Texture2D(spectrogramWidth, spectrogramHeight, TextureFormat.RGBA32, false);
        //spectrogramDisplay.texture = spectrogramTexture;

        microphoneDropdown.AddOptions(Microphone.devices.ToList());

        if (Microphone.devices.Length > 0)
        {
            selectedMicrophone = Microphone.devices[0];
            microphoneDropdown.onValueChanged.AddListener(OnMicrophoneSelected);
        }

        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
    }

    void Update()
    {
        if (isRecording)
        {
            audioClip.GetData(samples, Microphone.GetPosition(selectedMicrophone) - samples.Length);
            //UpdateMicrophoneVisualizer(samples);
        }
    }

    public void OnMicrophoneSelected(int index)
    {
        selectedMicrophone = Microphone.devices[index];
    }
    public void StartRecording()
    {
        if (selectedMicrophone != null)
        {
            audioClip = Microphone.Start(selectedMicrophone, false, 30, 44100);
            isRecording = true;
        }
    }
    public void StopRecording()
    {
        if (isRecording)
        {
            Microphone.End(selectedMicrophone);
            isRecording = false;

            SaveWav(audioClip, "Assets/user_input.wav");
        }
    }

    /*private void UpdateMicrophoneVisualizer(float[] samples)
    {
        float maxAmplitude = samples.Max();
        microphoneVisualizer.fillAmount = Mathf.Clamp(maxAmplitude, 0f, 1f);
    }*/

    /*private void UpdateSpectrogram(float[] spectrumData)
    {
        Color[] pixels = spectrogramTexture.GetPixels();
        for (int x = 1; x < spectrogramWidth; x++)
        {
            for (int y = 0; y < spectrogramHeight; y++)
            {
                spectrogramTexture.SetPixel(x - 1, y, spectrogramTexture.GetPixel(x, y));
            }
        }

        for (int y = 0; y < spectrumData.Length; y++)
        {
            float intensity = Mathf.Clamp01(spectrumData[y] * 10);
            Color color = new Color(intensity, intensity, intensity);
            spectrogramTexture.SetPixel(spectrogramWidth - 1, y, color);
        }

        spectrogramTexture.Apply();
    }*/

    // Converts the AudioClip to a WAV file
    private void SaveWav(AudioClip clip, string filePath)
    {
        var samples = new float[clip.samples];
        clip.GetData(samples, 0);
        byte[] wavData = ConvertToWav(samples, clip.channels, clip.frequency);

        File.WriteAllBytes(filePath, wavData);
        Debug.Log("Audio saved to: " + filePath);
    }

    private byte[] ConvertToWav(float[] samples, int channels, int sampleRate)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        int totalSamples = samples.Length;
        int totalDataLength = totalSamples * 2;
        int fileSize = 44 + totalDataLength;

        // WAV Header
        writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
        writer.Write(fileSize);
        writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));
        writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));

        // metadata 
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)channels);
        writer.Write(sampleRate);
        writer.Write(sampleRate * channels * 2);
        writer.Write((short)(channels * 2));
        writer.Write((short)16);

        // audio subchunk metadata
        writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
        writer.Write(totalDataLength);

        foreach (var sample in samples)
        {
            short intSample = (short)(sample * short.MaxValue);
            writer.Write(intSample);
        }

        return stream.ToArray();
    }
}
