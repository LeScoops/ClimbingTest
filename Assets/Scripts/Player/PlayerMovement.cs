using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool isClimbing;
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Transform groundCheck;
    [SerializeField] float baseSpeed = 12.0f;
    [SerializeField] float sprintModifier = 2.0f;
    [SerializeField] float jumpHeight = 3.0f;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float staminaRechargeRate = 1.0f;
    [SerializeField] bool isGrounded;

    Climbing climbingScript;
    Stamina playerStamina;
    Vector3 velocity;
    float delta;
    float currentSpeed;
    bool isSprinting;

    private void Start()
    {
        climbingScript = GetComponent<Climbing>();
        playerStamina = GetComponent<Stamina>();
    }

    void Update()
    {
        delta = Time.deltaTime;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isClimbing)
        {
            climbingScript.Tick(delta);

            if (Input.GetKeyDown(KeyCode.C))
                climbingScript.DetachFromWall();

            return;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.C) || currentSpeed > baseSpeed * 1.25f)
                isClimbing = climbingScript.CheckForClimb();

            if (isGrounded)
                playerStamina.RechargeStamina(staminaRechargeRate * Time.deltaTime);

            Movement();
        }
    }

    private void Movement()
    {
        if (isGrounded && velocity.y < 0)
            velocity.y = -2.0f;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift) & isGrounded)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed * sprintModifier, delta);
            isSprinting = true;
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed, delta);
            isSprinting = false;
        }

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);


        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void ResetRotation() { transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.y, 0)); }
    public void ResetCurrentSpeed() { currentSpeed = baseSpeed; }
}
