using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        // 1. Set nilai awal slider sesuai data yang tersimpan di PlayerPrefs
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // 2. Tambahkan listener agar saat slider digeser, fungsinya langsung dipanggil
        masterSlider.onValueChanged.AddListener(SetMaster);
        bgmSlider.onValueChanged.AddListener(SetBGM);
        sfxSlider.onValueChanged.AddListener(SetSFX);
    }

    private void SetMaster(float value)
    {
        AudioManager.Instance.SetMasterVolume(value);
    }

    private void SetBGM(float value)
    {
        AudioManager.Instance.SetBGMVolume(value);
    }

    private void SetSFX(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }
}