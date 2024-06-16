using UnityEngine;

public class PlayerAmmo : MonoBehaviour
{
    public int maxAmmo = 100; // Máxima cantidad de munición
    private int currentAmmo;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    public void ReduceAmmo(int amount)
    {
        currentAmmo -= amount;
        currentAmmo = Mathf.Clamp(currentAmmo, 0, maxAmmo);
        UpdateWeaponText();
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        UpdateWeaponText();
    }

    public int GetAmmoAmount()
    {
        return currentAmmo;
    }

    private void UpdateWeaponText()
    {
        GameDRController gameController = FindObjectOfType<GameDRController>();
        if (gameController != null)
        {
            gameController.UpdateWeaponText(currentAmmo);
        }
    }
}
