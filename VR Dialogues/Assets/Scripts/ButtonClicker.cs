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
    private bool canUnclick;
    private bool canSummon;
    [SerializeField] float speed;
    private bool isSummoning;


    // New method variables
    private bool canPress;
    private bool isHere;

    void Start()
    {
        isSummoning = false;
        canSummon = false;
        originalPos = transform.localPosition;
        otherGameObject = null;
        canUnclick = false;
        maxPress = originalPos - new Vector3(0, 0.26f, 0);
        wardrobeObject = GameObject.FindGameObjectWithTag("Wardrobe");
        originalWardrobePos = wardrobeObject.transform.position;

        canPress = true;

        isHere = true;
    }

    void Update()
    {
        /*
        if (otherGameObject != null && canUnclick)
        {
            if (Vector3.Distance(otherGameObject.transform.position, transform.position) >= distance)
            {
                transform.localPosition = originalPos;
            }
            canUnclick = false;
        }*/
        /*if (otherGameObject != null && !canPress)
        {
            if (Vector3.Distance(otherGameObject.transform.position, transform.position) >= distance)
            {
                canPress = true;
            }
        }*/
    }

    void OnTriggerEnter(Collider other)
    {
        /*if (!isSummoning)
        {
            canUnclick = false;
            otherGameObject = other.gameObject;
            Debug.Log("The button is currently colliding. The distance is " + Vector3.Distance(otherGameObject.transform.position, transform.position));
            transform.localPosition = maxPress;
        }*/
        MoveWardrobe();
        canPress = false;
    }

    void OnTriggerExit(Collider other)
    {
        /*
        if (!isSummoning)
        {
            canUnclick = true;
            Debug.Log("The distance on trigger exit is " + Vector3.Distance(otherGameObject.transform.position, transform.position));
            canSummon = !canSummon;
            SummonOrNot();
        }*/
        // while (Vector3.Distance(otherGameObject.transform.positi
        otherGameObject = other.gameObject;
        WaitForDistance();
    }

    void SummonOrNot()
    {
        isSummoning = true;
        if (canSummon)
        {
            wardrobeObject.transform.position = Vector3.Lerp(originalWardrobePos, hiddenPos.position, speed);
        }
        else
        {
            wardrobeObject.transform.position = Vector3.Lerp(hiddenPos.position, originalWardrobePos, speed);
        }
        isSummoning = false;
    }

    void SwitchState()
    {
        if (isHere)
        {
            transform.localPosition = maxPress;
        }
        else
        {
            transform.localPosition = originalPos;
        }
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

    void WaitForDistance()
    {
        float realDistance = -1.0f;
        Debug.Log("Beginning Distance Loop...");
        while (realDistance < distance)
        {
            Debug.Log("Real Distance is " + realDistance);
            realDistance = Vector3.Distance(otherGameObject.transform.position, transform.position);
        }
        canPress = true;
        Debug.Log("Distance achieved!");
    }

}
