using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swimming : MonoBehaviour
{
    [SerializeField] AudioSource swimmingAudio;
    [SerializeField] string animatorBool = "isSwimming";
    [SerializeField] float defaultStaminaRequirement = -10.0f;
    [SerializeField] float sprintStaminaRequirement = -20.2f;
    [SerializeField] float defaultSpeed = 5.0f;
    [SerializeField] float sprintSpeed = 10.0f;
    float currentSpeed = 5.0f;

    public bool SwimmingController(CharacterController controller, Vector3 movement, bool isSprinting = false, Stamina staminaScript = null, Animator anim = null)
    {
        float delta = Time.deltaTime;
        if (staminaScript != null && 
            isSprinting? !staminaScript.ApplyStaminaChangeIfAvailable(sprintStaminaRequirement * delta):
            !staminaScript.ApplyStaminaChangeIfAvailable(defaultStaminaRequirement * delta))
        {
            if (anim != null) ExitSwimming(anim);
            return false;
        }

        // if(movement != Vector3.zero && !swimmingAudio.isPlaying)
        //     swimmingAudio.Play();

        float swimSpeed = defaultSpeed;
        if (isSprinting)
            swimSpeed = sprintSpeed;

        currentSpeed = Mathf.Lerp(currentSpeed, swimSpeed, delta);
        Vector3 swimmingMovement = movement * currentSpeed;
        controller.Move(swimmingMovement * delta);

        if (anim != null) anim.SetBool(animatorBool, true);
        return true;
    }

    public void ExitSwimming(Animator anim)
    {
        anim.SetBool(animatorBool, false);
    }
}
