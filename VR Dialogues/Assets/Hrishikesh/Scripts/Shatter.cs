using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shatter : MonoBehaviour
{
    [SerializeField] private GameObject particles;
    private Vector3 previousPosition;
    private Vector3 velocity;

    private void Start()
    {
        particles.GetComponent<ParticleSystem>().loop = false;
    }

    private void FixedUpdate()
    {
        velocity = (transform.position - previousPosition)/Time.deltaTime;
        previousPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (velocity.magnitude > 4f)
        {
            GameObject newParticles = Instantiate(particles, transform.position, transform.rotation);
            transform.position = Vector3.one * -1000;
        }
    }
}
