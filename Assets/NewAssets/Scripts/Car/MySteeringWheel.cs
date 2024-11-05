//using UnityEngine;
//using UnityEngine.UI;

//namespace BNG
//{
//    public class MySteeringWheel : GrabbableEvents
//    {
//        private VehicleController vehicleController;

//        [Header("Rotation Limits")]
//        public float MinAngle = -360f;
//        public float MaxAngle = 360f;

//        [Header("Rotation Object")]
//        public Transform RotatorObject;

//        [Header("Rotation Speed")]
//        [Tooltip("How fast to move the wheel towards the target angle. 0 = Instant.")]
//        public float RotationSpeed = 0f;

//        [Header("Return to Center")]
//        public bool ReturnToCenter = false;
//        public float ReturnToCenterSpeed = 45;

//        [Header("Debug Options")]
//        public Text DebugText;


//        protected Vector3 rotatePosition;
//        private Vector3 previousPrimaryPosition;
//        protected Vector3 previousSecondaryPosition;

//        protected float targetAngle;
//        protected float previousTargetAngle;

//        private float steeringAngle;

//        public bool AllowTwoHanded = true;
//        public Grabber PrimaryGrabber
//        {
//            get
//            {
//                return GetPrimaryGrabber();
//            }
//        }
//        public Grabber SecondaryGrabber
//        {
//            get
//            {
//                return GetSecondaryGrabber();
//            }
//        }

//        protected float smoothedAngle;

//        private void Awake()
//        {
//            vehicleController = FindObjectOfType<VehicleController>();
//        }

//        void Update()
//        {
//            if (grab.BeingHeld)
//            {
//                UpdateSteeringAngle();
//            }
//            else if (ReturnToCenter)
//            {
//                ReturnToCenterAngle();
//            }

//            ApplyAngleToSteeringWheel(steeringAngle);
//            //UpdateDebugText();
//            UpdatePreviousAngle(targetAngle);
//            // Pass the normalized steering angle to the VehicleController
//            //float normalizedSteering = Mathf.InverseLerp(MinAngle, MaxAngle, steeringAngle);
//            //vehicleController.SetSteeringAngle(normalizedSteering);
//        }

//        private void UpdateSteeringAngle()
//        {
//            float angleAdjustment = 0f;

//            if (PrimaryGrabber != null)
//            {
//                Vector3 currentPosition = transform.InverseTransformPoint(PrimaryGrabber.transform.position);
//                currentPosition = new Vector3(currentPosition.x, currentPosition.y, 0);

//                if (previousPrimaryPosition != Vector3.zero)
//                {
//                    float angleChange = GetRelativeAngle(currentPosition, previousPrimaryPosition);
//                    steeringAngle = Mathf.Clamp(steeringAngle + angleChange, MinAngle, MaxAngle);
//                }

//                previousPrimaryPosition = currentPosition;
//            }
//            // Add first Grabber
//            //if (PrimaryGrabber)
//            //{
//            //    rotatePosition = transform.InverseTransformPoint(PrimaryGrabber.transform.position);
//            //    rotatePosition = new Vector3(rotatePosition.x, rotatePosition.y, 0);

//            //    Add in the angles to turn
//            //    angleAdjustment += GetRelativeAngle(rotatePosition, previousPrimaryPosition);

//            //    previousPrimaryPosition = rotatePosition;
//            //}


//            if (AllowTwoHanded && SecondaryGrabber != null)
//            {
//                rotatePosition = transform.InverseTransformPoint(SecondaryGrabber.transform.position);
//                rotatePosition = new Vector3(rotatePosition.x, rotatePosition.y, 0);

//                // Add in the angles to turn
//                angleAdjustment += GetRelativeAngle(rotatePosition, previousSecondaryPosition);

//                previousSecondaryPosition = rotatePosition;
//            }

//            if (PrimaryGrabber != null && SecondaryGrabber != null)
//            {
//                angleAdjustment *= 0.5f;
//            }

