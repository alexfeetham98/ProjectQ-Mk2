using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    GameObject aircraft;

    void Start()
    {
        aircraft = GameObject.FindGameObjectWithTag("Aircraft");
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Grounded");
            aircraft.GetComponent<FlightController>().grounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("!Grounded");
            aircraft.GetComponent<FlightController>().grounded = false;
        }
    }
}
