using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalManager : MonoBehaviour
{
    [SerializeField] private SODataJournal journalDatabase;

    [Header("Left Page Settings")]
    [SerializeField] private TMP_Text leftAnimalName;
    [SerializeField] private TMP_Text leftAnimalDescription;
    [SerializeField] private TMP_Text leftPageNumber;
    [SerializeField] private Button prevButton;
    [SerializeField] private JournalCamButton leftPhotoButton;
    [SerializeField] private Image leftAnimalPhoto;

    [Header("Right Page Settings")]
    [SerializeField] private TMP_Text rightAnimalName;
    [SerializeField] private TMP_Text rightAnimalDescription;
    [SerializeField] private TMP_Text rightPageNumber;
    [SerializeField] private Button nextButton;
    [SerializeField] private JournalCamButton rightPhotoButton;
    [SerializeField] private Image rightAnimalPhoto;

    private int currentPage = 0;

    public static event Action<bool> OnJournalPageOpenClose;

    private void Awake()
    {
        prevButton.onClick.AddListener(PreviousPage);
        nextButton.onClick.AddListener(NextPage);
    }

    private void OnEnable()
    {
        SnapshotSystem.OnAnimalPhotoUpdated += UpdateJournalPage;
        UIManager.OnTriggerUpdateJournal += UpdateJournalPage;
    }

    private void OnDisable()
    {
        SnapshotSystem.OnAnimalPhotoUpdated -= UpdateJournalPage;
        UIManager.OnTriggerUpdateJournal -= UpdateJournalPage;
    }

    void Start()
    {
        UpdateJournalPage();
        UpdateButton();
    }

    public void NextPage()
    {
        if (currentPage + 2 < journalDatabase.animalDatabase.Length)
        {
            nextButton.interactable = true;
            currentPage += 2;
            UpdateJournalPage();
            UpdateButton();
        }
    }

    public void PreviousPage()
    {
        if (currentPage - 2 >= 0)
        {
            prevButton.interactable = true;
            currentPage -= 2;
            UpdateJournalPage();
            UpdateButton();
        }
    }

    private void UpdateButton()
    {
        if (currentPage + 2 >= journalDatabase.animalDatabase.Length)
        {
            nextButton.interactable = false;
        }
        else
        {
            nextButton.interactable = true;
        }

        if (currentPage - 2 < 0)
        {
            prevButton.interactable = false;
        }
        else
        {
            prevButton.interactable = true;
        }
    }

    private void UpdateJournalPage()
    {
        SODataHewan leftAnimal = journalDatabase.animalDatabase[currentPage];
        SODataHewan rightAnimal = journalDatabase.animalDatabase[currentPage + 1];

        leftPhotoButton.animalIndexPhoto = currentPage;
        rightPhotoButton.animalIndexPhoto = currentPage + 1;

        leftAnimalName.text = leftAnimal.animalName;
        leftAnimalDescription.text = leftAnimal.animalDescription;
        leftPageNumber.text = (currentPage + 1).ToString();
        leftAnimalPhoto.sprite = leftAnimal.animalSprite;

        rightAnimalName.text = rightAnimal.animalName;
        rightAnimalDescription.text = rightAnimal.animalDescription;
        rightPageNumber.text = (currentPage + 2).ToString();
        rightAnimalPhoto.sprite = rightAnimal.animalSprite;
    }

    public void CloseJournal()
    {
        OnJournalPageOpenClose?.Invoke(false);
    }
}