//            targetAngle -= angleAdjustment;

//            // Instant Rotation
//            if (RotationSpeed == 0)
//            {
//                smoothedAngle = targetAngle;
//            }
//            // Apply smoothing based on RotationSpeed
//            else
//            {
//                smoothedAngle = Mathf.Lerp(smoothedAngle, targetAngle, Time.deltaTime * RotationSpeed);
//            }

//            // Scrub the final results
//            if (MinAngle != 0 && MaxAngle != 0)
//            {
//                targetAngle = Mathf.Clamp(targetAngle, MinAngle, MaxAngle);
//                smoothedAngle = Mathf.Clamp(smoothedAngle, MinAngle, MaxAngle);
//            }

//        }

//        private float GetRelativeAngle(Vector3 position1, Vector3 position2)
//        {
//            //return Vector3.Cross(position1, position2).z < 0 ? -Vector3.Angle(position1, position2) : Vector3.Angle(position1, position2);
//            // Are we turning left or right?
//            if (Vector3.Cross(position1, position2).z < 0)
//            {
//                return -Vector3.Angle(position1, position2);
//            }

//            return Vector3.Angle(position1, position2);
//        }

//        private void ApplyAngleToSteeringWheel(float angle)
//        {
//            RotatorObject.localEulerAngles = new Vector3(0, 0, angle);
//        }

//        private void UpdateDebugText()
//        {
//            if (DebugText)
//            {
//                DebugText.text = $"Steering Angle: {steeringAngle:F2}";
//            }
//        }

//        private void ReturnToCenterAngle()
//        {
//            steeringAngle = Mathf.Lerp(steeringAngle, 0, Time.deltaTime * ReturnToCenterSpeed);
//        }

//        public override void OnGrab(Grabber grabber)
//        {
//            //if (grabber == PrimaryGrabber)
//            //{
//            //    previousPrimaryPosition = transform.InverseTransformPoint(PrimaryGrabber.transform.position);
//            //    previousPrimaryPosition = new Vector3(previousPrimaryPosition.x, previousPrimaryPosition.y, 0);
//            //}
//            if (grabber == SecondaryGrabber)
//            {
//                previousSecondaryPosition = transform.InverseTransformPoint(SecondaryGrabber.transform.position);

//                // Discard the Z value
//                previousSecondaryPosition = new Vector3(previousSecondaryPosition.x, previousSecondaryPosition.y, 0);
//            }
//            // Primary
//            else
//            {
//                previousPrimaryPosition = transform.InverseTransformPoint(PrimaryGrabber.transform.position);

//                // Discard the Z value
//                previousPrimaryPosition = new Vector3(previousPrimaryPosition.x, previousPrimaryPosition.y, 0);
//            }

//        }

//        public Grabber GetPrimaryGrabber()
//        {
//            if (grab.HeldByGrabbers != null)
//            {
//                for (int x = 0; x < grab.HeldByGrabbers.Count; x++)
//                {
//                    Grabber g = grab.HeldByGrabbers[x];
//                    if (g.HandSide == ControllerHand.Right)
//                    {
//                        return g;
//                    }
//                }
//            }

//            return null;
//        }

//        public Grabber GetSecondaryGrabber()
//        {
//            if (grab.HeldByGrabbers != null)
//            {
//                for (int x = 0; x < grab.HeldByGrabbers.Count; x++)
//                {
//                    Grabber g = grab.HeldByGrabbers[x];
//                    if (g.HandSide == ControllerHand.Left)
//                    {
//                        return g;
//                    }
//                }
//            }

//            return null;
//        }
//        public virtual void UpdatePreviousAngle(float angle)
//        {
//            previousTargetAngle = angle;
//        }

//        //public Grabber PrimaryGrabber => grab.HeldByGrabbers?.Find(g => g.HandSide == ControllerHand.Right);
//        //public Grabber PrimaryGrabber1 => grab.HeldByGrabbers?.Find(g => g.HandSide == ControllerHand.Left);
//    }
//}
