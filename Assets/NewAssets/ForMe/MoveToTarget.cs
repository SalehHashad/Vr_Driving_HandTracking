using UnityEngine;
using UnityEngine.AI;

public class MoveToTarget : MonoBehaviour
{
    public Transform target; 
    private NavMeshAgent agent;
    private Animator animator;
    private bool hasReachedDestination;
    GameObject DialougeCanvas;
    GameObject Timer;
   // NavmeshPathDraw pathDraw;

    public Transform ClientPointTransform;

    GameObject pizzaModel;
    private void Awake()
    {
        DialougeCanvas = FindObjectOfType<DialougeCanvas_Tag>(true).gameObject;
       // pathDraw = FindObjectOfType<NavmeshPathDraw>();
        pizzaModel = FindObjectOfType<PizzaModel_Tag>().gameObject;
        Timer = FindObjectOfType<DeliveryManager>(true).gameObject;
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        hasReachedDestination = false;
        MoveToDestination();
    }

    void MoveToDestination()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    void Update()
    {
        if (target != null)
        {
            if (!hasReachedDestination && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                hasReachedDestination = true;
                OnDestinationReached();
            }
            else if (hasReachedDestination && agent.remainingDistance > agent.stoppingDistance)
            {
                hasReachedDestination = false;
            }

            UpdateAnimator();
        }
    }

    void UpdateAnimator()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
    }

    void OnDestinationReached()
    {
        Debug.Log("Destination reached!");
        DialougeCanvas.SetActive(true);
        animator.SetBool("Stop", true);
        pizzaModel.SetActive(false);
       // pathDraw.destination = ClientPointTransform;
        Timer.SetActive(true);
    }
}
