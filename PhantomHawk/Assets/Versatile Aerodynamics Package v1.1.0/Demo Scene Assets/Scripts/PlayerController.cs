using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ControlMaster))]
public class PlayerController : MonoBehaviour
{
    ControlMaster airplaneController;

    public float throttleSensitivity = .05f;


    private void Start()
    {
        airplaneController = GetComponent<ControlMaster>();
    }

    void Update()
    {
        
        if (airplaneController != null)
        {
            if (Input.GetJoystickNames() != null)
            {
                airplaneController.Pitch = Input.GetAxisRaw("Pitch");
                airplaneController.Roll = Input.GetAxisRaw("Roll");
                airplaneController.Yaw = Input.GetAxisRaw("Yaw");
                airplaneController.Throttle = Input.GetAxisRaw("Throttle");
            }
            else
            {
                airplaneController.Pitch = Input.GetAxisRaw("Vertical");
                airplaneController.Roll = -Input.GetAxisRaw("Horizontal");
                airplaneController.Yaw = (Input.GetKey(KeyCode.Q) ? 1f : 0f) + (Input.GetKey(KeyCode.E) ? -1f : 0f);
                airplaneController.Throttle += Input.mouseScrollDelta.y * throttleSensitivity;
            }
        }
    }
}