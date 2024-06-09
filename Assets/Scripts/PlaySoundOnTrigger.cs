using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour
{
    [SerializeField] AudioSource source;

    void OnTriggerEnter(Collider collider)
    {
        source.Play();
    }

    void OnTriggerExit(Collider collider)
    {
        source.Stop();
    }
}
