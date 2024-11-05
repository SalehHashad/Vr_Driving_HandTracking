using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(UL_MotorCycleControl))]
public class UL_MotorCycleController : MonoBehaviour
{

    public enum ControllerType
    {
        AI,
        PlayerCotrol
    }
    public ControllerType GetControllerType = ControllerType.PlayerCotrol;
    #region  PlayerVariable
    [Header("Bike Setup")]

    //
    [Header(" ")]
    public float OldRot;
    public bool UseInverseTilt = false;
    public float SteerBalanceFactor = .5f;
    [Header("a rigidbody of mass 1000 or higher is recomended")]
    public bool LooseControl;
    public Ul_Suspension FrontWheel;
    public Ul_Suspension RearWheel;
    public Transform FrontWheelMesh, RearWheelMesh;
    public float OverRallTorque = 5000f, Braketorque = 5000f;
    public float ReverseTorque = 500f;
    [Tooltip("above 10000 will do just fine")]
    public float RollTorque = 10000;
    public float SideLerpTorque = 5000f;
    public float MaxLerpAngle = 35f;
    public Rigidbody MotorRb;
    public float Balancingforce = 7000;
    public float CurrentFrictionForce = 0.1f;
    public float KillDriftSpeed = 40f, DriftSpeed = 35f;
    public Ul_SurfaceDetector surfaceDetector;
    public Transform BikeHead;
    public float NeckAngle = 25f;
    public bool IsDrifting;
    [Header("Riders Grip")]
    public float FallAngle = 25f;
    public float FallSpeed = 50f;
    //
    //private
    public float TopSpeed = 110f;
    [HideInInspector] public Vector3 pre_pos, newpos, movement, fwd;
    [HideInInspector] public float SideGrip;
    public float CurrentSpeed, CurrentRotateSpeed;
    [HideInInspector]
    public float Accel;
    [HideInInspector]
    public bool ReturnInputs;
    public float eulerx, eulery, eulerz;

    [HideInInspector] public float Lerp;
    [HideInInspector]
    public float pre_accel_mul;
    [HideInInspector]
    public float shoebrake;
    public int tab;
    private float waituntilrotstop;
    private float RotationDifference;
    private Quaternion oldrot;
    private float DriftCounter;
    private bool SeizeRot;
    private float backbodydrag, backZ, acc;
    float m;
    #endregion

    #region  AI Variable
    [Header("AI")]
    public float fowardsensor = 2f, sidesensor = 2f;

