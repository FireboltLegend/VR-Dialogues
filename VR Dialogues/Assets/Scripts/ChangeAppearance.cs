using System.Collections;
using System.Collections.Generic;
using GLTFast.Schema;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class ChangeAppearance : MonoBehaviour
{
    private int currentNum;

    void Start()
    {
        foreach(Transform child in transform)
        {
            if (child.gameObject.activeSelf == true)
            {
                currentNum = child.gameObject.GetComponent<AvatarComponent>().num;
            }
        }
    }

    void Update()
    {
        ChangeSkin();
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
            // ReturnHome(other.gameObject);
            Debug.Log("Original Position = " + other.gameObject.GetComponent<OriginalPosition>().GetOriginalPos());

            other.gameObject.GetComponent<OriginalPosition>().ResetPosition();
            Debug.Log("Object has been moved!");
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

    void ReturnHome(GameObject other)
    {
        /*
        OriginalPosition pos = other.GetComponent<OriginalPosition>();
        if (pos != null)
        {
            pos.ResetPosition();
        }
        */
        Debug.Log("Original Position = " + other.GetComponent<OriginalPosition>().GetOriginalPos());

        other.transform.position = other.GetComponent<OriginalPosition>().GetOriginalPos();
        Debug.Log("Object has been moved!");
    }
}
