using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ul_BikeStunts : MonoBehaviour
{
    // Start is called before the first frame update
    /*this is test script  that was generated to perform stunts it has been put outtta but u are to update this to ur like
    public bool IsStunting;
    public float StuntHeight = 0.3f, StuntAnimationHeight = 7.5f;
    public float Stuntforce = 200f;
    public float ForceMultiplier = 0.5f, looseGripHeight = 0.8f;
    public float RiderHeadDist = .9f;
    //[SerializeField] 
    Transform head;
    //[SerializeField] 
    private float Height, bacforce;
    //

    private void Start()
    {
        bacforce = Stuntforce;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        AnimatedStunted();
        StuntHandleBar();
        bikeStunts();
    }
    //
    private void AnimatedStunted()
    {
        //alright this method will put through on how you can use different animation layer 
        //i will code for only one animation layer and i would which through the animation layers;
        UL_MotorCycleController motorCycleController = this.GetComponent<UL_MotorCycleController>();
        ESGrabVehicle grabVehicle = this.GetComponent<ESGrabVehicle>();
        if (grabVehicle.Player != null)
        {

            grabVehicle.Player.GetComponent<Animator>().SetFloat("Height", Height);
            if (Height > StuntAnimationHeight)
            {
                if (!IsStunting)
                {
                    //temporary disable leg ik system so the stunt animation can play 
                    grabVehicle.Player.GetComponent<ESIkRider>().animatedstunts = true;

                    grabVehicle.Player.GetComponent<Animator>().SetLayerWeight(0, 0);
                    grabVehicle.Player.GetComponent<Animator>().SetLayerWeight(1, 1);
                }
            }
            else if (Height < 3)
            {
                //return ik controls 
                grabVehicle.Player.GetComponent<ESIkRider>().animatedstunts = false;
                //
                grabVehicle.Player.GetComponent<Animator>().SetLayerWeight(1, 0);
                grabVehicle.Player.GetComponent<Animator>().SetLayerWeight(0, 1);
            }
        }
    }
    //
    private void StuntHandleBar()
    {
        RaycastHit myhit = new RaycastHit();
        UL_MotorCycleController motorCycleController = this.GetComponent<UL_MotorCycleController>();
        ESGrabVehicle grabVehicle = this.GetComponent<ESGrabVehicle>();
        if (grabVehicle.Player != null)
        {
            head = grabVehicle.Player.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
            if (grabVehicle.OnSit && head != null && IsStunting)
                if (Physics.Raycast(head.position, transform.up, out myhit, RiderHeadDist))
                {
                    if (myhit.collider != null)
                    {
                        print(myhit.collider.name);
                    }
                    motorCycleController.callreset = true;
                    IsStunting = false;
                }
        }

    }
    //
    private void bikeStunts()
    {
        RaycastHit myhit = new RaycastHit();
        if (Physics.Raycast(this.transform.position, Vector3.down, out myhit, Mathf.Infinity))
        {
            Height = myhit.distance;
        }
        //
        UL_MotorCycleController motorCycleController = this.GetComponent<UL_MotorCycleController>();
        if (motorCycleController != null)
        {
            if (motorCycleController.RearWheel.m_isgrounded && motorCycleController.wheelieaxis > 0 && motorCycleController.FrontWheel.m_isgrounded == false)
            {
                IsStunting = true;
            }
            else
            {
                if (motorCycleController.FrontWheel.m_isgrounded)
                    IsStunting = false;
            }
        }
        //
        if (IsStunting && !motorCycleController.FrontWheel.m_isgrounded && Height > StuntHeight)
        {
            if (Stuntforce < 0)
            {
                Stuntforce = 0;
            }
            if (Stuntforce > 0)
                Stuntforce -= ForceMultiplier;
            if (Mathf.Abs(motorCycleController.Lerp) > 0)
                transform.Rotate(motorCycleController.wheelieaxis * -Stuntforce * Time.deltaTime, 0, 0);
        }
        else
        {
            Stuntforce = bacforce;
        }
    }
    */
}
