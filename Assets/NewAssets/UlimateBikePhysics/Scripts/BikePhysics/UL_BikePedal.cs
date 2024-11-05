using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UL_BikePedal : MonoBehaviour
{
    // Start is called before the first frame update
    public UL_MotorCycleController motorCycleController;
    
    [HideInInspector] public float pedallar;
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (motorCycleController == null) return;
        Vector3 euler = this.transform.localEulerAngles;

        if (motorCycleController.Accel > 0 && motorCycleController.IsDrifting == false)
        {

            pedallar = motorCycleController.MotorRb.velocity.magnitude;
            euler.x = Mathf.Abs(pedallar);
            transform.Rotate(pedallar, 0, 0);
            //this.transform.localEulerAngles = euler;
        }
        else
        {
            pedallar = 0;
        }

    }
}
