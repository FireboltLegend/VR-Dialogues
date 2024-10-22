using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splatter : MonoBehaviour
{
    public Grabbable grabbable;
    private bool grabbed = false;
    private bool splattered = false;
    public GameObject splatter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(grabbable.SelectingPointsCount > 0)
        {
            grabbed = true;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (grabbed && this.gameObject.GetComponent<Rigidbody>().velocity.magnitude > .7 && !splattered) 
        {
            splattered = true;
            Instantiate(splatter, this.transform.position, collision.gameObject.transform.rotation);
            Destroy(this.gameObject);
        }
    }
}