//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//namespace BNG
//{
//    public class GearJoystickController : MonoBehaviour
//    {
//        // [SerializeField] private FloatEventChannelSO gearChangeEvent;
//        // [SerializeField] private FloatEventChannelSO GearMinusOne;

//        public bool isForward = false;
//        public bool isBackward = false;

//        [Header("Grab Object")]
//        public Grabbable JoystickGrabbable;

//        [Header("Movement Speed")]
//        [Tooltip("Set to True to Lerp towards the held hand. Set to False for Instant movement")]
//        public bool UseSmoothLook = true;
//        public float SmoothLookSpeed = 15f;

//        [Header("Hinge X")]
//        public Transform HingeXTransform;
//        public float MinXAngle = -45f;
//        public float MaxXAngle = 45f;

//        [Header("Hinge Y")]
//        public Transform HingeYTransform;
//        public float MinYAngle = -45f;
//        public float MaxYAngle = 45f;

//        [Header("Return To Center")]
//        [Tooltip("How fast to return to center if nothing is holding the Joystick. Set to 0 if you do not wish to Return to Center")]
//        public float ReturnToCenterSpeed = 5f;

//        [Header("Deadzone")]
//        [Tooltip("Any values below this threshold will not be passed to events")]
//        public float DeadZone = 0.001f;
//        public FloatFloatEvent onJoystickChange;
//        public Vector2Event onJoystickVectorChange;

//        [Header("Shown for Debug : ")]

//        public float LeverPercentageX = 0;


//        public float LeverPercentageY = 0;

//        public Vector2 LeverVector;
//        public static float angleX;
//        public float angleY;

//        Quaternion originalRot = Quaternion.identity;

//        private VehicleController vehicleController;

//        private void Awake()
//        {
//            vehicleController = FindObjectOfType<VehicleController>();
//        }

//        void Update()
//        {
//            if (JoystickGrabbable != null)
//            {
//                if (JoystickGrabbable.BeingHeld)
//                {
//                    Transform lookAt = JoystickGrabbable.GetPrimaryGrabber().transform;

//                    // Look towards the Grabber
//                    if (HingeXTransform)
//                    {
//                        originalRot = HingeXTransform.rotation;

//                        HingeXTransform.LookAt(lookAt, Vector3.left);

//                        angleX = HingeXTransform.localEulerAngles.x;
//                        if (angleX > 180)
//                        {
//                            angleX -= 360;
//                        }

//                        HingeXTransform.localEulerAngles = new Vector3(Mathf.Clamp(angleX, MinXAngle, MaxXAngle), 0, 0);

//                        if (UseSmoothLook)
//                        {
//                            Quaternion newRot = HingeXTransform.rotation;
//                            HingeXTransform.rotation = originalRot;
//                            HingeXTransform.rotation = Quaternion.Lerp(HingeXTransform.rotation, newRot, Time.deltaTime * SmoothLookSpeed);
//                        }
//                    }

//                }

//                if (vehicleController.EngineOn == true)
//                {
//                    CallJoystickEvents();
//                }
//            }
//        }

//        public virtual void CallJoystickEvents()
//        {
//            // Call events
//            angleX = HingeXTransform.localEulerAngles.x;
//            if (angleX > 180)
//            {
//                angleX -= 360;
//            }

//            if (angleX > 0)
//            {
//                isBackward = false;
//                isForward = true;
//                Debug.Log("The Max joystick angle is  " + angleX);
//            }
//            else if (angleX < 0)
//            {
//                isForward = false;
//                isBackward = true;
//                Debug.Log("The Min joystick angle is  " + angleX);
//            }
//            else
//            {
//                Debug.Log("The joystick angle is  " + angleX);
//            }

//            LeverPercentageY = (angleX - MinXAngle) / (MaxXAngle - MinXAngle) * 100;

//            // OnJoystickChange(LeverPercentageX, LeverPercentageY);

//            // Lever Vector Changed Event
//            float xInput = Mathf.Lerp(-1f, 1f, LeverPercentageX / 100);
//            float yInput = Mathf.Lerp(-1f, 1f, LeverPercentageY / 100);

//            // Reset any values that are inside the deadzone
//            if (DeadZone > 0)
//            {
//                if (Mathf.Abs(xInput) < DeadZone)
//                {
//                    xInput = 0;
//                }
//                if (Mathf.Abs(yInput) < DeadZone)
//                {
//                    yInput = 0;
//                }
//            }
//            LeverVector = new Vector2(xInput, yInput);
//            // OnJoystickChange(LeverVector);
//            // yInput = -yInput;

//            // Set motor torque input based on joystick Y angle
//            // if (vehicleController != null)
//            // {
//            //     // vehicleController.SetMotorTorqueInput(yInput);
//            // }
//        }

//        // Callback for lever percentage change
//        // public virtual void OnJoystickChange(float leverX, float leverY)
//        // {
//        //     if (onJoystickChange != null)
//        //     {
//        //         onJoystickChange.Invoke(leverX, leverY);
//        //     }
//        // }

//        // public virtual void OnJoystickChange(Vector2 joystickVector)
//        // {
//        //     if (onJoystickVectorChange != null)
//        //     {
//        //         onJoystickVectorChange.Invoke(joystickVector);
//        //     }
//        // }
//    }
//}
