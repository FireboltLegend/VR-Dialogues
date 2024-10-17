using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToChange : MonoBehaviour
{
    static int currentNum;
    Vector3 originalPos;
    void Start()
    {
        originalPos = transform.position;
        currentNum = GetComponent<AvatarComponent>().num;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            transform.position = originalPos;
        }
    }
}
