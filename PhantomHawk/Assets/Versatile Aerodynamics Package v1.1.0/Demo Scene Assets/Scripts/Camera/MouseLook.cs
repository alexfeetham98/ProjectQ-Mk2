using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    /// <summary>
    /// a first person mouse look script to be placed as a component of a player object
    /// </summary>
    private Transform rig;
    public Transform player;

    public float horizontalSpeed = 2f;
    public float verticalSpeed = 2f;

    float mouseX;
    float mouseY;
    float xRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rig = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * horizontalSpeed;
        mouseY = Input.GetAxis("Mouse Y") * verticalSpeed;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        rig.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.Rotate(mouseX * Vector3.up);
    }
}
