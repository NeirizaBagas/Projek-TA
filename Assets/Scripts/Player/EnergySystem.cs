using System;
using UnityEngine;

public class EnergySystem : MonoBehaviour
{
    [Header("Energy Value")]
    [SerializeField] private float maxEnergy = 100f; // Jumlah energi maksimal
    private float currentEnergy; // Jumlah energi saat ini

    private float lastSentPercentage = -1f; // Variabel untuk menyimpan persentase energi terakhir yang dikirim ke UI

    public bool isPlayerMoving; // Status apakah player sedang bergerak    

    public static event Action<float> OnEnergyChanged; // Event untuk memberitahu perubahan energi
    public static event Action OnRegen; // Event untuk memberitahu energi sudah penuh (misalnya untuk memicu efek khusus)

    private void OnEnable()
    {
        BedSystemInteract.OnStartSleep += RegenEnergy; // Subscribe ke event untuk memulai regenerasi energi saat tidur
    }

    private void OnDisable()
    {
        BedSystemInteract.OnStartSleep -= RegenEnergy; // Unsubscribe dari event saat tidak diperlukan
    }

    private void Start()
    {
        currentEnergy = maxEnergy; // Mulai dengan energi penuh
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Base"))
        {
            RegenEnergy(); // Mulai regenerasi energi saat masuk ke area base
        }
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
            
            if (currentEnergy < 0) currentEnergy = 0; // Pastikan energi tidak negatif
            UpdateUI(); // Update UI setiap kali energi berubah
            return true; // Energi berhasil dikonsumsi
        }
        else
        {
            return false; // Tidak cukup energi untuk dikonsumsi
        }

    }

    private void RegenEnergy()
    {
        if (currentEnergy < maxEnergy/* && !isPlayerMoving*/)
        {
            currentEnergy = maxEnergy;
            OnRegen?.Invoke(); // Kirim event bahwa energi sudah penuh (misalnya untuk memicu efek khusus)
        }
    }

    private void UpdateUI()
    {
        float currentPercentage = currentEnergy / maxEnergy; // Hitung persentase energi saat ini

        if (Mathf.Abs(currentPercentage - lastSentPercentage) >= 0.01f) // Hanya kirim update jika persentase berubah signifikan (misalnya 1%)
        {
            OnEnergyChanged?.Invoke(currentPercentage); // Kirim event dengan persentase energi saat ini
            lastSentPercentage = currentPercentage; // Simpan persentase terakhir yang dikirim
        }
    }
}
