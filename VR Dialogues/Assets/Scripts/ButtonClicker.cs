using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonClicker : MonoBehaviour
{
    private Vector3 originalPos;
    private Vector3 maxPress; // 0.26 down from the original position
    [SerializeField] float distance;
    private GameObject otherGameObject;
    private GameObject wardrobeObject;
    private Vector3 originalWardrobePos;
    [SerializeField] Transform hiddenPos;
    [SerializeField] float speed;


    // New method variables
    private bool canPress;
    private bool isHere;
    private bool waitForDistance;

    void Start()
    {
        originalPos = transform.localPosition;
        otherGameObject = null;
        maxPress = originalPos - new Vector3(0, 0.26f, 0);
        wardrobeObject = GameObject.FindGameObjectWithTag("Wardrobe");
        originalWardrobePos = wardrobeObject.transform.position;

        canPress = true;
        isHere = true;
        waitForDistance = false;
    }

    void Update()
    {
        WaitForDistance();
    }

    void OnTriggerEnter(Collider other)
    {
        MoveWardrobe();
        canPress = false;
    }

    void OnTriggerExit(Collider other)
    {
        otherGameObject = other.gameObject;
        waitForDistance = true;
    }

    void MoveWardrobe()
    {
        if (canPress)
        {
            if (isHere)
            {
                wardrobeObject.transform.position = Vector3.Lerp(originalWardrobePos, hiddenPos.position, speed);
            }
            else
            {
                wardrobeObject.transform.position = Vector3.Lerp(hiddenPos.position, originalWardrobePos, speed);
            }
            SwitchState();
        }
        isHere = !isHere;
    }

    void SwitchState()
    {
        if (isHere)
        {
            Debug.Log("Moving to pressed state");
            transform.localPosition = maxPress;
        }
        else
        {
            Debug.Log("Moving to original state");
            transform.localPosition = originalPos;
        }
    }

    void WaitForDistance()
    {
        float realDistance = -1.0f;
        Debug.Log("Beginning Distance Loop...");
        if (waitForDistance)
        {
            Debug.Log("Real Distance is " + realDistance);
            realDistance = Vector3.Distance(otherGameObject.transform.position, transform.position);
        }
        if (realDistance >= distance)
        {
            canPress = true;
            Debug.Log("Distance achieved!");
            waitForDistance = false;
        }
    }

}
