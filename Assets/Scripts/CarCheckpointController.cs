using UnityEngine;

public class CarCheckpointController : MonoBehaviour
{
    private GameObject _lastCheckpoint;
    
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto con el que se colisiona es un checkpoint
        if (other.CompareTag("Checkpoint"))
        {
            // Actualizar el último checkpoint
            _lastCheckpoint = other.gameObject;
        }
    }

    public GameObject GetLastCheckpointObject()
    {
        if (_lastCheckpoint != null)
        {
            return _lastCheckpoint;
        }
        return null; // Asegúrate de que siempre se devuelva un GameObject
    }
}
