using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightController : MonoBehaviour
{
    public float rollPower, pitchPower, yawPower, throttlePower;

    Rigidbody aircraftRB;

    public float rawThrottle;
    public float rawPitch;
    public float rawRoll;
    public float rawYaw;

    private float activeRoll, activePitch, activeYaw;

    public bool grounded;

    void Start()
    {
        rawThrottle = Input.GetAxis("Throttle");
        rawPitch = Input.GetAxis("Pitch");
        rawRoll = Input.GetAxis("Roll");
        aircraftRB = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        GetInputs();

        transform.position += transform.forward * rawThrottle * throttlePower * Time.deltaTime;

        

        activeRoll = Input.GetAxisRaw("Roll") * rollPower * Time.deltaTime;
        activePitch = Input.GetAxisRaw("Pitch") * pitchPower * Time.deltaTime;
        activeYaw = Input.GetAxisRaw("Yaw") * yawPower * Time.deltaTime;

        //Debug.Log(Input.GetButton("Brake"));

        //Debug.Log("Roll: " + activeRoll + " Pitch: " + activePitch + " Yaw: " + activeYaw + " Throttle: " + rawThrottle);

        transform.Rotate(activePitch, activeYaw, activeRoll);
    }

    void GetInputs()
    {
        //Debug.Log(Input.GetJoystickNames().Rank);
        if (!grounded)
        {
            rawThrottle = Mathf.Clamp(Input.GetAxis("Throttle"), 0.0f, 1.0f);
        }
        else
        {
            rawThrottle = Input.GetAxis("Throttle");
        }
        
        rawPitch = Input.GetAxis("Pitch");
        rawRoll = Input.GetAxis("Roll");
        rawYaw = Input.GetAxis("Yaw");
    }
}