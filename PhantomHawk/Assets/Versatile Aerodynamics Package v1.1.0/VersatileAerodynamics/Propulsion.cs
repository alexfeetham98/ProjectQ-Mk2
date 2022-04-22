using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple thrust producing engine framework for any propulsion system.
/// </summary>
public class Propulsion : MonoBehaviour
{
    [Tooltip("The thrust produced at max throttle.")]
    public float maxThrust;

    [Tooltip("Speed the engine spools up to the thrust setting.")]
    public float responsiveness = .2f;

    [Header("State of the engine; 0 : off | 1 : spooling up | 2 : on | 3 : spooling down")]
    public int state = 0;

    private float _currentThrust = 0f;
    /// <summary>
    /// the current thrust output of the engine.
    /// </summary>
    public float GetCurrentThrust { get { return _currentThrust; } }

    Rigidbody body;
    ControlMaster controller;

    void Start()
    {
        body = GetComponentInParent<Rigidbody>();

        if (body == null)
        {
            Debug.LogError("A Propulsion component can only be placed on an object whose parent contains a RigidBody.");
            Debug.Break();
        }

        controller = GetComponentInParent<ControlMaster>();

        if (controller == null)
        {
            Debug.LogError("A Propulsion component can only be placed on an object whose parent contains a ControlMaster component.");
            Debug.Break();
        }
    }

    private void Update()
    {
        float delta = 0;

        switch (state)
        {
            case 0:
                break;

            case 1:
                break;

            case 2:
                delta = controller.Throttle - _currentThrust;
                break;

            case 3:
                delta = 0 - _currentThrust;
                break;
        }

        if (delta * delta > .0001f)
        {
            _currentThrust += Mathf.Sign(delta) * responsiveness * Time.deltaTime;
        }
        else
        {
            _currentThrust += delta;
        }
    }
    void FixedUpdate()
    {
        Vector3 force = _currentThrust * maxThrust * transform.forward;
        body.AddForceAtPosition(force, transform.position);
    }
}
