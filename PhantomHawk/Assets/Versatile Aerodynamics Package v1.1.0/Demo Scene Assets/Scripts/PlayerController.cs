using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ControlMaster))]
public class PlayerController : MonoBehaviour
{
    ControlMaster airplaneController;

    public float throttleSensitivity = .05f;
    public bool vtol = true;

    private void Start()
    {
        airplaneController = GetComponent<ControlMaster>();
    }

    //full flaps btn 9
    //mid flaps btn 10
    //no flaps btn 11
    //btn 1 vtol toggle
    //btn 0 brake
    //axis 6 horizontal btn
    //axis 7 vertical btn
    //dpad L3 R5 U4 D6
    //votl engine btn 2

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            vtol = vtol ? false : true;
        }

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