using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ul_ThrottleWrist : MonoBehaviour
{
    // Start is called before the first frame update
    [Range(0.00000000000001f, 2)]
    public float WristAccuracy = 0.1f;
    public float Anglemulitplier = 25;
    public float YMod = 0.015f;
    private Vector3 StartPoint;
    [HideInInspector]
    public float cmul;
    void Start()
    {
        StartPoint = this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (cmul > 1)
        {
            cmul = 1;
        }
        if (cmul < 0)
        {
            cmul = 0;
        }
        //
        if (Input.GetAxis("Vertical") > 0)
        {
            cmul += WristAccuracy;
        }
        else
        {
            cmul -= WristAccuracy + 0.5f;
        }
        Vector3 _euler = this.transform.localEulerAngles;

        _euler.x = cmul * -Anglemulitplier;


        this.transform.localEulerAngles = _euler;
        //
        Vector3 Pos = StartPoint;

        Pos.y = Pos.y - (YMod * cmul);

        this.transform.localPosition = Pos;
    }
}
