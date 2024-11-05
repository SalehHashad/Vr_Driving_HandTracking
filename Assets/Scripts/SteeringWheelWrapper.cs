using Oculus.Interaction;
using UnityEngine;

public class SteeringWheelWrapper : MonoBehaviour
{
    private OneGrabRotateTransformer rotateTransformer;
    private Grabbable grabbable;
    private Rigidbody rb;
    private float initialAngle;
    private float currentAngle;
    public float maxRotationAngle = 450f; // Maximum rotation angle in degrees
    public float rotationSpeed = 1f; // Rotation speed multiplier
    public float SteeringAngle { get; private set; }

    private bool wasGrabbed = false;

    private void Awake()
    {
        rotateTransformer = GetComponent<OneGrabRotateTransformer>();
        grabbable = GetComponent<Grabbable>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (rotateTransformer == null || grabbable == null || rb == null)
        {
            Debug.LogError("Required components not found on this GameObject");
            return;
        }
        initialAngle = transform.localEulerAngles.y;
        currentAngle = initialAngle;

        // Set up rotation constraints
        rotateTransformer.Constraints.MinAngle.Value = -maxRotationAngle / 2;
        rotateTransformer.Constraints.MaxAngle.Value = maxRotationAngle / 2;
        rotateTransformer.Constraints.MinAngle.Constrain = true;
        rotateTransformer.Constraints.MaxAngle.Constrain = true;

        // Disable gravity and set to kinematic
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void Update()
    {
        bool isGrabbed = IsGrabbed();

        if (isGrabbed)
        {
            // Get the current rotation from the transformer
            float targetAngle = rotateTransformer.Pivot.localEulerAngles.y;

            // Smoothly interpolate to the target angle
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

            // Apply the new rotation
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currentAngle, transform.localEulerAngles.z);

            UpdateSteeringAngle();
            wasGrabbed = true;
        }
        else if (wasGrabbed)
        {
            // The steering wheel was just released
            ResetSteeringWheel();
            wasGrabbed = false;
        }
    }

    void UpdateSteeringAngle()
    {
        float deltaAngle = Mathf.DeltaAngle(initialAngle, currentAngle);
        SteeringAngle = Mathf.Clamp(deltaAngle / (maxRotationAngle / 2), -1f, 1f);
    }

    void ResetSteeringWheel()
    {
        // Reset angular velocity
        rb.angularVelocity = Vector3.zero;

        // Optionally, you can add a small resistance to return the wheel to center
        float returnSpeed = 5f;
        currentAngle = Mathf.LerpAngle(currentAngle, initialAngle, returnSpeed * Time.deltaTime);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currentAngle, transform.localEulerAngles.z);
    }

    public bool IsGrabbed()
    {
        return grabbable != null && grabbable.GrabPoints.Count > 0;
    }
}