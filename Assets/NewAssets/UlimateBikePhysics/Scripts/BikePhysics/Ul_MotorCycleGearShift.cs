using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class Ul_MotorCycleGearShift : MonoBehaviour
{
    [Tooltip("ReadOnly")] public int CurrentGear;
    [Header("GearEffects")]
    public bool GearTransmissionEffect = false;
    public GameObject[] ExhaustFlame;
    [HideInInspector] public GameObject[] HeavyEngineSmoke;
    public float EffectTime = 2f;
    [Header("End")]
    public float ClutchForceFeedBack = 350f;
    public Ul_ThrottleWrist throttleWrist;
    public float WheelieForce = 2500;
    [Range(0, 7)] public int MaxGear = 4;
    [HideInInspector] public float PreviousGearRatio;
    public float CurrentGearRatio;
    public float EngineRpm;
    [HideInInspector] public int InverseGear;

    public float[] gearRatios;
    [HideInInspector] public int CurrentMaxGear;
    public UL_MotorCycleController motorCycleController;
    public Animator CharacterAnimator;
    public float MaxSpeed;
    [Tooltip("Do not touch unless needed to")]
    public float ShiftUpSpeed, ShiftDownSpeed;
    //
    //[SerializeField]
    private float effecttime;
    // [SerializeField]
    private bool ResetEffect;
    //[SerializeField]
    private bool CallGearEffect;

    // Start is called before the first frame update
    void Start()
    {
        motorCycleController = this.GetComponent<UL_MotorCycleController>();
        motorCycleController.TopSpeed = MaxSpeed;
        CurrentGearRatio = gearRatios[0];
        PreviousGearRatio = 1;
        CurrentGear = 1;
        ShiftUpSpeed = MaxSpeed * CurrentGearRatio;
        ShiftDownSpeed = 0.0f;
        InverseGear = MaxGear;
        if (Application.isPlaying)
            motorCycleController.TopSpeed = ShiftUpSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateEngineRPM();
        CalculateGearRatio();
        ShifterUP();
        ShifterDown();
        CharacterAnimator.SetFloat("Accel", motorCycleController.CurrentSpeed);
        if (CallGearEffect)
        {
            CreateGearEffect(EffectTime, true);
        }
    }
    //
    void OnDisable()
    {
        motorCycleController.TopSpeed = MaxSpeed;
    }
    //
    void CalculateEngineRPM()
    {
        if (motorCycleController == null) return;
        // EngineRpm = EngineRpm <= MaxEngineRpm ? EngineRpm = vehiclecontroller.CurrentSpeed * inverseGearRatio : EngineRpm;
        EngineRpm = motorCycleController.CurrentSpeed * PreviousGearRatio;
    }
    //
    void CalculateGearRatio()
    {
        if (CurrentMaxGear != MaxGear)
        {
            System.Array.Resize(ref gearRatios, MaxGear);
            for (int i = 0; i < gearRatios.Length; i++)
            {
                gearRatios[i] = (float)(i + 1) / (float)MaxGear;
            }
            CurrentMaxGear = MaxGear;
        }
        //
    }
    //
    //
    void ShifterUP()
    {
        if (motorCycleController == null || motorCycleController.Accel < 0) return;
        if (motorCycleController.CurrentSpeed > (ShiftUpSpeed - 2))
        {
            ShiftDownSpeed = ShiftUpSpeed;
            if (CurrentGear < MaxGear)
            {
                if (CurrentGear < 2)
                    motorCycleController.MotorRb.AddForceAtPosition(motorCycleController.FrontWheel.transform.up * ClutchForceFeedBack,
                        motorCycleController.FrontWheel.transform.position, ForceMode.Impulse);
                InverseGear--;
                CurrentGear++;
                if (throttleWrist != null)
                    throttleWrist.cmul = 0;

                if (CharacterAnimator != null)
                    CharacterAnimator.GetComponent<Animator>().SetTrigger("Cluch");

                //
                if (CurrentGear <= 3)
                    if (GearTransmissionEffect)
                    {
                        ResetEffect = true;
                        CallGearEffect = true;
                    }
                //

            }
            CurrentGearRatio = gearRatios[CurrentGear - 1];
            PreviousGearRatio = gearRatios[InverseGear - 1];
            ShiftUpSpeed = MaxSpeed * CurrentGearRatio;
            if (Application.isPlaying)
                motorCycleController.TopSpeed = ShiftUpSpeed;
        }
    }
    void ShifterDown()
    {
        if (motorCycleController == null) return;
        if (motorCycleController.CurrentSpeed < (ShiftDownSpeed - 2))
        {
            ShiftUpSpeed = ShiftDownSpeed;
            if (CurrentGear > 1)
            {
                if (throttleWrist != null)
                    throttleWrist.cmul = 0;
                //
                if (CharacterAnimator != null)
                    CharacterAnimator.GetComponent<Animator>().SetTrigger("Cluch");

                InverseGear++;
                CurrentGear--;
            }
            CurrentGearRatio = gearRatios[CurrentGear - 1];
            PreviousGearRatio = gearRatios[InverseGear - 1];
            ShiftDownSpeed = MaxSpeed * CurrentGearRatio;
            if (Application.isPlaying)
                motorCycleController.TopSpeed = ShiftUpSpeed;
        }
    }
    //
    private void CreateGearEffect(float maxtime, bool usesound)
    {

        if (ResetEffect)
        {

            for (int i = 0; i < ExhaustFlame.Length; i++)
            {
                ExhaustFlame[i].GetComponent<ParticleSystem>().Stop();
            }
            effecttime = 0f;
            ResetEffect = false;
        }
        if (effecttime < maxtime)
        {
            effecttime += Time.deltaTime;
            for (int i = 0; i < ExhaustFlame.Length; i++)
            {
                ExhaustFlame[i].GetComponent<ParticleSystem>().Emit(1);
            }

        }
        if (effecttime > maxtime)
        {
            for (int i = 0; i < ExhaustFlame.Length; i++)
            {
                ExhaustFlame[i].GetComponent<ParticleSystem>().Stop();
            }
            effecttime = 0f;
            CallGearEffect = false;
        }
    }
}