    public Transform TargetParent;
    public Transform TargetNode;
    public float SmoothTargetSpeed = 90f;
    public float SmoothTargetAngle = 10f;
    public float NodeDifference = 3f;
    public float KillBrakeSpeed = 60f;
    public float DistanceApart = 5;
    public bool callreset;
    public Transform TriggerObject;
    public bool TriggerBraking = false;
    public float DistanceCheck;
    public bool BackWheelSkidding;
    //public ESTrafficLghtCtrl trafficlightctrl;
    private float TotalDownWardForce, BackTopSpeed;
    private bool clampbody;
    public float diff;
    private Transform player;
    private Vector3 RelativePoint;
    public float wheelieaxis, frontbrakeaxis;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        MotorRb = this.GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        MotorRb.drag = 0.17f;
        backbodydrag = MotorRb.drag;
        MotorRb.angularDrag = 5.00f;
        FrontWheel.isFront = true;
        BackTopSpeed = TopSpeed;
        if (GetControllerType == ControllerType.AI)
        {
            if (TargetParent != null)
                TargetNode = TargetParent.GetChild(0).transform;
        }
    }
    //
    float ClampingAngleAt(float angle, float from, float to)
    {
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);

        return Mathf.Min(angle, to);
    }
    //
    public float wrapangle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }
    //wheelie
    //
    private void OnCollisionEnter()
    {
        checkeforloosecontrol();
    }

    private void checkeforloosecontrol()
    {
        if (LooseControl) return;
        //if (Mathf.Abs(eulerx) < FallAngle) return;
        if (CurrentSpeed < FallSpeed) return;
        callreset = true;
        LooseControl = true;
    }
    private void DoWheelie()
    {
        //print(wheelieaxis);
        if (Mathf.Abs(eulerx) > 30) return;
        if (Accel == 0) return;
        MotorRb.AddForceAtPosition(FrontWheel.transform.up * this.GetComponent<Ul_MotorCycleGearShift>().WheelieForce * wheelieaxis * Time.deltaTime,
                            FrontWheel.transform.position, ForceMode.Impulse);
    }
    // Update is called once per frame
    void Update()
    {
        if (true)
        {
            DoWheelie();
            newpos = transform.position;
            movement = (newpos - pre_pos);
            //checkdot = (Vector3.Dot(fwd, movement));
            //print(Vector3.Dot(fwd, movement));
            if (Vector3.Dot(fwd, movement) < 0f)
            {
                pre_accel_mul = -1;
            }
            else if ((Vector3.Dot(fwd, movement) > 0f))
            {
                pre_accel_mul = 1;
            }
            if (pre_accel_mul < 0 && Accel < 0)
                backZ = -1;
            else
                backZ = 0;
            //
            if (GetControllerType == ControllerType.PlayerCotrol)
            {
                FrontWheel.GetWorldPos(FrontWheelMesh, eulerz, pre_accel_mul);
                RearWheel.GetWorldPos(RearWheelMesh, eulerz, pre_accel_mul);
            }
            else
            {
                FrontWheel.GetWorldPos(FrontWheelMesh, eulerz, Accel);
                RearWheel.GetWorldPos(RearWheelMesh, eulerz, Accel);
            }
            SteerBalance();
        }
    }
    //
    void LateUpdate()
    {
        if (GetControllerType == ControllerType.PlayerCotrol)
        {
            pre_pos = transform.position;
            fwd = transform.forward;
        }
        else
        {
            UpdateTarget();
        }
    }
    private void ResetWheels()
    {
        clampbody = true;
        //print("Reset");
    }
    public void MotorCycleControl(ESCrossPlatformInputManager crossPlatformInputManager)
    {
        if (GetControllerType == ControllerType.PlayerCotrol)
        {
            Accel = shoebrake > 0 && CurrentSpeed < 1.5f ? 0.0f : crossPlatformInputManager.GetAxis("Vertical", false);
            float mul = UseInverseTilt ? -1 : 1;

            Lerp = mul * crossPlatformInputManager.GetAxis("Horizontal", false);
            shoebrake = crossPlatformInputManager.GetAxis("Jump", false);
            wheelieaxis = crossPlatformInputManager.GetInputAxisRaw(KeyCode.LeftShift);
            frontbrakeaxis = crossPlatformInputManager.GetInputAxisRaw(KeyCode.RightShift);
            FrontWheel.brake = Mathf.Abs(shoebrake) > 0 || Mathf.Abs(frontbrakeaxis) > 0 ? 1 : 0;
            RearWheel.brake = Mathf.Abs(shoebrake);
        }

    }
    void FixedUpdate()
    {
        if (true)
        {
            //AI
            AiBehaviour();
            //PlayerStuff
            if (Accel > 0)
            {
                clampbody = false;
            }
            if (Accel == 0 && CurrentSpeed < 1.5f)
            {
                Invoke("ResetWheels", 1.1f);

            }
            Vector3 dir = transform.position - pre_pos;
            if (LooseControl == false)
            {
                SurfaceManager();
                ApplyMotorTorque();
                SteerControl();
                Drifiting(shoebrake, Lerp);
                if (CurrentSpeed > 2)
                {
                    if (shoebrake > 0)
                        ApplyBrake(shoebrake);
                    else
                        ApplyBrake(0);
                    //
                    if (frontbrakeaxis > 0)
                        ApplyFrontBrake(frontbrakeaxis);
                    else
                        ApplyFrontBrake(0);
                }
                ApplyBrake(0);
                ApplyFrontBrake(0);
                BodyRotation(shoebrake);
            }
            else
            {
                ApplyBrake(0);
                ApplyFrontBrake(0);
                //CalculateRandomForce();
                eulerx = wrapangle(transform.eulerAngles.x);
                eulery = wrapangle(transform.eulerAngles.y);
                eulerz = wrapangle(transform.eulerAngles.z);
                MotorRb.angularDrag = 0.001f;
                //
                Vector3 locangvel = transform.InverseTransformDirection(MotorRb.angularVelocity);
                locangvel.z *= 1f;
                transform.TransformDirection(locangvel);
                //
                Vector3 locvel = transform.InverseTransformDirection(MotorRb.velocity);
                locvel.x *= 0.000000000001f;
                transform.TransformDirection(locvel);
            }
            //
            Vector3 rot = transform.eulerAngles;

            if (!LooseControl)
            {
                //rot.z = ClampingAngleAt(rot.z, -20f, 20f);
                /*
                                if (Accel < 0)
                                    if (CurrentSpeed < 11)
                                        rot.z = ClampingAngleAt(rot.z, -0.001f, 0.001f);
                                    else
                                        rot.z = ClampingAngleAt(rot.z, -MaxLerpAngle, MaxLerpAngle);
                                else
                                    rot.z = ClampingAngleAt(rot.z, -MaxLerpAngle, MaxLerpAngle);
                */

            }
            transform.eulerAngles = rot;
            //
            //
            OreintBodyRot();
        }
        else
        {
            //VehicleSpeed();
        }
    }
    //

    #region  AIStuff
    //
    void AiBehaviour()
    {
        if (GetControllerType == ControllerType.PlayerCotrol) return;
        Accel = shoebrake > 0 ? 0.0f : 1f;
        RelativePoint = transform.InverseTransformPoint(TargetNode.position);
        if (TargetNode != null)
        {
            if (TargetNode.GetComponent<ESNodeMover>().NextNode == null)
            {
                shoebrake = 1;
            }
            else
            {
                shoebrake = Mathf.Abs(RelativePoint.x) > SmoothTargetAngle && CurrentSpeed > KillBrakeSpeed ? 1 : 0.0f;
                frontbrakeaxis = Mathf.Abs(RelativePoint.x) > SmoothTargetAngle && CurrentSpeed > KillBrakeSpeed ? 1 : 0.0f;
            }
        }
        //
        //CheckRotDifferecne
        diff = NodeDifference;
        //
        if (Mathf.Abs(diff) > 3f)
        {
            if (RelativePoint.x > diff)
            {
                // print("TurnRight");
                Lerp = 1;
            }
            else if (RelativePoint.x < -diff)
            {
                //print("TurnLeft");
                Lerp = -1;
            }
            else
            {
                //print("Center");
                Lerp = 0;
            }
        }
        else
        {
            if (RelativePoint.x > diff)
            {
                // print("TurnRight");
                Lerp = .5f;
            }
            else if (RelativePoint.x < -diff)
            {
                //print("TurnLeft");
                Lerp = -.5f;
            }
            else
            {
                //print("Center");
                Lerp = 0;
            }
        }
    }
    //
    void checkfortriggerobject(Collider Other)
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Vector3 playerpositon = Other.transform.position - transform.position;
        float fwddot = Vector3.Dot(fwd, playerpositon);
        //
        Vector3 swd = transform.TransformDirection(Vector3.right);
        Vector3 playersidepositon = Other.transform.position - transform.position;
        float side = Vector3.Dot(swd, playersidepositon);
        if (fwddot > fowardsensor && Mathf.Abs(side) < sidesensor)
        {
            TriggerBraking = true;
            TriggerObject = Other.attachedRigidbody == null ? Other.transform : Other.attachedRigidbody.transform;
        }
    }
    //

    //
    void UpdateTarget()
    {
        if (TargetNode != null)
        {
            DistanceCheck = Vector3.Distance(this.transform.position, TargetNode.position);
            if (DistanceCheck < DistanceApart)
            {
                if (TargetNode.GetComponent<ESNodeMover>().NextNode != null)
                {
                    TargetNode = TargetNode.GetComponent<ESNodeMover>().NextNode;
                }

            }
        }
    }
    //

    #endregion
    //
    //boundries 
    //
    #region  PlayerStuff

    private void OreintBodyRot()
    {
        if (CurrentSpeed < 0.1f)
        {
            if (Mathf.Abs(Accel) > 0)
            {
                MotorRb.drag = backbodydrag;
                return;
            }
            if (!LooseControl)
            {
                if (RearWheel.m_isgrounded || FrontWheel.m_isgrounded)
                {
                    Vector3 bike_euler = transform.eulerAngles;
                    //bike_euler.z = 0;
                    transform.eulerAngles = bike_euler;
                }

                if (RearWheel.m_isgrounded && FrontWheel.m_isgrounded)
                {

                    if (clampbody)
                        MotorRb.drag = 5000f;
                    else
                        MotorRb.drag = backbodydrag;
                }
                else
                {
                    MotorRb.drag = backbodydrag;
                }
            }
            else
            {
                MotorRb.drag = backbodydrag;
                return;
            }
        }
    }
    //

    private void CalculateRandomForce()
    {
        if (LooseControl == false) return;
        if (Mathf.Abs(eulerz) < 35f)
        {
            if (eulerz < 0)
            {
                MotorRb.AddTorque(transform.forward * 1500f * Mathf.Sign(eulerz));
            }
            else if (eulerz > 0)
            {
                MotorRb.AddTorque(transform.forward * -1500f * Mathf.Sign(eulerz));
            }
        }
    }
    //
    void Drifiting(float brakeval, float sideval)
    {
        if (!FrontWheel.m_isgrounded)
        {
            IsDrifting = false;
            return;
        }
        //if (!LooseControl) return;
        RotationDifference = Quaternion.Angle(oldrot, transform.rotation);
        RotationDifference = Mathf.Abs(Lerp) > 0 ? surfaceDetector.RotationDifferenceLimit : RotationDifference;
        if (CurrentSpeed > DriftSpeed)
        {
            if (Mathf.Abs(Accel) > 0 && brakeval > 0 && Mathf.Abs(sideval) > 0)
            {
                IsDrifting = true;
            }
            if (IsDrifting)
            {
                if (CurrentSpeed < KillDriftSpeed || RotationDifference < surfaceDetector.RotationDifferenceLimit || Accel < 0)
                {
                    IsDrifting = false;
                }
            }
        }
        else
        {
            IsDrifting = false;
        }
        if (IsDrifting)
        {
            if (brakeval > 0)
            {
                DriftCounter += Time.deltaTime;
            }
            else
            {
                DriftCounter = 0.0f;
            }
            /*
                        if (RearWheel != null)
                        {
                            //MotorRb.AddForceAtPosition(FrontWheel.transform.right * -Lerp * surfaceDetector.MaxSideRotationalForce, FrontWheel.transform.position);
                            // MotorRb.AddForceAtPosition(RearWheel.transform.right * -Lerp * surfaceDetector.MaxSideRotationalForce,
                            // RearWheel.transform.position);
                            //MotorRb.AddRelativeForce(RearWheel.transform.right * -Input.GetAxis("Horizontal") * surfaceDetector.MaxSideSlipForce);
                            //MotorRb.AddRelativeForce(transform.forward * 1 * (surfaceDetector.MaxFowardSlipForce));
                        }
                        */
        }
        if (!IsDrifting)
        {
            DriftCounter = 0.0f;
        }
        oldrot = transform.rotation;
    }
    //
    void BodyRotation(float shoebrake)
    {
        eulerx = wrapangle(transform.eulerAngles.x);
        eulery = wrapangle(transform.eulerAngles.y);
        eulerz = wrapangle(transform.eulerAngles.z);
        //
        //transform.rotation = Quaternion.AngleAxis(25f * -Lerp, Vector3.forward);
        //pre_accel_mul = Accel > 0 ? 1 : Accel < 0 ? -1 : pre_accel_mul;
        //
        //BikeRigidBody.AddTorque(transform.forward * Lerp * Balancingforce);
        //continue from sign_mag you wanted checker great and less
        float havle = new float();
        havle = CurrentSpeed < 50f ? .5f : CurrentSpeed > 50f && CurrentSpeed < 90f ? 0.45f : 1;
        if (!LooseControl)
        {
            if (Mathf.Abs(Lerp) > 0)
            {

                if (CurrentSpeed > 20)
                {
                    if (Mathf.Abs(eulerz) > 75)
                    {
                        callreset = true;
                        LooseControl = true;
                    }

                    if (Mathf.Abs(eulerz) > MaxLerpAngle)
                    {
                        //
                        m = 0.5f;
                    }
                    else
                    {
                        m = Mathf.Abs(Lerp) > 0 ? 1 : 0;
                    }
                    float slerp = SideLerpTorque;
                    if (RearWheel.HeightAboveGround < 10f)
                        MotorRb.AddRelativeTorque(Vector3.forward * slerp * m * -(Lerp * havle));
                }
                else
                {
                    if (eulerz > 3f)
                    {
                        MotorRb.AddRelativeTorque(Vector3.forward * -Balancingforce);
                    }
                    if (eulerz < -3f)
                    {
                        MotorRb.AddRelativeTorque(Vector3.forward * Balancingforce);
                    }
                }
            }
            else
            {
                if (Mathf.Abs(eulerz) > 3f)
                {
                    if (eulerz > 3f)
                    {
                        MotorRb.AddRelativeTorque(Vector3.forward * -Balancingforce);

                    }
                    if (eulerz < -3f)
                    {
                        MotorRb.AddRelativeTorque(Vector3.forward * Balancingforce);
                    }
                }
                else
                {
                    if (eulerz > 0)
                    {
                        if (CurrentSpeed > .5f)
                            MotorRb.AddRelativeTorque(Vector3.forward * -0.50f);
                        else
                            MotorRb.AddRelativeTorque(Vector3.forward * -500.50f);
                    }
                    if (eulerz < 0)
                    {
                        if (CurrentSpeed > .5f)
                            MotorRb.AddRelativeTorque(Vector3.forward * 0.50f);
                        else
                            MotorRb.AddRelativeTorque(Vector3.forward * 500.50f);
                    }
                }
            }
        }
        BodyDrag(shoebrake);
        TweakSpeed();
        #region ManualBodyDrag
        Vector3 locvel = transform.InverseTransformDirection(MotorRb.angularVelocity);
        locvel.z *= !LooseControl ? 0.0000000001f : 1f;
        if (CurrentSpeed > 0.5f)
        {
            waituntilrotstop = 0.0f;
            SeizeRot = true;
            MotorRb.angularDrag = 5.00f;
            locvel.y *= Mathf.Abs(Lerp) > 0 ? 0.999f : CurrentFrictionForce;
        }
        else
        {
            locvel.y *= 0.000001f;
            if (!LooseControl)
            {
                if (SeizeRot == true)
                {
                    //MotorRb.angularDrag = 1000f;
                    MotorRb.angularDrag = !RearWheel.m_isgrounded || !FrontWheel.m_isgrounded ? 0.01f : 1000f;
                    SeizeRot = false;
                }
                else
                {
                    MotorRb.angularDrag = RearWheel.m_isgrounded && FrontWheel.m_isgrounded && Mathf.Abs(eulerz) < 3f ? 1000f : 0.01f;
                    #region oldJunks
                    /*
                                        if (CurrentRotateSpeed < 0.1f)
                                        {
                                            if (waituntilrotstop < 100f)
                                            {
                                                waituntilrotstop += Time.deltaTime;
                                            }
                                            if (waituntilrotstop > 2f)
                                                MotorRb.angularDrag = 5.00f;
                                        }
                                        else
                                        {
                                            if (Mathf.Abs(eulerz) < 3f)
                                            {
                                                waituntilrotstop = 0.0f;
                                            }
                                            MotorRb.angularDrag = !RearWheel.m_isgrounded || !FrontWheel.m_isgrounded ? 0.01f : 1000f;
                                            //MotorRb.angularDrag = 1000f;
                                        }
                    */
                    #endregion
                }
            }
            else
            {
                MotorRb.angularDrag = 0.001f;
            }
        }
        //locvel.x *= 0.0001f;
        MotorRb.angularVelocity = transform.TransformDirection(locvel);
        #endregion
    }
    //
    public void ApplyBrake(float brakeforce)
    {
        if (Mathf.Abs(eulerx) > 15f) return;
        FrontWheel.ApplyBrake(Braketorque, pre_accel_mul * brakeforce * 0.5f);
        RearWheel.ApplyBrake(Braketorque, pre_accel_mul * brakeforce);

    }
    public void ApplyFrontBrake(float brakeforce)
    {
        /*put outta service
        if (Mathf.Abs(eulerx) > 4.5f) return;
        FrontWheel.ApplyBrake(Braketorque, pre_accel_mul * brakeforce);
        if (Mathf.Abs(eulerx) < 4.5f && CurrentSpeed < 10f && CurrentSpeed > 2f && brakeforce > 0)
            MotorRb.AddForceAtPosition(RearWheel.transform.up * brakeforce * Time.deltaTime,
                       RearWheel.transform.position, ForceMode.Impulse);
                       */
    }
    void ApplyMotorTorque()
    {
        if (RearWheel != null)
        {
            if (backZ < 0)
            {
                if (GetControllerType == ControllerType.PlayerCotrol)
                    RearWheel.MotorTorque = 50 * -ReverseTorque;
            }
            else
            {
                if (shoebrake == 0 && frontbrakeaxis == 0)
                {
                    if (Accel == 0 && pre_accel_mul < 0 && RearWheel.m_isgrounded)
                    {
                        MotorRb.drag = 5000f;
                    }
                    if (Accel > 0)
                    {
                        acc = Accel;
                    }
                    else if (Accel == 0)
                    {
                        if (acc > 0)
                        {
                            acc -= 0.00000000000000000001f;
                        }


                        if (acc < 0)
                            acc = 0;
                    }

                }
                else
                {
                    acc = 0;
                }
                if (RearWheel.m_isgrounded)
                    RearWheel.MotorTorque = Accel * OverRallTorque;
            }
        }
    }
    //
    public void SteerBalance()
    {
        if (Mathf.Abs(OldRot - transform.eulerAngles.y) < 10f)
        {
            var alignturn = (transform.eulerAngles.y - OldRot) * SteerBalanceFactor;
            Quaternion angvelocity = Quaternion.AngleAxis(alignturn, Vector3.up);
            MotorRb.velocity = angvelocity * MotorRb.velocity;
        }
        OldRot = transform.eulerAngles.y;
    }
    //
    void SteerControl()
    {
        if (FrontWheel == null) return;
        if (BikeHead != null)
        {
            Vector3 neckEuler = BikeHead.localEulerAngles;
            if (CurrentSpeed < 50f)
                neckEuler.y = pre_accel_mul < 0 && Accel < 0 ? -NeckAngle * Lerp : NeckAngle * Lerp;
            else
                neckEuler.y = pre_accel_mul < 0 && Accel < 0 ? -5.5f * Lerp : 5.5f * Lerp;

            BikeHead.localEulerAngles = neckEuler;
        }
        float CRoll = 0.0f;
        CRoll = RollTorque;


        if (CurrentSpeed > .5f && FrontWheel.m_isgrounded)
            FrontWheel.RollTorque = CurrentSpeed < 10f && pre_accel_mul < 0 ? CRoll * Lerp : CRoll * Lerp;
    }
    //
    void BodyDrag(float shoebrake)
    {
        Vector3 localvel = transform.InverseTransformDirection(MotorRb.velocity);
        if (!IsDrifting)
        {
            localvel.x *= SideGrip;
        }
        else
        {
            localvel.x *= surfaceDetector.SideGrip;
        }
        if (Mathf.Abs(Accel) == 0)
        {
            localvel.z *= MotorRb.velocity.magnitude < 2f ? 0.000001f : 1f;
        }
        else
        {
            localvel.z *= 1f;
        }
        //
        if (CurrentSpeed < 2)
        {
            if (!IsDrifting)
            {
                localvel.x *= 0.0001f;
            }
        }
        //print(localvel.x);
        MotorRb.velocity = transform.TransformDirection(localvel);
        /*
        if (localvel.x > 0)
        {
            // totalforce.x *= 0f;
        }
        */
        TweakSpeed();
    }
    //
    void SurfaceManager()
    {
        if (RearWheel == null || surfaceDetector == null) return;
        //

        if (RearWheel.wheelhit.collider != null)
        {

            if (surfaceDetector.SurfacePresets.Count < 1) return;
            for (int i = 0; i < surfaceDetector.SurfacePresets.Count; ++i)
            {
                if (RearWheel.wheelhit.collider.tag == surfaceDetector.SurfacePresets[i].SurfaceTagName)
                {
                    //emit particles
                    if (surfaceDetector.SurfacePresets[i].SurfaceParticles != null)
                    {

                        //print(RearWheel.wheelhit.collider.tag);
                        if (CurrentSpeed > 10.3f)
                        {
                            surfaceDetector.SurfacePresets[i].SurfaceParticles.Emit(1);
                            surfaceDetector.SurfacePresets[i].SurfaceParticles.transform.position = RearWheel.wheelhit.point;
                            if (FrontWheel.wheelhit.collider != null)
                                surfaceDetector.SurfacePresets[i].SurfaceParticles.transform.position = FrontWheel.wheelhit.point;
                        }
                        else
                        {
                            surfaceDetector.SurfacePresets[i].SurfaceParticles.Stop();
                        }
                    }
                    // print(RearWheel.wheelhit.collider.tag);
                    if (IsDrifting == false)
                    {
                        MotorRb.AddForceAtPosition(RearWheel.transform.right * -Lerp * surfaceDetector.SurfacePresets[i].MaxSideRotationalForce,
               RearWheel.transform.position);
                        MotorRb.AddRelativeForce(RearWheel.transform.right * Lerp * surfaceDetector.SurfacePresets[i].MaxSideSlipForce);
                        MotorRb.AddRelativeForce(transform.forward * 1 * (surfaceDetector.SurfacePresets[i].MaxFowardSlipForce));
                        if (SideGrip > surfaceDetector.SurfacePresets[i].SideGrip)
                        {
                            SideGrip -= 0.01f;
                        }
                        else
                        {
                            SideGrip = surfaceDetector.SurfacePresets[i].SideGrip;
                        }
                        // SideGrip = surfaceDetector.SurfacePresets[i].SideGrip;
                        CurrentFrictionForce = surfaceDetector.SurfacePresets[i].Dynamicfriction;
                    }
                    else
                    {
                        SideGrip = 1f;
                        CurrentFrictionForce = surfaceDetector.Driftfriction;
                    }
                }
                else
                {
                    if (surfaceDetector.SurfacePresets[i].SurfaceParticles != null)
                    {
                        surfaceDetector.SurfacePresets[i].SurfaceParticles.Stop();
                    }
                }
            }
        }
    }
    public void TweakSpeed()
    {
        //km/h
        float Pi = Mathf.PI * 1.15f;
        float maxrotspeed;
        Vector3 rigidspeed = new Vector3(MotorRb.velocity.x, 0.0f, MotorRb.velocity.z);
        CurrentSpeed = rigidspeed.magnitude * Pi;
        CurrentRotateSpeed = MotorRb.angularVelocity.magnitude * Pi;
        maxrotspeed = 0.0f;
        if (!IsDrifting)
        {
            if (!RearWheel.m_isgrounded && !FrontWheel.m_isgrounded)
            {
                if (RearWheel.m_isgrounded || FrontWheel.m_isgrounded)
                    maxrotspeed = 0f;
            }
            else
            {
                /*basically here if bike speed is less then we full rotation force on bike handle and above 70km we want
                rotation to be a bit clamped inother to acheive smother tilts. 
                */

                if (RearWheel.m_isgrounded || FrontWheel.m_isgrounded)
                    maxrotspeed = CurrentSpeed > 80f ? .09f : CurrentSpeed > 60f && CurrentSpeed < 70f ? .099f : CurrentSpeed > 40f && CurrentSpeed < 60f ? 0.13f : 4.5f;
            }

        }
        else
        {
            maxrotspeed = 5f;
        }
        if (MotorRb.velocity.magnitude > 10)
        {
            if (CurrentSpeed > TopSpeed)
                MotorRb.velocity = (TopSpeed / Pi) * MotorRb.velocity.normalized;
        }
        else
        {
            if (pre_accel_mul < 0)
            {
                maxrotspeed = .9f;
                if (CurrentSpeed > 10f)
                    MotorRb.velocity = (10f / Pi) * MotorRb.velocity.normalized;
            }
        }
        //
        if (CurrentRotateSpeed > maxrotspeed)
        {
            MotorRb.angularVelocity = (maxrotspeed / Pi) * MotorRb.angularVelocity.normalized;
        }
    }
    #endregion
}

