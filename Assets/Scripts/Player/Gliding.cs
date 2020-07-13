using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gliding : MonoBehaviour
{
    [SerializeField] float staminaRequirement = -5.0f;
    [SerializeField] float downwardPull = -4.0f;
    [SerializeField] float defaultSpeed = 2.0f;
    [SerializeField] float maxSpeed = 10.0f;
    [SerializeField] string animatorBool = "isGliding";
    float currentSpeed = 2.0f;

    public bool GlidingMovement(CharacterController controller, Vector3 movement, Stamina staminaScript = null, Animator anim = null, GameObject gliderObject = null)
    {
        float delta = Time.deltaTime;
        if (staminaScript != null && !staminaScript.ApplyStaminaChangeIfAvailable(staminaRequirement * delta))
        {
            if (anim != null && gliderObject != null) { ResetGliding(anim, gliderObject); }
            ResetGlidingSpeed();
            return false;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, delta);
        Vector3 glidingMovement = transform.forward * currentSpeed;
        glidingMovement.y = downwardPull;
        controller.Move(glidingMovement * delta);
        controller.Move(movement * currentSpeed * delta);

        if (anim != null) { anim.SetBool(animatorBool, true); }
        if (gliderObject != null) { gliderObject.SetActive(true); }
        return true;
    }

    public void ResetGliding(Animator anim, GameObject gliderObject)
    {
        anim.SetBool(animatorBool, false);
        gliderObject.SetActive(false);
    }

    private void ResetGlidingSpeed() { currentSpeed = defaultSpeed; }
}
