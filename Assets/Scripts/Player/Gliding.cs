using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gliding : MonoBehaviour
{
    [SerializeField] float glidingStaminaRequirement = -5.0f;
    [SerializeField] float glidingGravity = -4.0f;
    [SerializeField] float glidingSpeed = 5.0f;

    public Vector3 GlidingMovement()
    {
        Vector3 test = transform.forward * glidingSpeed;
        test.y = glidingGravity;
        return test;
    }

    public float GetStaminaRequirement() { return glidingStaminaRequirement; }
}
