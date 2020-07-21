using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{ 
    [SerializeField] AudioSource climbingAudio;
    [SerializeField] MouseLook mouseLook;
    [SerializeField] float distanceCheckToWall = 5.0f;
    [SerializeField] float positionOffset = 1.0f;
    [SerializeField] float offsetFromWall = 0.75f;
    [SerializeField] float climbingSpeed = 5.0f;
    [SerializeField] float rotateSpeed = 5.0f;
    [SerializeField] float rayTowardsMoveDirection = 1.0f;
    [SerializeField] float rayForwardTowardsWall = 1.0f;
    [SerializeField] float baseStaminaDrainRate = 0.4f;
    [SerializeField] float climbingMovementStaminaDrainRate = 7.0f;
    [SerializeField] float wallJumpStaminaUsage = 20.0f;
    [SerializeField] string animatorBool = "isClimbing";

    Animator anim;
    Transform climbingHelper;
    PlayerMovement playerMovementScript;
    LayerMask groundMask;
    Vector3 startPosition;
    Vector3 targetPosition;
    bool isClimbing;
    bool inPosition;
    bool isLerping;
    float timer;
    float horizontalInput;
    float verticalInput;
    float lastHelperRotation;

    private void Start()
    {
        playerMovementScript = GetComponent<PlayerMovement>();
        climbingHelper = new GameObject().transform;
        climbingHelper.name = "Climbing Helper";
    }

    public void ClimbingMovement(Vector2 Movement, Stamina playerStamina = null, Animator animGiven = null)
    {
        float delta = Time.deltaTime;
        if (animGiven != null) { anim = animGiven; animGiven.SetBool(animatorBool, true); }
        if (playerStamina != null && !playerStamina.ApplyStaminaChangeIfAvailable(-baseStaminaDrainRate * delta))
            ExitClimbing();

        if (Input.GetButtonDown("Jump") && playerStamina != null && playerStamina.ApplyStaminaChangeIfAvailable(-wallJumpStaminaUsage))
        {
            ExitClimbing();
            playerMovementScript.WallJumping();
        }

        if (!inPosition)
        {
            GetInPosition(delta);
            return;
        }

        if (!isLerping)
        {
            horizontalInput = Movement.x;
            verticalInput = Movement.y;

            Vector3 horizontalMovement = climbingHelper.right * horizontalInput;
            Vector3 verticalMovement = climbingHelper.up * verticalInput;
            Vector3 moveDirection = (horizontalMovement + verticalMovement).normalized;

            bool canMove = CanMove(moveDirection);
            if (!canMove || moveDirection == Vector3.zero)
                return;

            timer = 0;
            isLerping = true;
            startPosition = transform.position;
            Vector3 currentTargetPosition = climbingHelper.position - transform.position;
            currentTargetPosition *= positionOffset;
            currentTargetPosition += transform.position;
            targetPosition = currentTargetPosition;
        }
        else
        {
            timer += delta * climbingSpeed;
            if (timer > 1)
            {
                timer = 1;
                isLerping = false;
            }

            Vector3 climbingPosition = Vector3.Lerp(startPosition, targetPosition, timer);
            transform.position = climbingPosition;
            transform.rotation = Quaternion.Slerp(transform.rotation, climbingHelper.rotation, delta * rotateSpeed);

            if (playerStamina != null && !playerStamina.ApplyStaminaChangeIfAvailable(-climbingMovementStaminaDrainRate * delta))
                ExitClimbing();

            if (anim != null) { anim.SetTrigger("ClimbMovement"); }

            if(!climbingAudio.isPlaying)
                climbingAudio.Play();
                
            CheckForGround();
        }
    }

    public bool CheckForClimb(LayerMask givenGroundMask)
    {
        groundMask = givenGroundMask;
        Vector3 originPoint = transform.position;
        originPoint.y += 1.4f;
        Vector3 direction = transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(originPoint, direction, out hit, distanceCheckToWall, groundMask))
        {
            if (hit.normal.y > 0.8f)
                return false;
            climbingHelper.transform.position = PosWithOffset(originPoint, hit.point);
            playerMovementScript.ResetCurrentSpeed();
            InitForClimb(hit);
            return true;
        }
        return false;
    }

    void InitForClimb(RaycastHit hit)
    {
        isClimbing = true;
        mouseLook.SetIsClimbing(true);
        climbingHelper.transform.rotation = Quaternion.LookRotation(-hit.normal);
        mouseLook.ResetRotation();
        startPosition = transform.position;
        targetPosition = hit.point + (hit.normal * offsetFromWall);
        timer = 0;
        inPosition = false;
    }

    bool CanMove(Vector3 moveDirection)
    {
        Vector3 originPoint = transform.position;
        float distance = rayTowardsMoveDirection;
        Vector3 direction = moveDirection;
        RaycastHit hit;

        // For going around inner corners and objects directly above
        if (Physics.Raycast(originPoint, direction, out hit, distance, groundMask))
        { 
            if (hit.normal.x > 0.9f || hit.normal.x < -0.9f || hit.normal.z > 0.9f || hit.normal.z < -0.9f)
            {
                climbingHelper.position = PosWithOffset(originPoint, hit.point);
                climbingHelper.rotation = Quaternion.LookRotation(-hit.normal);
                lastHelperRotation = climbingHelper.eulerAngles.y;
                return true;
            }
            return false;
        }

        // For traversing normal angles
        originPoint += moveDirection * distance;
        direction = climbingHelper.forward;
        float distance2 = rayForwardTowardsWall;
        if (Physics.Raycast(originPoint, direction, out hit, distance2))
        {
            climbingHelper.position = PosWithOffset(originPoint, hit.point);
            climbingHelper.rotation = Quaternion.LookRotation(-hit.normal);
            lastHelperRotation = climbingHelper.eulerAngles.y;
            return true;
        }

        // For going around outer corners
        originPoint = originPoint + (direction * distance2);
        direction = -moveDirection;
        if (Physics.Raycast(originPoint, direction, out hit, rayForwardTowardsWall))
        {
            climbingHelper.position = PosWithOffset(originPoint, hit.point);
            climbingHelper.rotation = Quaternion.LookRotation(-hit.normal);
            return true;
        }

        return false;
    }

    void GetInPosition(float delta)
    {
        timer += delta;
        if (timer > 1)
        {
            timer = 1;
            inPosition = true;
        }

        Vector3 targetPositionToMoveTo = Vector3.Lerp(startPosition, targetPosition, timer);
        transform.position = targetPositionToMoveTo;
        transform.rotation = Quaternion.Slerp(transform.rotation, climbingHelper.rotation, delta * rotateSpeed);
    }

    Vector3 PosWithOffset(Vector3 origin, Vector3 target)
    {
        Vector3 direction = origin - target;
        direction.Normalize();
        Vector3 offset = direction * offsetFromWall;

        return target + offset;
    }

    void CheckForGround()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;

        if (Physics.Raycast(origin, direction, out hit, 1.2f))
            ExitClimbing();
    }

    public void ResetClimbing()
    {
        anim.SetBool(animatorBool, false);
    }

    public void ExitClimbing()
    {
        isClimbing = false;
        inPosition = false;
        ResetClimbing();
        mouseLook.SetIsClimbing(false);
        mouseLook.ResetRotation();
        playerMovementScript.SetState(PlayerMovement.State.Base);
        playerMovementScript.ResetRotation(lastHelperRotation);
    }
}
