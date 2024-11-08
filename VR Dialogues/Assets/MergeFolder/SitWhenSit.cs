using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class SitWhenSit : MonoBehaviour
{
    public GameObject propMove1;
    public GameObject propMove2;
    public GameObject player;
    public float boundary;
    public float timer = 0f;
    public bool startTimer = false;
    private bool animationLock;
    // Update is called once per frame

    void Update()
    {
        if(propMove1.GetComponent<MoveProps>().moveToPosition2 || propMove2.GetComponent<MoveProps>().moveToPosition2)
        {
            this.gameObject.GetComponent<Animator>().SetBool("Sitting", false);
            animationLock = true;
        }
        else if(player.transform.localPosition.y < boundary)
        {
            this.gameObject.GetComponent<Animator>().SetBool("Sitting", true);
            animationLock = false;
        }
        else
        {
            this.gameObject.GetComponent<Animator>().SetBool("Sitting", false);
            animationLock = false;
        }
        if (startTimer)
        {
            timer += Time.deltaTime;
            if (timer > 10f && !animationLock)
            {
                startTimer = false;
                timer = 0;
                this.gameObject.GetComponent<Animator>().SetBool("Mad", false);
            }
        }
    }
}
