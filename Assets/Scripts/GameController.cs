using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int Laps = 3;
    public GameObject PlayerCar;
    public TMP_Text TextLap;
    public GameObject Checkpoints;

    // REPLAY
    public GameObject CamerasColliders;
    public GameObject Cameras;

    // REST
    public GameObject MainCamera;

    public string ghostFileName = "BestGhost.json";
    public string replayFileName = "Replay.json";

    private CarCheckpointController playerCarCheckpointController; // Referencia al script CarCheckpointController del coche del jugador

    private List<GameObject> checkpointList; // Lista para almacenar los checkpoints en orden
    private int currentCheckpointIndex = 0; // Índice del checkpoint actual
    private int lapCount = -1; // Contador de vueltas
    private bool lapCompleted = false; // Indica si se ha completado la vuelta actual
    private float startTime; // Tiempo de inicio de la carrera
    private float lapStartTime; // Tiempo de inicio de la vuelta actual

    private bool finish;
    private bool isPaused = false;

    public AudioClip pauseSound;
    public AudioClip raceSong;
    public AudioClip replaySong;
    public AudioClip lapSound;
    private AudioSource audioSource;
    public AudioSource backgroundMusic;

    void Start()
    {
        finish = false;

        // Obtener la referencia al script CarCheckpointController del coche del jugador
        playerCarCheckpointController = PlayerCar.GetComponent<CarCheckpointController>();

        // Obtener la lista de checkpoints del objeto Checkpoints
        checkpointList = new List<GameObject>();
        foreach (Transform checkpointTransform in Checkpoints.transform)
        {
            checkpointList.Add(checkpointTransform.gameObject);
        }

        // Mostrar 0 vueltas al principio
        TextLap.text = "Lap: " + (lapCount+1) + "/" + Laps;
        startTime = Time.time; // Guardar el tiempo de inicio de la carrera

        int gameMode = PlayerPrefs.GetInt("GameMode", 0);

        switch (gameMode)
        {
            case 0: // Normal mode
                DestroyReplayObjects();
                this.gameObject.GetComponent<GhostManager>().StartRecording();
                backgroundMusic.clip = raceSong;
                break;

            case 1: // Replay mode
                DestroyNonReplayObjects();
                this.gameObject.GetComponent<GhostManager>().StartPlaying(false);
                backgroundMusic.clip = replaySong;
                break;

            case 2: // Ghost mode
                DestroyReplayObjects();
                this.gameObject.GetComponent<GhostManager>().StartRecording();
                this.gameObject.GetComponent<GhostManager>().StartPlaying(true);
                backgroundMusic.clip = replaySong;
                break;
        }

        // Reproducir la música de fondo
        backgroundMusic.Play();

        // Obtener el componente AudioSource o agregar uno si no existe
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            // Llamar a un método para gestionar la pausa
            ManagePause();
        }
        
        if(this.gameObject.GetComponent<GhostManager>().finish || finish)
        {
            SceneManager.LoadScene("Results");
        }
        else
        {
            if (playerCarCheckpointController != null)
            {
                currentCheckpointIndex = checkpointList.IndexOf(playerCarCheckpointController.GetLastCheckpointObject());
                // Mostrar el nombre del último checkpoint tocado
                // TextCheckpoint.text = "Last checkpoint: " + playerCarCheckpointController.GetLastCheckpointObject().name + "\n" + "currentCheckpointIndex: " + currentCheckpointIndex + "\n" + "checkpointList.Count: " + checkpointList.Count;

                // Verificar si el jugador ha pasado por todos los checkpoints para completar una vuelta
                if (HasPassedAllCheckpoints())
                {
                    if (!lapCompleted) // Verifica si la vuelta actual ya ha sido completada
                    {
                        lapCount++;
                        audioSource.PlayOneShot(lapSound);
                        TextLap.text = "Lap: " + (lapCount+1) + "/" + Laps;
                        lapCompleted = true; // Marca la vuelta como completada
                        currentCheckpointIndex = 0; // Reinicia el índice del checkpoint actual

                        if (lapCount == Laps) // Si es la tercera vuelta
                        {
                            this.gameObject.GetComponent<GhostManager>().StopRecording();
                            finish = true;
                        }
                    }
                }
                else
                {
                    lapCompleted = false; // Si no se pasaron todos los checkpoints, marca la vuelta como no completada
                }
            }
            else
            {
                lapStartTime = Time.time; // Actualizar el tiempo de inicio de la vuelta actual
            }
        }
    }

    // Método para verificar si el jugador ha pasado por todos los checkpoints en el orden correcto
    private bool HasPassedAllCheckpoints()
    {
        // Verificar si el último checkpoint tocado es el primer checkpoint y el índice del checkpoint actual es 0
        return playerCarCheckpointController.GetLastCheckpointObject() == checkpointList[0] && currentCheckpointIndex == 0;
    }

    private void DestroyReplayObjects()
    {
        Destroy(CamerasColliders);
        Destroy(Cameras);
        SwitchCamera switchCameraComponent = PlayerCar.GetComponent<SwitchCamera>();

        // Verificamos si el componente existe
        if (switchCameraComponent != null)
        {
            // Si existe, lo destruimos
            Destroy(switchCameraComponent);
        }
    }

    private void DestroyNonReplayObjects()
    {
        Destroy(MainCamera);
    }

    void ManagePause()
    {
        audioSource.PlayOneShot(pauseSound);

        if (isPaused)
        {
            Time.timeScale = 0f; // Pausar el tiempo del juego
            backgroundMusic.Pause(); // Pausar la música de fondo
            // Aquí puedes mostrar un menú de pausa o realizar otras acciones necesarias cuando el juego esté pausado
        }
        else
        {
            Time.timeScale = 1f; // Reanudar el tiempo del juego
            backgroundMusic.UnPause(); // Reanudar la música de fondo
            // Aquí puedes ocultar el menú de pausa o realizar otras acciones necesarias cuando el juego se reanuda
        }
    }
}
