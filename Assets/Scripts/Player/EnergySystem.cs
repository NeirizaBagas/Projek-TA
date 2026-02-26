using UnityEngine;

public class EnergySystem : MonoBehaviour
{
    [Header("Energy Value")]
    public float maxEnergy = 100f; // Jumlah energi maksimal
    public float currentEnergy; // Jumlah energi saat ini
    [Header("Energy Regeneration")] 
    public float energyRegenRate = 5f; // Kecepatan regenerasi energi per detik
    //private float energyRegenTimer = 0f; // Timer untuk mengatur regenerasi energi
    public bool isPlayerMoving; // Status apakah player sedang bergerak    

    private void Start()
    {
        currentEnergy = maxEnergy; // Mulai dengan energi penuh
    }

    private void Update()
    {
        RegenEnergy();
    }

    public bool ConsumeEnergy(float amount)
    {
        
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            if (currentEnergy < 0)
            {
                currentEnergy = 0; // Pastikan energi tidak negatif
                
            }
            return true; // Energi berhasil dikonsumsi
        }
        else        {
            return false; // Tidak cukup energi untuk dikonsumsi
        }

    }

    private void RegenEnergy()
    {
        if (currentEnergy < maxEnergy && !isPlayerMoving)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
        }
    }
}
