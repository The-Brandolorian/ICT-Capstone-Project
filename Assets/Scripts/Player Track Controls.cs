using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerTrackCnntrols : MonoBehaviour
{
    [SerializeField] private float maximumSpeed;
    [SerializeField] private float acceleration;

    private SplineAnimate spline;
    private float speed;
    private float elapsedTime;

    // Start is called before the first frame update
    private void Start()
    {
        spline = GetComponentInParent<SplineAnimate>();
    }

    // Update is called once per frame
    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (speed < maximumSpeed) UpdateSpeed();
    }

    private void UpdateSpeed()
    {
        if (spline != null && Input.GetKey(KeyCode.W))
        {
            Debug.Log($"Increasing speed from {speed} ");
            speed += acceleration;
            Debug.Log($"to {speed}");
            
            spline.MaxSpeed = speed;
        }
        
        if (spline != null && Input.GetKey(KeyCode.S))
        {
            Debug.Log($"Decreasing speed from {speed} ");

            speed -= elapsedTime * (2f - elapsedTime);

            Debug.Log($"to {speed}");
            
            spline.MaxSpeed = speed;
        }

        if (spline != null && Input.GetKey(KeyCode.Space))
        {
            if (spline.IsPlaying)
            {
                Debug.Log("Pausing");
                spline.Pause();
            }
            
            else
            {
                Debug.Log("Playing");
                spline.Play();
            }
        }
    }
}
