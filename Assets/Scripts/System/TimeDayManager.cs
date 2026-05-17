using System;
using TMPro;
using UnityEngine;

public class TimeDayManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private int finalDay = 12; // Hari terakhir sebelum game over

    private int currentDay = 1; // Hari awal
    public static int _currentDay;
    private DayNightCycle dayNightCycle;
    
    public static event Action OnFinalDayReached; // Event untuk mencapai hari terakhir
    public static event Action<int> OnDayChanged; // Event untuk pergantian hari

    private void Start()
    {
        dayNightCycle = GetComponent<DayNightCycle>();
        _currentDay = currentDay; // Set nilai awal hari ke variabel statis
        DisplayDay(); // Tampilkan hari awal saat game mulai
    }

    private void OnEnable()
    {
        DayNightCycle.OnDayChanged += UpdateDay; // Subscribe ke event pergantian hari
    }

    private void OnDisable()
    {
        DayNightCycle.OnDayChanged -= UpdateDay; // Unsubscribe dari event saat tidak diperlukan
    }

    private void Update()
    {
        if (timeText == null || dayText == null) return; // Cegah error jika referensi belum diatur

        float time = dayNightCycle.currentTime;
        int hours = Mathf.FloorToInt(time);
        int minutes = Mathf.FloorToInt((time - hours) * 60);

        timeText.text = $"{hours:00}:{minutes:00}"; // Format waktu HH:MM
    }

    public void UpdateDay()
    {
        currentDay++;
        _currentDay = currentDay; // Update nilai hari ke variabel statis
        if (currentDay == finalDay)
        {
            OnFinalDayReached?.Invoke(); // Trigger event mencapai hari terakhir
        }
        DisplayDay();
        OnDayChanged?.Invoke(currentDay); // Trigger event pergantian hari
    }

    private void DisplayDay()
    {
        if (dayText != null)
        {
            dayText.text = $"DAY {currentDay}";
        }
    }
}
