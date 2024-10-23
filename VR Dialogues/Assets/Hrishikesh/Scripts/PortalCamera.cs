using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    [SerializeField] private Transform playerCam;
    [SerializeField] private Transform portalA;
    [SerializeField] private Transform portalB;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerOffsetFromPortal = portalA.position - playerCam.position;
        transform.position = portalB.position + playerOffsetFromPortal;
    }
}
