using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    Transform transform;
    [SerializeField] float moveSpeed = 0.05f;
    void Start()
    {
        transform = GetComponent<Transform>();
    }

    void Update()
    {
        MoveCube();
    }

    void MoveCube()
    {
        // Move the cube using the transform component and wasd keys
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, 0, moveSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0, 0, -moveSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-moveSpeed, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(moveSpeed, 0, 0);
        }
    }
}
