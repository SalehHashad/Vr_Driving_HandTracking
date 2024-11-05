using UnityEngine;
using Oculus.Interaction;

public class SetupSteeringWheel : MonoBehaviour
{
    [Header("Rotation Limits")]
    public float MaxSteeringWheelAngle = 180f; 

    [Header("Return to Center")]
    public bool ReturnToCenter = false;
    public float ReturnToCenterSpeed = 45f;

    [Header("Steering Output")]
    [SerializeField, Range(-1f, 1f)]
    private float steeringAngle = 0f;
    public float SteeringAngle => steeringAngle;

    [Header("Steering Sensitivity")]
    public float SteeringSensitivity = 0.5f;
    public float DeadZone = 0.08f;
    public float SmoothingFactor = 0.03f;

    private TwoGrabRotateTransformer rotateTransformer;
    private Grabbable grabbable;
    private Quaternion initialRotation;
    private float cumulativeAngle = 0f;
    private float lastFrameAngle = 0f;
    private float smoothedSteeringAngle = 0f;

    private void Awake()
    {
        rotateTransformer = GetComponent<TwoGrabRotateTransformer>();
        grabbable = GetComponent<Grabbable>();
        if (rotateTransformer == null || grabbable == null)
        {
            Debug.LogError("Required components (TwoGrabRotateTransformer or Grabbable) not found on this GameObject");
        }
    }

    private void Start()
    {
        initialRotation = transform.localRotation;
        lastFrameAngle = initialRotation.eulerAngles.y;

        if (rotateTransformer != null)
        {
            TwoGrabRotateTransformer.TwoGrabRotateConstraints constraints = new TwoGrabRotateTransformer.TwoGrabRotateConstraints
            {
                MinAngle = new FloatConstraint { Constrain = true, Value = -MaxSteeringWheelAngle },
                MaxAngle = new FloatConstraint { Constrain = true, Value = MaxSteeringWheelAngle }
            };
            rotateTransformer.InjectOptionalConstraints(constraints);
            rotateTransformer.InjectOptionalRotationAxis(TwoGrabRotateTransformer.Axis.Up);
        }
    }

    private void Update()
    {
        //if (IsGrabbed())
        //{
        //    UpdateSteeringAngle();
        //}
        //else if (ReturnToCenter)
        //{
        //    ReturnToCenterAngle();
        //}

        UpdateSteeringAngle();
    }

    private bool IsGrabbed()
    {
        return grabbable != null && grabbable.GrabPoints.Count > 0;
    }

    private void UpdateSteeringAngle()
    {
        float currentFrameAngle = transform.localEulerAngles.y;
        float deltaAngle = Mathf.DeltaAngle(lastFrameAngle, currentFrameAngle);
        cumulativeAngle += deltaAngle;
        cumulativeAngle = Mathf.Clamp(cumulativeAngle, -MaxSteeringWheelAngle, MaxSteeringWheelAngle);

        float rawSteeringAngle = cumulativeAngle / MaxSteeringWheelAngle;

        //if (Mathf.Abs(rawSteeringAngle) < DeadZone)
        //{
        //    rawSteeringAngle = 0f;
        //}
        //else
        //{
        //    rawSteeringAngle = Mathf.Sign(rawSteeringAngle) * ((Mathf.Abs(rawSteeringAngle) - DeadZone) / (1 - DeadZone));
        //    rawSteeringAngle *= SteeringSensitivity;
        //}
        rawSteeringAngle *= SteeringSensitivity; // Apply sensitivity directly
        rawSteeringAngle = Mathf.Clamp(rawSteeringAngle, -1f, 1f);

      
        steeringAngle = Mathf.Lerp(steeringAngle, rawSteeringAngle, SmoothingFactor);

        lastFrameAngle = currentFrameAngle;
    }



    private void ReturnToCenterAngle()
    {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, initialRotation, Time.deltaTime * ReturnToCenterSpeed);
        UpdateSteeringAngle();
    }
}