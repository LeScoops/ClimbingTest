using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] bool isClimbing = false;
    [SerializeField] float mouseSensitivity = 100.0f;
    [SerializeField] Transform playerBody;

    float xRotation = 0.0f;
    float yRotation = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);

        yRotation += mouseX;
        yRotation = Mathf.Clamp(yRotation, -90.0f, 90.0f);

        if (isClimbing)
        {
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
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
    }

    public Vector2 GetFinalRotation()
    {
        //Debug.Log("XRot: " + xRotation + ". YRot: " + yRotation);
        return new Vector2(xRotation, yRotation);
    }
}
