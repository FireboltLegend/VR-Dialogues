using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendWardrobeBack : MonoBehaviour
{
    [SerializeField] private MoveProps moveProps;
    /*[SerializeField] Transform originalTransform;
    [SerializeField] float speed;*/

    [SerializeField] float buttonPressLength;
    Vector3 pressedPosition;

    GameObject wardrobeObject;
    bool canLerp;

    void Start()
    {
        pressedPosition = transform.localPosition - new Vector3(0, buttonPressLength, 0);
        wardrobeObject = gameObject.transform.parent.gameObject.transform.parent.gameObject; // grandparent object
        canLerp = false;
    }

    void Update()
    {
        /*if (canLerp) 
        {
            wardrobeObject.transform.position = Vector3.Lerp(wardrobeObject.transform.position, originalTransform.position, speed);
        }
        if (wardrobeObject.transform.position == originalTransform.position)
            canLerp = false;*/
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Button has been pressed");
        transform.localPosition = pressedPosition;
        canLerp = true;
        if (other.CompareTag("Player"))
            moveProps.Toggle();
    }
}
