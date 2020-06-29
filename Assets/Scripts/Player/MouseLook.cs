using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 75.0f;
    [SerializeField] float maxCameraDistance = 10.0f;
    [SerializeField] float minimumCameraDistance = 0.0f;
    [SerializeField] Transform playerBody;
    [SerializeField] Transform gimbal;

    bool isClimbing = false;
    bool manualCamera = false;
    float currentDistance;
    float xRotation = 0.0f;
    float yRotation = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 mouse = MouseInput();

        SpringArm();
        gimbal.localRotation = Quaternion.Euler(xRotation, 0, 0);

        if (!isClimbing && !manualCamera)
            playerBody.Rotate(Vector3.up * mouse.x);
        else
            gimbal.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
    }

    private void SpringArm()
    {
        RaycastHit hit;
        Vector3 playerHeadPosition = playerBody.transform.position;
        playerHeadPosition.y += 1.7f;
        Physics.Raycast(playerHeadPosition, transform.position - playerHeadPosition, out hit, maxCameraDistance);
        if (hit.collider != null && Mathf.Abs(hit.distance) < currentDistance && hit.collider.tag != "Player")
            transform.localPosition = new Vector3(0.0f, 0.0f, -Mathf.Abs(hit.distance));
        else
            transform.localPosition = new Vector3(0.0f, 0.0f, -currentDistance);
    }

    Vector2 MouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        manualCamera = Input.GetMouseButton(1);

        currentDistance -= Input.GetAxis("Mouse ScrollWheel");
        currentDistance = Mathf.Clamp(currentDistance, minimumCameraDistance, maxCameraDistance);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80.0f, 80.0f);

        yRotation += mouseX;

        return new Vector2(mouseX, mouseY);
    }

    public void SetIsClimbing(bool isClimbing)
    {
        this.isClimbing = isClimbing;
    }

    public void ResetRotation()
    {
        xRotation = 0.0f;
        yRotation = 0.0f;
        gimbal.localRotation = Quaternion.Euler(0, 0, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
