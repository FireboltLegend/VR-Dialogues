using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SitWhenSit : MonoBehaviour
{
    public GameObject player;
    public Animator animator;
    public Vector3 positionHigh;
    public Vector3 positionLow;
    public float timer = 0f;
    public bool startTimer = false;
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
        if (startTimer)
        {
            timer += Time.deltaTime;
            if (timer > 10f)
            {
                startTimer = false;
                timer = 0;
                this.gameObject.GetComponent<Animator>().SetBool("Mad", false);
            }
        }
    }
}
