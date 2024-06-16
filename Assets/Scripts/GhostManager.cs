using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityStandardAssets.Vehicles.Car;

public class GhostManager : MonoBehaviour
{
    public float timeBetweenSamples = 0.25f;
    public GhostLapData ghostOwnData;           
    public GhostLapData ghostData;              
    public GhostLapData ghostPlay;              
    public GameObject carToRecord;
    public GameObject carToPlay;
    public bool finish;

    // RECORD VARIABLES
    private bool shouldRecord = false;
    private float totalRecordedTime = 0.0f;
    private float currenttimeBetweenSamples = 0.0f;

    // REPLAY VARIABLES
    private bool shouldPlay = false;
    private float totalPlayedTime = 0.0f;
    private float currenttimeBetweenPlaySamples = 0.0f;
    private int currentSampleToPlay = 0;

    // POSITIONS/ROTATIONS
    private Vector3 lastSamplePosition = Vector3.zero;
    private Quaternion lastSampleRotation = Quaternion.identity;
    private Vector3 nextPosition;
    private Quaternion nextRotation;

    // GHOST FILE
    public string ghostFileName = "BestGhost.json"; // Nombre del archivo de fantasma
    public string replayFileName = "Replay.json"; // Nombre del archivo de fantasma
    private string ghostFilePath; // Ruta completa del archivo de fantasma
    private string replayFilePath; // Ruta completa del archivo de la repetición

    void Awake()
    {
        finish = false;

        // Obtener la referencia al GameController
        GameController gameController = GetComponentInParent<GameController>();

        if (gameController != null)
        {
            ghostFilePath = Application.persistentDataPath + "/" + gameController.ghostFileName;
            replayFilePath = Application.persistentDataPath + "/" + gameController.replayFileName;
        }
        else
        {
            Debug.LogError("GhostManager: GameController reference is not set!");
        }
        ghostOwnData = new GhostLapData();
        ghostData = new GhostLapData();
        ghostPlay = new GhostLapData();
    }

    #region RECORD GHOST DATA
    public void StartRecording()
    {
        Debug.Log("START RECORDING");
        shouldRecord = true;

        // Seteamos los valores iniciales
        totalRecordedTime = 0;
        currenttimeBetweenSamples = 0;

        // Limpiamos el scriptable object
        ghostOwnData.Reset();
    }

    public void StopRecording()
    {
        Debug.Log("STOP RECORDING");
        shouldRecord = false;
        ghostOwnData.AddDuration(totalRecordedTime);

        if (!File.Exists(ghostFilePath))
        {
            // No hay un archivo de fantasma existente, guardar el nuevo fantasma
            SaveGhostData(ghostFilePath);
        }
        else
        {
            // Cargar el archivo de fantasma existente
            GhostLapData existingGhostData = LoadGhostData(ghostFilePath);
            SaveGhostData(replayFilePath);
            PlayerPrefs.SetFloat("PlayerTime", ghostOwnData.GetDuration());
            PlayerPrefs.Save();

            // Verificar si la duración del nuevo fantasma es menor que la del fantasma existente
            if (ghostOwnData.GetDuration() < existingGhostData.GetDuration())
            {
                // La duración del nuevo fantasma es menor, guardar el nuevo fantasma
                shouldPlay = false;
                SaveGhostData(ghostFilePath);
                PlayerPrefs.SetFloat("GhostTime", existingGhostData.GetDuration());
                PlayerPrefs.Save();
                Destroy(carToPlay);
            }
        }
    }
    #endregion

    #region PLAY GHOST DATA
    public void StartPlaying(bool ghost)
    {
        Debug.Log("START PLAYING");
        shouldPlay = true;

        // Seteamos los valores iniciales
        totalPlayedTime = 0;
        currentSampleToPlay = 0;
        currenttimeBetweenPlaySamples = 0;

        // Limpiamos el scriptable object
        ghostData.Reset();
        if(ghost)
        {
            ghostData = LoadGhostData(ghostFilePath);
        }
        else
        {
            carToPlay = carToRecord;
            carToPlay.GetComponent<Rigidbody>().isKinematic = true;

            Component[] components = GetComponents<Component>();

            // Iterar sobre todos los componentes
            foreach (Component component in components)
            {
                // Si el componente no es el Transform, destrúyelo
                if (component is CarController || component is CarUserControl || component is CarAudio)
                {
                    Destroy(component);
                }
            }
            ghostData = LoadGhostData(replayFilePath);
        }

        PlayerPrefs.SetFloat("GhostTime", ghostData.GetDuration());
        PlayerPrefs.Save();
    }

