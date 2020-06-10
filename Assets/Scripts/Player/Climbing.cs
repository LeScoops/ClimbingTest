using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    public bool isClimbing;
    [SerializeField] MouseLook mouseLook;
    [SerializeField] float positionOffset = 1.0f;
    [SerializeField] float offsetFromWall = 0.3f;
    [SerializeField] float climbingSpeed = 5.0f;
    [SerializeField] float rotateSpeed = 5.0f;
    [SerializeField] float rayTowardsMoveDir = 1.0f;
    [SerializeField] float rayForwardTowardsWall = 1.0f;
    [SerializeField] float baseStaminaDrainRate = 0.4f;
    [SerializeField] float staminaDrainRate = 10.0f;
    [SerializeField] float wallJumpStaminaUsage = 20.0f;

    bool inPosition;
    bool isLerping;
    float t;
    float horizontalInput;
    float verticalInput;
    Animator anim;
    Vector3 startPosition;
    Vector3 targetPosition;
    Vector3 lastHelperForward;
    Transform climbingHelper;
    PlayerMovement playerMovementScript;
    Stamina playerStamina;
    LayerMask layerMask;

    private void Start()
    {
        playerMovementScript = GetComponent<PlayerMovement>();
        layerMask = playerMovementScript.GetLayerMask();
        playerStamina = GetComponent<Stamina>();
        anim = playerMovementScript.GetAnim();
        Init();
    }

    void Init()
    {
        climbingHelper = new GameObject().transform;
        climbingHelper.name = "Climbing Helper";
        CheckForClimb();
    }

    public void Tick(float delta)
    {
        if (!playerStamina.ApplyStaminaChangeIfAvailable(-baseStaminaDrainRate * delta))
            DetachFromWall();

        if (Input.GetButtonDown("Jump") && playerStamina.ApplyStaminaChangeIfAvailable(-wallJumpStaminaUsage))
        {
            DetachFromWall();
            playerMovementScript.WallJumping();
        }

        if (!inPosition)
        {
            GetInPosition(delta);
            return;
        }

        if (!isLerping)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            Vector3 horizontalMovement = climbingHelper.right * horizontalInput;
            Vector3 verticalMovement = climbingHelper.up * verticalInput;
            Vector3 moveDirection = (horizontalMovement + verticalMovement).normalized;

            bool canMove = CanMove(moveDirection);
            if (!canMove || moveDirection == Vector3.zero)
                return;

            t = 0;
            isLerping = true;
            startPosition = transform.position;
            Vector3 tp = climbingHelper.position - transform.position;
            float d = Vector3.Distance(climbingHelper.position, startPosition) / 2;
            tp *= positionOffset;
            tp += transform.position;
            targetPosition = tp;
        }
        else
        {
            t += delta * climbingSpeed;
            if (t > 1)
            {
                t = 1;
                isLerping = false;
            }

            Vector3 climbingPosition = Vector3.Lerp(startPosition, targetPosition, t);
            transform.position = climbingPosition;
            transform.rotation = Quaternion.Slerp(transform.rotation, climbingHelper.rotation, delta * rotateSpeed);

            if (!playerStamina.ApplyStaminaChangeIfAvailable(-staminaDrainRate * delta))
                DetachFromWall();

            anim.SetTrigger("ClimbMovement");
            CheckForGround();
        }
    }

    public bool CheckForClimb()
    {
        Vector3 originPoint = transform.position;
        originPoint.y += 1.4f;
        Vector3 direction = transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(originPoint, direction, out hit, 5, layerMask))
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
        climbingHelper.transform.rotation = Quaternion.LookRotation(-hit.normal);
        mouseLook.ResetRotation();
        startPosition = transform.position;
        targetPosition = hit.point + (hit.normal * offsetFromWall);
        t = 0;
        inPosition = false;
    }

    bool CanMove(Vector3 moveDir)
    {
        Vector3 origin = transform.position;
        float distance = rayTowardsMoveDir;
        Vector3 direction = moveDir;
        DebugLine.singleton.SetLine(origin, origin + (direction * distance), 0);
        RaycastHit hit;

        // For going around inner corners and objects above
        if (Physics.Raycast(origin, direction, out hit, distance, layerMask))
        { 
            if (hit.normal.x > 0.9f || hit.normal.x < -0.9f || hit.normal.z > 0.9f || hit.normal.z < -0.9f)
            {
                climbingHelper.position = PosWithOffset(origin, hit.point);
                climbingHelper.rotation = Quaternion.LookRotation(-hit.normal);
                return true;
            }
            return false;
        }

        // For traversing normal angles
        origin += moveDir * distance;
        direction = climbingHelper.forward;
        float distance2 = rayForwardTowardsWall;
        DebugLine.singleton.SetLine(origin, origin + (direction * distance2), 1);
        if (Physics.Raycast(origin, direction, out hit, distance2))
        {
            climbingHelper.position = PosWithOffset(origin, hit.point);
            climbingHelper.rotation = Quaternion.LookRotation(-hit.normal);
            Debug.Log(climbingHelper.rotation.eulerAngles);
            return true;
        }

        // For going around outer corners
        origin = origin + (direction * distance2);
        direction = -moveDir;
        DebugLine.singleton.SetLine(origin, origin + direction, 2);
        if (Physics.Raycast(origin, direction, out hit, rayForwardTowardsWall))
        {
            climbingHelper.position = PosWithOffset(origin, hit.point);
            climbingHelper.rotation = Quaternion.LookRotation(-hit.normal);
            return true;
        }

        return false;
    }

    void GetInPosition(float delta)
    {
        t += delta;
        if (t > 1)
        {
            t = 1;
            inPosition = true;
        }

        Vector3 targetPositionToMoveTo = Vector3.Lerp(startPosition, targetPosition, t);
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
            DetachFromWall();
    }

    public void DetachFromWall()
    {
        isClimbing = false;
        inPosition = false;
        mouseLook.SetIsClimbing(false);
        playerMovementScript.isClimbing = false;
        playerMovementScript.ResetRotation(mouseLook.GetFinalRotation());
    }
}
