using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] bool isClimbing = false;
    [SerializeField] float mouseSensitivity = 100.0f;
    [SerializeField] float thirdPersonDistance = 4.0f;
    [SerializeField] float firstPersonDistance = 0.2f;
    [SerializeField] Transform playerBody;
    [SerializeField] Transform XGimbal;
    [SerializeField] LayerMask layerMask;

    CameraPerspective cameraPerspective;
    float zoom;
    float xRotation = 0.0f;
    float yRotation = 0.0f;

    private enum CameraPerspective
    {
        FirstPerson,
        ThirdPerson
    };

    void Start()
    {
        cameraPerspective = CameraPerspective.FirstPerson;
        Cursor.lockState = CursorLockMode.Locked;
        ToggleCamera();
    }

    void Update()
    {
        Vector2 mouse = InputController();

        if (isClimbing && cameraPerspective == CameraPerspective.FirstPerson)
        {
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
        else if (cameraPerspective == CameraPerspective.ThirdPerson)
        {
            ThirdPersonController();

            xRotation = Mathf.Clamp(xRotation, -30.0f, 80.0f);
            XGimbal.localRotation = Quaternion.Euler(xRotation, 0, 0);
            playerBody.Rotate(Vector3.up * mouse.x);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            playerBody.Rotate(Vector3.up * mouse.x);
        }
    }

    private void ThirdPersonController()
    {
        RaycastHit hit;
        Vector3 playerHeadPosition = playerBody.transform.position;
        playerHeadPosition.y += 1.7f;
        Physics.Raycast(playerHeadPosition, transform.position - playerHeadPosition, out hit, thirdPersonDistance);
        if (hit.collider != null && Mathf.Abs(hit.distance) < thirdPersonDistance && hit.collider.tag != "Player")
            transform.localPosition = new Vector3(0.0f, 0.0f, -Mathf.Abs(hit.distance));
        else
            transform.localPosition = new Vector3(0.0f, 0.0f, -zoom);
    }

    Vector2 InputController()
    {
        if (Input.GetKeyDown(KeyCode.F))
            ToggleCamera();

        zoom -= Input.GetAxis("Mouse ScrollWheel");
        zoom = Mathf.Clamp(zoom, firstPersonDistance, thirdPersonDistance);
        if ((cameraPerspective == CameraPerspective.ThirdPerson && zoom <= firstPersonDistance) ||
            (cameraPerspective == CameraPerspective.FirstPerson && zoom > firstPersonDistance))
            ToggleCamera();

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);

        yRotation += mouseX;
        yRotation = Mathf.Clamp(yRotation, -90.0f, 90.0f);

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
        XGimbal.localRotation = Quaternion.Euler(0, 0, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void ToggleCamera()
    {
        if (cameraPerspective == CameraPerspective.FirstPerson)
        {
            cameraPerspective = CameraPerspective.ThirdPerson;
            zoom = thirdPersonDistance;
            ResetRotation();
            transform.localPosition = new Vector3(0, 0, -zoom);
        }
        else
        {
            cameraPerspective = CameraPerspective.FirstPerson;
            zoom = firstPersonDistance;
            ResetRotation();
            transform.localPosition = new Vector3(0, 0, zoom);
        }
    }
}
