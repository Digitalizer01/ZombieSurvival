using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 25; // Cantidad de salud que se restaura

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ThirdPersonCharacterController playerController = other.GetComponent<ThirdPersonCharacterController>();
            if (playerController != null)
            {
                playerController.HealPlayer(healAmount);
                Destroy(gameObject);
            }
        }
    }
}
