using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPosition : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    [SerializeField] private float returnTime;
    private bool returning;
    private float timer;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
    void Update()
    {
        if (Vector3.Distance(startPosition, transform.position) > 1.5f && !returning)
            returning = true;
        if (returning)
            timer += Time.deltaTime;
        if (timer >= returnTime)
            ReturnToStart();
        if (GetComponent<Rigidbody>().isKinematic)
            timer = 0;
    }

    void ReturnToStart()
    {
        if (returning)
        {
            if (GetComponent<Rigidbody>() != null)
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = startPosition;
            transform.rotation = startRotation;
            GetComponent<Rigidbody>().useGravity = true;
            timer = 0;
            returning = false;
        }
    }
}
