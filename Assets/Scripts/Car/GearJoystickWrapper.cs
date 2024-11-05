using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearJoystickWrapper : MonoBehaviour
{
    private OneGrabRotateTransformer rotateTransformer;
    private float currentAngle;
    

    public float ForwardThreshold = 40f;
    public float ReverseThreshold = -40f;


    private void Awake()
    {
        rotateTransformer = GetComponent<OneGrabRotateTransformer>();

    }
    // Start is called before the first frame update
    void Start()
    {
        if (rotateTransformer == null)
        {
            Debug.LogError("OneGrabRotateTransformer not found on this GameObject");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rotateTransformer != null)
        {
            currentAngle = transform.localEulerAngles.x;
            if (currentAngle > 180f) currentAngle -= 360;
        }
    }

    public int GetGearState()
    {
        //Debug.Log($"Current Gear Angle: {currentAngle}");
        if (currentAngle >= ForwardThreshold)
        {
            Debug.Log("Gear: Forward");
            return -1;
        }
        if (currentAngle <= ReverseThreshold)
        {
            Debug.Log("Gear: Reverse");
            return 1;
        }
        //Debug.Log("Gear: Neutral");
        return 0;
    }
}
