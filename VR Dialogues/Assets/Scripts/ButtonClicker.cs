using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonClicker : MonoBehaviour
{
    private Vector3 originalPos;
    [SerializeField] Vector3 maxPress;
    [SerializeField] float distance;
    private Vector3 otherPosition;
    private GameObject otherGameObject;
    
    void Start()
    {
        originalPos = transform.position;
        //otherPosition = Vector3.zero;
        otherGameObject = null;
    }

    void Update()
    {
        if (otherGameObject != null)
        {
            if (Vector3.Distance(otherGameObject.transform.position, transform.position) >= distance)
            {
                transform.position = originalPos;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        otherGameObject = other.gameObject;
        otherPosition = other.transform.position;
        Debug.Log("The button is currently colliding. The distance is " + Vector3.Distance(otherPosition, transform.position));
        transform.localPosition = maxPress;
        
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("The distance on trigger exit is " + Vector3.Distance(otherPosition, transform.position));
    }


}
