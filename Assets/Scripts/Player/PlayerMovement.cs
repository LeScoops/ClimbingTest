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

    [SerializeField] float wallRunDistance = 5.0f;
    [SerializeField] float isJumpingTimer = 3.0f;
    [SerializeField] float isWallJumpingTimer = 1.0f;

    Climbing playerClimbing;
    Stamina playerStamina;
    Gliding playerGliding;
    Vector3 velocity;
    float delta;
    float currentSpeed;
    bool isGrounded;
    bool isJumping;
    bool isWallRunning;
    bool isWallJumping;
    bool isOnWall;
    bool isGliding;
    bool isSprinting;

    private void Start()
    {
        playerClimbing = GetComponent<Climbing>();
        playerStamina = GetComponent<Stamina>();
        playerGliding = GetComponent<Gliding>();
    }

    void Update()
    {
        delta = Time.deltaTime;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isClimbing)
        {
            playerClimbing.Tick(delta);

            if (Input.GetKeyDown(KeyCode.C))
                playerClimbing.DetachFromWall();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.C) || currentSpeed > baseSpeed * 1.25f || (!isGrounded && isGliding))
            {
                isClimbing = playerClimbing.CheckForClimb();
                if (isClimbing)
                    isGliding = false;
            }

            if (isGrounded)
                playerStamina.RechargeStamina(staminaRechargeRate * delta);

            if (isGliding && Input.GetKeyDown(KeyCode.E))
            {
                isGliding = false;
                return;
            }

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
            currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed * sprintModifier, delta);
        else
            currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed, delta);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
            StartCoroutine(JumpControl());
        }

        // Gliding
        if (Input.GetKeyDown(KeyCode.E) && !isGrounded && playerStamina.ApplyStaminaChangeIfAvailable(playerGliding.GetStaminaRequirement() * delta))
        {
            playerGliding.ResetGlidingSpeed();
            isGliding = true;
        }

        Vector3 move = transform.right * x + transform.forward * z;

 

        if (isGliding)
        {
            velocity.y = -2.0f;

            if (playerStamina.ApplyStaminaChangeIfAvailable(playerGliding.GetStaminaRequirement() * delta) && !isGrounded)
                playerGliding.GlidingMovement(controller, delta);
            else
                isGliding = false;

            controller.Move(move * currentSpeed * delta);
        }
        else
        {
            if (Input.GetButton("Jump") && WallRunning() && isJumping)
            {
                if (!isWallRunning)
                    StartCoroutine(WallRunControl());
                move = transform.forward;
                velocity.y += gravity / 2 * delta;
            }
            else if (isWallRunning && !isGrounded && !WallRunning())
            {
                move = transform.forward;
                velocity.y += gravity * delta;
            }
            else if (isWallJumping)
            {

            }
            else
                velocity.y += gravity * delta;

            controller.Move(move * currentSpeed * delta);
            controller.Move(velocity * delta);

        }
    }

    bool WallRunning()
    {
        int layermask = 1 << 9;
        layermask = ~layermask;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, wallRunDistance, layermask) || 
            Physics.Raycast(transform.position, -transform.right, out hit, wallRunDistance, layermask))
            return true;

        return false;
    }

    IEnumerator JumpControl()
    {
        isJumping = true;
        yield return new WaitForSeconds(isJumpingTimer);
        isJumping = false;
    }

    IEnumerator WallRunControl()
    {
        isWallRunning = true;
        yield return new WaitForSeconds(isJumpingTimer);
        isWallRunning = false;
    }

    IEnumerator WallJumpControl()
    {
        
        velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        Vector3 move = -playerClimbing.GetHelperForward();
        controller.Move(move * delta);
        isWallJumping = true;

        yield return new WaitForSeconds(isWallJumpingTimer);
        isWallJumping = false;
    }

    public void WallJumping()
    {
        StartCoroutine(WallJumpControl());
    }

    public void ResetRotation(Quaternion helperRotation) { transform.rotation = new Quaternion(0, transform.rotation.y, 0, helperRotation.w); }
    public void ResetCurrentSpeed() { currentSpeed = baseSpeed; }
}
