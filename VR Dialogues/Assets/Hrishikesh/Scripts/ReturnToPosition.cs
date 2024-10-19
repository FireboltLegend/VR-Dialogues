using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPosition : MonoBehaviour
{
    [SerializeField] private Vector3 startPosition;
    private Vector3 startPosition;
    [SerializeField] private float returnTime;
    private bool returning;

    private void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
            Return();
        if (Vector3.Distance(startPosition, transform.position) > 1 && !returning)
        {
            returning = true;
            Invoke("ReturnToStart", returnTime);
        }
    }

    private void Return()
    {
        transform.position = startPosition;
            returning = false;
        }
    }
}
