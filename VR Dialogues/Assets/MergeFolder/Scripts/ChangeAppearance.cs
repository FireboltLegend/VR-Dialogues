using System.Collections;
using System.Collections.Generic;
using GLTFast.Schema;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;
using System.Diagnostics;
using System.IO;

public class ChangeAppearance : MonoBehaviour
{
    private int currentNum;
    private bool returnClothes;

    Collider clothesCollider;

    private Process pythonProcess;

    void Start()
    {
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
                    SetOthersInactive(currentNum);
                    // if (pythonProcess != null && !pythonProcess.HasExited)
                    // {
                    //     pythonProcess.Kill();
                    //     pythonProcess.Dispose();
                    // }
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

}
