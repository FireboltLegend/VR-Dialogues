using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OriginalPosition : MonoBehaviour
{
    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.position;
        Debug.Log("The orignal position at the beginning of the script for " + gameObject.GetComponent<AvatarComponent>() + " is " + originalPos);
    }

    public void ResetPosition()
    {
        transform.position = originalPos;
    }

    public Vector3 GetOriginalPos()
    {
        return originalPos;
    }
}
