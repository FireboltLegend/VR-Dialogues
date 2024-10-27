using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using CSCore;
using CSCore.Streams;
using CSCore.Codecs.WAV;
using CSCore.Codecs; 

public class MicrophoneFactory : MonoBehaviour
{
    public TMP_Dropdown microphoneDropdown;  
    public Button startButton;
    public Button stopButton;

    private AudioClip audioClip;
    private string selectedMicrophone;
    private bool isRecording = false;
    private float[] samples = new float[256];

    void Awake()
    {
        microphoneDropdown = GameObject.Find("MicrophoneDropdown").GetComponent<TMP_Dropdown>();
        startButton = GameObject.Find("Record").GetComponent<Button>();
        stopButton = GameObject.Find("Stop").GetComponent<Button>();
    }

    void Start()
    {
        var microphones = Microphone.devices.ToList();
        microphoneDropdown.AddOptions(microphones);
        selectedMicrophone = microphones.Count > 0 ? microphones[0] : null;

        if (selectedMicrophone == null)
        {
            Debug.LogWarning("No microphones found.");
            return;
        }

        microphoneDropdown.onValueChanged.AddListener(OnMicrophoneSelected);
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
    }

    void Update()
    {
        if (isRecording && audioClip != null)
        {
            int currentPosition = Microphone.GetPosition(selectedMicrophone);
            if (currentPosition >= samples.Length)
            {
                audioClip.GetData(samples, currentPosition - samples.Length);
            }
        }
    }

    public void OnMicrophoneSelected(int index)
    {
        if (index >= 0 && index < Microphone.devices.Length)
        {
            selectedMicrophone = Microphone.devices[index];
        }
        else
        {
            Debug.LogError("Selected microphone index is out of range.");
        }
    }

    public void StartRecording()
    {
        if (selectedMicrophone != null)
        {
            audioClip = Microphone.Start(selectedMicrophone, false, 10, 44100);
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

    private void SaveWav(AudioClip clip, string filePath, float silenceThreshold = 0.01f)
    {
        if (clip == null)
        {
            Debug.LogError("AudioClip is null. Cannot save WAV file.");
            return;
        }

        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);
        string tempFilePath = Path.Combine(Path.GetTempPath(), "temp.wav");
        File.WriteAllBytes(tempFilePath, ConvertToWav(samples, clip.channels, clip.frequency));
        var trimmedSamples = TrimSilence(tempFilePath, silenceThreshold);

        if (trimmedSamples.Length == 0)
        {
            Debug.LogError("No audio data to save after trimming silence.");
            return;
        }

        byte[] wavData = ConvertToWav(trimmedSamples, clip.channels, clip.frequency);
        File.WriteAllBytes(filePath, wavData);
        Debug.Log("Audio saved to: " + filePath);
    }


    public float[] TrimSilence(string filePath, float silenceThreshold = 0.01f)
    {
        using (var waveSource = CodecFactory.Instance.GetCodec(filePath))
        {
            byte[] byteSamples = new byte[waveSource.Length];
            waveSource.Read(byteSamples, 0, byteSamples.Length);

            float[] samples = new float[byteSamples.Length / 2];
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = BitConverter.ToInt16(byteSamples, i * 2) / 32768f;
            }

            int startIndex = 0;
            int endIndex = samples.Length - 1;

            while (startIndex <= endIndex && Math.Abs(samples[startIndex]) < silenceThreshold)
            {
                startIndex++;
            }

            while (endIndex >= startIndex && Math.Abs(samples[endIndex]) < silenceThreshold)
            {
                endIndex--;
            }

            if (startIndex > endIndex)
            {
                Debug.LogWarning("No valid audio detected.");
                return new float[0]; 
            }

            int trimmedLength = endIndex - startIndex + 1;
            float[] trimmedSamples = new float[trimmedLength];
            Array.Copy(samples, startIndex, trimmedSamples, 0, trimmedLength);

            Debug.Log($"Trimmed {samples.Length} samples down to {trimmedLength} samples.");
            return trimmedSamples;
        }
    }
    
    private byte[] ConvertToWav(float[] samples, int channels, int sampleRate)
    {
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            int totalSamples = samples.Length;
            int totalDataLength = totalSamples * 2; // 2 bytes per sample
            int fileSize = 44 + totalDataLength; // 44 bytes for the header + data length

            // Write WAV header
            writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF")); // Chunk ID
            writer.Write(fileSize);                                   // Chunk size
            writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE")); // Format

            // fmt subchunk
            writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt ")); // Subchunk1 ID
            writer.Write(16);                                         // Subchunk1 size (16 for PCM)
            writer.Write((short)1);                                  // Audio format (1 for PCM)
            writer.Write((short)channels);                           // Number of channels
            writer.Write(sampleRate);                                // Sample rate
            writer.Write(sampleRate * channels * 2);               // Byte rate (sampleRate * numChannels * bytesPerSample)
            writer.Write((short)(channels * 2));                    // Block align (numChannels * bytesPerSample)
            writer.Write((short)16);                                 // Bits per sample

            // data subchunk
            writer.Write(System.Text.Encoding.UTF8.GetBytes("data")); // Subchunk2 ID
            writer.Write(totalDataLength);                           // Subchunk2 size

            // Write audio data
            foreach (var sample in samples)
            {
                short intSample = (short)(sample * short.MaxValue); // Convert float to short
                writer.Write(intSample);
            }

            return stream.ToArray(); // Return the complete WAV file as a byte array
        }
    }
}



// Because of a lack of time to work on this research project, this SilenceDetection library is saved for a future use!
/*
public class SilenceDetector
{
    private readonly int _sampleRate;
    private readonly double _silenceThreshold;
    private readonly TimeSpan _silenceTimeout;
    private readonly Action _onSilenceDetected;
    private DateTime _lastSoundTime;
    private readonly int _bufferSize;
    private List<double> _energyLevels = new List<double>();

    public int StartIndex { get; set; } = -1; 
    public int EndIndex { get; set; } = -1; 

    public SilenceDetector(int sampleRate, TimeSpan silenceTimeout, Action onSilenceDetected, double silenceThreshold = 0.1)
    {
        _sampleRate = sampleRate;
        _silenceTimeout = silenceTimeout;
        _onSilenceDetected = onSilenceDetected;
        _silenceThreshold = silenceThreshold;
        _bufferSize = (int)(sampleRate * 0.05); // 50 ms window
    }

    public void OnDataAvailable(float[] samples)
    {
        double energy = CalculateEnergy(samples);
        _energyLevels.Add(energy);

        if (energy > _silenceThreshold)
        {
            if (StartIndex == -1) // Set start index on first sound
            {
                StartIndex = _energyLevels.Count - samples.Length; 
            }

            EndIndex = _energyLevels.Count - 1; 
            _lastSoundTime = DateTime.Now; 
        }

        if (_energyLevels.Count >= _bufferSize)
        {
            double normalizedEnergy = NormalizeEnergy(_energyLevels.TakeLast(_bufferSize).ToArray());
            DetectSilence(normalizedEnergy);
        }
    }

    private double CalculateEnergy(float[] samples)
    {
        return samples.Sum(s => s * s);
    }

    private double NormalizeEnergy(double[] energyLevels)
    {
        double average = energyLevels.Average();
        double variance = energyLevels.Select(e => Math.Pow(e - average, 2)).Average();
        double stdDev = Math.Sqrt(variance);

        return (energyLevels.Last() - average) / stdDev; 
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
    }
}*/