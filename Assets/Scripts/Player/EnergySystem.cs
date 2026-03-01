using System;
using UnityEngine;

public class EnergySystem : MonoBehaviour
{
    [Header("Energy Value")]
    [SerializeField] private float maxEnergy = 100f; // Jumlah energi maksimal
    private float currentEnergy; // Jumlah energi saat ini
    [Header("Energy Regeneration")] 
    [SerializeField] private float energyRegenRate = 5f; // Kecepatan regenerasi energi per detik
    [SerializeField] private float energyRegenTimer = 0f; // Timer untuk mengatur regenerasi energi
    private float regenTimer;

    public bool isPlayerMoving; // Status apakah player sedang bergerak    

    public static event Action<float> OnEnergyChanged; // Event untuk memberitahu perubahan energi

    private void Start()
    {
        currentEnergy = maxEnergy; // Mulai dengan energi penuh
    }

    private void Update()
    {
        RegenEnergy();
    }

    public bool ConsumeEnergy(float amount, float minEnergy = 0f)
    {
        if (currentEnergy < minEnergy)
        {
            return false; // Tidak cukup energi untuk mencapai level minimum
        }

        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            regenTimer = energyRegenTimer; // Reset timer regenerasi saat energi dikonsumsi
            
            if (currentEnergy < 0) currentEnergy = 0; // Pastikan energi tidak negatif
            OnEnergyChanged?.Invoke(currentEnergy / maxEnergy); // Trigger event perubahan energi
            return true; // Energi berhasil dikonsumsi
        }
        else        {
            return false; // Tidak cukup energi untuk dikonsumsi
        }

    }

    private void RegenEnergy()
    {
        if (regenTimer > 0)
        {
            regenTimer -= Time.deltaTime; // Kurangi timer regenerasi
            if (regenTimer < 0 ) regenTimer = 0; // Pastikan timer tidak negatif
            return; // Tunggu hingga timer habis sebelum mulai regenerasi
        }

        if (currentEnergy < maxEnergy && !isPlayerMoving)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
            if (currentEnergy > maxEnergy) currentEnergy = maxEnergy; // Pastikan energi tidak melebihi maksimum
            OnEnergyChanged?.Invoke(currentEnergy / maxEnergy); // Trigger event perubahan energi
        }
    }
}
