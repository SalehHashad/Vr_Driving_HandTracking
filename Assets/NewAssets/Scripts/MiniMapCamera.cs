using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    private GameObject player;


    private void Awake()
    {
        player = FindObjectOfType<MyCar_Tag>().gameObject;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x, 40, player.transform.position.z);
        transform.rotation = Quaternion.Euler(90, player.transform.eulerAngles.y, 0);
    }
}
