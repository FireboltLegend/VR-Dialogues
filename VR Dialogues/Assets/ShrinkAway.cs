using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkAway : MonoBehaviour
{
    public GameObject fruit;
    // Start is called before the first frame update
    void whenMaxSmall()
    {
        Instantiate(fruit);
        Destroy(this);
    }
}
