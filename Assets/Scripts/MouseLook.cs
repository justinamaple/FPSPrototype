using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("GunCam")]
    [Tooltip("The transform component that holds the gun camera."), SerializeField]
    private Transform gunCam;

    [Header("Look Settings")]
    [Tooltip("Rotation speed of the fps controller."), SerializeField]
    private float mouseSensitivity = 4f;

    [Tooltip("Approximately the amount of time it will take for the fps controller to reach maximum rotation speed."), SerializeField]
    private float rotationSmoothness = 0.05f;

    [Tooltip("Minimum rotation of the arms and camera on the x axis."), SerializeField]
    private float minVerticalAngle = -90f;

    [Tooltip("Maximum rotation of the arms and camera on the axis."), SerializeField]
    private float maxVerticalAngle = 90f;

    private SmoothRotation _rotationX;
    private SmoothRotation _rotationY;

    void Start()
    {
        _rotationX = new SmoothRotation(RotationXRaw);
        _rotationY = new SmoothRotation(RotationYRaw);

        Cursor.lockState = CursorLockMode.Locked;
        ValidateRotationRestriction();
    }

    /// Clamps <see cref="minVerticalAngle"/> and <see cref="maxVerticalAngle"/> to valid values and
    /// ensures that <see cref="minVerticalAngle"/> is less than <see cref="maxVerticalAngle"/>.
    private void ValidateRotationRestriction()
    {
        minVerticalAngle = ClampRotationRestriction(minVerticalAngle, -90, 90);
        maxVerticalAngle = ClampRotationRestriction(maxVerticalAngle, -90, 90);

        if (maxVerticalAngle >= minVerticalAngle) return;

        Debug.LogWarning("maxVerticalAngle should be greater than minVerticalAngle.");
        var min = minVerticalAngle;
        minVerticalAngle = maxVerticalAngle;
        maxVerticalAngle = min;
    }

    private static float ClampRotationRestriction(float rotationRestriction, float min, float max)
    {
        if (rotationRestriction >= min && rotationRestriction <= max) return rotationRestriction;

        var message = string.Format("Rotation restrictions should be between {0} and {1} degrees.", min, max);
        Debug.LogWarning(message);
        return Mathf.Clamp(rotationRestriction, min, max);
    }

    /// Processes the character movement and the camera rotation every fixed framerate frame.
    private void FixedUpdate()
    {
        // FixedUpdate is used instead of Update because this code is dealing with physics and smoothing.
        RotateCameraAndCharacter();
    }

    private void RotateCameraAndCharacter()
    {
        float rotationX = _rotationX.Update(RotationXRaw, rotationSmoothness);
        float rotationY = _rotationY.Update(RotationYRaw, rotationSmoothness);

        float clampedY = RestrictVerticalRotation(rotationY);
        _rotationY.Current = clampedY;

        Vector3 worldUp = gunCam.InverseTransformDirection(Vector3.up);
        Quaternion rotation = gunCam.rotation *
                        Quaternion.AngleAxis(rotationX, worldUp) *
                        Quaternion.AngleAxis(clampedY, Vector3.left);
        transform.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
        gunCam.rotation = rotation;
    }

    /// Returns the target rotation of the camera around the y axis with no smoothing.
    private float RotationXRaw
    {
        get { return Input.GetAxisRaw("Mouse X") * mouseSensitivity; }
    }

    /// Returns the target rotation of the camera around the x axis with no smoothing.
    private float RotationYRaw
    {
        get { return Input.GetAxisRaw("Mouse Y") * mouseSensitivity; }
    }

    /// Clamps the rotation of the camera around the x axis
    /// between the <see cref="minVerticalAngle"/> and <see cref="maxVerticalAngle"/> values.
    private float RestrictVerticalRotation(float mouseY)
    {
        var currentAngle = NormalizeAngle(gunCam.eulerAngles.x);
        var minY = minVerticalAngle + currentAngle;
        var maxY = maxVerticalAngle + currentAngle;

        return Mathf.Clamp(mouseY, minY + 0.01f, maxY - 0.01f);
    }

    /// Normalize an angle between -180 and 180 degrees.
    /// <param name="angleDegrees">angle to normalize</param>
    /// <returns>normalized angle</returns>
    private static float NormalizeAngle(float angleDegrees)
    {
        while (angleDegrees > 180f)
        {
            angleDegrees -= 360f;
        }

        while (angleDegrees <= -180f)
        {
            angleDegrees += 360f;
        }

        return angleDegrees;
    }

    /// A helper for assistance with smoothing the camera rotation.
    private class SmoothRotation
    {
        private float _current;
        private float _currentVelocity;

        public SmoothRotation(float startAngle)
        {
            _current = startAngle;
        }

        /// Returns the smoothed rotation.
        public float Update(float target, float smoothTime)
        {
            return _current = Mathf.SmoothDampAngle(_current, target, ref _currentVelocity, smoothTime);
        }

        public float Current
        {
            set { _current = value; }
        }

    }
}
