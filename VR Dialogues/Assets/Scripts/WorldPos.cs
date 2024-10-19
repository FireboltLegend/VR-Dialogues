using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldPos : MonoBehaviour
{
    Vector3 originalPos;
    
    void Start()
    {
        originalPos = transform.position;
    }

    public Vector3 getOriginalPos()
    {
        return originalPos;
    }
}
