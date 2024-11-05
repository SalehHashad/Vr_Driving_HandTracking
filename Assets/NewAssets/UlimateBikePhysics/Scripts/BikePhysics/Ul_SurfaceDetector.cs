using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ul_SurfaceDetector : MonoBehaviour
{
    [System.Serializable]
    public class SurfacePreset
    {
        public string SurfaceName = "Normal";
        public string SurfaceTagName = "Untagged";
        public ParticleSystem SurfaceParticles;
        [Tooltip("Enable to use tyre marks")] public bool isEmit;
        [Tooltip("if IsEmit = true")] public Material SurfaceMaterial;
        [Header("Friction Settings")]
        [Range(0.0000000001f, 1f)] public float Dynamicfriction = 0.1f;
        [Range(0.0000000001f, 1f)] public float SideGrip = 0.1f;
        [Range(0.00000001f, 10000.0f)] public float MaxSideSlipForce = 0.00001f;
        [Range(0.00000001f, 10000.0f)] public float MaxFowardSlipForce = 0.0001f;
        [Range(0.00000001f, 10000.0f)] public float MaxSideRotationalForce = 0.00001f;
    }
    public List<SurfacePreset> SurfacePresets = new List<SurfacePreset>(1);
    [Header("Drift Settings")]
    [Range(0.0000000001f, 1f)] public float Driftfriction = 0.89898898f;
    [Range(0.001f, 1f)] public float SideGrip = 1f;
    //public float MaxSideSlipForce = 1000f;
    //public float MaxFowardSlipForce = 2000f;
    //public float MaxSideRotationalForce = 1000f;
    public float RotationDifferenceLimit = 0.2511f;
    public float KillDriftDelay = 0.98f;
    [Header("WheelEffect")]
    public Ul_Suspension front;
    public Ul_Suspension rear;
    private int SurfaceIndex = 0;
    private Material backmat;

    //
    [HideInInspector] public UL_MotorCycleController motorCycleController;
    private void Awake()
    {
        if (rear.GetComponent<UL_BikeSkid>() != null)
        {
            backmat = rear.GetComponent<UL_BikeSkid>().generateskid.material;
        }
    }

    public void Start()
    {
        motorCycleController = this.GetComponent<UL_MotorCycleController>();
    }
    //
    private void EmitTryeMarks()
    {
        for (int i = 0; i < SurfacePresets.Count; ++i)
        {
            if (rear.wheelhit.collider != null)
                if (rear.wheelhit.collider.tag == SurfacePresets[i].SurfaceTagName)
                {
                    SurfaceIndex = i;
                }
        }
        //
    }
    //
    private void Update()
    {
        if (motorCycleController == null) return;
        EmitTryeMarks();
        if (front != null && rear != null)
        {
            if (motorCycleController.IsDrifting)
            {
                if (front.GetComponent<UL_BikeSkid>() != null && rear.GetComponent<UL_BikeSkid>() != null)
                {
                    rear.GetComponent<UL_BikeSkid>().generateskid.material = backmat;
                    front.GetComponent<UL_BikeSkid>().Callskid = true;
                    rear.GetComponent<UL_BikeSkid>().Callskid = true;
                    front.GetComponent<UL_BikeSkid>().generateskid.ApplySound_Smoke = true;
                    rear.GetComponent<UL_BikeSkid>().generateskid.ApplySound_Smoke = true;
                }
            }
            else
            {
                if (front.brake > 0)
                {
                    front.GetComponent<UL_BikeSkid>().generateskid.ApplySound_Smoke = true;
                    if (motorCycleController.CurrentSpeed > 30f)
                        front.GetComponent<UL_BikeSkid>().Callskid = true;
                    else
                        front.GetComponent<UL_BikeSkid>().Callskid = false;
                }
                else
                {
                    front.GetComponent<UL_BikeSkid>().Callskid = false;
                }
                if (motorCycleController.BackWheelSkidding)
                {
                    rear.GetComponent<UL_BikeSkid>().generateskid.ApplySound_Smoke = true;
                    rear.GetComponent<UL_BikeSkid>().generateskid.material = backmat;
                    rear.GetComponent<UL_BikeSkid>().Callskid = true;
                }
                else
                {
                    if (SurfacePresets[SurfaceIndex].isEmit == false)
                    {
                        rear.GetComponent<UL_BikeSkid>().generateskid.material = backmat;
                        rear.GetComponent<UL_BikeSkid>().generateskid.ApplySound_Smoke = true;
                        if (rear.brake > 0)
                        {
                            if (motorCycleController.CurrentSpeed > 30f)
                                rear.GetComponent<UL_BikeSkid>().Callskid = true;
                            else
                                rear.GetComponent<UL_BikeSkid>().Callskid = false;
                        }
                        else
                        {
                            rear.GetComponent<UL_BikeSkid>().Callskid = false;
                        }
                    }
                    else
                    {
                        if (motorCycleController.CurrentSpeed > 0.2f && rear.m_isgrounded == true)
                        {
                            rear.GetComponent<UL_BikeSkid>().generateskid.ApplySound_Smoke = false;
                            rear.GetComponent<UL_BikeSkid>().generateskid.material = SurfacePresets[SurfaceIndex].SurfaceMaterial;
                            rear.GetComponent<UL_BikeSkid>().Callskid = true;
                            //   
                            front.GetComponent<UL_BikeSkid>().generateskid.ApplySound_Smoke = false;
                            front.GetComponent<UL_BikeSkid>().generateskid.material = SurfacePresets[SurfaceIndex].SurfaceMaterial;
                            front.GetComponent<UL_BikeSkid>().Callskid = true;
                        }
                        else
                        {
                            rear.GetComponent<UL_BikeSkid>().Callskid = false;
                            front.GetComponent<UL_BikeSkid>().Callskid = false;
                        }
                    }
                }
            }
        }
    }
}
