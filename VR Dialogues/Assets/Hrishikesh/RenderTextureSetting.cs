using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureSetting : MonoBehaviour
{
    [SerializeField] private RenderTexture[] renderTextures;
    [SerializeField] private VRTextureUsage[] vRTextureUsages;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < renderTextures.Length; i++)
        {
            renderTextures[i].vrUsage = vRTextureUsages[i];
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
