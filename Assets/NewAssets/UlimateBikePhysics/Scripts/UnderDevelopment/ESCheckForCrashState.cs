using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESCheckForCrashState : MonoBehaviour
{
    // Start is called before the first frame update
    // Update is called once per frame
    [Header("OPtimization")]
    public bool OptimizeForMobile = false;
    public float UpdateSpeed = 5f;
    [SerializeField] private Transform player;
    [SerializeField] private MeshRenderer mesh;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (mesh == null)
            mesh = transform.GetComponentInChildren<MeshRenderer>();

        //
        if (OptimizeForMobile)
            InvokeRepeating("UpdateVehicleState", 1f, UpdateSpeed);
    }
    void Update()
    {
        CheckForFlip();
    }
    void CheckForFlip()
    {
        if (Vector3.Dot(transform.up, Vector3.down) > 0)
        {
            this.gameObject.SetActive(false);
        }
        //check for side ways
        else if (Mathf.Abs(Vector3.Dot(transform.up, Vector3.down)) < 0.125f)
        {
            this.gameObject.SetActive(false);
        }
        else if (Mathf.Abs(Vector3.Dot(transform.right, Vector3.down)) > 0.825f)
        {
            this.gameObject.SetActive(false);
        }
        //
    }
    //
    void UpdateVehicleState()
    {
        if (mesh == null) return;
        if (player == null) return;
        if (Vector3.Distance(player.position, this.transform.position) > 100)
        {
            if (!mesh.isVisible)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
