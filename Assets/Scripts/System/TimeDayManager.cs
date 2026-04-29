using TMPro;
using UnityEngine;

public class TimeDayManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dayText;

    private int currentDay = 1; // Hari awal
    private DayNightCycle dayNightCycle;

    private void Start()
    {
        dayNightCycle = GetComponent<DayNightCycle>();
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

    private void UpdateDay()
    {
        currentDay++;
        DisplayDay();
    }

    private void DisplayDay()
    {
        if (dayText != null)
        {
            dayText.text = $"Day {currentDay}";
        }
    }
}
