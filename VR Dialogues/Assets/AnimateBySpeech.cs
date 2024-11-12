using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AnimateBySpeech : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string filePath;
    private bool newString = true;
    private string oldString;

    // Start is called before the first frame update
    void Start()
    {
        filePath = Path.Combine(Application.dataPath, "speaker.txt");
    }

    // Update is called once per frame
    void Update()
    {

        animator = this.gameObject.GetComponentInChildren<Animator>();
        if (File.Exists(filePath))
        {
            string speech = File.ReadAllText(filePath);
            string nocap = speech.ToLower();
            newString = oldString != nocap;
            oldString = nocap;
            String avatarname = this.gameObject.name == "Avatar 1" ? "Ezio" : "Kitana";
            if(newString)
            {
            switch (nocap)
            {
                case string when (nocap.Contains("happy") || nocap.Contains("good") || nocap.Contains("excellent") || nocap.Contains("nice!") || nocap.Contains("cool!") || nocap.Contains("perfect!")) && nocap.Contains(avatarname + ":"):
                    animator.SetBool("Happy", true);
                    break;
                case string when (nocap.Contains("...") || nocap.Contains("oh,") || nocap.Contains("oh.") || nocap.Contains("no") || nocap.Contains("low-key")) && nocap.Contains(avatarname + ":"):
                    animator.SetBool("Happy", false);
                    break;
                case string when (nocap.Contains("sorry") || nocap.Contains("sad") || nocap.Contains("sad") || nocap.Contains("shame") || nocap.Contains("oh...")) && nocap.Contains(avatarname + ":"):
                    animator.SetBool("Sad", true);
                    break;
                case string when (nocap.Contains("alright") || nocap.Contains("thank you") || nocap.Contains("thank goodness") || nocap.Contains("thanks") || nocap.Contains("cool") || nocap.Contains("chill")) && nocap.Contains(avatarname + ":"):
                    animator.SetBool("Sad", false);
                    break;
                case string when (nocap.Contains(" wow") || nocap.Contains("surprising") || nocap.Contains("surprised") || nocap.Contains(" oh!") || nocap.Contains(" ahh!")) && nocap.Contains(avatarname + ":"):
                    animator.SetBool("Shocked", true);
                    break;
                case string when (nocap.Contains(" mad") || nocap.Contains(" angry") || nocap.Contains("i can't stand")) && nocap.Contains(avatarname + ":"):
                    animator.SetBool("Mad", true);
                    animator.SetTrigger("Angry");
                    break;
                case string when (nocap.Contains("chill") || nocap.Contains("relax") || nocap.Contains("low-key") || nocap.Contains("phew") || nocap.Contains("relieved")) && nocap.Contains(avatarname + ":"):
                    animator.SetBool("Mad", false);
                    break;
                case string when (nocap.Contains("Horrible") || nocap.Contains("so sorry") || nocap.Contains("terrible") || nocap.Contains("tired of pretending")) && nocap.Contains(avatarname + ":"):
                    animator.SetBool("Sad", true);
                    animator.SetTrigger("Crying");
                    break;
                case string when (nocap.Contains("i agree") || nocap.Contains("i concur") || nocap.Contains(" aye") || nocap.Contains("indeed") || nocap.Contains("sounds good") || nocap.Contains("okay")) && nocap.Contains(avatarname + ":"):
                    animator.SetTrigger("Agreement");
                    break;
                case string when (nocap.Contains("doubt") || nocap.Contains("not sure") || nocap.Contains("maybe") || nocap.Contains("i don't think so") || nocap.Contains("i disagree")) && nocap.Contains(avatarname + ":"):
                    animator.SetTrigger("Disagreement");
                    break;
                case string when (nocap.Contains("congratulations") || nocap.Contains("impressive") || nocap.Contains("congratulate")) && nocap.Contains(avatarname + ":"):
                    animator.SetTrigger("Applause");
                    animator.SetBool("Happy", true);
                    break;
                case string when (nocap.Contains(" hmm") || nocap.Contains("i'm thinking") || nocap.Contains("i wonder") || nocap.Contains(" hm")) && nocap.Contains(avatarname + ":"):
                    animator.SetTrigger("Thinking");
                    break;
                case string when nocap.Contains("say something!"):
                    animator.SetBool("Attentive", true);
                    break;
                default:
                    animator.SetBool("Shocked", false);
                    break;

            }
            }
        }
    }
}
