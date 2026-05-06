using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SnapshotSystem : MonoBehaviour
{
    private Texture2D snapshot;
    private int animalSnapshotIndex;
    public bool canUpdatePhoto = true;
    [SerializeField] private SODataJournal journalDatabase;
    public static bool isCapturingPhoto { get; private set; }

    [Header("Snapshot Review")]
    [SerializeField] private Image snapshotReviewImage;
    private Sprite snapshotSprite;

    [Header("Flashlight Settings")]
    [SerializeField] private Light flashlight;
    [SerializeField] private float flashTime = 0.5f;

    [Header("Photo Fade Effect")]
    [SerializeField] private Animator fadingAnimation;

    public static event Action<bool> OnPhotoModeReadyToCapture;
    public static event Action<bool> OnPhotoReadyToView;
    public static event Action OnAnimalPhotoUpdated;

    private void OnEnable()
    {
        ItemManager.OnAnimalPhotoRequested += ChangeAnimalPhotoIndex;
    }

    private void OnDisable()
    {
        ItemManager.OnAnimalPhotoRequested -= ChangeAnimalPhotoIndex;
    }

    public void CaptureSnapshot() // Dipanggil dari skrip interacttoobject saat player menekan tombol foto(klik kanan mouse)
    {
        StartCoroutine(TakeSnapshot());
    }

    IEnumerator TakeSnapshot() // Coroutine untuk ngambil snapshot setelah frame selesai dirender
    {
        isCapturingPhoto = true; // Set flag untuk menandakan proses pengambilan snapshot sedang berlangsung
        OnPhotoModeReadyToCapture?.Invoke(false); // Beri tahu UI untuk tutup mode foto saat mulai proses pengambilan snapshot
        yield return new WaitForEndOfFrame(); // Tunggu hingga frame selesai untuk memastikan semua sudah dirender

        int width = Screen.width;
        int height = Screen.height;

        Texture2D currentSnapshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        Rect regionToRead = new Rect(0, 0, width, height);
        currentSnapshot.ReadPixels(regionToRead, 0, 0);
        currentSnapshot.Apply();

        SavingPhoto(currentSnapshot);
        isCapturingPhoto = false; // Reset flag setelah proses pengambilan snapshot selesai
    }

    public void ChangeAnimalPhotoIndex(int index)
    {
        animalSnapshotIndex = index;
    }

    public void SavingPhoto(Texture2D capturedTex) // Fungsi untuk menyimpan snapshot ke database dan update UI, dipanggil setelah snapshot diambil
    {
        if (animalSnapshotIndex < 0 || animalSnapshotIndex >= journalDatabase.animalDatabase.Length) return;

        snapshotSprite = Sprite.Create(capturedTex, new Rect(0, 0, capturedTex.width, capturedTex.height), new Vector2(0.5f, 0.5f), 100.0f);
        snapshotReviewImage.sprite = snapshotSprite;
        


        OnPhotoReadyToView?.Invoke(true);
        StartCoroutine(CameraFlashEffect());
        fadingAnimation.Play("PhotoFade"); // Trigger animasi fade saat snapshot diambil
    }


    IEnumerator CameraFlashEffect()
    {
        // Audio efek suara foto
        flashlight.enabled = true;
        yield return new WaitForSeconds(flashTime);
        flashlight.enabled = false;
    }

    public void SavePhotoToJournal()
    {
        SODataHewan targetHewan = journalDatabase.animalDatabase[animalSnapshotIndex];

        if (targetHewan != null)
        {
            snapshotSprite.name = "Snapshot_Review " + targetHewan.animalName; // Beri nama agar mudah diidentifikasi
            targetHewan.animalSprite = (UnityEngine.Sprite)snapshotSprite;
            OnAnimalPhotoUpdated?.Invoke();
            Debug.Log($"Slot hewan index {animalSnapshotIndex} berhasil diisi");
        }
        else
        {
            Debug.Log($"Hewan dengan index {animalSnapshotIndex} masih kosong di database");
        }
        OnPhotoReadyToView?.Invoke(false); // Beri tahu UI untuk tutup review snapshot setelah foto disimpan ke jurnal
        OnPhotoModeReadyToCapture?.Invoke(false); // Beri tahu UI untuk tutup mode foto setelah foto disimpan ke jurnal
        ItemManager.Instance.ResetExclusiveItemState(); // Reset state item eksklusif setelah menyimpan foto ke jurnal
    }

    public void RetakePhoto()
    {
        OnPhotoReadyToView?.Invoke(false); // Beri tahu UI untuk tutup review snapshot saat mulai proses pengambilan snapshot baru
        OnPhotoModeReadyToCapture?.Invoke(true); // Mulai proses pengambilan snapshot baru
    }

    public void ClearSnapshot()
    {
        OnPhotoModeReadyToCapture?.Invoke(true); // Beri tahu UI untuk buka mode foto saat mulai proses pengambilan snapshot
        OnPhotoReadyToView?.Invoke(false); // Beri tahu UI untuk tampilkan review snapshot yang baru diambil
    }

}
