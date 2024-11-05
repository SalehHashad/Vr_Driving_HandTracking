using Oculus.Interaction;
using UnityEngine;

public class SpeedJoystickWrapper : MonoBehaviour
{
    private OneGrabRotateTransformer rotateTransformer;
    private float currentAngle;
    //public float DeadZone = 5f;
    //public float MaxAngle = 30f;

    public float NeutralAngle = 270f; // Set this to 270 in the Inspector
    public float MaxRotation = 45f;

    private void Awake()
    {
        rotateTransformer = GetComponent<OneGrabRotateTransformer>();
    }

    void Start()
    {
        if (rotateTransformer == null)
        {
            Debug.LogError("OneGrabRotateTransformer not found on this GameObject");
        }
    }

    void Update()
    {
        currentAngle = transform.localEulerAngles.x;
        if (currentAngle > 180)
        {
            currentAngle -= 360;
        }
        //Debug.Log($"Current Raw Angle: {currentAngle}");
    }

    public float GetSpeedInput()
    {
        float normalizedInput = currentAngle / MaxRotation;
        normalizedInput = Mathf.Clamp(normalizedInput, -1f, 1f);
        //Debug.Log($"Speed Input: {normalizedInput}");
        return normalizedInput;
    }

    public bool ShouldApplyBrake()
    {
        return currentAngle <= -40f;
    }
}