using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObstaclesArea : MonoBehaviour
{
    private GameObject player;
    public float checkRadius = 1.0f; // Radius for NavMesh sampling
    public int obstacleAreaMask;     // NavMesh area mask for the obstacle area

    private void Awake()
    {
        player = FindObjectOfType<MyCar_Tag>().gameObject;
        // Convert area index to bit mask
        obstacleAreaMask = 1 << NavMesh.GetAreaFromName("SideWalk");
    }

    private void Update()
    {
        if (IsPlayerInObstacleArea())
        {
            Debug.Log("Player is in the obstacle area.");
        }
        else
        {
            Debug.Log("Player is NOT in the obstacle area.");
        }
    }

    private bool IsPlayerInObstacleArea()
    {
        NavMeshHit hit;
        // Sample the nearest point on the NavMesh to the player's position
        if (NavMesh.SamplePosition(player.transform.position, out hit, checkRadius, NavMesh.AllAreas))
        {
            // Check if the area matches the obstacle area mask
            if ((hit.mask & obstacleAreaMask) != 0)
            {
                return true;
            }
        }
        return false;
    }
}