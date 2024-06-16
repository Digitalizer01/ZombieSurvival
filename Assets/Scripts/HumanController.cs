using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class HumanController : MonoBehaviour
{
    public float walkSpeed = 1.5f; // Velocidad de caminar
    public float runSpeed = 4f;    // Velocidad de correr
    public float detectionRadius = 10f; // Radio de detección de zombies
    public LayerMask zombieLayer; // Capa que representa a los zombies
    public float walkChangeInterval = 10f; // Intervalo para cambiar el destino mientras camina

    private Animator animator; // Referencia al Animator del personaje
    private NavMeshAgent navMeshAgent; // Referencia al NavMeshAgent
    private bool isWalking = true;  // Indicador de si el humano está caminando
    private Vector3 currentDestination; // Destino actual del NavMeshAgent

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component not found on Human GameObject.");
        }
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not assigned in HumanController.");
        }

        // Configurar velocidad de caminar inicial
        navMeshAgent.speed = walkSpeed;

        // Iniciar la rutina de movimiento continuo
        StartCoroutine(ContinuousMovementRoutine());
    }

    private void Update()
    {
        // Actualizar los parámetros de la animación según el estado de movimiento
        animator.SetBool("walking", isWalking);
    }

    private IEnumerator ContinuousMovementRoutine()
    {
        while (true)
        {
            if (!DetectZombiesInRange())
            {
                if (isWalking)
                {
                    // Cambiar a caminar aleatoriamente
                    currentDestination = GetRandomPointOnNavMesh();
                    navMeshAgent.SetDestination(currentDestination);

                    // Esperar hasta el próximo cambio de destino
                    yield return new WaitForSeconds(walkChangeInterval);
                }
            }
            else
            {
                // Esperar un poco antes de revisar de nuevo si hay zombies
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private bool DetectZombiesInRange()
    {
        // Detectar zombies dentro del radio de detección
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, zombieLayer);

        if (colliders.Length > 0)
        {
            // Cambiar a correr hacia un punto seguro
            isWalking = false;
            navMeshAgent.speed = runSpeed;

            // Encontrar el punto más cercano libre de zombies y dirigirse hacia allí
            Vector3 safePoint = FindSafePoint();
            navMeshAgent.SetDestination(safePoint);

            return true;
        }
        else
        {
            // Si no hay zombies cerca, volver a caminar aleatoriamente
            isWalking = true;
            if(navMeshAgent != null)
                navMeshAgent.speed = walkSpeed;

            return false;
        }
    }

    private Vector3 FindSafePoint()
    {
        // Encontrar un punto aleatorio dentro del NavMesh que esté libre de zombies
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas);

        return hit.position;
    }

    private Vector3 GetRandomPointOnNavMesh()
    {
        // Generar un punto aleatorio dentro del NavMesh
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas);

        return hit.position;
    }
}
