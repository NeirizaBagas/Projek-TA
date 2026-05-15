using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // Pola Singleton agar bisa diakses dari script mana saja tanpa FindObject
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer Reference")]
    [SerializeField] private AudioMixer mainAudioMixer;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [Header("Dedicated Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfx2DSource;

    [Header("Audio Library (Berdasarkan Index)")]
    // Pakai array biasa agar gampang diisi lewat Inspector
    public AudioClip[] bgmLibrary;
    public AudioClip[] sfxLibrary;

    // Kunci string untuk PlayerPrefs agar tidak typo
    private const string MASTER_KEY = "MasterVolume";
    private const string BGM_KEY = "BGMVolume";
    private const string SFX_KEY = "SFXVolume";

    private void Awake()
    {
        // Setup Singleton & DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Muat volume yang tersimpan saat game pertama kali dijalankan
        LoadVolumeSettings();
    }


    public void PlayBGM(int index)
    {
        // Cek apakah index ada di dalam rentang array bgmLibrary
        if (index >= 0 && index < bgmLibrary.Length)
        {
            AudioClip clipToPlay = bgmLibrary[index];
            // Mainkan jika klipnya berbeda dari yang sedang main (biar gak restart kalau klipnya sama)
            if (bgmSource.clip != clipToPlay)
            {
                bgmSource.clip = clipToPlay;
                bgmSource.Play();
            }
        }
        else
        {
            Debug.LogWarning($"BGM index {index} tidak ditemukan!");
        }
    }

    // Gunakan ini untuk UI Button, Menu, dll
    public void PlaySFX(int index)
    {
        if (index >= 0 && index < sfxLibrary.Length)
        {
            sfx2DSource.PlayOneShot(sfxLibrary[index]);
        }
        else
        {
            Debug.LogWarning($"SFX index {index} tidak ditemukan!");
        }
    }

    // Gunakan ini untuk suara di dunia game (Hewan, Trap, dll)
    public void PlaySFX3D(int index, Vector3 position)
    {
        if (index >= 0 && index < sfxLibrary.Length)
        {
            AudioClip clipToPlay = sfxLibrary[index];

            // Bikin objek "speaker" tak kasat mata secara instan di lokasi kejadian
            GameObject tempAudio = new GameObject("TempSFX_" + index);
            tempAudio.transform.position = position;

            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
            audioSource.clip = clipToPlay;
            audioSource.outputAudioMixerGroup = sfxMixerGroup; // Masukkan ke Mixer SFX biar bisa dikecilin
            audioSource.spatialBlend = 1f; // Angka 1 artinya suaranya Full 3D
            audioSource.Play();

            // Speaker ghoib ini otomatis hancur setelah durasi audionya selesai
            Destroy(tempAudio, clipToPlay.length);
        }
        else
        {
            Debug.LogWarning($"SFX 3D index {index} tidak ditemukan!");
        }
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
            bgmSource.clip = null; // Opsional: hapus referensi klip agar bersih
        }
    }

    // Menghentikan semua SFX 2D yang sedang berbunyi (UI, Klik, dll)
    public void StopAllSFX()
    {
        if (sfx2DSource.isPlaying)
        {
            sfx2DSource.Stop();
        }
    }

    // --- FUNGSI UNTUK DIHUBUNGKAN KE SLIDER UI ---
    public void SetMasterVolume(float volume)
    {
        ApplyVolume("MasterVolume", volume);
        PlayerPrefs.SetFloat(MASTER_KEY, volume); // Catat permanen
    }

    public void SetBGMVolume(float volume)
    {
        ApplyVolume("BGMVolume", volume);
        PlayerPrefs.SetFloat(BGM_KEY, volume); // Catat permanen
    }

    public void SetSFXVolume(float volume)
    {
        ApplyVolume("SFXVolume", volume);
        PlayerPrefs.SetFloat(SFX_KEY, volume); // Catat permanen
    }

    private void ApplyVolume(string parameterName, float volume)
    {
        // Konversi 0-1 ke desibel -80 sampai 0
        float decibel = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        mainAudioMixer.SetFloat(parameterName, decibel);
    }
    // --- FUNGSI INTERNAL ---

    private void LoadVolumeSettings()
    {
        // Ambil data dari PlayerPrefs. Jika belum ada, default-nya adalah 1 (Maksimal)
        float masterVol = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
        float bgmVol = PlayerPrefs.GetFloat(BGM_KEY, 1f);
        float sfxVol = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        SetMasterVolume(masterVol);
        SetBGMVolume(bgmVol);
        SetSFXVolume(sfxVol);
    }

    // --- FUNGSI PEMBANTU UNTUK MEMUTAR AUDIO ---

    // Bisa dipanggil dari script lain dengan: AudioManager.Instance.PlaySFX(clip, posisi);
    //public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
    //{
    //    if (clip == null) return;
    //    AudioSource.PlayClipAtPoint(clip, position, volume);
    //    // Catatan: Pastikan prefab suara sementara dari PlayClipAtPoint diarahkan ke grup SFX
    //}
}