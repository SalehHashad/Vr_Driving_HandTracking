using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManagment : MonoBehaviour
{
    [SerializeField] private LayerMask Wall;
    private bool hasCollided = false;
    private float violationCooldown = 2f; // Cooldown period between violations
    private float lastViolationTime = 0f;
    static int numberOfViolation = 0;

    int numberOfTrafficViolations = 0;

    public StringEventChannelSo TrafficViolation;
    public EnumEventChannelSo HitSoundEffect;
    public EnumEventChannelSo TrafficViolation_SoundEffects;

    
    public int NumberOfViolations
    {
        get { return numberOfViolation; }
        set { numberOfViolation = value; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((Wall & (1 << collision.gameObject.layer)) != 0 && !hasCollided && Time.time - lastViolationTime > violationCooldown)
        {
            Debug.Log("Traffic Violation Detected!");
            hasCollided = true; 
            lastViolationTime = Time.time;
            numberOfViolation++;
            TrafficViolation.RaiseEvent(numberOfViolation.ToString());
            HitSoundEffect.RaiseEvent(SoundType.HitWall);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((Wall & (1 << collision.gameObject.layer)) == 0)
        {
            hasCollided = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Stop") /*&& !hasCollided && Time.time - lastViolationTime > violationCooldown*/)
        {
            numberOfViolation++;
            Debug.Log("The traffic is Red, Dude : " + numberOfViolation);
            TrafficViolation.RaiseEvent(numberOfViolation.ToString());
            TrafficViolation_SoundEffects.RaiseEvent(SoundType.trafficViolation);
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Stop")){
    //        hasCollided = false;
    //    }
    //}


    
}
