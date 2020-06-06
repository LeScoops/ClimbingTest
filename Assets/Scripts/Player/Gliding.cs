using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gliding : MonoBehaviour
{
    [SerializeField] float glidingStaminaRequirement = -5.0f;
    [SerializeField] float glidingGravity = -4.0f;
    [SerializeField] float baseGlidingSpeed = 2.0f;
    [SerializeField] float maxGlidingSpeed = 10.0f;
    float currentGlidingSpeed = 2.0f;

    public void GlidingMovement(CharacterController controller, float delta)
    {
        currentGlidingSpeed = Mathf.Lerp(currentGlidingSpeed, maxGlidingSpeed, delta);
        Vector3 glidingMovement = transform.forward * currentGlidingSpeed;
        glidingMovement.y = glidingGravity;
        controller.Move(glidingMovement * delta);
    }

    public float GetStaminaRequirement() { return glidingStaminaRequirement; }
    public float GetGlidingSpeed() { return currentGlidingSpeed; }
    public void ResetGlidingSpeed() { currentGlidingSpeed = baseGlidingSpeed; }
}
