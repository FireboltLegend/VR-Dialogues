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
    [SerializeField] private Transform[] agents;
    [SerializeField] private Transform[] agent1Positions;
    [SerializeField] private Transform[] agent2Positions;
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
            door.localPosition = Vector3.Lerp(door.localPosition, doorStartPos, 4 * Time.deltaTime);
        else
            door.localPosition = Vector3.Lerp(door.localPosition, new Vector3(doorStartPos.x, doorStartPos.y, 0), 4 * Time.deltaTime);

        if (Vector3.Distance(door.localPosition, doorStartPos) < 0.01f)
            door.gameObject.SetActive(false);
        else
            door.gameObject.SetActive(true);
    }

    public void StartTeleporting(int floor)
    {
        StopAllCoroutines();
        StartCoroutine(Teleport(floor));
    }

    IEnumerator Teleport(int floor)
    {
        doorClosing = true;
        yield return new WaitForSeconds(timeToReach);
        playerRig.parent = transform;
        transform.position = elevatorPositions[floor].position;
        transform.rotation = elevatorPositions[floor].rotation;

        agents[0].position = agent1Positions[floor].position;
        agents[1].position = agent2Positions[floor].position;
        agents[0].rotation = agent1Positions[floor].rotation;
        agents[1].rotation = agent2Positions[floor].rotation;

        playerRig.parent = null;
        doorClosing = false;
    }
}
