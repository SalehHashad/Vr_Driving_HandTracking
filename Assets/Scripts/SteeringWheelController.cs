using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(OneGrabRotateTransformer))]
public class SteeringWheelController : MonoBehaviour
{
    [SerializeField]
    private Transform[] _carWheels;

    [SerializeField]
    private float _wheelRotationMultiplier = 0.5f;

    private OneGrabRotateTransformer _rotateTransformer;

    private void Start()
    {
        _rotateTransformer = GetComponent<OneGrabRotateTransformer>();
    }

    private void Update()
    {
        RotateCarWheels();
    }

    private void RotateCarWheels()
    {
        if (_carWheels == null || _carWheels.Length == 0 || _rotateTransformer == null) return;

        // Access the relative angle from the OneGrabRotateTransformer
        float relativeAngle = _rotateTransformer.Constraints.MinAngle.Value;
        float wheelRotationAngle = relativeAngle * _wheelRotationMultiplier;

        foreach (Transform wheel in _carWheels)
        {
            wheel.localRotation = Quaternion.Euler(0, wheelRotationAngle, 0);
        }
    }

    public void SetCarWheels(Transform[] wheels)
    {
        _carWheels = wheels;
    }

}
