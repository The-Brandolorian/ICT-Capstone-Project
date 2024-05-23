using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixer : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    public void SetMasterVolume(float level)
    {
        mixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
    }

    public void SetFXVolume(float level)
    {
        mixer.SetFloat("FXVolume", Mathf.Log10(level) * 20f);
    }
}
