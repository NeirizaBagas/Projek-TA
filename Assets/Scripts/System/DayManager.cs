using UnityEngine;

public class DayManager : MonoBehaviour
{
    [SerializeField] private int currentDay = 1; // Hari awal

    private void OnEnable()
    {
        DayNightCycle.OnDayChanged += UpdateDay; // Subscribe ke event pergantian hari
    }

    private void OnDisable()
    {
        DayNightCycle.OnDayChanged -= UpdateDay; // Unsubscribe dari event saat tidak diperlukan
    }

    private void UpdateDay()
    {
        currentDay++;
    }
}
