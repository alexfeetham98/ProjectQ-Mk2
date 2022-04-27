using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SurfaceMovementProfile
{
    Smoothed,
    Linear
}

/// <summary>
/// manages the movement of the control surfaces and throttle coming from external inputs from users or ai.
/// </summary>

[RequireComponent(typeof(Aircraft))]
public class ControlMaster : MonoBehaviour
{

    #region Public Fields

    [Tooltip("Movement style of the control surfaces.")]
    public SurfaceMovementProfile movementProfile;

    [Header("Control Surface Parameters")]

    [Tooltip("Speed at which the control surfaces respond to input. Higher values create faster moving deflections.")]
    public float responsiveness = 0.5f;

    [Tooltip("Max force that can be used to deflect a surface. Low values will make the controls 'lock up' at high speeds.")]
    public float controlSurfaceForce = 1000f;

    [Tooltip("Max deflection angle in degrees.")]
    public float maxAileronDeflection = 20f;

    [Tooltip("Max deflection angle in degrees.")]
    public float maxElevatorDeflection = 20f;

    [Tooltip("Max deflection angle in degrees.")]
    public float maxRudderDeflection = 20f;

    #endregion

    #region Private Fields

    [Header("Control Inputs")]
    [SerializeField] private float pitch;
    [SerializeField] private float yaw;
    [SerializeField] private float roll;
    [SerializeField] private float throttle;

    float _elevatorPosition;
    float _rudderPosition;
    float _aileronsPosition;

    Aircraft airplane;

    ElevatorComponent[] _elevator;
    RudderComponent[] _rudder;
    LeftAileronComponent[] _leftAileron;
    RightAileronComponent[] _rightAileron;

    Aerodynamics[] _elevatorDynamics;
    Aerodynamics[] _rudderDynamics;
    Aerodynamics[] _leftAileronDynamics;
    Aerodynamics[] _rightAileronDynamics;

    #endregion

    #region Properties

    /// <summary>
    /// Set the pitch input for the elevators. 
    /// Returns the current pitch of the elevators.
    /// </summary>
    public float Pitch { set { pitch = Mathf.Clamp(value, -1f, 1f); } get { return _elevatorPosition; } }
    /// <summary>
    /// Set the yaw input for the rudders. 
    /// Returns the current pitch of the rudders.
    /// </summary>
    public float Yaw { set { yaw = Mathf.Clamp(value, -1f, 1f); } get { return _rudderPosition; } }
    /// <summary>
    /// Set the roll input for the ailerons. 
    /// Returns the current pitch of the ailerons.
    /// </summary>
    public float Roll { set { roll = Mathf.Clamp(value, -1f, 1f); } get { return _aileronsPosition; } }
    /// <summary>
    /// Set the throttle input for the aircraft. 
    /// Returns the current throttle setting.
    /// </summary>
    public float Throttle { set { throttle = Mathf.Clamp(value, 0f, 1f); } get { return throttle; } }

    #endregion

    private void Start()
    {
        airplane = GetComponent<Aircraft>();

        _elevator = GetComponentsInChildren<ElevatorComponent>();
        _rudder = GetComponentsInChildren<RudderComponent>();
        _leftAileron = GetComponentsInChildren<LeftAileronComponent>();
        _rightAileron = GetComponentsInChildren<RightAileronComponent>();
       
        _elevatorDynamics = new Aerodynamics[_elevator.Length];
        _rudderDynamics = new Aerodynamics[_rudder.Length];
        _leftAileronDynamics = new Aerodynamics[_leftAileron.Length];
        _rightAileronDynamics = new Aerodynamics[_rightAileron.Length];

        for (int i = 0; i < _elevator.Length; i++)
        {
            _elevatorDynamics[i] = _elevator[i].gameObject.GetComponent<Aerodynamics>();
        }
        for (int i = 0; i < _rudder.Length; i++)
        {
            _rudderDynamics[i] = _rudder[i].gameObject.GetComponent<Aerodynamics>();
        }
        for (int i = 0; i < _leftAileron.Length; i++)
        {
            _leftAileronDynamics[i] = _leftAileron[i].gameObject.GetComponent<Aerodynamics>();
        }
        for (int i = 0; i < _rightAileron.Length; i++)
        {
            _rightAileronDynamics[i] = _rightAileron[i].gameObject.GetComponent<Aerodynamics>();
        }
    }

