using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjects : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToDisable;
    [SerializeField] private MoveProps moveProps;
    private bool bringObjectsBack;

    void Update()
    {
        if (moveProps.moveToPosition2)
        {
            for (int i = 0; i < objectsToDisable.Length; i++)
                objectsToDisable[i].SetActive(false);
        }
        else
        {

            for (int i = 0; i < objectsToDisable.Length; i++)
            {
                objectsToDisable[i].SetActive(true);
            }
        }
    }
}
