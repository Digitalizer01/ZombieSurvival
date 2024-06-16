using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.Vehicles.Car;

public class GameDRController : MonoBehaviour
{
    public GameObject PlayerObject;
    public GameObject CarCamera;
    public GameObject PlayerCamera;
    public bool FPSCameraBool;
    public float detectionDistance = 5.0f;
    public TMP_Text TakeCarText;
    public TMP_Text ZombiesKilledText;
    public TMP_Text WeaponText;
    private List<GameObject> carObjects = new List<GameObject>();
    private GameObject currentCar;
    private PlayerHealth playerHealth;
    private PlayerAmmo playerAmmo;
    private int zombiesKilled = 0;
    public float exitDistance = 2.0f;
    private bool isPlayerInCar = false; // Indica si el jugador está dentro de un coche

    void Start()
    {
        FPSCameraBool = true;
        InitializeCars();

        // Obtener el componente PlayerHealth
        playerHealth = PlayerObject.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("No PlayerHealth component found on PlayerObject.");
        }

        // Obtener el componente PlayerAmmo
        playerAmmo = PlayerObject.GetComponent<PlayerAmmo>();
        if (playerAmmo == null)
        {
            Debug.LogError("No PlayerAmmo component found on PlayerObject.");
        }

        UpdateZombiesKilledText();
        UpdateWeaponText(playerAmmo.GetAmmoAmount());
    }

    void Update()
    {
        bool carDetected = DetectCar();
        UpdateTakeCarText(carDetected);
        HandleCameraSwitch(carDetected);
        SwitchCameras();

        // Activar o desactivar la inmunidad del jugador según si está en un coche o no
        if (isPlayerInCar)
        {
            playerHealth.ActivateImmunity();
        }
        else
        {
            playerHealth.DeactivateImmunity();
        }
    }

    void InitializeCars()
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
        foreach (GameObject car in cars)
        {
            carObjects.Add(car);
            if (car != null)
            {
                car.GetComponent<Rigidbody>().isKinematic = true;
                CarUserControl carUserControl = car.GetComponent<CarUserControl>();
                if (carUserControl != null)
                {
                    carUserControl.enabled = false;
                }
                // Añadir el componente de detección de colisiones
                car.AddComponent<CarCollisionDetector>();
                car.GetComponent<CarCollisionDetector>().enabled = false;
            }
        }
    }

    bool DetectCar()
    {
        // Ajustar la posición del rayo para que se origine desde los pies del jugador
        Vector3 rayOrigin = PlayerObject.transform.position + Vector3.up * 0.5f; // Un poco más arriba de los pies
        Ray ray = new Ray(rayOrigin, PlayerObject.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionDistance))
        {
            if (hit.collider.transform.root.CompareTag("Car"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    void UpdateTakeCarText(bool carDetected)
    {
        TakeCarText.text = carDetected ? "Press 'P' to take the car" : "";
    }

    void HandleCameraSwitch(bool carDetected)
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (FPSCameraBool && carDetected)
            {
                FPSCameraBool = false;

                currentCar = FindDetectedCar();
                if (currentCar != null)
                {
                    FollowCar(currentCar);
                    currentCar.GetComponent<Rigidbody>().isKinematic = false;
                    RemoveNavigationComponents(currentCar);
                    CarUserControl carUserControl = currentCar.GetComponent<CarUserControl>();
                    if (carUserControl != null)
                    {
                        carUserControl.enabled = true;
                    }
                    // Activar el detector de colisiones
                    currentCar.GetComponent<CarCollisionDetector>().enabled = true;
                    PlayerObject.SetActive(false);
                    isPlayerInCar = true; // El jugador está ahora en un coche
                }
            }
            else
            {
                FPSCameraBool = true;

                if (currentCar != null)
                {
                    CarUserControl carUserControl = currentCar.GetComponent<CarUserControl>();
                    if (carUserControl != null)
                    {
                        carUserControl.StopCar();
                        carUserControl.enabled = false;
                    }
                    // Desactivar el detector de colisiones
                    currentCar.GetComponent<CarCollisionDetector>().enabled = false;

                    // Mover al jugador a una posición de salida segura a la izquierda del coche
                    Vector3 exitPosition = currentCar.transform.position - currentCar.transform.right * exitDistance;
                    PlayerObject.transform.position = exitPosition;

                    currentCar = null;
                    isPlayerInCar = false; // El jugador ha salido del coche
                }
                PlayerObject.SetActive(true);
            }
        }
    }

    GameObject FindDetectedCar()
    {
        Vector3 rayOrigin = PlayerObject.transform.position + Vector3.up * 0.5f; // Un poco más arriba de los pies
        Ray ray = new Ray(rayOrigin, PlayerObject.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionDistance))
        {
            if (hit.collider.transform.root.CompareTag("Car"))
            {
                return hit.collider.transform.root.gameObject;
            }
        }
        return null;
    }

    void RemoveNavigationComponents(GameObject car)
    {
        NavMeshAgent navMeshAgent = car.GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            Destroy(navMeshAgent);
        }

        WaypointMover waypointMover = car.GetComponent<WaypointMover>();
        if (waypointMover != null)
        {
            Destroy(waypointMover);
        }
    }

    void SwitchCameras()
    {
        if (FPSCameraBool)
        {
            CarCamera.SetActive(false);
            PlayerCamera.SetActive(true);
        }
        else
        {
            CarCamera.SetActive(true);
            PlayerCamera.SetActive(false);
        }
    }

    void FollowCar(GameObject car)
    {
        AutoCam autoCam = CarCamera.GetComponent<AutoCam>();
        if (autoCam != null)
        {
            autoCam.SetTarget(car.transform);
        }
    }

    // Método para incrementar el contador de zombies muertos
    public void IncrementZombiesKilled()
    {
        zombiesKilled++;
        UpdateZombiesKilledText();
    }

    // Método para actualizar el texto del contador de zombies muertos
    void UpdateZombiesKilledText()
    {
        ZombiesKilledText.text = zombiesKilled + " KILLED";
    }

    // Método para actualizar el texto de la munición
    public void UpdateWeaponText(int ammoAmount)
    {
        WeaponText.text = ammoAmount + "";
    }
}
