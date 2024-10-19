using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenChange : MonoBehaviour
{
    [SerializeField] private GameObject screenParent;
    [SerializeField] private GameObject[] screens;
    private int currentScreen;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 1)
        {
            Destroy(screenParent.GetComponentInChildren<Transform>().gameObject);
            GameObject newScreen = Instantiate(screens[currentScreen++], screenParent.transform);
            newScreen.transform.parent = screenParent.transform;
            if (currentScreen >= screens.Length) 
                currentScreen = 0;
        }
    }
}
