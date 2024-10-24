using Meta.WitAi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OriginalPosition : MonoBehaviour
{
    private Vector3 originalPos;
    private Quaternion originalRot;
    private Transform child;

    void Start()
    {
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
        child = transform.Find("[BuildingBlock] HandGrab");
        Debug.Log("The orignal position at the beginning of the script for " + gameObject.GetComponent<AvatarComponent>() + " is " + originalPos);
    }

    private void Update()
    {
        if (originalPos == transform.localPosition && originalRot == transform.localRotation)
        {
            child.gameObject.SetActive(true);
        }
    }

    public Vector3 GetOriginalPos()
    {
        return originalPos;
    }

    public Quaternion GetOriginalRot()
    {
        return originalRot;
    }

    public Transform GetChild()
    {
        return child;
    }

}
