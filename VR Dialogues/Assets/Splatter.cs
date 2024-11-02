using Oculus.Interaction;
using Oculus.Interaction.GrabAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splatter : MonoBehaviour
{
    public Grabbable grabbable;
    private bool grabbed = false;
    private bool splattered = false;
    private bool grabbing = false;
    public GameObject splatter;
    private GameObject player;
    Animator animator;
    float timer = .5f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("MainCamera");
        animator = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(grabbable.SelectingPointsCount > 0)
        {
            grabbing = true;
            grabbed = true;
        }
        else
        grabbing = false;
        RaycastHit hit;
        if (grabbing && Physics.Raycast(player.transform.position, player.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, 4))
        {
            timer = .5f;
            animator = hit.collider.gameObject.GetComponent<Animator>();
            animator.SetBool("Shocked", true);
            animator.SetBool("Terrified", true);
        }
        else if (!animator.Equals(this.gameObject.GetComponent<Animator>()))
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                animator.SetBool("Shocked", false);
                if (animator.GetBool("Sitting"))
                    animator.SetTrigger("sit");
                else
                    animator.SetTrigger("stand");
                animator.SetBool("Terrified", false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (grabbed && this.gameObject.GetComponent<Rigidbody>().velocity.magnitude > .7 && !splattered) 
        {
            splattered = true;
            if (!animator.Equals(this.gameObject.GetComponent<Animator>()))
            {
                animator.SetBool("Shocked", false);
                if (animator.GetBool("Sitting"))
                    animator.SetTrigger("sit");
                else
                    animator.SetTrigger("stand");
                animator.SetBool("Terrified", false);
            }
            if ((collision.gameObject.name == "Girl Avatar" || collision.gameObject.name == "Boy Avatar") && !animator.Equals(this.gameObject.GetComponent<Animator>()))
            {
                animator.SetTrigger("Angry");
                animator.SetBool("Mad", true);
                collision.gameObject.GetComponent<SitWhenSit>().startTimer = true;
            }
            Instantiate(splatter, this.transform.position, collision.gameObject.transform.rotation);
            Destroy(this.gameObject);
        }
    }
}