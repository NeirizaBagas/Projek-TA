using System;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0, 24)] public float currentTime = 12f; // Jam awal
    public float dayDurationInMinutes = 10f;

    [Header("Lights")]
    public Light sunLight;
    public Light moonLight;

    [Header("Intensity Curves (X = Jam 0-24, Y = Intensitas)")]
    [Tooltip("Atur kurva: Naik di jam 6, Puncak jam 12, Turun jam 18")]
    public AnimationCurve sunIntensityCurve;
    [Tooltip("Atur kurva: Puncak di jam 0 dan 24, Turun di jam 6 dan 18")]
    public AnimationCurve moonIntensityCurve;

    [Header("Skybox")]
    [SerializeField] private Material daySkybox;
    [SerializeField] Gradient skyTintGradient;
    [SerializeField] private float maxExposure = 1f;
    [SerializeField] private float minExposure = 0.2f;

    public static event Action OnDayChanged; // Event untuk memberitahu pergantian hari

    private void OnEnable()
    {
        BedSystemInteract.OnTimeSkip += SkipTime; // Subscribe ke event untuk memulai timeskip saat tidur
    }

    private void OnDisable()
    {
        BedSystemInteract.OnTimeSkip += SkipTime; // Unsubscribe dari event saat tidak diperlukan
    }

    private void Start()
    {
        // Pastikan material skybox tersetting
        RenderSettings.skybox = daySkybox;
    }

    private void Update()
    {
        UpdateTime();
        UpdateVisuals();
    }

    private void UpdateTime()
    {
        float timeMultiplier = 24f / (dayDurationInMinutes * 60f);
        currentTime += Time.deltaTime * timeMultiplier;

        if (currentTime >= 24)
        {
            currentTime -= 24f; // Kurangi 24 agar sisa detiknya tidak hilang (lebih presisi dari = 0)
            // TODO: Panggil Event Ganti Hari di sini
            OnDayChanged?.Invoke();
        }
    }

    // Gabungkan rotasi dan warna di satu fungsi yang murni bergantung pada currentTime
    private void UpdateVisuals()
    {
        // 1. ROTASI
        float sunRotation = (currentTime / 24f) * 360f - 90f;
        sunLight.transform.localRotation = Quaternion.Euler(sunRotation, 170f, 0f);

        float moonRotation = sunRotation + 180f;
        moonLight.transform.localRotation = Quaternion.Euler(moonRotation, 170f, 0f);

        // 2. INTENSITAS (Langsung instan membaca dari kurva)
        sunLight.intensity = sunIntensityCurve.Evaluate(currentTime);
        moonLight.intensity = moonIntensityCurve.Evaluate(currentTime);

        // 3. SKYBOX TINT & EXPOSURE
        float timePercent = currentTime / 24f;
        RenderSettings.skybox.SetColor("_Tint", skyTintGradient.Evaluate(timePercent));

        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        float targetExposure = Mathf.Lerp(minExposure, maxExposure, Mathf.Clamp01(dotProduct));
        RenderSettings.skybox.SetFloat("_Exposure", targetExposure);
    }

    // ==========================================
    // FUNGSI UNTUK SISTEM TIDUR / TIMESKIP
    // ==========================================
    public void SkipTime(float hoursToSkip)
    {
        currentTime += hoursToSkip;

        if (currentTime >= 24f)
        {
            currentTime -= 24f;
            // TODO: Panggil Event Ganti Hari di sini juga
        }

        // Langsung paksa visual update detik itu juga agar tidak ada delay/transisi aneh
        UpdateVisuals();

        // Update GI hanya saat terjadi timeskip ekstrim, JANGAN di dalam Update()
        DynamicGI.UpdateEnvironment();

        Debug.Log($"Waktu diskip sebanyak {hoursToSkip} jam. Sekarang jam: {currentTime}");
    }
}