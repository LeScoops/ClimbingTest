using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    public bool isClimbing;
    [SerializeField] float positionOffset = 1.0f;
    [SerializeField] float offsetFromWall = 0.3f;
    [SerializeField] float climbSpeedMultiplier = 0.2f;
    [SerializeField] float climbingSpeed = 3.0f;
    [SerializeField] float rotateSpeed = 5.0f;
    [SerializeField] float rayTowardsMoveDir = 1.0f;
    [SerializeField] float rayForwardTowardsWall = 1.0f;

    bool inPosition;
    bool isLerping;
    bool isMid;
    float t;
    //float delta;
    float horiz;
    float vert;
    Vector3 startPosition;
    Vector3 targetPosition;
    Quaternion startRotation;
    Quaternion targetRotation;
    Transform helper;
    PlayerMovement playerMovementScript;

    private void Start()
    {
        playerMovementScript = GetComponent<PlayerMovement>();
        Init();
    }

    void Init()
    {
        helper = new GameObject().transform;
        helper.name = "Climbing Helper";
        CheckForClimb();
    }

    //private void Update()
    //{
    //    delta = Time.deltaTime;
    //    Tick();
    //}

    public void Tick(float delta)
    {
        if (!inPosition)
        {
            GetInPosition(delta);
            return;
        }

        if (!isLerping)
        {
            horiz = Input.GetAxis("Horizontal");
            vert = Input.GetAxis("Vertical");
            float m = Mathf.Abs(horiz) + Mathf.Abs(vert);

            Vector3 h = helper.right * horiz;
            Vector3 v = helper.up * vert;
            Vector3 moveDir = (h + v).normalized;

            if (isMid)
            {
                if (moveDir == Vector3.zero)
                    return;
            }
            else
            {
                bool canMove = CanMove(moveDir);
                if (!canMove || moveDir == Vector3.zero)
                    return;
            }

            isMid = !isMid;

            t = 0;
            isLerping = true;
            startPosition = transform.position;
            Vector3 tp = helper.position - transform.position;
            float d = Vector3.Distance(helper.position, startPosition) / 2;
            tp *= positionOffset;
            tp += transform.position;
            targetPosition = (isMid) ? tp : helper.position;

            //targetPosition = helper.position;
        }
        else
        {
            t += delta * climbingSpeed;
            if (t > 1)
            {
                t = 1;
                isLerping = false;
            }

            // cp  is climb position
            Vector3 cp = Vector3.Lerp(startPosition, targetPosition, t);
            transform.position = cp;
            transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * rotateSpeed);

            LookForGround();
        }
    }

    public bool CheckForClimb()
    {
        Vector3 origin = transform.position;
        origin.y += 1.4f;
        Vector3 dir = transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, 5))
        {
            helper.transform.position = PosWithOffset(origin, hit.point);
            InitForClimb(hit);
            return true;
        }
        return false;
    }

    void InitForClimb(RaycastHit hit)
    {
        isClimbing = true;
        helper.transform.rotation = Quaternion.LookRotation(-hit.normal);
        startPosition = transform.position;
        targetPosition = hit.point + (hit.normal * offsetFromWall);
        t = 0;
        inPosition = false;
    }

    bool CanMove(Vector3 moveDir)
    {
        Vector3 origin = transform.position;
        // Raycast pointing parallel to wall
        float distance = rayTowardsMoveDir;
        Vector3 direction = moveDir;
        DebugLine.singleton.SetLine(origin, origin + (direction * distance), 0);
        RaycastHit hit;

        //if (Physics.Raycast(origin, direction, out hit, distance))
        //{
        //    return false;
        //}


        // Raycast forward towards the wall
        origin += moveDir * distance;
        direction = helper.forward;
        float distance2 = rayForwardTowardsWall;
        DebugLine.singleton.SetLine(origin, origin + (direction * distance2), 1);
        if (Physics.Raycast(origin, direction, out hit, distance2))
        {
            helper.position = PosWithOffset(origin, hit.point);
            helper.rotation = Quaternion.LookRotation(-hit.normal);
            return true;
        }

        origin = origin + (direction * distance2);
        direction = -moveDir;
        DebugLine.singleton.SetLine(origin, origin + direction, 2);
        if (Physics.Raycast(origin, direction, out hit, rayForwardTowardsWall))
        {
            helper.position = PosWithOffset(origin, hit.point);
            helper.rotation = Quaternion.LookRotation(-hit.normal);
            return true;
        }

        origin += direction * distance2;
        direction = -Vector3.up;
        DebugLine.singleton.SetLine(origin, origin + direction, 3);
        if (Physics.Raycast(origin, direction, out hit, distance2))
        {
            float angle = Vector3.Angle(-helper.forward, hit.normal);
            if (angle < 40)
            {
                helper.position = PosWithOffset(origin, hit.point);
                helper.rotation = Quaternion.LookRotation(-hit.normal);
                return true;
            }
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

        // tp is short for targetposition
        Vector3 tp = Vector3.Lerp(startPosition, targetPosition, t);
        transform.position = tp;
        transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * rotateSpeed);
    }

    Vector3 PosWithOffset(Vector3 origin, Vector3 target)
    {
        Vector3 direction = origin - target;
        direction.Normalize();
        Vector3 offset = direction * offsetFromWall;
        return target + offset;
    }

    void LookForGround()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;

        if (Physics.Raycast(origin, direction, out hit, 1.2f))
        {
            isClimbing = false;
            inPosition = false;
            playerMovementScript.isClimbing = false;
            playerMovementScript.ResetRotation();
        }
    }

    public void DetachFromWall()
    {
        isClimbing = false;
        inPosition = false;
        playerMovementScript.isClimbing = false;
        playerMovementScript.ResetRotation();
    }
}

[System.Serializable]
public class IKSnapshot
{
    public Vector3 rh, lh, rf, lf;
}
