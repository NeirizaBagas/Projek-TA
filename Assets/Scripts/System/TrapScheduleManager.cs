using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    //public TextMeshProUGUI animalStatusUpdate;
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
    public TextMeshProUGUI animalStatusUpdate;

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
                    if (animalStatusUpdate != null) StartCoroutine(ShowStatusText(animalStatusUpdate, $"Semua Trap di area {zone.animalType} sudah di-defuse! Hewan aman selamanya.", 3f));
                }
                else
                {
                    zone.currentState = AnimalLocationState.Lost;
                    lostCount++;
                    OnAnimalLost?.Invoke(zone.animalType);
                    totalAnimal--; // Kurangi total hewan yang tersisa

                    if (animalStatusUpdate != null) StartCoroutine(ShowStatusText(animalStatusUpdate, $"{zone.animalType} Hilang!", 3f));
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

    private IEnumerator ShowStatusText(TextMeshProUGUI statusText, string message, float displayDuration)
    {
        // 1. Ganti isi teksnya sesuai konteks
        statusText.text = message;

        // 2. Nyalakan GameObject teksnya agar terlihat di layar
        statusText.gameObject.SetActive(true);

        // 3. Tunggu selama beberapa detik (sesuai parameter displayDuration)
        yield return new WaitForSeconds(displayDuration);

        // 4. Matikan kembali GameObject teksnya
        statusText.gameObject.SetActive(false);
    }
}