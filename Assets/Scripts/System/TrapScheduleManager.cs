using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct DailyTrapSchedule
{
    [Tooltip("Titik lokasi jebakan akan muncul sesuai harinya")]
    public Transform[] trapSpawnPoints;
}

public class TrapScheduleManager : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private GameObject trapPrefab;
    [SerializeField] private int maxTrapsPerDay = 3;

    [Header("Daily Trap Schedule)")]
    public DailyTrapSchedule[] dailyTrapSchedules;

    private List<GameObject> activeTraps = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < maxTrapsPerDay; i++)
        {
            GameObject newTrap = Instantiate(trapPrefab, transform);
            newTrap.SetActive(false); // Nonaktifkan jebakan baru hingga jadwal harian diatur
            activeTraps.Add(newTrap);
        }

        UpdateTraps(1); // Atur jebakan untuk hari pertama saat game dimulai
    }

    private void OnEnable()
    {
        TimeDayManager.OnDayChanged += UpdateTraps; // Subscribe ke event pergantian hari
    }

    private void OnDisable()
    {
        TimeDayManager.OnDayChanged -= UpdateTraps; // Unsubscribe dari event saat tidak diperlukan
    }

    private void UpdateTraps(int dayNumber)
    {
        foreach (GameObject trap in activeTraps)
        {
            trap.SetActive(false); // Nonaktifkan semua jebakan terlebih dahulu
        }

        int scheduleIndex = dayNumber - 1; // Dapatkan jadwal jebakan berdasarkan nomor hari

        if (scheduleIndex >= 0 && scheduleIndex < dailyTrapSchedules.Length)
        {
            Transform[] todayPoints = dailyTrapSchedules[scheduleIndex].trapSpawnPoints;

            for (int i = 0; i < todayPoints.Length; i++)
            {
                if (i >= activeTraps.Count) break; // Pastikan tidak melebihi jumlah jebakan yang tersedia

                activeTraps[i].transform.position = todayPoints[i].position; // Pindahkan jebakan ke titik spawn yang sesuai
                activeTraps[i].transform.rotation = todayPoints[i].rotation; // Sesuaikan rotasi jebakan dengan titik spawn
                activeTraps[i].SetActive(true); // Aktifkan jebakan
            }
        }
        else
        {
            Debug.LogWarning($"No trap schedule found for day {dayNumber}. All traps will remain inactive.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
