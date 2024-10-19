using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    [SerializeField] private Transform playerCam;
    [SerializeField] private Transform portalA;
    [SerializeField] private Transform portalB;

    private void Update()
    {
        Vector3 playerOffsetFromPortal = playerCam.position - portalB.position;
        transform.position = portalA.position + playerOffsetFromPortal;

        float angleBtwPortalRotations = Quaternion.Angle(portalA.rotation, portalB.rotation) + 180;
        Quaternion portalRotationDifference = Quaternion.AngleAxis(angleBtwPortalRotations, Vector3.up);
        Vector3 newCameraDirection = portalRotationDifference * playerCam.forward;

        transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
    }
}
