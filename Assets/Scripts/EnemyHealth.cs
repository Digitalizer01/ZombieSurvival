using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private Animator animator;
    private Renderer renderer;

    private ZombieController zombieController;

    public float blinkDuration = 0.1f;
    public int blinkCount = 10;

    public GameObject deathParticlesPrefab;
    public float particlesDuration = 3.0f;

    public GameObject ammoPrefab;
    public GameObject healthPrefab;
    public float dropChance = 0.5f;

    public AudioClip deathSound; // Sonido de muerte
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
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

        zombieController = GetComponentInParent<ZombieController>();

        // Configurar el AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (animator != null)
        {
            animator.SetBool("isHit", true);
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Invoke("ResetHitAnimation", 0.1f);
        }
    }

    void Die()
    {
        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }

        GameDRController gameController = FindObjectOfType<GameDRController>();
        if (gameController != null)
        {
            gameController.IncrementZombiesKilled();
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
            Debug.Log("CapsuleCollider disabled for zombie.");
        }
        else
        {
            Debug.LogWarning("CapsuleCollider not found on zombie.");
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

        if (zombieController != null)
        {
            zombieController.TransitionToState(zombieController.deadState);
        }

        DropRandomItem();
    }

    void ResetHitAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("isHit", false);
        }
    }

    private IEnumerator BlinkAndDestroy()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            SetRendererVisible(false);
            yield return new WaitForSeconds(blinkDuration);
            SetRendererVisible(true);
            yield return new WaitForSeconds(blinkDuration);
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

    private void DropRandomItem()
    {
        float randomValue = UnityEngine.Random.value;
        if (randomValue < dropChance)
        {
            float randomItem = UnityEngine.Random.value;
            if (randomItem < 0.5f && ammoPrefab != null)
            {
                Instantiate(ammoPrefab, transform.position, Quaternion.identity);
            }
            else if (healthPrefab != null)
            {
                Instantiate(healthPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
