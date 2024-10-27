using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour {

	public Transform playerCamera;
	public Transform portal;
	public Transform otherPortal;
	public float rotationOffset;
	private Transform startTransform;

    private void Start()
    {
        startTransform = transform;
    }

    // Update is called once per frame
    void Update () {
		Vector3 playerOffsetFromPortal = playerCamera.position - otherPortal.position;
		transform.position = portal.position + playerOffsetFromPortal;

        float angularDifferenceBetweenPortalRotations = Quaternion.Angle(portal.rotation, otherPortal.rotation) + rotationOffset;

        Quaternion portalRotationalDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);
		Vector3 newCameraDirection = portalRotationalDifference * new Vector3(playerCamera.forward.x, playerCamera.forward.y, playerCamera.forward.z);
		transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
	}
}