using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ul_BodyIkRotation : MonoBehaviour
{
    // Start is called before the first frame update
    public enum BikeType
    {
        motorbike,
        Bicycle
    }
    public BikeType _biketype = BikeType.motorbike;

    [Header("Explict to bicycle physics")]
    [Tooltip("Works only for bicycle")]
    [Range(0.00000001f, 100f)]
    public float PedalYposMultipler = 0.01f;
    [Range(0.00000001f, 100f)]
    public float PedalYposDamper = 0.0005f;
    [Range(0.00000001f, 100f)]
    public float PedalImpact = 0.0005f;
    [Header("GeneralSetup")]
    [Tooltip("NeckHinge of the bike")]
    public Transform NeckHinge;
    [Tooltip("How Accurate Should Rider Body Follow The Lerp Angle of Bike")]
    public float Neckmulitplier = 10f;
    [Range(0.001f, 100f)]
    public float YposMultipler = 0.1f;
    public float YHieght = 0.5f;
    public float FowardmaxAngle = 25f;
    [Range(0.001f, 100f)]
    public float AccelMultiplier = 0.1f;
    [Range(0.001f, 100f)]
    public float ZMultiplier = 0.1f;
    public UL_MotorCycleController motorCycleController;

    public Ul_Suspension WheelCollider;
    [HideInInspector]
    public bool isbike, up, down, UpPed;
    //
    private float C_pos, Xrot, ypos, Ycounter;
    private Vector3 StartPoint;

    void Start()
    {
        if (_biketype == BikeType.Bicycle)
        {
            isbike = true;
        }
        StartPoint = this.transform.localPosition;
        Xrot = this.transform.localEulerAngles.x;
        ypos = this.transform.localPosition.y;
        Ycounter = this.transform.localPosition.y;
        up = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 euler = NeckHinge.localEulerAngles;
        Vector3 _euler = this.transform.localEulerAngles;
        //

        if (motorCycleController.CurrentSpeed > 20)
        {
            _euler.y = Input.GetAxis("Horizontal") * Neckmulitplier;
            if (motorCycleController.Accel > 0)
            {
                if (_euler.x < (Xrot + FowardmaxAngle))
                    _euler.x += AccelMultiplier;

                if (_euler.x > (Xrot + FowardmaxAngle))
                    _euler.x = (Xrot + FowardmaxAngle);
            }
            else
            {
                if (_euler.x > (Xrot))
                    _euler.x -= AccelMultiplier;

                if (_euler.x < (Xrot))
                    _euler.x = Xrot;
            }
            _euler.z = -Input.GetAxis("Horizontal") * ZMultiplier;

        }
        else
        {
            if (_euler.x > (Xrot))
                _euler.x -= AccelMultiplier;

            if (_euler.x < (Xrot))
                _euler.x = Xrot;
            //
            _euler.y = euler.y;

        }

        this.transform.localEulerAngles = _euler;



        if (WheelCollider != null)
        {
            if (isbike)
            {
                if (motorCycleController.Accel > 0 && motorCycleController.IsDrifting == false)
                {
                    float Y = ypos;
                    if (Ycounter > (Y + PedalYposMultipler))
                    {
                        down = true;
                        up = false;
                    }
                    // Ycounter = (Y + PedalYposMultipler);
                    if (Ycounter < (Y))
                    {
                        up = true;
                        down = false;
                    }
                    if (up)
                    {
                        Ycounter += motorCycleController.MotorRb.velocity.magnitude * PedalYposDamper;
                        if (Mathf.Abs(motorCycleController.eulerz) < 5)
                        {
                            Vector3 motoeuler = motorCycleController.MotorRb.transform.eulerAngles;
                            motoeuler.z += motorCycleController.MotorRb.velocity.magnitude * Time.smoothDeltaTime * PedalImpact;
                            motorCycleController.MotorRb.transform.eulerAngles = motoeuler;
                        }
                    }
                    if (down)
                    {
                        Ycounter -= motorCycleController.MotorRb.velocity.magnitude * PedalYposDamper;
                        if (Mathf.Abs(motorCycleController.eulerz) < 5)
                        {
                            Vector3 motoeuler = motorCycleController.MotorRb.transform.eulerAngles;
                            motoeuler.z -= motorCycleController.MotorRb.velocity.magnitude * Time.smoothDeltaTime * PedalImpact;
                            motorCycleController.MotorRb.transform.eulerAngles = motoeuler;
                        }
                    }
                    //
                    if (!UpPed)
                    {
                        if (!down)
                        {
                            StartPoint.y = Ycounter;
                        }
                        else
                        {
                            UpPed = true;
                        }
                    }
                    if (UpPed)
                    {
                        float g = ypos;
                        StartPoint.y = (g + YposMultipler);
                    }
                }
                else
                {
                    UpPed = false;
                    StartPoint.y = ypos;
                    Ycounter = ypos;
                }
            }
            Vector3 _pos = this.transform.localPosition;
            if (WheelCollider.m_isgrounded)
            {
                C_pos = StartPoint.y;
            }
            else
            {
                if (C_pos < YHieght)
                    C_pos += YposMultipler;
                if (C_pos > (YHieght + StartPoint.y))
                    C_pos = (YHieght + StartPoint.y);
            }
            _pos.y = C_pos;

            this.transform.localPosition = _pos;
        }


    }
}
