using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0, 24)] public float currentTime = 12f; // Jam awal (12 = Siang)
    public float dayDurationInMinutes = 10f; // 1 hari asli = 10 menit game
    [SerializeField] private float smoothSpeed = 1.5f; // intensity units per second (tweak as needed)

    [Header("Lights")]
    public Light sunLight;
    public Light moonLight;
    public Gradient sunColor; // Warna matahari berubah (Oranye -> Putih -> Oranye)

    [Header("Skybox")]
    [SerializeField] private Material daySkybox;
    [SerializeField] Gradient skyTintGradient; // Warna langit berubah (Biru terang -> Biru gelap)
    [SerializeField] private float maxExposure;
    [SerializeField] private float minExposure;

    private void Start()
    {
        sunLight.intensity = 5f; // Intensitas awal matahari
        moonLight.intensity = 0f; // Intensitas awal bulan
    }

    private void Update()
    {
        UpdateTime();
        UpdateSunRotation();
        UpdateSkyboxColour();
    }

    private void UpdateTime()
    {
        // Menghitung penambahan waktu berdasarkan menit asli
        float timeMultiplier = 24f / (dayDurationInMinutes * 60f); // 24 jam dibagi total 1 hari dalam menit asli
        currentTime += Time.deltaTime * timeMultiplier; // Tambahkan waktu berdasarkan waktu nyata yang berlalu

        if (currentTime >= 24) currentTime = 0; // Reset ke hari baru, // event untuk tanda pergantian hari 
    }

    private void UpdateSunRotation()
    {
        // Menghitung rotasi matahari (0-24 jam menjadi 0-360 derajat)
        // Kita kurangi 90 agar jam 12 siang tepat di atas kepala
        float sunRotation = (currentTime / 24f) * 360f - 90f;
        sunLight.transform.localRotation = Quaternion.Euler(sunRotation, 170f, 0f);
        float moonRotation = sunRotation + 180f; // Bulan selalu berlawanan dengan matahari
        moonLight.transform.localRotation = Quaternion.Euler(moonRotation, 170f, 0f);

        // Update warna matahari berdasarkan waktu (Opsional)
        //sunLight.color = sunColor.Evaluate(currentTime / 24f);
        /*moonLight.color = sunColor.Evaluate((currentTime + 12f) / 24f);*/ // Warna bulan juga berubah tapi offset 12 jam

        float sunTargetIntensity;
        float moonTargetIntensity;

        if (currentTime >= 16f && currentTime < 19f)
        {
            // Sore ke malam => target 0
            sunTargetIntensity = 0f;
            moonTargetIntensity = 3f; // Bulan mulai muncul
        }
        else if (currentTime >= 6f && currentTime < 10f)
        {
            // Malam ke pagi => target 5
            sunTargetIntensity = 5f;
            moonTargetIntensity = 0f; // Bulan mulai menghilang
        }
        else if (currentTime >= 8f && currentTime < 17f)
        {
            // Siang => full
            sunTargetIntensity = 5f;
            moonTargetIntensity = 0f; // Bulan tidak muncul di siang hari
        }
        else
        {
            // Full malam => off
            sunTargetIntensity = 0f;
            moonTargetIntensity = 3f; // Bulan tetap muncul di malam hari
        }

        DynamicGI.UpdateEnvironment(); // Update GI envi di pohon dan objek lainnya untuk perubahan skybox


        // Ngatur intensitas matahari secara halus mendekati target
        sunLight.intensity = Mathf.MoveTowards(sunLight.intensity, sunTargetIntensity, smoothSpeed * Time.deltaTime);
        moonLight.intensity = Mathf.MoveTowards(moonLight.intensity, moonTargetIntensity, smoothSpeed * Time.deltaTime);

        // Optional debug near key transition times
        if (Mathf.Abs(currentTime - 5f) < 0.05f || Mathf.Abs(currentTime - 8f) < 0.05f || Mathf.Abs(currentTime - 16f) < 0.05f)
        {
            //Debug.Log($"[SUN DEBUG] time={currentTime:F2} target={sunTargetIntensity:F2} intensity={sunLight.intensity:F2}");
        }
    }

    private void UpdateSkyboxColour()
    {
        float timePercent = currentTime / 24f;

        Color targetSkyTint = skyTintGradient.Evaluate(timePercent);

        RenderSettings.skybox.SetColor("_Tint", targetSkyTint);

        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        float targetExposure = Mathf.Lerp(minExposure, maxExposure, Mathf.Clamp01(dotProduct));
        RenderSettings.skybox.SetFloat("_Exposure", targetExposure);
    }
}
