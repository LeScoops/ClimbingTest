using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gliding : MonoBehaviour
{
    [SerializeField] float glidingStaminaRequirement = -5.0f;
    [SerializeField] float glidingGravity = -4.0f;
    [SerializeField] float glidingSpeed = 15.0f;

    public void GlidingMovement(CharacterController controller, float delta)
    {
        Vector3 glidingMovement = transform.forward * glidingSpeed;
        glidingMovement.y = glidingGravity;
        controller.Move(glidingMovement * delta);
    }

    public float GetStaminaRequirement() { return glidingStaminaRequirement; }
    public float GetGlidingSpeed() { return glidingSpeed; }
}
