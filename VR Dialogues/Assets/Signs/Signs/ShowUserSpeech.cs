using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ShowUserSpeech : MonoBehaviour
{
    [SerializeField] private TMP_Text speechText;
    [SerializeField] private string filePath;
    [SerializeField] private GameObject boy;
    [SerializeField] private GameObject girl;
    private bool thinkingEnabled;
    // Start is called before the first frame update
    void Start()
    {
        filePath = Path.Combine(Application.dataPath, "speaker.txt");
        File.WriteAllText(filePath, "");
        thinkingEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (File.Exists(filePath))
        {
            string speech = File.ReadAllText(filePath);
            speechText.text = speech;
            speechText.outlineWidth = 0f;
            if (speech.Equals("Say Something!")) 
            {
                speechText.outlineWidth = 0.2f;

                if (!thinkingEnabled)
                { 
                    PlayAnimation(boy, "Base Layer.thinking");
                    PlayAnimation(girl, "Base Layer.thinking");
                    thinkingEnabled = true;
                }
            }
            else { 
                thinkingEnabled = false;
            }
        }
    }

    public void PlayAnimation(GameObject eca, string stateAnimation)
    {
        Animator[] animators = eca.GetComponentsInChildren<Animator>(true);
        
        foreach (Animator animator in animators)
        {
            if (animator.gameObject.activeInHierarchy)
            {
                Debug.Log($"Playing animation {stateAnimation} on {animator.gameObject.name}");
                animator.Play(stateAnimation);
            }
        }     
    }
}
