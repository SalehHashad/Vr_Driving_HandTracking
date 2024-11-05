using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveOrder : MonoBehaviour
{

    GameObject ChiefObject;

    private void Awake()
    {
        ChiefObject = FindObjectOfType<Chief_Tag>().gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Captin"))
        {
            Debug.Log("Reached");
            ChiefObject.GetComponent<MoveToTarget>().enabled = true;
        }
    }
}
