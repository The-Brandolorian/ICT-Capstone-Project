using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerControls : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private bool bEngineIsOn = false;
    [SerializeField] private bool bLightsAreOn = false;
    [SerializeField] private bool bSoundingHorn = false;
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
    }

    private void GetPlayerInput()
    {
        // system controls
        if (Input.GetKeyDown(KeyCode.E)) ToggleEngine();
        if (Input.GetKeyDown(KeyCode.L)) ToggleLights();
        if (Input.GetKeyDown(KeyCode.H)) SoundHorn();
        if (Input.GetKeyDown(KeyCode.R)) Reset();

        // dev controls
        if (Input.GetKeyDown(KeyCode.B) && bDevMode) ToggleForcedBraking();

        // movement controls
        if (Input.GetKey(KeyCode.W) && bEngineIsOn && !bForcedBraking)
        {
            brakingSpeed = startingBrakingSpeed;

            acceleration = Mathf.Min(acceleration + startingAcceleration * accelerationFactor, maximumAcceleration);
            speed = Mathf.Min(speed + acceleration * Time.deltaTime, maximumSpeed);

            if (SoundManager.instance.GetLoopingSoundClip("rail") == null) SoundManager.instance.PlayLoopingSoundClip("rail", railSoundClip, transform, true, 0.5f, 0.25f);

            AudioSource engineSource = SoundManager.instance.GetLoopingSoundClip("engine");
            engineSource.volume = Mathf.Min(engineSource.volume + speed / 100, 0.6f);

            AudioSource railSource = SoundManager.instance.GetLoopingSoundClip("rail");
            railSource.volume = Mathf.Min(railSource.volume + speed / 100, 1f);
        }

        if (Input.GetKey(KeyCode.S) || bForcedBraking)
        {
            acceleration = startingAcceleration;

            brakingSpeed = Mathf.Min(brakingSpeed + startingBrakingSpeed * brakingFactor, maximumBrakingSpeed);
            speed = Mathf.Max(speed - brakingSpeed * Time.deltaTime, 0);

            if (!bIsBreaking)
            {
                SoundManager.instance.PlaySoundClip(breakingSoundClip, transform, true, 0.7f);
                bIsBreaking = !bIsBreaking;
            }

            AudioSource source = SoundManager.instance.GetLoopingSoundClip("engine");
            source.volume = Mathf.Max(source.volume - speed / 75, 0.25f);

            AudioSource railSource = SoundManager.instance.GetLoopingSoundClip("rail");
            railSource.volume = Mathf.Min(railSource.volume - speed / 100, 1f);
        }
    }

    private void ToggleEngine()
    {
        bEngineIsOn = !bEngineIsOn;

        if (bEngineIsOn)
            SoundManager.instance.PlayLoopingSoundClip("engine", engineSoundClip, transform, true, 1f, 0.25f);

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
        if (bEngineIsOn) ToggleEngine();
        if (bLightsAreOn) ToggleLights();
        if (bSoundingHorn) SoundHorn();
        if (bForcedBraking) ToggleForcedBraking();

        spline.ElapsedTime = 0;
        speed = 0;
        acceleration = startingAcceleration;
        brakingSpeed = startingBrakingSpeed;
    }
}
