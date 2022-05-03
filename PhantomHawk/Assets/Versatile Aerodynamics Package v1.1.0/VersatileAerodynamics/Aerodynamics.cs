using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LiftProfile
{ 
    Logarithmic,
    Linear
}

public class Aerodynamics : MonoBehaviour
{

    #region Public Fields

    [Header("Airframe Properties")]
    [Tooltip("The point where aerodynamic force is applied. Sometimes called 'center of lift'.")]
    public Vector3 aerodynamic_Center;

    [Tooltip("Wing area seen from above.")]
    public float planform_Area;

    [Tooltip("Wing area seen from the side.")]
    public float side_Area;

    [Tooltip("Ratio of the square of the wingspan and the wing area. Important for induced drag and roll characteristics.")]
    public float aspectRatio = 1f;


    [Header("Lift Parameters")]

    [Tooltip("Scales the lift outside default parameters.")]
    public float liftMultiplier = 1.0f;

    // change to degrees?
    [Tooltip("Angle where lift begins to drop off, simulating airflow seperation.")]
    public float stallThreshold = 19.5f;

    [Tooltip("Profile of the lift drop-off. Logarithmic gives a more severe and noticable stall. Linear gives a more gentle stall if any at all.")]
    public LiftProfile liftDropOff;


    [Header("Drag Parameters")]

    [Tooltip("Scales the drag resulting from viscous air resistance, usually the smallest source of drag. Increase to simulate un-areodynamic objects, a.k.a. a fuselage.")]
    public float weight_Air_Friction = .02f;

    [Tooltip("Scales the drag resulting from the angle of attack. Follows a hyperbolic curve, meaning as the a.o.a. increases this type of drag increases slowly then very rapidly then slowly again. lower this value if your plane is struggling to take off.")]
    public float weight_Inclination = 1f;

    [Tooltip("Scales the drag resulting from wingtip vortices, highly dependent on the aspect ratio.")]
    public float weight_Induced = .8f;

    [Tooltip("Efficiency derived from lift distribution from tip to tip. Optimal value of 1.0 when the distribution is elliptical, like a Spitfire, 0.7 for a rectangular wing and never 0.")]
    public float efficiency = .7f;

    [Header("Show Forces In Editor")]
    public bool showForces;

    #endregion

    #region Private Fields

    private Rigidbody body;
    private Aircraft airplane;
    private const float alpha = 9.5f;
    private const float beta = 4f;

    #endregion

    #region Properties

    private Vector3 showLift;
    /// <summary>
    /// Returns the current lift vector on the aerodynamic surface
    /// </summary>
    public Vector3 GetLift { get { return showLift; } }

    private Vector3 showDrag;
    /// <summary>
    /// Returns the current net drag vector on the aerodynamic surface
    /// </summary>
    public Vector3 GetDrag { get { return showDrag; } }

    #endregion

    private void Start()
    {
        body = GetComponentInParent<Rigidbody>();

        if (body == null)
        {
            Debug.LogError("An Aerodynamics component can only be placed on an object whose parent contains a RigidBody.");
            Debug.Break();
        }

        airplane = GetComponentInParent<Aircraft>();

        if (airplane == null)
        {
            Debug.LogError("An Aerodynamics component can only be placed on an object whose parent contains an Airplane component.");
            Debug.Break();
        }
    }

    void FixedUpdate()
    {
        (Vector3 lift, Vector3 drag) = Aerodynamic_Force(airplane.RelativeVelocity, airplane.fluid_density);
        showLift = lift;
        showDrag = drag;
        Vector3 netForce = lift + drag;
        body.AddForceAtPosition(netForce, transform.TransformPoint(aerodynamic_Center));

        Vector3 torque = Roll_Torque(airplane.transform.forward, body.angularVelocity, airplane.RelativeVelocity, airplane.fluid_density, airplane.rollTorqueMultiplier);
        body.AddRelativeTorque(torque);
    }

