using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZombiesController : MonoBehaviour
{
    public GameObject zombiePrefab; // Prefab del zombie que queremos spawnear
    public List<GameObject> spawnPoints; // Lista de puntos de spawn donde aparecerán los zombies
    public float spawnInterval = 5f; // Intervalo de tiempo entre cada spawn
    public int zombiesPerSpawn = 1; // Cantidad de zombies a spawnear en cada intervalo
    public int maxEnemyCount = 10; // Cantidad máxima de enemigos permitidos

    private void Start()
    {
        // Iniciar la corrutina para spawnear zombies cada cierto intervalo
        StartCoroutine(SpawnZombiesRoutine());
    }

    private IEnumerator SpawnZombiesRoutine()
    {
        while (true)
        {
            // Verificar si la cantidad de enemigos existentes es menor o igual al máximo permitido
            int currentEnemyCount = CountEnemies();
            if (currentEnemyCount <= maxEnemyCount)
            {
                // Esperar el intervalo de tiempo antes de realizar el próximo spawn
                yield return new WaitForSeconds(spawnInterval);

                // Realizar el spawn de zombies
                SpawnZombies();
            }
            else
            {
                // Si hay demasiados enemigos, esperar un poco más antes de revisar de nuevo
                yield return new WaitForSeconds(2f);
            }
        }
    }

    private void SpawnZombies()
    {
        // Verificar que el prefab del zombie esté asignado
        if (zombiePrefab == null)
        {
            Debug.LogError("Zombie prefab is not assigned in SpawnZombiesController.");
            return;
        }

        // Iterar sobre todos los puntos de spawn y spawnear zombies en cada uno
        foreach (GameObject spawnPoint in spawnPoints)
        {
            for (int i = 0; i < zombiesPerSpawn; i++)
            {
                // Calcular una posición aleatoria dentro del área del spawn point
                Vector3 spawnPosition = spawnPoint.transform.position + Random.insideUnitSphere * 2f;

                // Ajustar la altura al nivel del suelo (opcional, dependiendo de la configuración de tu escena)
                RaycastHit hit;
                if (Physics.Raycast(spawnPosition + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity))
                {
                    spawnPosition = hit.point;
                }

                // Spawnear el zombie en la posición calculada
                GameObject newZombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
                // Aquí podrías configurar más propiedades del zombie si es necesario
            }
        }
    }

    private int CountEnemies()
    {
        // Contar la cantidad de enemigos en la escena con el tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies.Length;
    }
}
