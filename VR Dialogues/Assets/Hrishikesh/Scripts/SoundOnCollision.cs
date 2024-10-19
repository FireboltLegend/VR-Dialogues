using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnCollision : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] AudioClip[] clips;


    private void OnCollisionEnter(Collision collision)
    {
        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.Play();
    }
}
