using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [SerializeField]
    public Transform focus;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Transform rig;
    [SerializeField]
    private Transform cam;
    [SerializeField]
    private float aimDistance = 500f;

    public float cameraSensitivity = 0.5f;
    public float cameraSmoothSpeed = 2f;
    public bool enableSmoothFollow = true;
    public float followSmoothSpeed = 10f;

    private float camX;
    private float camY;

    public Vector3 targetPosition
    {
        get
        {
            return target.position + (target.forward * aimDistance);
        }
    }
    public Vector3 ReticlePosition
    {
        get
        {
            return focus.position + (focus.forward * aimDistance);
        }
    }

    private void Update()
    {
        UpdatePosition();
        RotateRig();
        if (Input.GetKey(KeyCode.JoystickButton2))
        {
            ResetPosition();
        }
    }

    void RotateRig()
    {
        if (Input.GetKey(KeyCode.JoystickButton3)) //Dpad left - pan right
        {
            camX = cameraSensitivity * -1;
        }
        else if (Input.GetKey(KeyCode.JoystickButton5)) //Dpad right - pan left
        {
            camX = cameraSensitivity * 1;
        }
        else
        {
            camX = 0;
        }

        if (Input.GetKey(KeyCode.JoystickButton4)) //Dpad up - pan down
        {
            camY = cameraSensitivity * -1;
        }
        else if (Input.GetKey(KeyCode.JoystickButton6)) //Dpad down - pan up
        {
            camY = cameraSensitivity * 1;
        }
        else
        {
            camY = 0;
        }

        target.Rotate(cam.up, camX, Space.World);
        target.Rotate(cam.right, camY, Space.World);

        Vector3 up = (focus.parent == null) ? Vector3.up : focus.parent.up;

        rig.rotation = Damp(rig.rotation,
                            Quaternion.LookRotation(target.forward, up),
                            cameraSmoothSpeed,
                            Time.deltaTime);
    }

    void UpdatePosition()
    {
        if (focus != null)
        {
            if (enableSmoothFollow)
            {
                transform.position = Damp(transform.position,
                                      focus.position,
                                      followSmoothSpeed,
                                      Time.deltaTime);
            }
            else { transform.position = focus.position; }
        }
    }

    void ResetPosition()
    {
        target.transform.rotation = focus.transform.rotation;
    }

    private Quaternion Damp(Quaternion a, Quaternion b, float lambda, float dt)
    {
        return Quaternion.Slerp(a, b, 1f - Mathf.Exp(-lambda * dt));
    }

    private Vector3 Damp(Vector3 a, Vector3 b, float lambda, float dt)
    {
        return Vector3.Slerp(a, b, 1f - Mathf.Exp(-lambda * dt));
    }
}