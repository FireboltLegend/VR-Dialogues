using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPosition : MonoBehaviour
{
    [SerializeField] private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (transform.position.y < -1200)
            Return();
    }

    private void Return()
    {
        transform.position = startPosition;
    }
}
