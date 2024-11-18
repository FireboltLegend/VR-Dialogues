using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    public int floor;
    public bool pressed;
    public GameObject elevator;

    private void OnTriggerEnter(Collider other)
    {
    }
}