    void StopPlaying()
    {
        Debug.Log("STOP PLAYING");
        shouldPlay = false;
        Destroy(carToPlay);
    }
    #endregion

    #region GHOST DATA FILE
    public void SaveGhostData(string filePath)
    {
        // Guardar el nuevo fantasma en un archivo
        string json = JsonUtility.ToJson(ghostOwnData);
        File.WriteAllText(filePath, json);
    }

    private GhostLapData LoadGhostData(string filePath)
    {
        Debug.Log("CARGA");
        try
        {
            // Cargar el fantasma desde el archivo
            string json = File.ReadAllText(filePath);
            GhostLapData ghostLoadData = ScriptableObject.CreateInstance<GhostLapData>();
            JsonUtility.FromJsonOverwrite(json, ghostLoadData);
            return ghostLoadData;
        }
        catch (Exception e)
        {
            Debug.LogError("Error al cargar los datos del fantasma: " + e.Message);
            return null; // O maneja el error de alguna otra manera, según tu lógica de aplicación.
        }
    }
    #endregion

    private void Update()
    {
        if (shouldRecord)
        {
            try
            {
                // A cada frame incrementamos el tiempo transcurrido 
                totalRecordedTime += Time.deltaTime;
                currenttimeBetweenSamples += Time.deltaTime;

                // Si el tiempo transcurrido es mayor que el tiempo de muestreo
                if (currenttimeBetweenSamples >= timeBetweenSamples)
                {
                    // Guardamos la información para el fantasma
                    ghostOwnData.AddNewData(carToRecord.transform);
                    // Dejamos el tiempo extra entre una muestra y otra
                    currenttimeBetweenSamples -= timeBetweenSamples;
                }
            }
            catch(Exception e)
            {
                Debug.LogException(e, this);
                shouldRecord = false;
            }
        }
        if (shouldPlay)
        {
            try
            {
                // A cada frame incrementamos el tiempo transcurrido 
                totalPlayedTime += Time.deltaTime;
                currenttimeBetweenPlaySamples += Time.deltaTime;

                // Si el tiempo transcurrido es mayor que el tiempo de muestreo
                if (currenttimeBetweenPlaySamples >= timeBetweenSamples)
                {
                    // De cara a interpolar de una manera fluida la posición del coche entre una muestra y otra,
                    // guardamos la posición y la rotación de la anterior muestra
                    lastSamplePosition = nextPosition;
                    lastSampleRotation = nextRotation;

                    // Cogemos los datos del scriptable object
                    ghostData.GetDataAt(currentSampleToPlay, out nextPosition, out nextRotation);

                    // Dejamos el tiempo extra entre una muestra y otra
                    currenttimeBetweenPlaySamples -= timeBetweenSamples;

                    // Incrementamos el contador de muestras
                    currentSampleToPlay++;
                }

                // De cara a crear una interpolación suave entre la posición y rotación entre una muestra y la otra, 
                // calculamos a nivel de tiempo entre muestras el porcentaje en el que nos encontramos
                float percentageBetweenFrames = currenttimeBetweenPlaySamples / timeBetweenSamples;
                Debug.Log(percentageBetweenFrames);

                // Aplicamos un lerp entre las posiciones y rotaciones de la muestra anterior y la siguiente según el procentaje actual.
                carToPlay.transform.position = Vector3.Slerp(lastSamplePosition, nextPosition, percentageBetweenFrames);
                carToPlay.transform.rotation = Quaternion.Slerp(lastSampleRotation, nextRotation, percentageBetweenFrames);
            }
            catch(Exception e)
            {
                Debug.LogException(e, this);
                shouldPlay = false;
            }
        }
        if(!shouldPlay && !shouldRecord)
        {
            finish = true;;
        }
    }
}

