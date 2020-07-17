using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] AudioSource footStepAudio;
    [SerializeField] Transform model;
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask waterMask;
    [SerializeField] Transform groundCheck;
    [SerializeField] GameObject glider;
    [SerializeField] float baseSpeed = 6.0f;
    [SerializeField] float sprintModifier = 2.0f;
    [SerializeField] float sprintingStaminaRequirement = -7.5f;
    [SerializeField] float jumpHeight = 2.0f;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float groundDownwardForce = -5.0f;
    [SerializeField] float staminaRechargeRate = 1.0f;
    [SerializeField] float wallRunDistance = 3.0f;
    [SerializeField] float isJumpingTimer = 1.5f;
    [SerializeField] float isWallJumpingTimer = 0.25f;
    [SerializeField] float glidingTimer = 0.5f;
    Climbing playerClimbing;
    Gliding playerGliding;
    Spawning playerSpawning;
    Stamina playerStamina;
    Swimming playerSwimming;
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
    bool isSprinting;

    public enum State
    {
        Base,
        Swimming,
        Gliding,
        Climbing
    };

    State state;

    private void Start()
    {
        playerClimbing = GetComponent<Climbing>();
        playerGliding = GetComponent<Gliding>();
        playerSwimming = GetComponent<Swimming>();
        playerStamina = GetComponent<Stamina>();
        playerSpawning = FindObjectOfType<Spawning>();
        transform.position = playerSpawning.GetSpawnLocation().position;
    }

    void Update()
    {
        delta = Time.deltaTime;
        Inputs();
        AnimationAndAudioController();

        switch (state)
        {
            case State.Climbing:
                ClimbingController();
                break;
            case State.Swimming:
                SwimmingController();
                break;
            case State.Gliding:
                GlidingController();
                break;
            case State.Base:
                GroundMovement();
                break;
        }
    }

    private void Inputs()
    {
        // General Movement
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
        if (Input.GetKeyDown(KeyCode.E) && !isGrounded)
        {
            if (state == State.Gliding)
                state = State.Base;
            else
            {
                ResetDownwardVelocity();
                state = State.Gliding;
            }
        }

        // Climbing
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (playerClimbing.CheckForClimb(groundMask))
            {
                if (state == State.Climbing)
                    playerClimbing.DetachFromWall();
                else
                {
                    state = State.Climbing;
                    isGrounded = false;
                }
            }
        }
    }

    private void GroundCheck()
    {
        if (isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask))
            state = State.Base;
        else if (Physics.CheckSphere(groundCheck.position, groundDistance, waterMask))
            state = State.Swimming;
    }

    private void GroundMovement()
    {
        GroundCheck();
        anim.transform.localPosition = new Vector3(0, 0, 0);
        if (isGrounded && velocity.y < groundDownwardForce)
            ResetDownwardVelocity();

        if (isGrounded && !isSprinting)
            playerStamina.RechargeStamina(staminaRechargeRate * delta);

        SprintingController();

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
        Vector2 Movement = new Vector2(xMovement, zMovement);
        playerClimbing.ClimbingMovement(Movement, playerStamina, anim);
    }

    private void GlidingController()
    {   
        GroundCheck();         
        if (!playerGliding.GlidingMovement(controller, movementVector, playerStamina, anim, glider) && !isGrounded)
            state = State.Base;
    }

    private void SwimmingController()
    {
        GroundCheck();  
        if (playerSwimming.SwimmingController(controller, movementVector, isSprinting, playerStamina, anim))
            anim.transform.localPosition = new Vector3(0, -1.5f, 0);
        else
            Death();
    }

    private void SprintingController()
    {
        if(xMovement > 0.01f || xMovement < -0.01f || zMovement > 0.01f || zMovement < -0.01f)
        {
            if (isSprinting && isGrounded && playerStamina.ApplyStaminaChangeIfAvailable(sprintingStaminaRequirement * delta))
                currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed * sprintModifier, delta);
        }
        else
            currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed, delta * 2);
    }

    private void AnimationAndAudioController()
    {
        if (state != State.Gliding)
            playerGliding.ResetGliding(anim, glider);

        if(state != State.Swimming)
            playerSwimming.ResetSwimming(anim);


        anim.SetFloat("xMovement", xMovement);
        anim.SetFloat("zMovement", zMovement);
        if (xMovement > 0.01f || xMovement < -0.01f || zMovement > 0.01f || zMovement < -0.01f)
        {
            if(!footStepAudio.isPlaying && isGrounded)
                footStepAudio.Play();
            else if (!isGrounded)
                footStepAudio.Stop();

            anim.SetBool("isMoving", true);
        }
        else
        {
            footStepAudio.Stop();
            anim.SetBool("isMoving", false);
        }

        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSprinting", isSprinting);

        if ( !isGrounded && !isWallRunning && !isJumping && state == State.Base)
            anim.SetBool("isFalling", true);
        else
            anim.SetBool("isFalling", false);
    }

    bool WallRunning()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, wallRunDistance, groundMask) || 
            Physics.Raycast(transform.position, -transform.right, out hit, wallRunDistance, groundMask))
            return true;

        return false;
    }

    private void Death()
    {
        state = State.Base;
        transform.position = playerSpawning.GetSpawnLocation().position;
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
    public void ResetRotation(float yRotation) { transform.rotation = Quaternion.Euler(0, yRotation, 0); }
    public void ResetCurrentSpeed() { currentSpeed = baseSpeed; }
    public void SetState(State stateToSet) { state = stateToSet; }
    private void ResetDownwardVelocity() { velocity.y = groundDownwardForce; }
}
