using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0, 24)] public float currentTime = 12f; // Jam awal (12 = Siang)
    public float dayDurationInMinutes = 10f; // 1 hari asli = 10 menit game

    [Header("Lights")]
    public Light sunLight;
    public Gradient sunColor; // Warna matahari berubah (Oranye -> Putih -> Oranye)

    private void Update()
    {
        UpdateTime();
        UpdateSunRotation();
    }

    private void UpdateTime()
    {
        // Menghitung penambahan waktu berdasarkan menit asli
        float timeMultiplier = 24f / (dayDurationInMinutes * 60f); // 24 jam dibagi total 1 hari dalam menit asli
        currentTime += Time.deltaTime * timeMultiplier; // Tambahkan waktu berdasarkan waktu nyata yang berlalu

        if (currentTime >= 24) currentTime = 0; // Reset ke hari baru
    }

    private void UpdateSunRotation()
    {
        // Menghitung rotasi matahari (0-24 jam menjadi 0-360 derajat)
        // Kita kurangi 90 agar jam 12 siang tepat di atas kepala
        float sunRotation = (currentTime / 24f) * 360f - 90f;
        sunLight.transform.localRotation = Quaternion.Euler(sunRotation, 170f, 0f);

        // Update warna matahari berdasarkan waktu (Opsional)
        sunLight.color = sunColor.Evaluate(currentTime / 24f);
    }
}
