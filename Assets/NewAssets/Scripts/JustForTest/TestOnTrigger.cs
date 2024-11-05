using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("hand"))
        {
            Debug.Log("Trigggggeeeeerrrr");
        }
    }
}
