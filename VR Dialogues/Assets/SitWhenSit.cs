using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitWhenSit : MonoBehaviour
{
    public GameObject player;
    public Animator animator;
    public Vector3 positionHigh;
    public Vector3 positionLow;

    // Update is called once per frame

    void Update()
    {
        if(player.transform.position.x > positionLow.x && player.transform.position.x < positionHigh.x && player.transform.position.y > positionLow.y && player.transform.position.y < positionHigh.y && player.transform.position.z > positionLow.z && player.transform.position.z < positionHigh.z)
        {
            animator.SetBool("Sitting", true);
        }
        else
        {
            animator.SetBool("Sitting", false);
        }
    }
}
