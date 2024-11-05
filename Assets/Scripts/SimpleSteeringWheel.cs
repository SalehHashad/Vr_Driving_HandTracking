using UnityEngine;
using Oculus.Interaction;

public class SimpleSteeringWheel : MonoBehaviour
{
    [Header("Rotation Limits")]
    public float MinAngle = -45f;
    public float MaxAngle = 45f;

    [Header("Return to Center")]
    public bool ReturnToCenter = true;
    public float ReturnSpeed = 90f; // degrees per second

    [Header("Steering Output")]
    [SerializeField, Range(-1f, 1f)]
    private float steeringAngle = 0f;
    public float SteeringAngle => steeringAngle;

    private Grabbable grabbable;
    private Quaternion initialRotation;
    private float currentAngle = 0f;
    private float lastFrameAngle = 0f;
    private bool wasGrabbedLastFrame = false;

    private void Start()
    {
        grabbable = GetComponent<Grabbable>();
        initialRotation = transform.localRotation;
        lastFrameAngle = initialRotation.eulerAngles.y;
    }

    private void Update()
    {
        bool isGrabbed = IsGrabbed();

        if (isGrabbed)
        {
            UpdateRotation(wasGrabbedLastFrame);
        }
        else if (ReturnToCenter)
        {
            ReturnToCenterRotation();
        }

        wasGrabbedLastFrame = isGrabbed;
    }

    private bool IsGrabbed()
    {
        return grabbable != null && grabbable.GrabPoints.Count > 0;
    }

    private void UpdateRotation(bool wasGrabbedLastFrame)
    {
        float rawAngle = transform.localEulerAngles.y;

        if (!wasGrabbedLastFrame)
        {
            // Just grabbed, initialize lastFrameAngle
            lastFrameAngle = rawAngle;
        }

        float deltaAngle = Mathf.DeltaAngle(lastFrameAngle, rawAngle);
        currentAngle += deltaAngle;
        currentAngle = Mathf.Clamp(currentAngle, MinAngle, MaxAngle);

        // Apply the clamped rotation
        transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);

        steeringAngle = Mathf.Clamp(currentAngle / (MaxAngle / 2), -1f, 1f);
        lastFrameAngle = transform.localEulerAngles.y;

        Debug.Log($"Grabbed. Current Angle: {currentAngle}, Steering Angle: {steeringAngle}");
    }

    private void ReturnToCenterRotation()
    {
        if (Mathf.Abs(currentAngle) > 0.1f)
        {
            currentAngle = Mathf.MoveTowards(currentAngle, 0f, ReturnSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
            steeringAngle = Mathf.Clamp(currentAngle / (MaxAngle / 2), -1f, 1f);
            lastFrameAngle = transform.localEulerAngles.y;
            Debug.Log($"Returning to center. Current Angle: {currentAngle}, Steering Angle: {steeringAngle}");
        }
        else
        {
            transform.localRotation = initialRotation;
            currentAngle = 0f;
            steeringAngle = 0f;
            lastFrameAngle = initialRotation.eulerAngles.y;
            Debug.Log("Returned to center.");
        }
    }
}