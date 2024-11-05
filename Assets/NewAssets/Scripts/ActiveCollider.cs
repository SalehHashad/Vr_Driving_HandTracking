using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCollider : MonoBehaviour
{

    public BoxCollider boxCollider;

    private void Start()
    {
        StartCoroutine(SetActiveColliders());
    }


    IEnumerator SetActiveColliders()
    {
        yield return new WaitForSeconds(4);
        Debug.Log("The time is end");
        //gameObject.GetComponent<BoxCollider>().enabled = true;
        boxCollider.enabled = true;
    }


    //private void Update()
    //{
    //    boxCollider.enabled = true;
    //}


}
