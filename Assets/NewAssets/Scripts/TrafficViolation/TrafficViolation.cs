using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TrafficViolation : MonoBehaviour
{
    Text numberOfTrafficViolations;
    [SerializeField] private StringEventChannelSo trafficViolation;
    

    private void Awake()
    {
        numberOfTrafficViolations = FindObjectOfType<TrafficViolationText_Tag>().gameObject.GetComponent<Text>();
    }

    private void OnEnable()
    {
        trafficViolation.OnEventRaised += TrafficViolationTextChanged;
    }

    private void OnDisable()
    {
        trafficViolation.OnEventRaised -= TrafficViolationTextChanged;
    }


    private void TrafficViolationTextChanged(string txt)
    {
        numberOfTrafficViolations.text = txt;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}