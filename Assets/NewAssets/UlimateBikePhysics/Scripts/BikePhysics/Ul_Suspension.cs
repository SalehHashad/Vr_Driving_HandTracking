using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ul_Suspension : MonoBehaviour
{
    public float WheelYOffset = 0.075f;
    public float GroundLevel = 0.01f;
    public float WheelRaduis = 0.25f;
    public float RestLenght = 0.3f;
    public float RiderWeight = 5000f;
    public float SuspensionDistance = 0.02f;
    public float SpringConstant = 20000f;
    public float DamperConstant = 2000f;

    [Header("DeBug Values")]
    public bool m_isgrounded = false;
    [HideInInspector]
    public float MotorTorque;
    [HideInInspector] public float RollTorque;
    //public float mul;
    [HideInInspector] public RaycastHit wheelhit;
    public Rigidbody MotorRb;
    [HideInInspector] public float forwardslip;
    //public Transform FollowObject;
    //public float TractionMultiplier = 100f;
    [HideInInspector] public float TotalDownWardForce;
    //public float Currentdownforce;
    [HideInInspector] public float wheelheight = 1.13f;
    public float HeightAboveGround;
    [HideInInspector] public bool isFront;

    [HideInInspector]
    public float brake;
    private float PreLenght;

    private float CurrentLenght;
    private float SpringVelocity;
    [SerializeField] private float SpringForce;
    [SerializeField] private float Damperforce;
    [HideInInspector] public Vector3 totalforce;
    private Vector3 prevqaut;
    Vector3 bacpos;
    Transform WheelMesh;
    private Vector3 m_previouspos;
    // Start is called before the first frame update
    private void Start()
    {
        if (this.transform.parent.GetComponent<Rigidbody>() != null)
            MotorRb = this.transform.parent.GetComponent<Rigidbody>();
        else if (this.transform.parent.parent.GetComponent<Rigidbody>() != null)
            MotorRb = this.transform.parent.parent.GetComponent<Rigidbody>();
        else
            MotorRb = this.transform.root.GetComponent<Rigidbody>();

        bacpos = transform.position;

        wheelheight = WheelRaduis + RestLenght;
        //
        //if (FollowObject != null)
        //   prevqaut = FollowObject.eulerAngles;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 RayTrans = transform.position;
        RayTrans -= transform.up * WheelYOffset;
        Gizmos.DrawLine(RayTrans, transform.position + (-transform.up) * (RestLenght + WheelRaduis));
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        //Lpos = LeftFootIKTarget.TransformPoint(Vector3.zero);
        // if (FollowObject != null)
        // FollowObject.eulerAngles = prevqaut;

        //Vector3 RayTrans = transform.TransformPoint(Vector3.zero);
        Vector3 RayTrans = transform.position;
        RayTrans -= transform.up * WheelYOffset;
        if (MotorRb.GetComponent<UL_MotorCycleController>().LooseControl) return;
        m_isgrounded = Physics.Raycast(RayTrans, -transform.up, out wheelhit, (wheelheight), Physics.AllLayers, QueryTriggerInteraction.Ignore);

        if (m_isgrounded)
        {
            if (WheelMesh != null)
            {
                WheelMesh.position = wheelhit.point + (transform.up * (wheelheight - 0.013f));
            }
        }
        else
        {
            if (WheelMesh != null)
            {
                Vector3 wheelpoint = transform.position;
                WheelMesh.position = transform.position - (transform.up * SuspensionDistance);

            }
        }

        MotorRb.AddForceAtPosition(Vector3.down * 500, transform.position);
        if (!isFront)
        {
            if (Mathf.Abs(MotorRb.GetComponent<UL_MotorCycleController>().eulerz) > 25)
                MotorRb.AddForceAtPosition(Vector3.down * RiderWeight, transform.position);
        }
        if (m_isgrounded)
        {
            SuspensionForce(wheelhit);
            ApplyTorque();
            GroundLevel = MotorRb.transform.position.y;

        }
        //DownWardForce();
        CalculateHeight();
        ApplySteer();
    }
    //

    //
    public void CalculateHeight()
    {
        HeightAboveGround = MotorRb.transform.position.y - GroundLevel;
    }
    private void SuspensionForce(RaycastHit wheelhit)
    {
        PreLenght = CurrentLenght;
        CurrentLenght = RestLenght - (wheelhit.distance - WheelRaduis);
        float _SpringVelocity = (CurrentLenght - PreLenght) / Time.smoothDeltaTime;
        SpringVelocity = Mathf.Abs(_SpringVelocity) < 4.8831910f ? _SpringVelocity : 2.1167212817f;
        SpringForce = SpringConstant * CurrentLenght;
        Damperforce = DamperConstant * SpringVelocity;

        //
        //Debug.DrawRay(transform.position, -transform.up * (RestLenght + WheelRaduis), Color.red);

        totalforce = transform.up * (SpringForce + Damperforce);

        //print(SpringVelocity);


        //
        MotorRb.AddForceAtPosition(totalforce, transform.position);
    }
    //
    private void ApplyTorque()
    {
        //MotorRb.AddForceAtPosition(transform.forward * MotorTorque, transform.position);
        MotorRb.AddRelativeForce(Vector3.forward * MotorTorque);
        //
    }
    //
    public void ApplyBrake(float BrakeTorque, float Dir)
    {
        //MotorRb.AddRelativeForce(Vector3.forward * BrakeTorque * -Dir);
        MotorRb.AddForceAtPosition(Time.fixedDeltaTime * transform.forward * BrakeTorque * -Dir,
                    transform.position, ForceMode.Impulse);

    }
    //
    private void ApplySteer()
    {
        //
        MotorRb.AddRelativeTorque(Vector3.up * RollTorque);
        //Quaternion.AngleAxis(SteerAngle, Vector3.up);
        //MotorRb.AddForceAtPosition(transform.right * RollTorque, transform.position);
    }
    //
    public void GetWorldPos(Transform _WheelMesh, float angle, float Acceleration)
    {
        //Debug.Log(totalforce.sqrMagnitude);
        //
        WheelMesh = _WheelMesh;
        Vector3 velocity = (transform.position - m_previouspos) / Time.fixedDeltaTime;
        m_previouspos = transform.position;
        //
        Vector3 foward = transform.forward;
        Vector3 sideways = -transform.right;
        //
        Vector3 fwdvelocity = Vector3.Dot(velocity, foward) * foward;
        Vector3 sidewayvelocity = Vector3.Dot(velocity, sideways) * sideways;
        //
        forwardslip = -Mathf.Sign(Vector3.Dot(foward, fwdvelocity) * fwdvelocity.magnitude + (TotalDownWardForce * Mathf.PI / 180.0f * 10f));

        //print(forwardslip);
        if (MotorTorque == 0)
        {
            if (Acceleration > 0)
                TotalDownWardForce += MotorRb.velocity.sqrMagnitude;
            else if (Acceleration < 0)
                TotalDownWardForce -= MotorRb.velocity.sqrMagnitude;

        }
        //        print(MotorRb.velocity.sqrMagnitude);
        if (MotorRb.GetComponent<UL_MotorCycleController>().CurrentSpeed < 20f && MotorRb.GetComponent<UL_MotorCycleController>().Accel > 0)
        {
            if (m_isgrounded)
            {
                if (Acceleration > 0)
                    TotalDownWardForce += MotorTorque / WheelRaduis / 5;
                //
                MotorRb.GetComponent<UL_MotorCycleController>().BackWheelSkidding = true;
            }
        }
        else
        {
            MotorRb.GetComponent<UL_MotorCycleController>().BackWheelSkidding = false;
            if (TotalDownWardForce < 2050)
            {
                if (Acceleration > 0)
                    TotalDownWardForce += MotorTorque / WheelRaduis / 10;
                else if (Acceleration < 0)
                    TotalDownWardForce -= MotorTorque / WheelRaduis / 10;
            }
            else
            {
                if (Acceleration > 0)
                    TotalDownWardForce += MotorRb.velocity.sqrMagnitude;
                else if (Acceleration < 0)
                    TotalDownWardForce -= MotorRb.velocity.sqrMagnitude;
            }
        }
        WheelMesh.localEulerAngles = new Vector3(TotalDownWardForce, 0, 0);
        //used mostly suspenson object.

        //WheelMesh.Rotate(TotalDownWardForce, 0, 0);
    }
}
