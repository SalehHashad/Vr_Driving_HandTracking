using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;


public class CarDashboardControl : MonoBehaviour
{
    private CarController carController;
    private CarManagment carManagment;

    private bool violationCounted = false;

    [SerializeField] StringEventChannelSo SpeedOver;
    public enum RotateAround { X, Y, Z }

    [Space()]
    public SpeedMotorDial speedDial;

    [System.Serializable]
    public class SpeedMotorDial
    {
        public GameObject dial;
        public float multiplier = 1f;
        public RotateAround rotateAround = RotateAround.Z;
        private Quaternion dialOrgRotation = Quaternion.identity;
        //public Text text;

        public void Init()
        {

            if (dial)
                dialOrgRotation = dial.transform.localRotation;

        }

        public void Update(float value)
        {

            Vector3 targetAxis = Vector3.forward;

            switch (rotateAround)
            {

                case RotateAround.X:

                    targetAxis = Vector3.right;

                    break;

                case RotateAround.Y:

                    targetAxis = Vector3.up;

                    break;

                case RotateAround.Z:

                    targetAxis = Vector3.forward;

                    break;

            }

            dial.transform.localRotation = dialOrgRotation * Quaternion.AngleAxis(-multiplier * value, targetAxis);

            //if (text)
            //    text.text = value.ToString("F0");

        }
    }




    private void Awake()
    {
        carController = GetComponent<CarController>();
        carManagment = GetComponent<CarManagment>();

        speedDial.Init();
    }



    void Start()
    {
    }

    void Update()
    {
        if (!carController)
            return;

        Dials();
        OverSpeed();
    }

    void Dials()
    {
        if (speedDial.dial != null)
            speedDial.Update(carController.CurrentSpeed);
    }

    private void OverSpeed()
    {
        if (carController.CurrentSpeed > 80)
        {
            if (!violationCounted)
            {
                Debug.Log("Your Speed now is Over 7 Km/h");
                carManagment.NumberOfViolations += 1;
                SpeedOver.RaiseEvent(carManagment.NumberOfViolations.ToString());
                violationCounted = true;
                Debug.Log("The total number of speed over Violation is : " + carManagment.NumberOfViolations);
            }
        }
        else
        {
            violationCounted = false;
        }
    }


}
