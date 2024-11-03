using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    private GameObject _light;
    private Vector3 previousPosition;
    public Vector3 velocity;
    private float timer = 0;
    private bool wasKinematic;

    private void Start()
    {
        _light = GetComponentInChildren<Light>().gameObject;
    }

    private void FixedUpdate()
    {
        timer -= Time.deltaTime;
        velocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
    }

    public void ToggleLight()
    {
        print("ADDADAd");
        _light.SetActive(!_light.activeInHierarchy);
    }
}
