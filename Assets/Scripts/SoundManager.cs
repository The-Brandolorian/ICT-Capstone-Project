using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioSource soundObject;
    [SerializeField] private Dictionary<string, AudioSource> loopingSoundClips;

    private void Start()
    {
        loopingSoundClips = new Dictionary<string, AudioSource>();
    }

    private void Awake()
    {
        // make class singleton
        instance = this;
    }

    public void PlaySoundClip(AudioClip clip, Transform spawnTransform, bool bFade = false, float fadeTime = 0.5f, float volume = 1f)
    {
        // create sound object
        AudioSource source = Instantiate(soundObject, spawnTransform.position, Quaternion.identity);

        // play sound
        source.clip = clip;

        if (bFade)
        {
            source.volume = 0.1f;
            StartCoroutine(FadeIn(source, fadeTime, volume));
        }
        else
        {
            source.volume = volume;
            source.Play();
        }

        // destroy sound object
        Destroy(source.gameObject, source.clip.length);
    }

    public void PlayLoopingSoundClip(string name, AudioClip clip, Transform spawnTransform, bool bFade = false, float fadeTime = 0.5f, float volume = 1f)
    {
        // create sound object
        AudioSource source = Instantiate(soundObject, spawnTransform.position, Quaternion.identity);
        loopingSoundClips.Add(name, source);

        // play sound
        source.clip = clip;
        source.loop = true;

        if (bFade)
        {
            source.volume = 0.1f;
            StartCoroutine(FadeIn(source, fadeTime, volume));
        }
        else
        {
            source.volume = volume;
            source.Play();
        }
    }

    public AudioSource GetLoopingSoundClip(string name)
    {
        AudioSource source;
        loopingSoundClips.TryGetValue(name, out source);

        return source;
    }

    public void StopLoopingSoundClip(string name, bool bFade = false, float fadeTime = 0.5f)
    {
        // get sound from looping sounds
        AudioSource source;
        loopingSoundClips.TryGetValue(name, out source);

        // remove sound
        loopingSoundClips.Remove(name);

        if (bFade) StartCoroutine(FadeOut(source, fadeTime, 0, true));
        else
        {
            source?.Stop();
            Destroy(source?.gameObject);
        }
    }

    public static IEnumerator FadeIn(AudioSource source, float fadeTime, float targetVolume)
    {
        float startVolume = 0.2f;
        source.Play();

        while (source != null && source.volume < targetVolume)
        {
            source.volume += startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        if (source != null) source.volume = targetVolume;
    }

    public static IEnumerator FadeOut(AudioSource source, float fadeTime, float targetVolume, bool bDestroy = false)
    {
        float startVolume = source.volume;

        while (source.volume > targetVolume + 0.002f)
        {
            source.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        source.volume = targetVolume;

        source.Stop();
        if (bDestroy) Destroy(source.gameObject);
    }
}
