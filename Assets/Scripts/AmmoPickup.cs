using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 30; // Cantidad de munición que se añade

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ThirdPersonCharacterController playerController = other.GetComponent<ThirdPersonCharacterController>();
            if (playerController != null)
            {
                playerController.AddAmmo(ammoAmount);
                Destroy(gameObject);
            }
        }
    }
}
