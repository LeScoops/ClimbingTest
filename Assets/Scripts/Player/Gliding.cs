using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gliding : MonoBehaviour
{
    [SerializeField] float staminaRequirement = -5.0f;
    [SerializeField] float glidingGravity = -4.0f;
    [SerializeField] float defaultSpeed = 2.0f;
    [SerializeField] float maxSpeed = 10.0f;
    float currentSpeed = 2.0f;

    public void GlidingMovement(CharacterController controller, float delta)
    {
        currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, delta);
        Vector3 glidingMovement = transform.forward * currentSpeed;
        glidingMovement.y = glidingGravity;
        controller.Move(glidingMovement * delta);
    }

    public float GetStaminaRequirement() { return staminaRequirement; }
    public float GetGlidingSpeed() { return currentSpeed; }
    public void ResetGlidingSpeed() { currentSpeed = defaultSpeed; }
}
