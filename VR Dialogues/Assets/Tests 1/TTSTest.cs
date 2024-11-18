using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.IO;

public class TTSTests
{
    private TTS ttsScript;
    private string syncFilePath;
    private AudioClip girlClip;
    private AudioClip boyClip;

    [OneTimeSetUp]
    public void LoadAudioClips()
    {
        // Load audio clips once before all tests
        girlClip = Resources.Load<AudioClip>("girlAudio");
        boyClip = Resources.Load<AudioClip>("boyAudio");

        // Verify clips loaded successfully
        if (girlClip == null)
            Debug.LogError("Failed to load girlAudio clip from Resources folder");
        if (boyClip == null)
            Debug.LogError("Failed to load boyAudio clip from Resources folder");
    }

    [SetUp]
    public void Setup()
    {
        GameObject gameObject = new GameObject("GameObject");
        ttsScript = gameObject.AddComponent<TTS>();
        ttsScript.girlAudioSource = gameObject.AddComponent<AudioSource>();
        ttsScript.boyAudioSource = gameObject.AddComponent<AudioSource>();
        ttsScript.girlAudioSource.clip = girlClip;
        ttsScript.boyAudioSource.clip = boyClip;

        Debug.Log($"Girl Clip assigned: {ttsScript.girlAudioSource.clip != null}");
        Debug.Log($"Boy Clip assigned: {ttsScript.boyAudioSource.clip != null}");

        syncFilePath = Path.Combine(Application.dataPath, "sync.txt");
        gameObject.AddComponent<AudioListener>();

        if (!File.Exists(syncFilePath))
        {
            File.WriteAllText(syncFilePath, "");
        }
    }

    [TearDown]
    public void Cleanup()
    {
        if (ttsScript != null)
        {
            Object.DestroyImmediate(ttsScript.gameObject);
        }
    }

    // Basic Setup Tests
    [Test]
    public void Test_ComponentsInitialization()
    {
        Assert.IsNotNull(ttsScript, "TTS Script should be initialized");
        Assert.IsNotNull(ttsScript.girlAudioSource, "Girl AudioSource should be initialized");
        Assert.IsNotNull(ttsScript.boyAudioSource, "Boy AudioSource should be initialized");
    }

    [Test]
    public void Test_AudioClipsLoaded()
    {
        Assert.IsNotNull(girlClip, "Girl audio clip should be loaded");
        Assert.IsNotNull(boyClip, "Boy audio clip should be loaded");
    }

    // File Tests
    [Test]
    public void Test_SyncFileExists()
    {
        Assert.IsTrue(File.Exists(syncFilePath), "Sync file should exist");
    }

    [Test]
    public void Test_SyncFileReadable()
    {
        Assert.DoesNotThrow(() => File.ReadAllText(syncFilePath), "Should be able to read sync file");
    }

    [Test]
    public void Test_SyncFileWritable()
    {
        string testContent = "test";
        Assert.DoesNotThrow(() => File.WriteAllText(syncFilePath, testContent), "Should be able to write to sync file");
    }

    // Content Tests
    [UnityTest]
    public IEnumerator Test_FileContentContainsA()
    {
        File.WriteAllText(syncFilePath, "a");
        string content = File.ReadAllText(syncFilePath);
        Assert.IsTrue(content.Contains("a"), "File content should contain 'a'");
        yield break;
    }

    [UnityTest]
    public IEnumerator Test_FileContentContains1Or2()
    {
        File.WriteAllText(syncFilePath, "1");
        string content = File.ReadAllText(syncFilePath);
        bool containsOneOrTwo = content.Contains("1") || content.Contains("2");
        Assert.IsTrue(containsOneOrTwo, "File content should contain '1' or '2'");
        yield break;
    }

    // Audio Source Tests
    [UnityTest]
    public IEnumerator Test_GirlAudioSourceProperties()
    {
        Assert.IsNotNull(ttsScript.girlAudioSource.clip, "Girl AudioSource should have a clip assigned");
        Assert.IsFalse(ttsScript.girlAudioSource.isPlaying, "Girl AudioSource should not be playing initially");
        Assert.IsTrue(ttsScript.girlAudioSource.enabled, "Girl AudioSource should be enabled");
        yield break;
    }

    [UnityTest]
    public IEnumerator Test_BoyAudioSourceProperties()
    {
        Assert.IsNotNull(ttsScript.boyAudioSource.clip, "Boy AudioSource should have a clip assigned");
        Assert.IsFalse(ttsScript.boyAudioSource.isPlaying, "Boy AudioSource should not be playing initially");
        Assert.IsTrue(ttsScript.boyAudioSource.enabled, "Boy AudioSource should be enabled");
        yield break;
    }

    // Playback Tests
    [UnityTest]
    public IEnumerator Test_GirlAudioSourceLoadingAndPlaying()
    {
        Assert.IsNotNull(girlClip, "Girl clip should be loaded from Resources");

        File.WriteAllText(syncFilePath, "a1");
        yield return new WaitForSeconds(0.1f);

        ttsScript.Update();
        Assert.IsNotNull(ttsScript.girlAudioSource.clip, "Girl AudioSource clip should not be null after Update");

        ttsScript.girlAudioSource.Play();
        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(ttsScript.girlAudioSource.isPlaying, "Girl audio source should be playing");
        yield return ttsScript.StartCoroutine(ttsScript.CheckAudioPlayback(ttsScript.girlAudioSource));
        Assert.AreEqual("b", File.ReadAllText(syncFilePath), "Sync file should be updated to 'b'");
    }

    [UnityTest]
    public IEnumerator Test_BoyAudioSourceLoadingAndPlaying()
    {
        Assert.IsNotNull(boyClip, "Boy clip should be loaded from Resources");

        File.WriteAllText(syncFilePath, "a2");
        yield return new WaitForSeconds(0.1f);

        ttsScript.Update();
        Assert.IsNotNull(ttsScript.boyAudioSource.clip, "Boy AudioSource clip should not be null after Update");

        ttsScript.boyAudioSource.Play();
        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(ttsScript.boyAudioSource.isPlaying, "Boy audio source should be playing");
        yield return ttsScript.StartCoroutine(ttsScript.CheckAudioPlayback(ttsScript.boyAudioSource));
        Assert.AreEqual("b", File.ReadAllText(syncFilePath), "Sync file should be updated to 'b'");
    }

    // Edge Cases
    [UnityTest]
    public IEnumerator Test_InvalidSyncFileContent()
    {
        File.WriteAllText(syncFilePath, "invalid");
        yield return new WaitForSeconds(0.1f);
        ttsScript.Update();
        Assert.IsFalse(ttsScript.girlAudioSource.isPlaying, "Girl audio should not play with invalid content");
        Assert.IsFalse(ttsScript.boyAudioSource.isPlaying, "Boy audio should not play with invalid content");
    }

    [UnityTest]
    public IEnumerator Test_EmptySyncFile()
    {
        File.WriteAllText(syncFilePath, "");
        yield return new WaitForSeconds(0.1f);
        ttsScript.Update();
        Assert.IsFalse(ttsScript.girlAudioSource.isPlaying, "Girl audio should not play with empty file");
        Assert.IsFalse(ttsScript.boyAudioSource.isPlaying, "Boy audio should not play with empty file");
    }
}