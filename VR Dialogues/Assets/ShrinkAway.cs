using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkAway : MonoBehaviour
{
    public GameObject fruit;
    public GameObject secretFruit;
    // Start is called before the first frame update
    private void Start()
    {
        if(Random.Range(0, 1) > .8 && fruit.name == "orange")
        {
            fruit = secretFruit;
        }
    }
    void whenMaxSmall()
    {
        Destroy(this.gameObject);
        Instantiate(fruit);
    }
}
