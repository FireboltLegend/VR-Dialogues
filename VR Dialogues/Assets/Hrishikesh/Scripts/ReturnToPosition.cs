using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPosition : MonoBehaviour
{
    private Vector3 startPosition;
    [SerializeField] private float returnTime;
    private bool returning;

    void Start()
    {
        startPosition = transform.position;
    }
    void Update()
    {
        if (Vector3.Distance(startPosition, transform.position) > 1 && !returning)
        {
            returning = true;
            Invoke("ReturnToStart", returnTime);
        }
    }

    void ReturnToStart()
    {
        if (returning)
        {
            transform.position = startPosition;
            if (GetComponent<Rigidbody>() != null)
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            returning = false;
        }
    }
}
