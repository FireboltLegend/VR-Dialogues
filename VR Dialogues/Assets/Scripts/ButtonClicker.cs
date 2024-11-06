using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Oculus.Interaction.Body.Input;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class ButtonClicker : MonoBehaviour
{
    private Vector3 originalPos;
    private Vector3 maxPress; // 0.26 down from the original position
    [SerializeField] float distance;
    [SerializeField, ReadOnly(true)]private GameObject otherGameObject;
    [SerializeField, ReadOnly(true)]private GameObject wardrobeObject;
    [SerializeField, ReadOnly(true)]private Vector3 originalWardrobePos;
    [SerializeField, ReadOnly(true)] Transform hiddenPos;
    [SerializeField] float speed;


    // New method variables
    [SerializeField, ReadOnly(true)]private bool canPress;
    [SerializeField, ReadOnly(true)]private bool isHere;
    [SerializeField, ReadOnly(true)]private bool waitForDistance;


    private bool isAccessible;
    private bool canMove;
    private bool[] readyToPress;

    void Start()
    {
        originalPos = transform.localPosition;
        otherGameObject = null;
        maxPress = originalPos - new Vector3(0, 0.26f, 0);
        wardrobeObject = GameObject.FindGameObjectWithTag("Wardrobe");
        originalWardrobePos = wardrobeObject.transform.position;

        // button clicker 2.0
        canPress = true;
        isHere = true;

        waitForDistance = false;

        // button clicker 3.0
        isAccessible = true;
        canMove = false;
        /*readyToPress = new bool[2];
        readyToPress[0] = true;
        readyToPress[1] = true;*/
    }

    void Update()
    {
        WaitForDistance();
        Move();
    }

    void OnTriggerEnter(Collider other)
    {
        //canPress = false;
        if (canPress)
        {
            canMove = true;
            canPress = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        otherGameObject = other.gameObject;
        waitForDistance = true;
    }

    void SwitchState()
    {
        if (isAccessible) // maybe change to isHere
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
        if (waitForDistance)
        {
            Debug.Log("Beginning Distance\nReal Distance is " + realDistance);
            realDistance = Vector3.Distance(otherGameObject.transform.position, transform.position);
            if (realDistance >= distance)
            {
                canPress = true;
                Debug.Log("Distance achieved!");
                waitForDistance = false;
            }
        }

    }

    void Move()
    {
        if (canMove)
        {
            if (isAccessible) // wardrobe is on the ground
            {
                if (wardrobeObject.transform.position != hiddenPos.position)
                {
                    wardrobeObject.transform.position = Vector3.Lerp(originalWardrobePos, hiddenPos.position, speed);
                }
                else
                {
                    isAccessible = false;
                    canMove = false;
                }
            }
            else // wardrobe is in the sky
            {
                if (wardrobeObject.transform.position != originalWardrobePos)
                {
                    wardrobeObject.transform.position = Vector3.Lerp(hiddenPos.position, originalWardrobePos, speed);
                }
                else
                {
                    isAccessible = true;
                    canMove = false;
                }
            }
        }
    }
}
