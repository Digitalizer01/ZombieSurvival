using System.Collections;
using UnityEngine;

public class ZombieSoundManager : MonoBehaviour
{
    public AudioClip[] zombieSounds; // Array de sonidos de zombie
    public float minInterval = 3f; // Intervalo mínimo de tiempo entre sonidos
    public float maxInterval = 10f; // Intervalo máximo de tiempo entre sonidos

    private AudioSource audioSource;

    void Start()
    {
        if (zombieSounds.Length == 0)
        {
            Debug.LogError("No zombie sounds assigned.");
            return;
        }

        // Obtener el componente AudioSource o agregar uno si no existe
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Iniciar la corrutina para reproducir sonidos periódicamente
        StartCoroutine(PlayZombieSounds());
    }

    IEnumerator PlayZombieSounds()
    {
        while (true)
        {
            // Seleccionar un intervalo aleatorio entre minInterval y maxInterval
            float randomInterval = Random.Range(minInterval, maxInterval);

            // Esperar el intervalo de tiempo aleatorio
            yield return new WaitForSeconds(randomInterval);

            // Seleccionar un sonido aleatorio del array
            AudioClip randomSound = zombieSounds[Random.Range(0, zombieSounds.Length)];

            // Reproducir el sonido
            audioSource.PlayOneShot(randomSound);
        }
    }
}
