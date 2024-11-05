using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRigidBodyManager : MonoBehaviour
{
    public Transform groundTransform;
    private Rigidbody carRigidbody;
    private Collider carCollider;
    private float groundLevel;
    public string handLayerName = "LeftHand"; // The name of the hand layer


    private void Awake()
    {
        carRigidbody =gameObject.GetComponent<Rigidbody>();
        carCollider = gameObject.GetComponent<Collider>();
    }
    void Start()
    {
        
        groundLevel = groundTransform.position.y;

        // Adjust Rigidbody settings
        carRigidbody.mass = 1000f; // Increase mass
        carRigidbody.drag = 1f;    // Increase drag
        carRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        // Set layer for car
        gameObject.layer = LayerMask.NameToLayer("Car");

        // Set collider as trigger if needed (if you want to use triggers instead of physics collisions)
        // carCollider.isTrigger = true;

        // Ignore collisions with hands
        IgnoreHandCollisions();
    }

    void Update()
    {
        // Keep car at ground level
        Vector3 carPosition = transform.position;
        carPosition.y = groundLevel;
        transform.position = carPosition;
    }

    void IgnoreHandCollisions()
    {
        int handLayer = LayerMask.NameToLayer(handLayerName);
        if (handLayer == -1)
        {
            Debug.LogError($"Layer '{handLayerName}' not found. Make sure you have created this layer.");
            return;
        }

        // Ignore collisions between the car and the hand layer
        Physics.IgnoreLayerCollision(gameObject.layer, handLayer);
    }
}
