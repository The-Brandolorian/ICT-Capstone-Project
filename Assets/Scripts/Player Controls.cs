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

    private float startingAcceleration;
    private float startingBrakingSpeed;
    private Light[] lights;

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
        if (Input.GetKeyDown(KeyCode.H)) ToggleHorn();
        if (Input.GetKeyDown(KeyCode.R)) Reset();

        // dev controls
        if (Input.GetKeyDown(KeyCode.B) && bDevMode) ToggleForcedBraking();

        // movement controls
        if (Input.GetKey(KeyCode.W) && bEngineIsOn && !bForcedBraking)
        {
            brakingSpeed = startingBrakingSpeed;

            acceleration = Mathf.Min(acceleration + startingAcceleration * accelerationFactor, maximumAcceleration);
            speed = Mathf.Min(speed + acceleration * Time.deltaTime, maximumSpeed);
        }

        if (Input.GetKey(KeyCode.S) || bForcedBraking)
        {
            acceleration = startingAcceleration;

            brakingSpeed = Mathf.Min(brakingSpeed + startingBrakingSpeed * brakingFactor, maximumBrakingSpeed);
            speed = Mathf.Max(speed - brakingSpeed * Time.deltaTime, 0);
        }
    }

    private void ToggleEngine()
    {
        bEngineIsOn = !bEngineIsOn;
        // todo play sound
    }

    private void ToggleLights()
    {
        bLightsAreOn = !bLightsAreOn;

        foreach (var light in lights)
        {
            if (light) light.enabled = !light.enabled;
        }
    }

    private void ToggleHorn()
    {
        bSoundingHorn = !bSoundingHorn;
        // todo play sound
    }

    private void ToggleForcedBraking()
    {
        bForcedBraking = !bForcedBraking;
    }

    private void Reset()
    {
        if (bEngineIsOn) ToggleEngine();
        if (bLightsAreOn) ToggleLights();
        if (bSoundingHorn) ToggleHorn();
        if (bForcedBraking) ToggleForcedBraking();

        spline.ElapsedTime = 0;
        speed = 0;
        acceleration = startingAcceleration;
        brakingSpeed = startingBrakingSpeed;
    }
}
