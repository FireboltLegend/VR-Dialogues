using Meta.WitAi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OriginalPosition : MonoBehaviour
{
    private Vector3 originalPos;
    private Transform child;

    void Start()
    {
        originalPos = transform.localPosition;
        child = transform.Find("[BuildingBlock] HandGrab");
        Debug.Log("The orignal position at the beginning of the script for " + gameObject.GetComponent<AvatarComponent>() + " is " + originalPos);
    }

    private void Update()
    {
        if(originalPos == transform.localPosition)
        {
            // Find HandGrab Building Block Child
            // Enable it
            child.gameObject.SetActive(true);
        }
    }

    public void ResetPosition()
    {
        transform.position = originalPos;
    }

    public Vector3 GetOriginalPos()
    {
        return originalPos;
    }

    public Transform GetChild()
    {
        return child;
    }

}
