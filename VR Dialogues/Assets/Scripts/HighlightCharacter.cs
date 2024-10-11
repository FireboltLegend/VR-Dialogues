using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightCharacter : MonoBehaviour
{
    private Outline outline;
    [SerializeField] LineRenderer rightLineRenderer;
    [SerializeField] LineRenderer leftLineRenderer;
    private Transform leftPos;
    private Transform rightPos;

    void Start()
    {
        outline = gameObject.GetComponent<Outline>();
        outline.enabled = false;
        leftPos = leftLineRenderer.gameObject.GetComponent<Transform>();
        rightPos = rightLineRenderer.gameObject.GetComponent<Transform>();
    }

    void Update()
    {
        CheckRayCollision(rightLineRenderer);
        CheckRayCollision(leftLineRenderer);
        CheckRayCollisionMouse();
    }

    void OnSpaceHighlight()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Highlighting character...");
            outline.enabled = !outline.enabled;
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Gaze" || other.gameObject == rightLineRenderer.gameObject
            || other.gameObject == leftLineRenderer.gameObject)
        {
            Debug.Log("Highlighting character...");
            OutlineCharacter();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Gaze" || other.gameObject.tag == "MainCamera")
        {
            Debug.Log("Removing highlight from character");
            RemoveOutline();
        }
    }

    void OutlineCharacter()
    {
        Debug.Log("Highlighting character...");
        outline.enabled = true;
    }

    void RemoveOutline()
    {
        outline.enabled = false;
    }

    void CheckRayCollision(LineRenderer lineRenderer)
    {
        // Declare raycast hit object and set it to global position of line renderer
        RaycastHit hit;
        //Get start and end positions of the line in world space
        Vector3 startPosition = lineRenderer.transform.TransformPoint(lineRenderer.GetPosition(0));
        Vector3 endPosition = lineRenderer.transform.TransformPoint(lineRenderer.GetPosition(1));

        // Calculate ray direction
        Vector3 direction = (endPosition - startPosition).normalized;
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
    }
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

    void ChangeHighlightColor(Color color)
    {
        outline.OutlineColor = color;
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
