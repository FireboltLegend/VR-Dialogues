using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAndWardrobeController : MonoBehaviour
{
    // Button variables
    private Vector3 originalButtonPos;
    private Vector3 pressedButtonPos;
    [SerializeField] float pressedDistance;

    // Wardrobe variables
    private GameObject wardrobeObject;
    private Vector3 originalWardrobePos;
    [SerializeField] Transform wardrobeHiddenPos;
    [SerializeField] float wardrobeSpeed;
    private bool wardrobeIsOnGround;

    // Other object variables
    private GameObject otherGameObject;

    // Conditional variables
    private bool canPressButton;
    private bool canMoveWardrobe;
    private bool canCheckDistance;
    [SerializeField] float expectedDistance;

    void Start()
    {
        // Button variables
        originalButtonPos = transform.localPosition;
        pressedButtonPos = originalButtonPos - new Vector3(0, pressedDistance, 0);

        // Wardrobe variables
        wardrobeObject = GameObject.FindGameObjectWithTag("Wardrobe");
        originalWardrobePos = wardrobeObject.transform.position;
        wardrobeIsOnGround = true;

        // Other object variables
        otherGameObject = null;

        // Conditional variables
        canPressButton = true; // canPressButton -> canMoveWardrobe -> canCheckDistances
        canMoveWardrobe = false;
        canCheckDistance = false;
    }

    void Update()
    {
        MoveWardrobe();
        CheckDistance();
    }

    void OnTriggerEnter(Collider other)
    {
        if (canPressButton)
        {
            PressButton();
            canMoveWardrobe = true;
        }
    }

    void PressButton()
    {
        if (canPressButton)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition,
                pressedButtonPos, wardrobeSpeed * Time.deltaTime);
        }
        if (transform.localPosition == pressedButtonPos)
        {
            canPressButton = false;
        }
    }

    void MoveWardrobe()
    {
        if (canMoveWardrobe)
        {
            if (wardrobeIsOnGround)
            {
                wardrobeObject.transform.position = Vector3.MoveTowards(wardrobeObject.transform.position,
                    wardrobeHiddenPos.position, wardrobeSpeed * Time.deltaTime);
            }
            else
            {
                wardrobeObject.transform.position = Vector3.MoveTowards(wardrobeObject.transform.position,
                    originalWardrobePos, wardrobeSpeed * Time.deltaTime);
            }
        }
        if (wardrobeObject.transform.position == originalWardrobePos 
            || wardrobeObject.transform.position == wardrobeHiddenPos.position)
        {
            wardrobeIsOnGround = !wardrobeIsOnGround;
            canMoveWardrobe = false;
            canCheckDistance = true;
        }
    }

    void CheckDistance()
    {
        float actualDistance = -1.0f;
        if (canCheckDistance)
        {
            Debug.Log("Beginning Distance\nReal Distance is " + actualDistance);
            actualDistance = Vector3.Distance(otherGameObject.transform.position, transform.position);
            if (actualDistance >= expectedDistance)
            {
                Debug.Log("Distance achieved!");
                canCheckDistance = false;
                canPressButton = true;
            }
        }
    }
}
