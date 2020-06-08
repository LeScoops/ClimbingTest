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
    [SerializeField] float baseSpeed = 8.0f;
    [SerializeField] float sprintModifier = 2.0f;
    [SerializeField] float sprintingStaminaRequirement = -7.5f;
    [SerializeField] float jumpHeight = 2.0f;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float staminaRechargeRate = 1.0f;
    [SerializeField] float wallRunDistance = 3.0f;
    [SerializeField] float isJumpingTimer = 1.5f;
    [SerializeField] float isWallJumpingTimer = 0.25f;
    [SerializeField] float glidingTimer = 0.5f;

    Climbing playerClimbing;
    Gliding playerGliding;
    Stamina playerStamina;
    Vector3 velocity;
    Vector3 movementVector;
    Vector3 helperForward;

    float delta;
    float currentSpeed;
    float xMovement;
    float zMovement;
    bool isGrounded;
    bool isJumping;
    bool isWallRunning;
    bool isWallJumping;
    bool isGliding;
    bool glidingTriggered;
    bool isSprinting;
    bool isFalling;

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
            ClimbingController();
        else
        {
            Inputs();
            AnimationController();
            if (isGliding)
                GlidingController();
            else
                GroundMovement();
        }
    }

    private void GroundMovement()
    {
        if (isGrounded && velocity.y < 0)
            ResetDownwardVelocity();

        if (isGrounded && !isSprinting)
            playerStamina.RechargeStamina(staminaRechargeRate * delta);

        SprintController();

        if (isWallRunning && WallRunning())
            velocity.y += gravity / 2 * delta;
        else
            velocity.y += gravity * delta;

        if (isWallJumping)
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);

        controller.Move(movementVector * currentSpeed * delta);
        controller.Move(velocity * delta);
    }

    private void ClimbingController()
    {
        playerClimbing.Tick(delta);

        if (Input.GetKeyDown(KeyCode.C))
            playerClimbing.DetachFromWall();
    }


    private void GlidingController()
    {
        if (Input.GetKeyDown(KeyCode.E) && !glidingTriggered)
        {
            isGliding = false;
            return;
        }

        if (playerStamina.ApplyStaminaChangeIfAvailable(playerGliding.GetStaminaRequirement() * delta) && !isGrounded)
            playerGliding.GlidingMovement(controller, delta);
        else
            isGliding = false;

        controller.Move(movementVector * currentSpeed * delta);
    }

    private void SprintController()
    {
        if (isSprinting && isGrounded && playerStamina.ApplyStaminaChangeIfAvailable(sprintingStaminaRequirement * delta))
            currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed * sprintModifier, delta);
        else
            currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed, delta * 2);
    }

    private void AnimationController()
    {
        anim.SetFloat("xMovement", xMovement);
        anim.SetFloat("zMovement", zMovement);

        if (xMovement > 0.1f || xMovement < -0.1f || zMovement > 0.1f || zMovement < -0.1f)
            anim.SetBool("isMoving", true);
        else
            anim.SetBool("isMoving", false);

        anim.SetBool("isGliding", isGliding);
        anim.SetBool("isClimbing", isClimbing);

        if (!isGliding && !isGrounded && !isWallRunning && !isJumping)
            anim.SetBool("isFalling", true);
        else
            anim.SetBool("isFalling", false);
    }

    private void Inputs()
    {
        xMovement = Input.GetAxis("Horizontal");
        zMovement = Input.GetAxis("Vertical");
        movementVector = transform.right * xMovement + transform.forward * zMovement;

        // Sprinting
        if (Input.GetKey(KeyCode.LeftShift))
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
        if (Input.GetKeyDown(KeyCode.E) && !isGrounded && !isGliding && playerStamina.ApplyStaminaChangeIfAvailable(playerGliding.GetStaminaRequirement() * delta))
        {
            ResetDownwardVelocity();
            playerGliding.ResetGlidingSpeed();
            isGliding = true;
            StartCoroutine(GlidingControl());
        }

        // Climbing
        if (Input.GetKeyDown(KeyCode.C))
        {
            isClimbing = playerClimbing.CheckForClimb();
            if (isClimbing)
                isGliding = false;
        }
    }

    bool WallRunning()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, wallRunDistance, groundMask) || 
            Physics.Raycast(transform.position, -transform.right, out hit, wallRunDistance, groundMask))
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

    IEnumerator GlidingControl()
    {
        glidingTriggered = true;
        yield return new WaitForSeconds(glidingTimer);
        glidingTriggered = false;
    }

    public void WallJumping() { StartCoroutine(WallJumpControl()); }
    public void ResetRotation(Quaternion helperRotation) { transform.rotation = new Quaternion(0, transform.rotation.y, 0, helperRotation.w); }
    public void ResetCurrentSpeed() { currentSpeed = baseSpeed; }
    public LayerMask GetLayerMask() { return groundMask; }
    private void ResetDownwardVelocity() { velocity.y = -2.0f; }
    public Animator GetAnim() { return anim; }
}