    private void Update()
    {

        if (_elevator != null && _elevatorDynamics != null)
        {
            for (int i = 0; i < _elevator.Length; i++)
            {
                moveSurface(ref _elevatorPosition, pitch, maxElevatorDeflection, _elevator[i].transform, airplane, _elevatorDynamics[i]);
            }
        }

        if (_rudder != null && _rudderDynamics != null)
        {
            for (int i = 0; i < _rudder.Length; i++)
            {
                moveSurface(ref _rudderPosition, yaw, maxRudderDeflection, _rudder[i].transform, airplane, _rudderDynamics[i], Vector3.up);
            }
        }    

        if (_leftAileron != null && _leftAileronDynamics != null)
        {
            for (int i = 0; i < _leftAileron.Length; i++)
            {
                moveSurface(ref _aileronsPosition, roll, maxAileronDeflection, _leftAileron[i].transform, airplane, _leftAileronDynamics[i], -1f * Vector3.right);
            }
        }

        if (_rightAileron != null && _rightAileronDynamics != null)
        {
            for (int i = 0; i < _rightAileron.Length; i++)
            {
                moveSurface(ref _aileronsPosition, roll, maxAileronDeflection, _rightAileron[i].transform, airplane, _rightAileronDynamics[i]);
            } 
        }
    }

    /// <summary>
    /// Rotates control surfaces to a set position.
    /// </summary>
    /// <param name="actualPosition"></param>
    /// <param name="setPosition"></param>
    /// <param name="maxDeflection"></param>
    /// <param name="surface"></param>
    /// <param name="airplane"></param>
    /// <param name="surfaceDynamics"></param>
    /// <param name="rotationAxis"></param>
    void moveSurface(ref float actualPosition, float setPosition, float maxDeflection, Transform surface, Aircraft airplane, Aerodynamics surfaceDynamics, Vector3 rotationAxis)
    {
        float trueMaxDeflection = Mathf.Min(maxDeflection, Mathf.Rad2Deg * controlSurfaceForce 
            / Mathf.Clamp(airplane.RelativeVelocity.sqrMagnitude * airplane.fluid_density 
            * surfaceDynamics.planform_Area * Mathf.PI, 1f, Mathf.Infinity));

        switch(movementProfile)
        {
            case SurfaceMovementProfile.Smoothed:
                actualPosition = Mathf.Clamp(actualPosition + (-1f * actualPosition / trueMaxDeflection + 1f * setPosition)
                    * responsiveness * trueMaxDeflection * Time.deltaTime, -trueMaxDeflection, trueMaxDeflection);
                break;

            case SurfaceMovementProfile.Linear:
                actualPosition = Mathf.Clamp(actualPosition + Mathf.Sign(setPosition - actualPosition / trueMaxDeflection)
                    * Mathf.Min(responsiveness * 10f * Time.deltaTime,
                    Mathf.Abs(setPosition * trueMaxDeflection - actualPosition)), -trueMaxDeflection, trueMaxDeflection);
                break;
        }

        surface.localRotation = Quaternion.AngleAxis(actualPosition, rotationAxis);
    }

    void moveSurface(ref float actualPosition, float setPosition, float maxDeflection, Transform surface, Aircraft airplane, Aerodynamics surfaceDynamics)
    {
        moveSurface(ref actualPosition, setPosition, maxDeflection, surface, airplane, surfaceDynamics, Vector3.right);
    }
}