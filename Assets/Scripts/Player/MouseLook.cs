using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] bool isClimbing = false;
    [SerializeField] float mouseSensitivity = 100.0f;
    [SerializeField] float thirdPersonDistance = -3.0f;
    [SerializeField] float firstPersonDistance = 0.2f;
    [SerializeField] Transform playerBody;
    [SerializeField] Transform XGimbal;

    bool isFirstPerson = false;
    float xRotation = 0.0f;
    float yRotation = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        ToggleCamera();
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);

        yRotation += mouseX;
        yRotation = Mathf.Clamp(yRotation, -90.0f, 90.0f);

        if (isClimbing && isFirstPerson)
        {
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
        else if (!isFirstPerson)
        {
            yRotation = Mathf.Clamp(yRotation, 0.0f, 90.0f);
            XGimbal.localRotation = Quaternion.Euler(xRotation, 0, 0);
            playerBody.Rotate(Vector3.up * mouseX);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            playerBody.Rotate(Vector3.up * mouseX);
        }
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
        if (isFirstPerson)
        {
            isFirstPerson = false;
            ResetRotation();
            transform.localPosition = new Vector3(0, 0, thirdPersonDistance);
        }
        else
        {
            isFirstPerson = true;
            ResetRotation();
            transform.localPosition = new Vector3(0, 0, firstPersonDistance);
        }
    }
}
