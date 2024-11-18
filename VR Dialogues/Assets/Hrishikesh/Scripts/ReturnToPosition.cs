using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPosition : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    [SerializeField] private float returnTime;
    [SerializeField] private Transform returnPosition;
    [SerializeField] private float checkDistance;
    private bool returning;
    public float timer;

    void Start()
    {
        startPosition = returnPosition.position;
        startRotation = transform.rotation;
    }
    void Update()
    {
        if (returning)
            timer += Time.deltaTime;
        if (timer > returnTime)
            ReturnToStart();
        if (Vector3.Distance(returnPosition.position, transform.position) > checkDistance && !returning)
            returning = true;
    }

    void ReturnToStart()
    {
        if (returning)
        {
            if (GetComponent<Rigidbody>() != null)
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().MovePosition(returnPosition.position);
            GetComponent<Rigidbody>().MoveRotation(startRotation);
            transform.position =(returnPosition.position);
            transform.rotation = startRotation;
            GetComponent<Rigidbody>().useGravity = true;
            timer = 0;
            returning = false;
        }
    }
}
