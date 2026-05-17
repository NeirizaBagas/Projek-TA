using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SODataJournal journalDatabase;
    [SerializeField] private int targetPhotosToComplete = 5;
    [SerializeField] private int winSceneIndex;
    [SerializeField] private int loseSceneIndex;
    private bool isGameOver = false;
    private bool isGameWon = false;

    public static event Action onGameStarted;


    public static event Action onGameCompleted;

    private void Start()
    {
        AudioManager.Instance.PlayBGM(1); // Mainkan BGM gameplay (asumsi index 1 adalah BGM gameplay)
        onGameStarted?.Invoke(); // Beri tahu sistem lain bahwa game telah dimulai
    }

    private void OnEnable()
    {
        SnapshotSystem.OnAnimalPhotoUpdated += CheckWinCondition;
        TrapScheduleManager.OnGameOver += LoseCondition;
    }

    private void OnDisable()
    {
        SnapshotSystem.OnAnimalPhotoUpdated -= CheckWinCondition;
        TrapScheduleManager.OnGameOver -= LoseCondition;
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

        if (currentPhotoCount >= targetPhotosToComplete && TimeDayManager._currentDay == 12)
        {
            Debug.Log("Selamat! Kamu telah menyelesaikan game dengan mengambil cukup foto hewan!");
            WinCondition();
        }
        else if (TimeDayManager._currentDay > 12)
        {
            Debug.Log("Waktu habis! Kamu gagal menyelesaikan game karena tidak cukup foto yang diambil.");
            LoseCondition();
        }
    }

    private void WinCondition()
    {
        AudioManager.Instance.StopBGM(); // Hentikan BGM saat masuk ke scene kemenangan
        onGameCompleted?.Invoke(); // Beri tahu sistem lain bahwa game telah selesai
        SceneManager.LoadScene(winSceneIndex);
    }

    private void LoseCondition()
    {
        AudioManager.Instance.StopBGM(); // Hentikan BGM saat masuk ke scene kekalahan
        onGameCompleted?.Invoke(); // Beri tahu sistem lain bahwa game telah selesai
        SceneManager.LoadScene(loseSceneIndex);
    }
}
