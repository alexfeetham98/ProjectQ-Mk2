using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAim : MonoBehaviour
{
    [SerializeField]
    public Transform focus;
    [SerializeField]
    private Transform mouseAim;
    [SerializeField]
    private Transform rig;
    [SerializeField]
    private Transform cam;
    [SerializeField]
    private float aimDistance = 500f;

    public float mouseSensitivity = 2f;
    public float cameraSmoothSpeed = 2f;
    public bool enableSmoothFollow = true;
    public float followSmoothSpeed = 10f;

    public Vector3 MouseAimPosition
    {
        get
        {
            return mouseAim.position + (mouseAim.forward * aimDistance);
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
    }

    void RotateRig()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivity;

        mouseAim.Rotate(cam.up, mouseX, Space.World);
        mouseAim.Rotate(cam.right, mouseY, Space.World);

        Vector3 up = (focus.parent == null) ? Vector3.up : focus.parent.up;

        rig.rotation = Damp(rig.rotation,
                            Quaternion.LookRotation(mouseAim.forward, up),
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

    private Quaternion Damp(Quaternion a, Quaternion b, float lambda, float dt)
    {
        return Quaternion.Slerp(a, b, 1f - Mathf.Exp(-lambda * dt));
    }

    private Vector3 Damp(Vector3 a, Vector3 b, float lambda, float dt)
    {
        return Vector3.Slerp(a, b, 1f - Mathf.Exp(-lambda * dt));
    }
}
