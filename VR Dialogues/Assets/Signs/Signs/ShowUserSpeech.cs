using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowUserSpeech : MonoBehaviour
{
    [SerializeField] private TMP_Text speechText;
    [SerializeField] private string filePath;

    // Start is called before the first frame update
    void Start()
    {
        filePath = Path.Combine(Application.dataPath, "speaker.txt");
    }

    // Update is called once per frame
    void Update()
    {
        if (File.Exists(filePath))
        {
            string speech = File.ReadAllText(filePath);
            speechText.text = speech;
        }
    }
}
