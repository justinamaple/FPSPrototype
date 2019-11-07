using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookBrackeys : MonoBehaviour
{
    public float mouseSensitivity = 400f; //Eventually split into X, Y
    public float mouseInvertX = 1;
    public float mouseInvertY = 1;
    public Transform playerBody;

    float xRotation = 0f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;


        xRotation -= mouseY * mouseInvertY; 
        xRotation = Mathf.Clamp(xRotation, -89f, 89f);

        transform.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX * mouseInvertX);
    }
}