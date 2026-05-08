using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

// 1. Tambahkan Enum untuk melacak status dan jenis hewan
public enum AnimalType { Harimau, Gajah, Orangutan, Rangkong, Badak, Bekantan, Tarsius, Lutung }
public enum AnimalLocationState { AtRisk, Safe, Lost }

[System.Serializable]
public class AnimalTrapZone
{
    public AnimalType animalType;
    [Tooltip("Hari terakhir pemain bisa men-defuse trap sebelum hewan ini hilang")]
    public int criticalDay;
    public Transform[] trapSpawnPoints;

    [HideInInspector] public AnimalLocationState currentState = AnimalLocationState.AtRisk;
    [HideInInspector] public List<GameObject> spawnedTraps = new List<GameObject>();
}

public class TrapScheduleManager : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private GameObject trapPrefab;
    [SerializeField] private int maxLostAnimalsForGameOver = 4;

    [Header("Animal Zones Setting")]
    public AnimalTrapZone[] animalZones;

    private int lostCount = 0;
    public static int totalAnimal { get; private set;}

    // Event untuk disambungkan ke UI atau sistem lain
    public static event Action<AnimalType> OnAnimalLost;
    public static event Action<AnimalType> OnAnimalSafe;
    public static event Action OnGameOver;

    void Start()
    {
        // 1. Spawn semua jebakan di awal, tapi masukkan ke dalam list masing-masing hewan
        foreach (var zone in animalZones)
        {
            foreach (var point in zone.trapSpawnPoints)
            {
                GameObject newTrap = Instantiate(trapPrefab, point.position, point.rotation, transform);
                newTrap.SetActive(false);
                zone.spawnedTraps.Add(newTrap);
            }
        }
        totalAnimal = animalZones.Length;
        // Panggil untuk hari pertama
        UpdateTraps(1);
    }

    private void OnEnable()
    {
        TimeDayManager.OnDayChanged += UpdateTraps;
    }

    private void OnDisable()
    {
        TimeDayManager.OnDayChanged -= UpdateTraps;
    }

    private void UpdateTraps(int currentDay)
    {
        // FASE 1: EVALUASI HARI SEBELUMNYA
        // Cek apakah ada hewan yang batas waktunya (critical day) sudah terlewat
        foreach (var zone in animalZones)
        {
            // Jika status hewan masih AtRisk dan hari ini lebih dari hari batas amannya
            if (zone.currentState == AnimalLocationState.AtRisk && currentDay > zone.criticalDay)
            {
                bool allDefused = true;

                // Cek apakah masih ada trap yang aktif di area hewan ini
                foreach (var trap in zone.spawnedTraps)
                {
                    // Asumsi: Trap yang belum di-defuse masih berstatus Active
                    if (trap != null && trap.activeSelf)
                    {
                        allDefused = false;
                        break;
                    }
                }

                if (allDefused)
                {
                    zone.currentState = AnimalLocationState.Safe;
                    OnAnimalSafe?.Invoke(zone.animalType);
                    Debug.Log($"[Berhasil] Semua Trap di area {zone.animalType} sudah di-defuse! Hewan aman selamanya.");
                }
                else
                {
                    zone.currentState = AnimalLocationState.Lost;
                    lostCount++;
                    OnAnimalLost?.Invoke(zone.animalType);
                    Debug.LogWarning($"[Gagal] Waktu habis! {zone.animalType} telah hilang/diburu.");
                    totalAnimal--; // Kurangi total hewan yang tersisa
                    // Matikan sisa trap di area ini agar tidak mengganggu area lain
                    foreach (var trap in zone.spawnedTraps)
                    {
                        if (trap != null) trap.SetActive(false);
                    }
                }
            }
        }

        // FASE 2: CEK KONDISI GAME OVER
        if (lostCount >= maxLostAnimalsForGameOver)
        {
            Debug.LogError("GAME OVER! 4 Hewan telah hilang!");
            OnGameOver?.Invoke();
            return; // Hentikan eksekusi, biarkan UIManager yang mengurus Game Over
        }

        // FASE 3: SPAWN TRAP UNTUK HARI INI
        // Hanya munculkan trap pada area hewan yang statusnya masih AtRisk (belum Safe, belum Lost)
        foreach (var zone in animalZones)
        {
            if (zone.currentState == AnimalLocationState.AtRisk)
            {
                foreach (var trap in zone.spawnedTraps)
                {
                    // Hanya aktifkan trap yang memang belum pernah di-defuse/dihancurkan
                    if (trap != null) trap.SetActive(true);
                }
            }
        }
    }
}