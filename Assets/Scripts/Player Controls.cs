using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class PlayerControls : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private bool bEngineIsOn = false;
    [SerializeField] private bool bLightsAreOn = false;
    [SerializeField] private bool bIsBreaking = false;
    [SerializeField] private bool bForcedBraking = false;
    [SerializeField] private bool bDevMode = false;

    [Header("Movement")]
    [SerializeField] private SplineAnimate spline;
    [SerializeField] private float speed = 0;
    [SerializeField] private float maximumSpeed = 0.5f;

    [SerializeField] private float acceleration = 0.01f;
    [SerializeField] private float maximumAcceleration = 1f;
    [SerializeField] private float accelerationFactor = 0.01f;

    [SerializeField] private float brakingSpeed = 0.05f;
    [SerializeField] private float maximumBrakingSpeed = 0.25f;
    [SerializeField] private float brakingFactor = 0.05f;
    [SerializeField] private float gravityBreakingFactor = 0.05f;

    [Header("Sounds")]
    [SerializeField] private AudioClip hornSoundClip;
    [SerializeField] private AudioClip engineSoundClip;
    [SerializeField] private AudioClip breakingSoundClip;
    [SerializeField] private AudioClip railSoundClip;

    private float startingAcceleration;
    private float startingBrakingSpeed;
    private Light[] lights;
    private float hornWaitTime;
    private AudioSource breakingSource;
    private bool bPlayingRailSound = false;

    // Start is called before the first frame update
    private void Start()
    {
        // spline to follow
        spline = GetComponentInParent<SplineAnimate>();

        // store initial values
        startingAcceleration = acceleration;
        startingBrakingSpeed = brakingSpeed;

        // setup lights
        lights = GetComponentsInChildren<Light>();
        foreach (var light in lights)
        {
            if (light) light.enabled = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        GetPlayerInput();

        if (spline != null)
        {
            // gradually slow down if the engine is shut off
            if (!bEngineIsOn) speed = Mathf.Max(speed - gravityBreakingFactor * Time.deltaTime, 0);

            // move based on current speed
            spline.ElapsedTime += speed;
        }

        // no longer moving
        if (speed <= 0)
        {
            if (breakingSource && breakingSource.isPlaying) breakingSource.Stop();
            if (bPlayingRailSound) SoundManager.instance.StopLoopingSoundClip("rail", true);
            bPlayingRailSound = false;
        }
    }

    private void GetPlayerInput()
    {
        // system controls
        if (Input.GetKeyDown(KeyCode.E)) ToggleEngine();
        if (Input.GetKeyDown(KeyCode.L)) ToggleLights();
        if (Input.GetKeyDown(KeyCode.H)) SoundHorn();
        if (Input.GetKeyDown(KeyCode.R)) Reset();
        if (Input.GetKeyDown(KeyCode.Escape)) Exit();

        // dev controls
        if (Input.GetKeyDown(KeyCode.B) && bDevMode) ToggleForcedBraking();

        // movement controls
        if (Input.GetKey(KeyCode.W) && bEngineIsOn && !bForcedBraking)
        {
            // reset braking
            brakingSpeed = startingBrakingSpeed;
            bIsBreaking = false;

            // calculate speed
            acceleration = Mathf.Min(acceleration + startingAcceleration * accelerationFactor, maximumAcceleration);
            speed = Mathf.Min(speed + acceleration * Time.deltaTime, maximumSpeed);

            // play rail sound if not already
            if (!bPlayingRailSound && SoundManager.instance.GetSoundClip("rail") == null)
            {
                SoundManager.instance.PlayLoopingSoundClip("rail", railSoundClip, transform, true, 0.5f, 0.1f);
                bPlayingRailSound = !bPlayingRailSound;
            }

            // increase volumes
            AudioSource engineSource = SoundManager.instance.GetSoundClip("engine");
            engineSource.volume = Mathf.Min(engineSource.volume + speed / 100, 0.2f);

            AudioSource railSource = SoundManager.instance.GetSoundClip("rail");
            railSource.volume = Mathf.Min(railSource.volume + speed / 100, 1f);
        }

        if (Input.GetKey(KeyCode.S) || bForcedBraking)
        {
            // reset acceleration
            acceleration = startingAcceleration;

            // calculate speed
            brakingSpeed = Mathf.Min(brakingSpeed + startingBrakingSpeed * brakingFactor, maximumBrakingSpeed);
            speed = Mathf.Max(speed - brakingSpeed * Time.deltaTime, 0);

            // initiate breaking
            if (!bIsBreaking)
            {
                breakingSource = SoundManager.instance.PlaySoundClip(breakingSoundClip, transform);
                bIsBreaking = !bIsBreaking;
            }

            // decrease volumes
            if (bEngineIsOn)
            {
                AudioSource source = SoundManager.instance.GetSoundClip("engine");
                source.volume = Mathf.Max(source.volume - speed / 75, 0.25f);
            }

            if (bPlayingRailSound)
            {
                AudioSource railSource = SoundManager.instance.GetSoundClip("rail");
                railSource.volume = Mathf.Min(railSource.volume - speed / 100, 1f);
            }
        }
    }

    private void ToggleEngine()
    {
        bEngineIsOn = !bEngineIsOn;

        if (bEngineIsOn)
            SoundManager.instance.PlayLoopingSoundClip("engine", engineSoundClip, transform, true, 1f, 0.01f);

        else
            SoundManager.instance.StopLoopingSoundClip("engine", true, 0.7f);
    }

    private void ToggleLights()
    {
        bLightsAreOn = !bLightsAreOn;

        foreach (var light in lights)
        {
            if (light) light.enabled = !light.enabled;
        }
    }

    private void SoundHorn()
    {
        if(hornWaitTime < Time.time)
        { 
            hornWaitTime = Time.time + hornSoundClip.length;
            SoundManager.instance.PlaySoundClip(hornSoundClip, transform, true, 0.7f);
        }
    }

    private void ToggleForcedBraking()
    {
        bForcedBraking = !bForcedBraking;
    }

    private void Reset()
    {
        SceneManager.LoadScene(1);
    }

    private void Exit()
    {
        SceneManager.LoadScene(0);
    }
}
