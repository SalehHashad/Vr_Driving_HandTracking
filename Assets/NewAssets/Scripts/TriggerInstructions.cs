using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInstructions : MonoBehaviour
{
    public event Action TaskIsFinished;

    TaskSO task;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "SteeringWheel")
    //    {
    //        Debug.Log("My hand Touch SteeringWheel");
    //        task = ScriptableObject.CreateInstance<TaskSO>();
    //        //task.Model.GetComponent<MeshRenderer>().material = task.OriginalMaterial;   
    //        TaskIsFinished?.Invoke();
    //    }
    //}

}
