using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Transition")]
    [SerializeField] private int nextSceneIndex;

    [Header("UI Panels (Canvas Groups)")]
    [SerializeField] private CanvasGroup settingPanel;
    [SerializeField] private CanvasGroup creditPanel;

    [Header("Setting Tabs (GameObjects)")]
    [SerializeField] private GameObject audioTab;
    [SerializeField] private GameObject controlTab;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.3f;

    private void Start()
    {
        // 1. Putar BGM Menu
        AudioManager.Instance.PlayBGM(0); 

        // 2. Siapkan kondisi awal UI
        if (settingPanel != null) SetupPanel(settingPanel);
        if (creditPanel != null) SetupPanel(creditPanel);
    }

    private void SetupPanel(CanvasGroup cg)
    {
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    // --- FUNGSI TOMBOL UTAMA ---

    public void StartGame()
    {
        AudioManager.Instance.PlaySFX(0); 
        AudioManager.Instance.StopBGM(); 
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void QuitGame()
    {
        AudioManager.Instance.PlaySFX(0);
        Debug.Log("Game Keluar!");
        Application.Quit();
    }

    // --- FUNGSI TOMBOL SETTING & TABS ---

    public void OpenSetting()
    {
        AudioManager.Instance.PlaySFX(0);
        
        // Atur agar setiap buka Setting, selalu mulai dari Tab Audio secara default
        audioTab.SetActive(true);
        controlTab.SetActive(false);

        StopAllCoroutines();
        StartCoroutine(FadePanel(settingPanel, settingPanel.alpha, 1f));
    }

    public void CloseSetting()
    {
        AudioManager.Instance.PlaySFX(0);
        StartCoroutine(FadePanel(settingPanel, settingPanel.alpha, 0f));
    }

    // Fungsi untuk Tab Audio
    public void SwitchToAudioTab()
    {
        AudioManager.Instance.PlaySFX(0);
        audioTab.SetActive(true);
        controlTab.SetActive(false);
    }

    // Fungsi untuk Tab Control
    public void SwitchToControlTab()
    {
        AudioManager.Instance.PlaySFX(0);
        audioTab.SetActive(false);
        controlTab.SetActive(true);
    }

    // --- FUNGSI TOMBOL CREDIT ---

    public void OpenCredit()
    {
        AudioManager.Instance.PlaySFX(0);
        StopAllCoroutines();
        StartCoroutine(FadePanel(creditPanel, creditPanel.alpha, 1f));
    }

    public void CloseCredit()
    {
        AudioManager.Instance.PlaySFX(0);
        StartCoroutine(FadePanel(creditPanel, creditPanel.alpha, 0f));
    }

    // --- MESIN ANIMASI FADE ---

    private IEnumerator FadePanel(CanvasGroup cg, float startAlpha, float targetAlpha)
    {
        cg.interactable = false;
        cg.blocksRaycasts = false;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }

        cg.alpha = targetAlpha;

        if (targetAlpha >= 1f)
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }
}