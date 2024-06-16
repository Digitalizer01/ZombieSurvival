using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public Button ButtonPlay;
    public Button ButtonSettings;
    public Slider VolumeSlider;
    public TMP_Dropdown ResolutionDropdown;
    public GameObject Options; // Objeto de opciones

    public AudioClip hoverSound; // Sonido al colocar el puntero sobre el botón
    public AudioClip clickSound; // Sonido al hacer clic en el botón

    private AudioSource audioSource;
    private bool loadingScene = false; // Indica si la escena está cargándose
    private List<Resolution> resolutions; // Lista de resoluciones disponibles

    void Start()
    {
        PlayerPrefs.DeleteAll();

        ButtonPlay.onClick.AddListener(PlayGame);
        ButtonSettings.onClick.AddListener(OpenSettings);

        // Obtener el componente AudioSource o agregar uno si no existe
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Agregar EventTrigger a cada botón para manejar el evento PointerEnter
        AddPointerEnterEvent(ButtonPlay);
        AddPointerEnterEvent(ButtonSettings);

        // Configuración del volumen
        if (VolumeSlider != null)
        {
            VolumeSlider.onValueChanged.AddListener(SetVolume);
            VolumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        }

        // Configuración de resolución
        if (ResolutionDropdown != null)
        {
            resolutions = new List<Resolution>(GetUniqueResolutions(Screen.resolutions));
            ResolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Count; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            ResolutionDropdown.AddOptions(options);
            ResolutionDropdown.value = currentResolutionIndex;
            ResolutionDropdown.RefreshShownValue();
            ResolutionDropdown.onValueChanged.AddListener(SetResolution);
        }

        // Inicialmente desactivar el objeto de opciones
        if (Options != null)
        {
            Options.SetActive(false);
        }
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
            StartCoroutine(PlayClickSoundAndLoadScene());
        }
    }

    IEnumerator PlayClickSoundAndLoadScene()
    {
        // Reproducir el sonido de clic
        audioSource.PlayOneShot(clickSound);
        
        // Esperar hasta que termine el sonido
        yield return new WaitForSeconds(clickSound.length);

        // Cargar la escena
        SceneManager.LoadScene("Game2");
    }

    void OnPointerEnter(Button button)
    {
        // Reproducir el sonido al colocar el puntero sobre el botón
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    void OpenSettings()
    {
        // Alternar el estado activo del objeto de opciones
        if (Options != null)
        {
            Options.SetActive(!Options.activeSelf);
        }
    }

    void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
        PlayerPrefs.Save();
    }

    void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    List<Resolution> GetUniqueResolutions(Resolution[] allResolutions)
    {
        HashSet<(int, int)> seenResolutions = new HashSet<(int, int)>();
        List<Resolution> uniqueResolutions = new List<Resolution>();

        foreach (Resolution res in allResolutions)
        {
            if (seenResolutions.Add((res.width, res.height)))
            {
                uniqueResolutions.Add(res);
            }
        }

        return uniqueResolutions;
    }
}
