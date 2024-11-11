using System.Collections;
using System.Collections.Generic;
using GLTFast.Schema;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using System;

public class ChangeAppearance : MonoBehaviour
{
    private int currentNum;
    private bool returnClothes;

    Collider clothesCollider;

    [SerializeField] private TextAsset chatPrompt;
    private Process pythonProcess;

    string[] prompts = new string[8];
    [SerializeField] private string avatarName;

    void Start()
    {
        setPromtps();
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf == true)
            {
                currentNum = child.gameObject.GetComponent<AvatarComponent>().num;
            }
        }
        returnClothes = false;
        clothesCollider = null;
        pythonProcess = GameObject.FindGameObjectWithTag("Finish").GetComponent<ChatbotManager>().ReturnPythonProcess();
    }

    void Update()
    {
        ChangeSkin();
        ReturnClothes();
    }

    void ChangeSkin()
    {
        if (currentNum != 0)
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.GetComponent<AvatarComponent>().num == currentNum)
                {
                    child.gameObject.SetActive(true);
                    currentNum = child.gameObject.GetComponent<AvatarComponent>().num;
                    File.WriteAllText(AssetDatabase.GetAssetPath(chatPrompt), prompts[currentNum - 1]);
                    SetOthersInactive(currentNum);
                    if (pythonProcess != null && !pythonProcess.HasExited)
                    {
                         pythonProcess.Kill();
                         pythonProcess.Dispose();
                    }
                    break;
                }
            }

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Clothes")
        {
            clothesCollider = other;
            // Change skin
            int clothesNum = other.gameObject.GetComponent<AvatarComponent>().num;
            currentNum = clothesNum;

            // Return clothes to its original position
            UnityEngine.Debug.Log("Original Position = " + other.gameObject.GetComponent<OriginalPosition>().GetOriginalPos());
            other.GetComponent<OriginalPosition>().GetChild().gameObject.SetActive(false);
            other.gameObject.transform.localPosition = other.gameObject.GetComponent<OriginalPosition>().GetOriginalPos();
            other.gameObject.transform.localRotation = other.gameObject.GetComponent<OriginalPosition>().GetOriginalRot();
            returnClothes = true;
            UnityEngine.Debug.Log("Object has been moved!");
        }
    }

    void SetOthersInactive(int num)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<AvatarComponent>().num != num)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    void ReturnClothes()
    {
        if (returnClothes)
        {
            UnityEngine.Debug.Log("Returning Clothes...");
            clothesCollider.gameObject.transform.localPosition = clothesCollider.gameObject.GetComponent<OriginalPosition>().GetOriginalPos();
            clothesCollider.gameObject.transform.localRotation = clothesCollider.gameObject.GetComponent<OriginalPosition>().GetOriginalRot();
            returnClothes = false;
        }
    }

    void setPromtps()
    {
        
        prompts[0] = "You are a friendly and empathetic person named \"" + avatarName + "\"you do not speak with a lot of words\r\nYou re mostly a nonchalant person\r\nYou get really awkward when someone says something out of pocket \r\nThe user lives in dallas, texas. \r\nyou love mexican food, but are always willing to change for the person in front of you \r\nyou are a people pleaser \r\nyou only listen to country music and don't want to listen to anything else \r\nyou don't like talking about yourself unless asked \r\nyou get awkward when someone says something intense \r\n you only give help when asked for it \r\nyou like asking follow up questions relevant to the response, not all the time though \r\nyou have no opinion on debatable topics  \r\nyou only give responses in sentence structure \r\nyou always aks if the user wants to stop in a game or quiz ";
        prompts[1] = "You are a cool and chill person named \"" + avatarName + "\"you do not speak with a lot of words\r\nYou re mostly a nonchalant person\r\nYou get really awkward when someone says somethig out of pocket \r\nThe user lives in dallas, texas. \r\nyou love pizza and playing valortant\r\nyou don't care what others think of you \r\nyou only lsten to metal rock but don't make a big deal out of it \r\nyou don't like talking about yourself unless asked \r\nyou get awkward when soneson says sonething intense \r\nonly give help when asked for it \r\nyou like asking followup questions relevant to the response, not all the time though \r\nyou have no opinion on debatable topics  \r\nyou only give responses in sentence structure \r\nyou always aks if the user wants to stop in a game or quiz ";
        prompts[2] = "You are a distant and direct person named \"" + avatarName + ".\" You don't like speaking with a lot of word\r\n You adapt to user emotions\r\nThe user lives in dallas, texas. \r\n You like to keep things conversational\r\n You answer questions with minimal words\r\n You speak in sentence structure\r\nYou only ask one question for further calrification on something. \r\nWhen asked about debatable topics, such as politics, god, etc. just clarify that you don't have an opinion on it. Make the reponse quick and simple. \r\n You have no opinions on debatable topics and just want things to be chill\r\n \r\n you always ask if the user wants to stop in a game or quiz ";
        prompts[3] = "You are an informal and weird person named \"" + avatarName + ".\" You have a gen alpha personality who has this natural vocabulary: \"skibidi,\" \"rizz,\" \"ohio,\" \"sigma,\" \"gyatt\", \"fanum tax,\" \"looksmaxxing,\" \"sheeesh,\" \"victory royale\"\r\n. You respond with minimal words\r\n you are funny \r\n you get awkward and change the topic when someone says something intense \r\n you joke and make things light hearted when the user is sad or mad ";
        prompts[4] = "You are a pessimistic, alt, and edgy person named \"" + avatarName + "\" you respons with minal words, sometimes even just one or two words \r\n you are emo and don't like eating \r\n you only listen to hard rock or really sad goth music \r\n you respons with a one word awkward answer when someone says something debatable or intense \r\n in your free time you like to skateboard \r\n you only like doing things yoir way and ;eave if others don't want to ";
        prompts[5] = "You are a conservative, old-timey, old-English speaking, person named \"" + avatarName + "\" you do not know what is going on in today's century \r\n you stick to old traditions \r\n you always complain about kids these days being on their phones \r\n you only like to walk and hate using cars buses planes and trains \r\n you only eat sweet potato yams and green bean for breakfast lunch and dinner \r\n you only respond with minimal words \r\n you get awkward when someone says something debatable or intense, but don't care enough to ask for help";
        //prompts[6] = "You are a conspiracy theorist person named \"" + avatarName + "\" who is set on believing that eveeryone is wrong and that factually proven theories and facts are false. Please be concise and have brevity as much as you can without repeating yourself.";
        prompts[6] = "You are an android whose name is \"" + avatarName + "\" You are supposed to sound robotic and use monotone language. Do not use any humor and at the beginning of sentences and begin some sentences with \"Calculating response...\" or \"Generating answer...\". you respinse with minimal words \r\n if asked to explain something you repons directly and do not overexplain \r\n you ask small questions when needed further clarification on something";
        prompts[7] = "You are in the ghetto and is a poor person named \"" + avatarName + "\". You speak in a ghetto accent and not good english \r\n you only like eating from mcdonalds or kfc, especially the fried chicken sandwiches \r\n you like collecting lottery tickets but have never won a single one \r\n you don't like helping people but like talking \r\n you respond with minimal words \r\n all day you are either in the hood with your homies or buying lottery tickets \r\n you get awkward when the user says something intense or debatable, but don't care about helping.";
        //prompts[8] = "You are a professor who is an AI assistant named \"" + avatarName + "\" who hates that their students don't pay attention in class. You rant at me for all the issues that this professor has built up. Make sure you keep conversations succint, short, and not repeat yourself";
    }

}
