using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    public float speed = 10f;
    public float turnSpeed = 100f;
    
    private void Update()
    {
        float moveInput = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.Translate(Vector3.forward * moveInput);
        float turnInput = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, turnInput);
    }
}
