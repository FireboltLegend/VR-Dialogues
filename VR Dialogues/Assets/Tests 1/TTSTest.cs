using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.IO;

public class TTSTests
{
    private TTS ttsScript;
    private string syncFilePath;

    [SetUp]
    public void Setup()
    {
        GameObject gameObject = new GameObject("GameObject");
        ttsScript = gameObject.AddComponent<TTS>();
        ttsScript.girlAudioSource = gameObject.AddComponent<AudioSource>();
        ttsScript.boyAudioSource = gameObject.AddComponent<AudioSource>();

        ttsScript.girlAudioSource.clip = Resources.Load<AudioClip>("girlAudio.wav");
        ttsScript.boyAudioSource.clip = Resources.Load<AudioClip>("boyAudio.wav");

        Debug.Log($"Girl Clip: {ttsScript.girlAudioSource.clip}, Boy Clip: {ttsScript.boyAudioSource.clip}"); 
        syncFilePath = Path.Combine(Application.dataPath, "sync.txt");

        gameObject.AddComponent<AudioListener>();

    }

    // Test #1 - Sanity Check for "sync.txt" file
    [UnityTest]
    public IEnumerator TestFileExistence()
    {
        Assert.IsTrue(File.Exists(syncFilePath), "✗ Sync File existence");
        yield break;
    }

    // Test #2 - File read operation to see if "a" is in the sync file
    [UnityTest]
    public IEnumerator TestStringContentContainsA()
    {
        string content = File.ReadAllText(syncFilePath);
        Assert.IsTrue(content.Contains("a"), "✗ File content contains 'a'");
        yield break;
    }

    // Test #3 - Check for 1 or 2 to signify appropriate ECA
    [UnityTest]
    public IEnumerator TestStringContentContains1Or2()
    {
        string content = File.ReadAllText(syncFilePath);
        bool containsOneOrTwo = content.Contains("1") || content.Contains("2");
        Assert.IsTrue(containsOneOrTwo, "✗ File content contains '1' or '2'");
        yield break;
    }

    // Test #4 - Girl Audio Source Load and Play Operation
    [UnityTest]
    public IEnumerator TestGirlAudioSourceLoadingAndPlaying()
    {
        File.WriteAllText(syncFilePath, "a1");
        yield return null;

        ttsScript.Update();
        ttsScript.girlAudioSource.Play();
        yield return null;

        Assert.IsNotNull(ttsScript.girlAudioSource.clip, "✗ Loading Girl audio source clip object");
        Assert.IsTrue(ttsScript.girlAudioSource.isPlaying, "✗ Girl audio source playing");

        yield return ttsScript.StartCoroutine(ttsScript.CheckAudioPlayback(ttsScript.girlAudioSource));
        Assert.AreEqual("b", File.ReadAllText(syncFilePath), "✗ Sync file updated to 'b'");
    }

    // Test #5 - Boy Audio Source Load and Play Operation
    [UnityTest]
    public IEnumerator TestBoyAudioSourceLoadingAndPlaying()
    {
        // 5. Write content that triggers boyAudioSource
        File.WriteAllText(syncFilePath, "a2");
        yield return null;

        ttsScript.Update();
        ttsScript.boyAudioSource.Play();
        yield return null;

        Assert.IsNotNull(ttsScript.boyAudioSource.clip, "✗ Loading Boy audio source clip object");
        Assert.IsTrue(ttsScript.boyAudioSource.isPlaying, "✗ Boy audio source playing");

        yield return ttsScript.StartCoroutine(ttsScript.CheckAudioPlayback(ttsScript.boyAudioSource));
        Assert.AreEqual("b", File.ReadAllText(syncFilePath), "✗ Sync file updated to 'b'.");
    }
}