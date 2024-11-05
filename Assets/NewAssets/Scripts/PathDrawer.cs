using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;


[RequireComponent(typeof(LineRenderer))]
public class PathDrawer : MonoBehaviour
{

    public Transform destination;
    public bool recalculatePath;
    public float recalculationTime = 0.1f;

    LineRenderer lineRenderer;
    NavMeshPath path;
    public LayerMask groundLayers;
    float time = 0f;
    bool stopped = false;

    public int segmentsPerCurve = 10;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        path = new NavMeshPath();

        if (lineRenderer.materials.Length == 0)
        {
            lineRenderer.material = Resources.Load("material/path_mat", typeof(Material)) as Material;
        }

        Draw();
    }

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        Draw();
    }

    private void Update()
    {
        if (!stopped) time += Time.deltaTime;
        if (time >= recalculationTime && !stopped)
        {
            time = 0f;
            Draw();
        }
    }

    public void Draw()
    {
        if (destination == null) return;
        stopped = false;

        RaycastHit downHit;
        Vector3 validatedDesPos;
        Vector3 validatedOriginPos;

        if (Physics.Raycast(destination.position, -Vector3.up, out downHit, Mathf.Infinity, groundLayers))
        {
            validatedDesPos = new Vector3(destination.position.x, downHit.transform.position.y, destination.position.z);
        }
        else
        {
            validatedDesPos = destination.position;
        }

        if (Physics.Raycast(transform.position, -Vector3.up, out downHit, Mathf.Infinity, groundLayers))
        {
            validatedOriginPos = new Vector3(transform.position.x, downHit.transform.position.y, transform.position.z);
        }
        else
        {
            validatedOriginPos = transform.position;
        }

        NavMesh.CalculatePath(validatedOriginPos, validatedDesPos, NavMesh.AllAreas, path);
        Vector3[] corners = path.corners;

        lineRenderer.positionCount = corners.Length;
        lineRenderer.SetPositions(corners);
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }
}


/* Using NavMeshAgent
public Transform destination; // The target point
public LineRenderer lineRenderer; // The LineRenderer component
public NavMeshAgent navMeshAgent; // The NavMeshAgent component
public Transform car; // Reference to the car
public float updateInterval = 0.5f; // Update interval in seconds
public float destinationThreshold = 1.0f; // Threshold distance to update the destination

private float nextUpdateTime;

void Start()
{
    if (lineRenderer == null)
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
    }
    if (navMeshAgent == null)
    {
        navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
    }

    lineRenderer.startWidth = 0.1f;
    lineRenderer.endWidth = 0.1f;
    lineRenderer.positionCount = 0;

    nextUpdateTime = Time.time;
}

void Update()
{
    if (Time.time >= nextUpdateTime)
    {
        UpdatePath();
        nextUpdateTime = Time.time + updateInterval;
    }

    MoveAgent();
}

public void SetDestination(Transform newDestination)
{
    destination = newDestination;
    UpdatePath();
}

void UpdatePath()
{
    if (destination == null || navMeshAgent == null)
    {
        return;
    }

    NavMeshPath path = new NavMeshPath();
    navMeshAgent.CalculatePath(destination.position, path);

    if (path.status == NavMeshPathStatus.PathComplete)
    {
        lineRenderer.positionCount = path.corners.Length;
        lineRenderer.SetPositions(path.corners);
    }
    else
    {
        lineRenderer.positionCount = 0;
    }
}

void MoveAgent()
{
    if (car != null)
    {
        float distanceToCar = Vector3.Distance(navMeshAgent.transform.position, car.position);
        if (distanceToCar > destinationThreshold)
        {
            navMeshAgent.SetDestination(car.position);
        }
    }
}
}*/
