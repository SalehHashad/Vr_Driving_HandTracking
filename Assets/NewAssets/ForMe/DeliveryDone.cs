using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryDone : MonoBehaviour
{

    public GameObject ClientCanvasResponse;
    public GameObject pizzaModel;
    private void Awake()
    {
        //ClientCanvasResponse = FindObjectOfType<CanvasClient>().gameObject;
        //pizzaModel = FindObjectOfType<PizzaModel_Tag>(true).gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Client"))
        {
            Debug.Log("Reached to the client");
            pizzaModel.SetActive(true);
            ClientCanvasResponse.SetActive(true);
        }
    }
}
