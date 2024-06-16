using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    public GameObject camerasColliders;
    public GameObject cameras;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CameraCollider"))
        {
            // Desactivar todas las cámaras primero
            DisableAllCameras();

            // Obtener el índice del collider chocado
            int colliderIndex = other.transform.GetSiblingIndex();

            // Activar la cámara correspondiente al índice del collider
            GameObject activatedCamera = cameras.transform.GetChild(colliderIndex).gameObject;
            activatedCamera.SetActive(true);
            
            // Imprimir el nombre de la cámara activa
            Debug.Log("Cámara activa: " + activatedCamera.name);
        }
    }

    private void DisableAllCameras()
    {
        // Desactivar todas las cámaras
        foreach (Transform cameraTransform in cameras.transform)
        {
            cameraTransform.gameObject.SetActive(false);
        }
    }
}
