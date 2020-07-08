using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swimming : MonoBehaviour
{
    [SerializeField] float staminaRequirement = -10.0f;
    [SerializeField] float defaultSpeed = 5.0f;
    [SerializeField] float sprintModifier = 2.0f;
    float currentSpeed = 5.0f;

    public void SwimmingController(CharacterController controller, Vector3 movement, float delta)
    {
        currentSpeed = Mathf.Lerp(currentSpeed, SwimSpeed(), delta);
        Vector3 swimmingMovement = movement * currentSpeed;
        controller.Move(swimmingMovement * delta);
    }

    float SwimSpeed()
    {
        float swimSpeed = defaultSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            swimSpeed *= sprintModifier;
        return swimSpeed;
    }

    public float GetStaminaRequirement() { return staminaRequirement; }
    public void ResetGlidingSpeed() { currentSpeed = defaultSpeed; }
}
