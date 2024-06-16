using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class WaypointMover : MonoBehaviour
{
    public List<Transform> waypoints;  // Lista de waypoints
    private int currentWaypointIndex = 0;  // Índice del waypoint actual
    private NavMeshAgent agent;  // El NavMeshAgent

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (waypoints.Count > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void Update()
    {
        if (waypoints.Count == 0)
            return;

        // Check if the agent has reached its current destination
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Move to the next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
}
