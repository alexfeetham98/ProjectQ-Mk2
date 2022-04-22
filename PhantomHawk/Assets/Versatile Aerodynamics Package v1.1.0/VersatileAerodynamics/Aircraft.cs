using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component specifying an airplane object.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Aircraft : MonoBehaviour
{

    Rigidbody body;

    [Header("Airframe Properties")]

    [Tooltip("A user defined center of mass. You should always specify this to ensure stability of the airplane.")]
    public Vector3 newCenterOfMass;

    [Tooltip("Scales the amount of torque generated from roll. Decrease if you desire a faster roll rate and vice versa.")]
    public float rollTorqueMultiplier = 1f;


    [Header("Aerodynamic Flow Properties")]

    [Tooltip("the fluid/air density applied to all aerodynamic surfaces belonging to this aircraft.")]
    public float fluid_density = .5f;

    [Tooltip("The wind vector applied to this aircraft")]
    public Vector3 wind;


    private Vector3 relativeVelocity;
    /// <summary>
    /// Relative velocity to the vector of the airflow, or combined ground speed and wind speed.
    /// </summary>
    public Vector3 RelativeVelocity
    {
        get
        {
            return relativeVelocity;
        }
    }

    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.centerOfMass = newCenterOfMass;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.TransformPoint(newCenterOfMass), .25f);
    }

    void FixedUpdate()
    {
        Update_Relative_Velocity();
    }

    void Update_Relative_Velocity()
    {
        relativeVelocity = wind - body.velocity;
    }
}
