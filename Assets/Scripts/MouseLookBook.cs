using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookBook : MonoBehaviour
{
    public enum RotationAxes {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }

    public RotationAxes axes = RotationAxes.MouseXAndY;

    public float sensX = 9f;
    public float sensY = 9f;

    public float minY = -90f;
    public float maxY = 90f;

    private float _rotationX = 0;
    private float _rotationY = 0;

    void Start()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
            body.freezeRotation = true;
    }

    void Update()
    {
        if (axes == RotationAxes.MouseX) {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensX, 0);
        }
        else if (axes == RotationAxes.MouseY) {
            _rotationX -= Input.GetAxis("Mouse Y") * sensY;
            _rotationX = Mathf.Clamp(_rotationX, minY, maxY);

            _rotationY = transform.localEulerAngles.y;
        }
        else { //MouseX&Y
            _rotationX -= Input.GetAxis("Mouse Y") * sensY;
            _rotationX = Mathf.Clamp(_rotationX, minY, maxY);

            float delta = Input.GetAxis("Mouse X") * sensX;
            _rotationY = transform.localEulerAngles.y + delta;

            transform.localEulerAngles = new Vector3(_rotationX, _rotationY, 0); 
        }
    }
}
