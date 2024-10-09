using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TTS))]
public class TTSInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TTS ttsScript = (TTS)target;

        if (GUILayout.Button("Play TTS"))
        {
            ttsScript.PlayTTS();
        }
    }
}
