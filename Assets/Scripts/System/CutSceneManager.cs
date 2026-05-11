using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class StorySlide
{
    public Sprite artwork;
}

public class CutSceneManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image displayImage;
    //[SerializeField] private TextMeshProUGUI displayText;

    [Header("Story Content")]
    public List<StorySlide> slides = new List<StorySlide>();
    [SerializeField] private int nextSceneIndex;

    private int currentSlideIndex = 0;

    private void Start()
    {
        if (slides.Count > 0)
        {
            ShowSlide(0);
        }
    }

    // FUNGSI UPDATE DIHAPUS. Kita tidak butuh ngecek klik tiap frame lagi.

    private void ShowSlide(int index)
    {
        displayImage.sprite = slides[index].artwork;
        /*displayText.text = slides[index].dialogueText*/;
    }

    // WAJIB PUBLIC agar bisa dipanggil oleh UI Button di Inspector
    public void NextSlide()
    {
        currentSlideIndex++;

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
}
