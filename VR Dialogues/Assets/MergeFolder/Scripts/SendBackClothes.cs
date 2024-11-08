using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendBackClothes : MonoBehaviour
{
    private Vector3 originalPos;
    private Quaternion originalRot;
    void Start()
    {
        originalPos = gameObject.transform.position;
        originalRot = gameObject.transform.rotation;
    }

    void Update()
    {
        if (transform.position != originalPos)
        {
            transform.position = originalPos;
            transform.rotation = originalRot;
        }
    }
}
