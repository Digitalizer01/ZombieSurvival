using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject healthBar; // Referencia al RectTransform de la barra de salud
    private RectTransform healthBarRectTransform; // Referencia al RectTransform de la barra de salud
    private Animator animator; // Referencia al Animator
    private bool isImmune = false; // Indica si el jugador es inmortal
    private bool isDead = false; // Indica si el jugador ya ha muerto

    // Sonido de muerte del jugador
    public AudioClip deathSound;
    public AudioClip hitSound; // Sonido al recibir daño
    private AudioSource audioSource;

    // Tiempo antes de cargar la escena GameOver
    public float gameOverDelay = 3.0f;

    void Start()
    {
        currentHealth = maxHealth;
        healthBarRectTransform = healthBar.GetComponent<RectTransform>();
        animator = GetComponent<Animator>(); // Obtiene la referencia al Animator del jugador
        UpdateHealthBar(); // Actualiza la barra de salud al inicio

        // Configurar el AudioSource para reproducir sonidos
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void TakeDamage(int damage)
    {
        if (!isImmune && !isDead) // Solo recibe daño si no es inmortal y no está muerto
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                // Reproducir el sonido de golpe a un volumen bajo
                if (hitSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(hitSound, 0.3f); // Ajustar el volumen aquí (0.3f es un ejemplo, puedes ajustarlo según tus necesidades)
                }

                // Activa la animación de "Hit"
                //animator.SetTrigger("Hit");
            }

            // Actualiza la interfaz gráfica de salud del jugador si es necesario
            UpdateHealthBar();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Actualiza la interfaz gráfica de salud del jugador
        UpdateHealthBar();
    }

    private void Die()
    {
        // Verifica si el jugador ya ha muerto para evitar ejecuciones múltiples
        if (isDead)
        {
            return;
        }

        // Lógica de muerte del jugador
        isDead = true; // Marca al jugador como muerto
        Debug.Log("Player has died.");
        animator.SetBool("Dead", true); // Activa el parámetro "Dead" en el Animator

        // Reproducir sonido de muerte
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Cargar la escena GameOver después de un retardo
        StartCoroutine(LoadGameOverScene());
    }

    private IEnumerator LoadGameOverScene()
    {
        yield return new WaitForSeconds(gameOverDelay);

        // Cargar la escena GameOver si el juego aún no ha terminado
        if (!SceneManager.GetSceneByName("GameOver").isLoaded)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    private void UpdateHealthBar()
    {
        float healthPercentage = (float)currentHealth / maxHealth;
        float newWidth = 266.8604f * healthPercentage;
        float newX = Mathf.Lerp(423.9961f, 245f, healthPercentage);

        healthBarRectTransform.sizeDelta = new Vector2(newWidth, healthBarRectTransform.sizeDelta.y);
        healthBarRectTransform.anchoredPosition = new Vector2(newX, healthBarRectTransform.anchoredPosition.y);
    }

    // Método para activar la inmunidad
    public void ActivateImmunity()
    {
        isImmune = true;
    }

    // Método para desactivar la inmunidad
    public void DeactivateImmunity()
    {
        isImmune = false;
    }

    // Método para verificar si el jugador es inmortal
    public bool IsImmune()
    {
        return isImmune;
    }
}
