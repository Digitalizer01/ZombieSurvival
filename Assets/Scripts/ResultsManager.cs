using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class ResultsManager : MonoBehaviour
{
    public TMP_Text TextWinLose;
    public TMP_Text TextGhostTime;
    public TMP_Text TextPlayerTime;
    public Button ButtonPlay;
    public Button ButtonReplay;
    public Button ButtonGhost;

    public string ReplayFileName = "Replay.json"; // Nombre del archivo de replay
    public string GhostFileName = "BestGhost.json"; // Nombre del archivo de ghost

    public AudioClip hoverSound; // Sonido al colocar el puntero sobre el botón
    public AudioClip clickSound; // Sonido al hacer clic en el botón

    private AudioSource audioSource;
    private string replayFilePath; // Ruta completa del archivo de replay
    private string ghostFilePath; // Ruta completa del archivo de ghost

    private bool loadingScene = false; // Indica si la escena está cargándose

    void Start()
    {
        TextGhostTime.text = TextGhostTime.text + Math.Round(PlayerPrefs.GetFloat("GhostTime", 0.0f), 2) + " seconds";
        TextPlayerTime.text = TextPlayerTime.text + Math.Round(PlayerPrefs.GetFloat("PlayerTime", 0.0f), 2) + " seconds";

        PlayerPrefs.DeleteAll();

        ButtonPlay.onClick.AddListener(() => SetGameMode(0)); // 0 para jugar normal
        ButtonReplay.onClick.AddListener(() => SetGameMode(1)); // 1 para replay
        ButtonGhost.onClick.AddListener(() => SetGameMode(2)); // 2 para ghost

        // Establecer las rutas completas de los archivos de replay y ghost
        replayFilePath = Path.Combine(Application.persistentDataPath, ReplayFileName);
        ghostFilePath = Path.Combine(Application.persistentDataPath, GhostFileName);

        // Verificar si el archivo de replay existe y deshabilitar el botón si no existe
        if (!File.Exists(replayFilePath))
        {
            ButtonReplay.gameObject.SetActive(false);
        }

        // Verificar si el archivo de ghost existe y deshabilitar el botón si no existe
        if (!File.Exists(ghostFilePath))
        {
            ButtonGhost.gameObject.SetActive(false);
        }

        // Obtener el componente AudioSource o agregar uno si no existe
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Agregar EventTrigger a cada botón para manejar el evento PointerEnter
        AddPointerEnterEvent(ButtonPlay);
        AddPointerEnterEvent(ButtonReplay);
        AddPointerEnterEvent(ButtonGhost);
    }

    void AddPointerEnterEvent(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) => { OnPointerEnter(button); });

        trigger.triggers.Add(entry);
    }

    void PlayGame()
    {
        if (!loadingScene)
        {
            loadingScene = true;
            SceneManager.LoadScene("Game");
        }
    }

    void SetGameMode(int mode)
    {
        if (!loadingScene)
        {
            DisableAllButtons();
            StartCoroutine(PlayClickSoundAndLoadScene(mode));
        }
    }

    IEnumerator PlayClickSoundAndLoadScene(int mode)
    {
        // Reproducir el sonido de clic
        audioSource.PlayOneShot(clickSound);
        
        // Esperar hasta que termine el sonido
        yield return new WaitForSeconds(clickSound.length);

        // Guardar el modo de juego y cargar la escena
        PlayerPrefs.SetInt("GameMode", mode);
        PlayerPrefs.Save();
        PlayGame();
    }

    void OnPointerEnter(Button button)
    {
        // Reproducir el sonido al colocar el puntero sobre el botón
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    void DisableAllButtons()
    {
        ButtonPlay.interactable = false;
        ButtonReplay.interactable = false;
        ButtonGhost.interactable = false;
    }
}
