using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SODataJournal journalDatabase;
    [SerializeField] private int targetPhotosToComplete = 5;

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
        int currentPhotoCount = 0;

        foreach (var hewan in journalDatabase.animalDatabase)
        {
            if (hewan.animalSprite != null)
            {
                currentPhotoCount++;
            }
        }

        Debug.Log($"Jumlah foto yang sudah diambil: {currentPhotoCount}/{journalDatabase.animalDatabase.Length}");

        if (currentPhotoCount >= journalDatabase.animalDatabase.Length)
        {
            Debug.Log("Selamat! Kamu telah menyelesaikan game dengan mengambil cukup foto hewan!");
            onGameCompleted?.Invoke();
        }
    }
}
