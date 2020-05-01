using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool isClimbing;
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Transform groundCheck;
    [SerializeField] float speed = 12.0f;
    [SerializeField] float jumpHeight = 3.0f;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] float gravity = -9.81f;

    Climbing climbingScript;
    Vector3 velocity;
    [SerializeField] bool isGrounded;
    float delta;

    private void Start()
    {
        climbingScript = GetComponent<Climbing>();
    }

    void Update()
    {
        delta = Time.deltaTime;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isClimbing)
        {
            climbingScript.Tick(delta);
            if (Input.GetKeyDown(KeyCode.C))
            {
                climbingScript.DetachFromWall();
            }
            return;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                isClimbing = climbingScript.CheckForClimb();
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

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);


        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.y, 0));
    }
}