    private void OnDrawGizmos()
    {
        if (showForces)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.TransformPoint(aerodynamic_Center), transform.TransformPoint(aerodynamic_Center) + .1f * showDrag);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.TransformPoint(aerodynamic_Center), transform.TransformPoint(aerodynamic_Center) + .1f * showLift);
        }
    }
    
    public (Vector3, Vector3) Aerodynamic_Force(Vector3 relativeVelocity, float fluidDensity)
    {
        Vector3 u = Vector3.ProjectOnPlane(relativeVelocity, transform.right);
        Vector3 v = Vector3.ProjectOnPlane(relativeVelocity, transform.up);
        Vector3 w = Vector3.ProjectOnPlane(relativeVelocity, transform.forward);

        (float inclination, float declination, float roll) = ComputeAngles(relativeVelocity);

        // calculate forces

        Vector3 lift = Vector3.Dot(transform.forward, (-1 * u)) * Vector3.Cross(transform.right, u);
        lift = Cl(inclination) * fluidDensity * planform_Area
            * u.sqrMagnitude / 2f * lift.normalized;

        Vector3 drag = Cd(inclination, aspectRatio) * fluidDensity * planform_Area
            * u.sqrMagnitude / 2f * relativeVelocity.normalized;

        // calculate forces that are on the horizontal plane of the airframe

        Vector3 vlift = Vector3.Dot(transform.forward, (-1 * v)) * Vector3.Cross(transform.up, v);
        vlift = Cl(declination) * fluidDensity * side_Area
            * v.sqrMagnitude / 2f * vlift.normalized;

        Vector3 vdrag = Cd(declination, aspectRatio) * fluidDensity * side_Area
            * v.sqrMagnitude / 2f * relativeVelocity.normalized;

        //calculate forces from lateral aerodynamic flow

        Vector3 wlift = Vector3.Dot(transform.right, (-1 * w)) * Vector3.Cross(-transform.forward, w);
        wlift = Cl(roll) * fluidDensity * planform_Area
            * w.sqrMagnitude / 2f * wlift.normalized;

        Vector3 wdrag = Cd(roll, (1 / aspectRatio)) * fluidDensity * planform_Area
            * w.sqrMagnitude / 2f * relativeVelocity.normalized;

        return (lift + vlift + wlift, drag + vdrag + wdrag);
    }


    private Vector3 Roll_Torque(Vector3 forward, Vector3 rbAngularVelocity, Vector3 relativeVelocity, float fluidDensity, float multiplier)
    {
        float torque;
        float angularVelocity = Vector3.Dot(forward, rbAngularVelocity);

        if (angularVelocity == 0 || relativeVelocity.magnitude == 0)
        {
            torque = 0f;
        }
        else
        {
            Vector3 u = Vector3.ProjectOnPlane(relativeVelocity, transform.right);

            float _beta = angularVelocity / relativeVelocity.magnitude;

            float upperBound = Mathf.Sqrt(transform.localPosition.x * transform.localPosition.x + transform.localPosition.y * transform.localPosition.y) 
                + Mathf.Sqrt(aspectRatio * planform_Area) / 2;
            float lowerBound = Mathf.Sqrt(transform.localPosition.x * transform.localPosition.x + transform.localPosition.y * transform.localPosition.y)
                - Mathf.Sqrt(aspectRatio * planform_Area) / 2;

            float integralUpperBound = multiplier * Mathf.PI * fluidDensity * u.sqrMagnitude * 
                (Mathf.Sqrt(aspectRatio * planform_Area) / aspectRatio) * ((upperBound * upperBound * _beta * _beta + 1) * Mathf.Atan(upperBound * _beta) - upperBound * _beta) /
                (2 * _beta * _beta);
            float integralLowerBound = multiplier * Mathf.PI * fluidDensity * u.sqrMagnitude *
                (Mathf.Sqrt(aspectRatio * planform_Area) / aspectRatio) * ((lowerBound * lowerBound * _beta * _beta + 1) * Mathf.Atan(lowerBound * _beta) - lowerBound * _beta) /
                (2 * _beta * _beta);

            torque = integralUpperBound - integralLowerBound;
        }

        return (Vector3.forward * -torque);
    }
    
    /// <summary>
    /// Calculates the coefficient of lift
    /// </summary>
    /// <param name="inclination">the inclination of the surface to the airflow</param>
    /// <returns>The lift coefficient</returns>
    float Cl(float inclination)
    {
        
        float i = Mathf.Abs(inclination);
        float a = 2f * Mathf.PI * i;
        float b = 0;

        float _stallThreshold = stallThreshold * Mathf.Deg2Rad;

        switch(liftDropOff)
        {
            case LiftProfile.Logarithmic:
                b = Mathf.Log10(Mathf.PI / 2f - _stallThreshold) - Mathf.Log10(Mathf.Max(i, _stallThreshold + .000001f) - _stallThreshold);
                break;

            case LiftProfile.Linear:
                b = (2 * Mathf.PI * _stallThreshold / (1 - 2 * _stallThreshold / Mathf.PI)) * (1 - 2 * i / Mathf.PI);
                break;
        }

        float Cl = SmoothMin(a, b, .15f);

        if (Cl < 0) { Cl = 0; } 

        return liftMultiplier * Cl * Mathf.Sign(inclination);
    }

    /// <summary>
    /// Calculates the Coefficient of drag from the inclination and aspect ratio.
    /// </summary>
    /// <param name="inclination"></param>
    /// <param name="AR"></param>
    /// <returns></returns>
    float Cd(float inclination, float AR)
    {
        return Mathf.Clamp01(weight_Air_Friction + weight_Inclination / 2f * (tanh(alpha * Mathf.Abs(inclination) - beta) + 1f)
            + weight_Induced * Cl(inclination) * Cl(inclination) / Mathf.PI / AR / efficiency);
    }

    (float, float, float) ComputeAngles(Vector3 relativeVelocity)
    {
        float inclination = .5f * Mathf.PI - Mathf.Deg2Rad * Vector3.Angle(transform.up, Vector3.ProjectOnPlane(relativeVelocity, transform.right));

        float declination = Mathf.Deg2Rad * Vector3.Angle(transform.right, Vector3.ProjectOnPlane(relativeVelocity, transform.up)) - .5f * Mathf.PI;

        float roll = .5f * Mathf.PI - Mathf.Deg2Rad * Vector3.Angle(transform.up, Vector3.ProjectOnPlane(relativeVelocity, transform.forward));

        return (inclination, declination, roll);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">angle x in radians</param>
    /// <returns>The hyperbolic tangent of x.</returns>
    static float tanh(float x)
    {
        float n = (float)System.Math.Tanh((double)x);
        return n;
    }
    /// <summary>
    /// the smoothed minumum of functions a and b in window k.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="k"></param>
    /// <returns></returns>
    static float SmoothMin(float a, float b, float k)
    {
        float h = Mathf.Clamp01((b - a + k) / (2 * k));
        return a * h + b * (1 - h) - k * h * (1 - h);
    }
}