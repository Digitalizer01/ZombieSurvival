using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class HumanHealth : MonoBehaviour
{
    private Animator animator;
    private Renderer renderer;

    public GameObject deathParticlesPrefab;
    public float particlesDuration = 3.0f;

    public AudioClip deathSound; // Sonido de muerte
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        renderer = GetComponentInChildren<Renderer>();

        if (animator == null)
        {
            Debug.LogError("No Animator component found on " + gameObject.name);
        }
        if (renderer == null)
        {
            Debug.LogError("No Renderer component found on " + gameObject.name);
        }

        // Configurar el AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void Die()
    {
        if (animator != null)
        {
            animator.SetBool("dead", true);
        }

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = false;
            Debug.Log("CapsuleCollider disabled for human.");
        }
        else
        {
            Debug.LogWarning("CapsuleCollider not found on human.");
        }

        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            Destroy(navMeshAgent);
            Debug.Log("NavMeshAgent destroyed for human.");
        }

        if (deathParticlesPrefab != null)
        {
            GameObject particles = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
            Destroy(particles, particlesDuration);
        }

        // Reproducir el sonido de muerte
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        StartCoroutine(BlinkAndDestroy());
    }

    private IEnumerator BlinkAndDestroy()
    {
        for (int i = 0; i < 10; i++)
        {
            SetRendererVisible(false);
            yield return new WaitForSeconds(0.1f);
            SetRendererVisible(true);
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(gameObject);
    }

    private void SetRendererVisible(bool visible)
    {
        if (renderer != null)
        {
            renderer.enabled = visible;
        }
    }
}
