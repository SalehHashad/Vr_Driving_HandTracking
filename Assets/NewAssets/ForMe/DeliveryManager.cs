using UnityEngine;
using UnityEngine.UI;

public class DeliveryManager : MonoBehaviour
{
    public Text timerText;
    public float deliveryTime = 60f;
    private float remainingTime;

    void Start()
    {
        remainingTime = deliveryTime;
    }

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.Round(remainingTime).ToString();
        }
        else
        {
            // Handle end of delivery time
        }
    }
}
