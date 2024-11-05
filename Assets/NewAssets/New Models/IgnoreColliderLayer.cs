using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreColliderLayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreLayerCollision(0,6);
        Physics.IgnoreLayerCollision(28, 6);
        Physics.IgnoreLayerCollision(29, 6);
        Physics.IgnoreLayerCollision(30, 6);
    }

}
