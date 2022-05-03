using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ControlMaster))]
public class PlayerController : MonoBehaviour
{
    ControlMaster airplaneController;
    Rigidbody rb;

    public float throttleSensitivity = .05f;
    public bool vtol = true;

    private void Start()
    {
        airplaneController = GetComponent<ControlMaster>();
        rb = GetComponent<Rigidbody>();
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
    //btn 2 cam reset

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            vtol = vtol ? false : true;
        }


        airplaneController.Pitch = Input.GetAxisRaw("Pitch");
        airplaneController.Roll = Input.GetAxisRaw("Roll");
        airplaneController.Yaw = Input.GetAxisRaw("Yaw");
        airplaneController.Throttle = Input.GetAxisRaw("Throttle");

        if (Input.GetKey(KeyCode.JoystickButton0))
        {
            rb.drag = 0.5f;
        }
        if (Input.GetKeyUp(KeyCode.JoystickButton0))
        {
            rb.drag = 0;
        }
    }
}