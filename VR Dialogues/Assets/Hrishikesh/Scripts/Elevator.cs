using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private float timeToReach;
    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform playerRig;
    [SerializeField] private Transform[] elevatorPositions;
    [SerializeField] private int floor;
    [SerializeField] private Transform door;
    private int previousFloor;
    private Vector3 doorStartPos;
    private bool doorClosing;

    
    void Start()
    {
        doorStartPos = door.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (previousFloor != floor)
            StartTeleporting(floor);
        previousFloor = floor;
        if (!doorClosing)
            door.localPosition = Vector3.MoveTowards(door.localPosition, doorStartPos, 10 * Time.deltaTime);
        else
            door.localPosition = Vector3.MoveTowards(door.localPosition, doorStartPos + Vector3.up * 2.25f, 10 * Time.deltaTime);
    }

    public void StartTeleporting(int floor)
    {
        StartCoroutine(Teleport(floor));
    }

    IEnumerator Teleport(int floor)
    {
        doorClosing = true;
        yield return new WaitForSeconds(timeToReach);
        transform.position = elevatorPositions[floor].position;
        transform.rotation = elevatorPositions[floor].rotation;
        playerRig.position = elevatorPositions[floor].position;
        playerRig.rotation = elevatorPositions[floor].rotation;
        OVRManager.display.RecenterPose();
        print(OVRPlugin.GetLocalTrackingSpaceRecenterCount());
        doorClosing = false;
    }
}
