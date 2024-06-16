using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class GameOverScreen : MonoBehaviour
{
    public Button restartButton;
    public Button quitButton;
    public AudioClip hoverSound; // Sonido al colocar el puntero sobre el botón
    public AudioClip clickSound; // Sonido al hacer clic en el botón

    private AudioSource audioSource;

    void Start()
    {
        // Asignar funciones a los botones
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);

        // Obtener el componente AudioSource o agregar uno si no existe
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Agregar EventTrigger a cada botón para manejar el evento PointerEnter
        AddPointerEnterEvent(restartButton);
        AddPointerEnterEvent(quitButton);
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

    void RestartGame()
    {
        StartCoroutine(PlayClickSoundAndRestartGame());
    }

    IEnumerator PlayClickSoundAndRestartGame()
    {
        // Reproducir el sonido de clic
        audioSource.PlayOneShot(clickSound);
        
        // Esperar hasta que termine el sonido
        yield return new WaitForSeconds(clickSound.length);

        // Reiniciar la escena actual (en este caso, "Game" podría ser tu escena principal)
        SceneManager.LoadScene("Menu");
    }

    void QuitGame()
    {
        StartCoroutine(PlayClickSoundAndQuitGame());
    }

    IEnumerator PlayClickSoundAndQuitGame()
    {
        // Reproducir el sonido de clic
        audioSource.PlayOneShot(clickSound);
        
        // Esperar hasta que termine el sonido
        yield return new WaitForSeconds(clickSound.length);

        // Salir de la aplicación (funciona en builds standalone)
        Application.Quit();

        // Esto no funcionará en el editor de Unity, solo en builds standalone
        // Para el editor, puedes hacer algo como cerrar la ventana del juego o dejar un mensaje
#if UNITY_EDITOR
        Debug.Log("Quit button pressed in editor.");
#endif
    }

    void OnPointerEnter(Button button)
    {
        // Reproducir el sonido al colocar el puntero sobre el botón
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
}
