using System.Collections;
using System.Collections.Generic;
using GLTFast.Schema;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class ChangeAppearance : MonoBehaviour
{
    private int currentNum;

    void Update()
    {
        //currentNum = 
        ChangeSkin();
    }

    void ChangeSkin()
    {
        if (currentNum != 0)
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.GetComponent<AvatarComponent>().num == GetInputNum())
                {
                    child.gameObject.SetActive(true);
                    currentNum = child.gameObject.GetComponent<AvatarComponent>().num;
                    setOthersInactive(currentNum);
                    break;
                }
            }

        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Clothes")
        {
            int clothesNum = other.gameObject.GetComponent<AvatarComponent>().num;
            currentNum = clothesNum;
        }
    }

    int GetInputNum()
    {
        Debug.Log("Checking for input...");
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            return 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            return 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            return 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            return 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            return 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            return 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            return 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            return 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            return 9;
        }
        else return 0;
    }

    void setOthersInactive(int num)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<AvatarComponent>().num != num)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
