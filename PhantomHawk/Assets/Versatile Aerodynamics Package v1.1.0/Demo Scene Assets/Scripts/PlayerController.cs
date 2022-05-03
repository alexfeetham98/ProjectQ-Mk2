using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(ControlMaster))]
public class PlayerController : MonoBehaviour
{
    ControlMaster airplaneController;
    Rigidbody rb;
    
    public TextMeshProUGUI vtolText;

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
    //btn 7 free
    //btn 8 reset

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            vtol = vtol ? false : true;
            if (vtol)
            {
                vtolText.color = Color.green;
            }
            else if (!vtol)
            {
                vtolText.color = Color.red;
            }
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

        if(Input.GetKeyDown(KeyCode.JoystickButton8))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }    
    }
}