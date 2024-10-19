using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightCharacter : MonoBehaviour
{
    /*[SerializeField] LineRenderer rightLineRenderer;
    [SerializeField] LineRenderer leftLineRenderer;*/
    private Transform leftPos;
    private Transform rightPos;

    void Start()
    {
        /*rightLineRenderer.useWorldSpace = true;
        leftLineRenderer.useWorldSpace = true;
        leftPos = leftLineRenderer.gameObject.GetComponent<Transform>();
        rightPos = rightLineRenderer.gameObject.GetComponent<Transform>();*/
    }

    void Update()
    {
        /*CheckRayCollision(rightLineRenderer);
        CheckRayCollision(leftLineRenderer);*/
        //CheckRayCollisionMouse();
    }

    void OutlineCharacter()
    {
        Debug.Log("Highlighting character...");
        foreach (Transform child in transform)
        {
            Outline outline = child.gameObject.GetComponent<Outline>();
            if (child.gameObject.activeSelf)
            {
                outline.enabled = true;
            }
            else outline.enabled = false;
        }
    }

    void RemoveOutline()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                child.gameObject.GetComponent<Outline>().enabled = false;
            }
        }
    }

    /*void CheckRayCollision(LineRenderer lineRenderer)
    {
        // Declare raycast hit object and set it to global position of line renderer
        RaycastHit hit;
        //Get start and end positions of the line in world space
        Vector3 startPosition = lineRenderer.transform.TransformPoint(lineRenderer.GetPosition(0));
        Vector3 endPosition = lineRenderer.transform.TransformPoint(lineRenderer.GetPosition(1));


        // Calculate ray direction
        Vector3 direction = (endPosition - startPosition);
        Ray ray = new Ray(startPosition, endPosition);
        // Check if ray hits object
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                OutlineCharacter();
            }
            else
            {
                RemoveOutline();
            }
        }
    }*/
    void CheckRayCollisionMouse()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                OutlineCharacter();
            }
            else
            {
                RemoveOutline();
            }
        }
    }

    Color GetColor()
    {
        if (gameObject.tag == "Avatar")
        {
            return Color.red;
        }
        else if (gameObject.tag == "Interactable")
        {
            return Color.green;
        }
        else
        {
            return Color.blue;
        }
    }

}
