using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;
    [HideInInspector]
    public Transform player;
    private ZombieBaseState currentState;
    [HideInInspector]
    public CapsuleCollider capsuleCollider;

    public ZombieSearchState searchState = new ZombieSearchState();
    public ZombieAttackState attackState = new ZombieAttackState();
    public ZombieDeadState deadState = new ZombieDeadState();

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        // Buscar el objeto con la etiqueta "Player"
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object not found. Please ensure the player object has the 'Player' tag.");
        }

        TransitionToState(searchState);
    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    public void TransitionToState(ZombieBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
        PrintState(state); // Imprimir estado actual
    }

    public void SetBool(string parameter, bool value)
    {
        animator.SetBool(parameter, value);
    }

    public void MoveToPlayer()
    {
        if (player != null)
        {
            agent.destination = player.position;
        }
    }

    public void MoveToRandomPoint()
    {
        Vector3 randomPoint = GetRandomPointOnNavMesh();
        agent.destination = randomPoint;
    }

    public void StopMoving()
    {
        agent.isStopped = true;
    }

    public void StartMoving()
    {
        agent.isStopped = false;
    }

    private void PrintState(ZombieBaseState state)
    {
        Debug.Log("Zombie current state: " + state.GetType().Name);
    }

    private Vector3 GetRandomPointOnNavMesh()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas);

        return hit.position;
    }
}
