using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CarCollisionDetector : MonoBehaviour
{
    private CarController carController;

    void Start()
    {
        carController = GetComponentInParent<CarController>();
        if (carController == null)
        {
            Debug.LogError("CarController component not found on parent of CarCollisionDetector.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (carController != null && carController.AccelInput > 0)
        {
            // Verifica colisión con un zombie
            if (collision.gameObject.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    // Aplicar daño al zombie
                    enemyHealth.TakeDamage(100);
                    Debug.Log("El coche ha chocado con un zombie!");
                }
            }

            // Verifica colisión con un humano
            else if (collision.gameObject.CompareTag("Human"))
            {
                HumanHealth humanHealth = collision.gameObject.GetComponent<HumanHealth>();
                if (humanHealth != null)
                {
                    // Eliminar al humano
                    humanHealth.Die();
                    Debug.Log("El coche ha chocado con un humano!");
                }
            }
        }
    }
}
