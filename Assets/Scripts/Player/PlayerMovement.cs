using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool isClimbing;

    [SerializeField] Animator anim;
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Transform groundCheck;
    [SerializeField] float baseSpeed = 12.0f;
    [SerializeField] float sprintModifier = 2.0f;
    [SerializeField] float sprintingStaminaRequirement = -7.5f;
    [SerializeField] float jumpHeight = 3.0f;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float staminaRechargeRate = 1.0f;
    [SerializeField] float wallRunDistance = 5.0f;
    [SerializeField] float isJumpingTimer = 3.0f;
    [SerializeField] float isWallJumpingTimer = 1.0f;

    Climbing playerClimbing;
    Gliding playerGliding;
    Stamina playerStamina;
    Vector3 velocity;
    Vector3 move;
    Vector3 helperForward;

    float delta;
    float currentSpeed;
    float xMovement;
    float zMovement;
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
            //isClimbing = playerClimbing.CheckForClimb();
            if (Input.GetKeyDown(KeyCode.C) || currentSpeed > baseSpeed * 1.25f || (!isGrounded && isGliding))
            {
                isClimbing = playerClimbing.CheckForClimb();
                if (isClimbing)
                    isGliding = false;
            }

            if (isGrounded && !isSprinting)
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
            ResetDownwardVelocity();

        Inputs();

        if (isSprinting && isGrounded)
            currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed * sprintModifier, delta);
        else
            currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed, delta);

        anim.SetFloat("xMovement", xMovement);
        anim.SetFloat("zMovement", zMovement);
        anim.SetBool("isGliding", isGliding);

        move = transform.right * xMovement + transform.forward * zMovement;

        if (isGliding)
        {
            if (playerStamina.ApplyStaminaChangeIfAvailable(playerGliding.GetStaminaRequirement() * delta) && !isGrounded)
                playerGliding.GlidingMovement(controller, delta);
            else
                isGliding = false;

            controller.Move(move * currentSpeed * delta);
            return;
        }

        if (isWallRunning && WallRunning())
            velocity.y += gravity / 2 * delta;
        else if (isWallRunning && !isGrounded && !WallRunning())
            velocity.y += gravity * delta;
        else if (isWallJumping)
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        else
            velocity.y += gravity * delta;

        controller.Move(move * currentSpeed * delta);
        controller.Move(velocity * delta);
    }

    private void Inputs()
    {
        xMovement = Input.GetAxis("Horizontal");
        zMovement = Input.GetAxis("Vertical");

        if (xMovement > 0.1f || xMovement < -0.1f || zMovement > 0.1f || zMovement < -0.1f)
            anim.SetBool("isMoving", true);
        else
            anim.SetBool("isMoving", false);

        if (Input.GetKey(KeyCode.LeftShift) && playerStamina.ApplyStaminaChangeIfAvailable(sprintingStaminaRequirement * delta))
            isSprinting = true;
        else
            isSprinting = false;

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
            StartCoroutine(JumpControl());

        // Wall Running
        if (Input.GetButton("Jump") && WallRunning() && isJumping)
            if (!isWallRunning)
                StartCoroutine(WallRunControl());

        // Gliding
        if (Input.GetKeyDown(KeyCode.E) && !isGrounded && playerStamina.ApplyStaminaChangeIfAvailable(playerGliding.GetStaminaRequirement() * delta))
        {
            ResetDownwardVelocity();
            playerGliding.ResetGlidingSpeed();
            isGliding = true;
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
        anim.SetTrigger("isJumping");
        isJumping = true;
        velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
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
        isWallJumping = true;
        yield return new WaitForSeconds(isWallJumpingTimer);            
        isWallJumping = false;
    }

    public void WallJumping() { StartCoroutine(WallJumpControl()); }
    public void ResetRotation(Quaternion helperRotation) { transform.rotation = new Quaternion(0, transform.rotation.y, 0, helperRotation.w); }
    public void ResetCurrentSpeed() { currentSpeed = baseSpeed; }
    private void ResetDownwardVelocity() { velocity.y = -2.0f; }
}
