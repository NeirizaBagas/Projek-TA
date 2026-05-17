using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class StorySlide
{
    public Sprite artwork;
    public string dialogueText;
}

public enum CutsceneType
{
    Intro,
    Tutorial,
    Outro
}

public class CutSceneManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image displayImage;
    [SerializeField] private TextMeshProUGUI displayText;

    [Header("Story Content")]
    public List<StorySlide> slides = new List<StorySlide>();
    [SerializeField] private int nextSceneIndex;
    public CutsceneType cutsceneType;

    private int currentSlideIndex = 0;

    public static event Action OnCutsceneFinished;

    private void Start()
    {
        if (slides.Count > 0)
        {
            ShowSlide(0);
        }
        if (cutsceneType == CutsceneType.Intro)
        {
            AudioManager.Instance.PlayBGM(0); // Mainkan BGM intro (asumsi index 0 adalah BGM intro)
        }
        else 
        {
            AudioManager.Instance.PlayBGM(1); // Mainkan BGM tutorial (asumsi index 1 adalah BGM tutorial)
        }
    }

    // FUNGSI UPDATE DIHAPUS. Kita tidak butuh ngecek klik tiap frame lagi.

    private void ShowSlide(int index)
    {
        displayImage.sprite = slides[index].artwork;
        if (displayText != null) displayText.text = slides[index].dialogueText;
        /*displayText.text = slides[index].dialogueText*/
        ;
    }

    // WAJIB PUBLIC agar bisa dipanggil oleh UI Button di Inspector
    public void NextSlide()
    {
        AudioManager.Instance.PlaySFX(0); // Mainkan SFX klik tombol (asumsi index 0 adalah suara klik)
        currentSlideIndex++;
        if (cutsceneType == CutsceneType.Intro && currentSlideIndex == 8)
        {
            AudioManager.Instance.StopBGM(); // Hentikan BGM intro saat mencapai slide terakhir
            AudioManager.Instance.PlayBGM(1); // Mainkan BGM gameplay (asumsi index 1 adalah BGM gameplay)
        }

        if (currentSlideIndex < slides.Count)
        {
            ShowSlide(currentSlideIndex);
        }
        else
        {
            Debug.Log("Cutscene Selesai!");
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    public void NextTutorial()
    {
        AudioManager.Instance.PlaySFX(0); // Mainkan SFX klik tombol (asumsi index 0 adalah suara klik)
        if (currentSlideIndex < slides.Count - 1)
        {
            currentSlideIndex++;
            ShowSlide(currentSlideIndex);
        }
        else
        {
            Debug.Log("Cutscene Selesai!");
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    public void PreviousTutorial()
    {
        AudioManager.Instance.PlaySFX(0); // Mainkan SFX klik tombol (asumsi index 0 adalah suara klik)
        if (currentSlideIndex > 0)
        {
            currentSlideIndex--;
            ShowSlide(currentSlideIndex);

            if (currentSlideIndex == slides.Count - 1)
            {
                Debug.Log("Ini adalah slide terakhir!");
                OnCutsceneFinished?.Invoke(); // Trigger event saat mencapai slide terakhir
            }
        }
        else
        {
            Debug.Log("Ini adalah slide pertama!");
        }
    }
}
