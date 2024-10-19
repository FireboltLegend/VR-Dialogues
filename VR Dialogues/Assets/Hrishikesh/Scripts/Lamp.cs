using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    private Light _light;
    private Vector3 previousPosition;
    private Vector3 velocity;

    private void Start()
    {
        _light = GetComponentInChildren<Light>();
    }

    private void Update()
    {
        velocity = transform.position - previousPosition;
        previousPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (velocity.magnitude > 0.5f)
            _light.enabled = !_light.enabled;
    }
}
