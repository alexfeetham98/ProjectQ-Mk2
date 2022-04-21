using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBFlight : MonoBehaviour
{
    public float rollTorque, pitchTorque, yawTorque, throttlePower;
    private float activeRoll, activePitch, activeYaw;

    Rigidbody aircraft;
    Vector3 moveDir;
    public bool grounded;

    [SerializeField]
    private float rawRoll, rawPitch, rawYaw, rawThrottle;

    private void Start()
    {
        aircraft = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveDir = new Vector3(transform.position.x, transform.position.y, rawThrottle).normalized;
    }

    private void FixedUpdate()
    {
        GetInputs();

        transform.position += transform.forward * rawThrottle * throttlePower * Time.deltaTime;

        activeRoll = Input.GetAxisRaw("Roll") * rollTorque * Time.deltaTime;
        activePitch = Input.GetAxisRaw("Pitch") * pitchTorque * Time.deltaTime;
        activeYaw = Input.GetAxisRaw("Yaw") * yawTorque * Time.deltaTime;

        Movement(moveDir);

        //aircraft.AddRelativeTorque(Vector3.up * yawTorque * rawYaw,mode);
        aircraft.AddTorque(Vector3.up * yawTorque * rawYaw);

        
    }

    private void Movement(Vector3 direction)
    {
        //aircraft.velocity = direction * throttlePower * Time.deltaTime;
        aircraft.MovePosition(direction * Time.deltaTime * throttlePower);
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
