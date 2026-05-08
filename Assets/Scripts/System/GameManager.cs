using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SODataJournal journalDatabase;
    [SerializeField] private int targetPhotosToComplete = 5;

    private int currentPhotoCount = 0;

    public static event Action onGameCompleted;

    private void OnEnable()
    {
        SnapshotSystem.OnAnimalPhotoUpdated += CheckWinCondition;
    }

    private void OnDisable()
    {
        SnapshotSystem.OnAnimalPhotoUpdated -= CheckWinCondition;
    }

    private void CheckWinCondition()
    {
        foreach (var hewan in journalDatabase.animalDatabase)
        {
            if (hewan.animalSprite != null)
            {
                currentPhotoCount++;
            }
        }

        Debug.Log($"Jumlah foto yang sudah diambil: {currentPhotoCount}/{targetPhotosToComplete}");

        if (currentPhotoCount >= targetPhotosToComplete)
        {
            Debug.Log("Selamat! Kamu telah menyelesaikan game dengan mengambil cukup foto hewan!");
            onGameCompleted?.Invoke();
        }
    }
}
