using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    

    private float gear = 0;

    [Header("Engine Properties")]
    public float MotorTorque = 500f;
    public float MaxSpeed = 30f;
    public float MaxSteeringAngle = 25f;
    public float BrakeForce = 3000f;

    public bool IsBrake = false;

    [Header("Steering Grabbable")]
    public Grabbable SteeringGrabbable;

    [Header("Steering")]
    public float SteeringReturnSpeed = 100f;
    public SetupSteeringWheel steeringWheel;
    public float maxWheelRotationAngle = 30f;
    public float wheelRotationMultiplier = 1f;
    public AnimationCurve SteeringCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Add this line

    [Header("Engine Status")]
    public bool EngineOn = false;
    public float CrankTime = 0.1f;

    [Header("Vehicle Control")]
    public GearJoystickWrapper GearJoystick;
    public SpeedJoystickWrapper SpeedJoystick;
    private int currentGear = 0; 
    private float currentSpeedInput = 0f;

    public float Acceleration = 10f; 
    public float Deceleration = 20f; 
    public float MaxForwardSpeed = 100f;
    public float MaxReverseSpeed = 50f;

    [Header("Speedometer")]
    public Text SpeedLabel;

    [Header("Audio Setup")]
    public AudioSource EngineAudio;
    public AudioClip IdleSound;
    public AudioClip CrankSound;
    public AudioClip CollisionSound;

    [Header("Wheel Rotation")]
    public float WheelRotationMultiplier = 1f;
    public float MaxWheelRotationAngle = 30f;

    //[HideInInspector]
    public float SteeringAngle = 0;
    [HideInInspector]
    public float MotorInput = 0;
    [HideInInspector]
    public float CurrentSpeed;

    [Header("Wheel Configuration")]
    public List<WheelObject> Wheels;

    Vector3 initialPosition;
    Rigidbody rb;

    protected bool crankingEngine = false;

    
    private float targetSpeed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //steeringWheel = GetComponent<SetupSteeringWheel>();
    }
    void Start()
    {
        
        if (rb != null)
        {
            rb.centerOfMass = new Vector3(0, -0.5f, 0); // Adjust the Y value as needed
        }

        initialPosition = transform.position;
        if (steeringWheel == null)
        {
            Debug.LogError("SteeringWheelWrapper not found on SteeringGrabbable");
        }


        foreach (WheelObject wheel in Wheels)
        {
            wheel.Wheel.suspensionDistance = 0.3f;  // Increased from 0.2f
            wheel.Wheel.forceAppPointDistance = 0.1f;

            JointSpring spring = wheel.Wheel.suspensionSpring;
            spring.spring = 25000f;  // Reduced from 35000
            spring.damper = 3000f;   // Reduced from 4500
            wheel.Wheel.suspensionSpring = spring;

            WheelFrictionCurve forwardFriction = wheel.Wheel.forwardFriction;
            forwardFriction.stiffness = 1.5f;  // Reduced from 2
            wheel.Wheel.forwardFriction = forwardFriction;

            WheelFrictionCurve sidewaysFriction = wheel.Wheel.sidewaysFriction;
            sidewaysFriction.stiffness = 1.5f;  // Reduced from 2
            wheel.Wheel.sidewaysFriction = sidewaysFriction;
        }
    }

    void Update()
    {

        UpdateEngineAudio();

        UpdateGearAndSpeed();

        CheckOutOfBounds();

        UpdateSteering();
        UpdateSpeed();
    }

    void UpdateGearAndSpeed()
    {
        if (GearJoystick != null)
        {
            currentGear = GearJoystick.GetGearState();
        }

        if (SpeedJoystick != null)
        {
            float rawSpeedInput = SpeedJoystick.GetSpeedInput();
            if (SpeedJoystick.ShouldApplyBrake())
            {
                IsBrake = true;
                currentSpeedInput = 0;
            }
            else
            {
                IsBrake = false;

                if ((currentGear == 1 && rawSpeedInput >= 0) || (currentGear == -1 && rawSpeedInput >= 0))
                {
                    if (Mathf.Abs(rawSpeedInput) > Mathf.Abs(currentSpeedInput))
                    {
                        currentSpeedInput = Mathf.MoveTowards(currentSpeedInput, rawSpeedInput, Acceleration * Time.deltaTime);
                    }
                    else
                    {
                        currentSpeedInput = Mathf.MoveTowards(currentSpeedInput, rawSpeedInput, Deceleration * Time.deltaTime);
                    }
                }
                else
                {
                    // If in neutral or wrong direction, gradually slow down
                    currentSpeedInput = Mathf.MoveTowards(currentSpeedInput, 0, Deceleration * Time.deltaTime);
                }
            }
        }
    }
   

    void UpdateSteering()
    {
        if (steeringWheel != null)
        {
            SteeringAngle = steeringWheel.SteeringAngle * MaxSteeringAngle * 0.5f;

            for (int i = 0; i < Wheels.Count; i++)
            {
                if (Wheels[i].ApplySteering)
                {
                    Wheels[i].Wheel.steerAngle = SteeringAngle;
                }
            }
        }
    }


    //void UpdateSteering()
    //{
    //    if (steeringWheel != null)
    //    {
    //        float wheelAngle = steeringWheel.WheelAngle;
    //        // Apply this angle to your wheel colliders
    //        // You might want to adjust this further based on the car's speed (Ackermann steering principle)
    //        SteeringAngle = steeringWheel.SteeringAngle * MaxSteeringAngle;
    //        Debug.Log($"Steering Angle: {SteeringAngle}");
    //    }
    //}
    public virtual void CrankEngine()
    {
        if (crankingEngine || EngineOn)
        {
            return;
        }
        StartCoroutine(crankEngine());
    }

    IEnumerator crankEngine()
    {
        crankingEngine = true;

        if (CrankSound != null)
        {
            EngineAudio.clip = CrankSound;
            EngineAudio.loop = false;
            EngineAudio.Play();
        }

        yield return new WaitForSeconds(CrankTime);

        if (IdleSound != null)
        {
            EngineAudio.clip = IdleSound;
            EngineAudio.loop = true;
            EngineAudio.Play();
        }

        yield return new WaitForEndOfFrame();

        crankingEngine = false;
        EngineOn = true;
    }

    public void ChangeGear(float newGear)
    {
        gear = newGear;
    }

    public virtual void CheckOutOfBounds()
    {
        if (transform.position.y < -500f)
        {
            transform.position = initialPosition;
        }
    }

    void FixedUpdate()
    {
        CurrentSpeed = correctValue(rb.velocity.magnitude * 3.6f);
        UpdateWheelTorque();
        ApplyStabilityControl();
        ApplyDownforce();
    }

    void ApplyDownforce()
    {
        float downforceAmount = 100f;
        rb.AddForce(-transform.up * downforceAmount * rb.velocity.magnitude);
    }

    void ApplyStabilityControl()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        float sidewaysVelocity = localVelocity.x;

        if (Mathf.Abs(sidewaysVelocity) > 2f) // Adjust threshold as needed
        {
            Vector3 counterForce = transform.right * -sidewaysVelocity * 0.5f; // Adjust multiplier as needed
            rb.AddForce(counterForce, ForceMode.Acceleration);
        }
    }

    void UpdateSpeed()
    {
        if (!EngineOn)
        {
            targetSpeed = 0;
        }
        else
        {
            float maxSpeed = currentGear == 1 ? MaxForwardSpeed : (currentGear == -1 ? MaxReverseSpeed : 0);
            targetSpeed = Mathf.Abs(currentSpeedInput) * maxSpeed * Mathf.Sign(currentGear);
        }

        // Calculate the new speed
        float newSpeed = Mathf.MoveTowards(CurrentSpeed, targetSpeed,
            (Mathf.Abs(CurrentSpeed) < Mathf.Abs(targetSpeed) ? Acceleration : Deceleration) * Time.deltaTime);

        // Clamp the new speed to the allowed range before assigning it
        CurrentSpeed = Mathf.Clamp(newSpeed, -MaxReverseSpeed, MaxForwardSpeed);
    }
   

    public void SetTargetSpeed(float speed)
    {
        currentSpeedInput = Mathf.Clamp(speed / MaxForwardSpeed, -1f, 1f);
    }

    //public virtual void UpdateWheelTorque()
    //{
    //    if (!EngineOn)
    //    {
    //        // If the engine isn't started, don't apply any torque
    //        for (int x = 0; x < Wheels.Count; x++)
    //        {
    //            Wheels[x].Wheel.motorTorque = 0;
    //            Wheels[x].Wheel.brakeTorque = BrakeForce;
    //        }
    //        return;
    //    }

    //    float torqueInput = currentSpeedInput * Mathf.Sign(currentGear);
    //    float speedRatio = Mathf.Abs(CurrentSpeed) / (currentGear == 1 ? MaxForwardSpeed : MaxReverseSpeed);
    //    float motorTorque = MotorTorque * torqueInput;

    //    for (int x = 0; x < Wheels.Count; x++)
    //    {
    //        WheelObject wheel = Wheels[x];

    //        if (wheel.ApplySteering)
    //        {
    //            wheel.Wheel.steerAngle = SteeringAngle;
    //        }

    //        if (wheel.ApplyTorque)
    //        {
    //            // Reduce torque as speed approaches max speed
    //            float adjustedTorque = MotorTorque * (1 - speedRatio);
    //            wheel.Wheel.motorTorque = adjustedTorque * torqueInput;
    //        }

    //        if (IsBrake || (currentGear == 0 && Mathf.Abs(CurrentSpeed) < 0.1f))
    //        {
    //            wheel.Wheel.brakeTorque = BrakeForce;
    //        }
    //        else
    //        {
    //            wheel.Wheel.brakeTorque = 0;
    //        }

    //        UpdateWheelVisuals(wheel);
    //    }
    //}

    public virtual void UpdateWheelTorque()
    {
        float torqueInput = currentSpeedInput * Mathf.Sign(currentGear);
        float motorTorque = MotorTorque * torqueInput;

        for (int x = 0; x < Wheels.Count; x++)
        {
            WheelObject wheel = Wheels[x];

            if (wheel.ApplyTorque)
            {
                wheel.Wheel.motorTorque = motorTorque;
            }
            else
            {
                wheel.Wheel.motorTorque = 0;
            }

            if (IsBrake)
            {
                wheel.Wheel.brakeTorque = BrakeForce;
            }
            else
            {
                wheel.Wheel.brakeTorque = 0;
            }

            UpdateWheelVisuals(wheel);
        }
    }


    //public void UpdateWheelVisuals(WheelObject wheel)
    //{
    //    if (wheel != null && wheel.WheelVisual != null)
    //    {
    //        Vector3 position;
    //        Quaternion rotation;
    //        wheel.Wheel.GetWorldPose(out position, out rotation);
    //        wheel.WheelVisual.transform.position = position;

    //        if (wheel.ApplySteering)
    //        {
    //            wheel.WheelVisual.transform.rotation = rotation * Quaternion.Euler(SteeringAngle * WheelRotationMultiplier, 0, 0);
    //            //wheel.WheelVisual.transform.rotation = rotation * Quaternion.Euler(0, SteeringAngle, 0);
    //        }
    //        else
    //        {
    //            wheel.WheelVisual.transform.rotation = rotation;
    //        }

    //        wheel.WheelVisual.transform.Rotate(Vector3.right, wheel.Wheel.rpm / 60 * 360 * Time.deltaTime);
    //    }
    //}
    public virtual void UpdateWheelVisuals(WheelObject wheel)
    {
        if (wheel != null && wheel.WheelVisual != null)
        {
            Vector3 position;
            Quaternion rotation;
            wheel.Wheel.GetWorldPose(out position, out rotation);
            wheel.WheelVisual.transform.position = position;

            if (wheel.ApplySteering)
            {
                // Calculate the steering rotation angle
                float steeringRotationAngle = -SteeringAngle * WheelRotationMultiplier;

                // Clamp the rotation angle
                steeringRotationAngle = Mathf.Clamp(steeringRotationAngle, -MaxWheelRotationAngle, MaxWheelRotationAngle);

                // Create a rotation quaternion for steering
                Quaternion steeringRotation = Quaternion.Euler(0f, steeringRotationAngle, 0f);

                // Combine the steering rotation with the wheel's rotation from physics
                Quaternion finalRotation = steeringRotation * rotation;

                // Apply the final rotation
                wheel.WheelVisual.transform.rotation = finalRotation;
            }
            else
            {
                // For non-steering wheels, just apply the rotation from the wheel collider
                wheel.WheelVisual.transform.rotation = rotation;
            }
        }
    }
    public virtual void SetMotorTorqueInput(float input)
    {
        MotorInput = input;
    }

    public void Brake()
    {
        if (IsBrake)
        {
            MotorInput = 0;
            for (int x = 0; x < Wheels.Count; x++)
            {
                Wheels[x].Wheel.brakeTorque = BrakeForce;
            }
        }
        IsBrake = false;
    }

    public virtual void UpdateEngineAudio()
    {
        if (EngineAudio && EngineOn)
        {
            float maxSpeed = currentGear == 1 ? MaxForwardSpeed : MaxReverseSpeed;
            EngineAudio.pitch = Mathf.Clamp(0.5f + (Mathf.Abs(CurrentSpeed) / maxSpeed), 0.5f, 2f);
        }
    }

    public float GetCurrentSpeed()
    {
        return Mathf.Abs(MotorInput) * MaxSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        float colVelocity = collision.relativeVelocity.magnitude;
        if (colVelocity > 0.1f && CollisionSound != null)
        {
            AudioSource.PlayClipAtPoint(CollisionSound, collision.GetContact(0).point);
        }
    }

    float correctValue(float inputValue)
    {
        return (float)System.Math.Round(inputValue * 1000f) / 1000f;
    }
}

[System.Serializable]
public class WheelObject
{
    public WheelCollider Wheel;
    public Transform WheelVisual;
    public bool ApplyTorque;
    public bool ApplySteering;
}