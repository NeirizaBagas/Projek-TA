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

    [Header("Snapshot Review")]
    [SerializeField] private Image snapshotReviewImage;
    private Sprite snapshotSprite;

    [Header("Flashlight Settings")]
    [SerializeField] private GameObject flashlight;
    [SerializeField] private float flashTime = 0.5f;

    public static event Action<bool> OnPhotoModeReadyToCapture;
    public static event Action<bool> OnPhotoReadyToView;
    public static event Action OnAnimalPhotoUpdated;

    private void OnEnable()
    {
        JournalCamButton.OnAnimalPhotoRequested += ChangeAnimalPhotoIndex;
    }

    private void OnDisable()
    {
        JournalCamButton.OnAnimalPhotoRequested -= ChangeAnimalPhotoIndex;
    }

    public void CaptureSnapshot() // Dipanggil dari skrip interacttoobject saat player menekan tombol foto(klik kanan mouse)
    {
        StartCoroutine(TakeSnapshot());
    }

    IEnumerator TakeSnapshot() // Coroutine untuk ngambil snapshot setelah frame selesai dirender
    {
        OnPhotoModeReadyToCapture?.Invoke(false); // Beri tahu UI untuk tutup mode foto saat mulai proses pengambilan snapshot
        yield return new WaitForEndOfFrame(); // Tunggu hingga frame selesai untuk memastikan semua sudah dirender
        int width = Screen.width;
        int height = Screen.height;
        snapshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        Rect regionToRead = new Rect(0, 0, width, height);
        snapshot.ReadPixels(regionToRead, 0, 0);
        snapshot.Apply();
        SavingPhoto();
    }

    public void ChangeAnimalPhotoIndex(int index)
    {
        animalSnapshotIndex = index;
    }

    public void SavingPhoto() // Fungsi untuk menyimpan snapshot ke database dan update UI, dipanggil setelah snapshot diambil
    {
        if (animalSnapshotIndex < 0 || animalSnapshotIndex >= journalDatabase.animalDatabase.Length) return;

        if (snapshotReviewImage.sprite != null)
        {
            Destroy(snapshotReviewImage.sprite.texture); // Hapus snapshot sebelumnya untuk menghindari memory leak
        }

        snapshotSprite = Sprite.Create(snapshot, new Rect(0, 0, snapshot.width, snapshot.height), new Vector2(0.5f, 0.5f), 100.0f);

        snapshotReviewImage.sprite = snapshotSprite; // Tampilkan snapshot di UI review

        OnPhotoReadyToView?.Invoke(true); // Beri tahu UI untuk tampilkan review snapshot yang baru diambil

        StartCoroutine(CameraFlashEffect()); // Mulai efek flash kamera
    }


    IEnumerator CameraFlashEffect()
    {
        // Audio efek suara foto
        flashlight.SetActive(true);
        yield return new WaitForSeconds(flashTime);
        flashlight.SetActive(false);
    }

    public void SavePhotoToJournal()
    {
        SODataHewan targetHewan = journalDatabase.animalDatabase[animalSnapshotIndex];

        if (targetHewan != null)
        {
            //if (targetHewan.animalSprite != null)
            //{
            //    Destroy(targetHewan.animalSprite.texture); // Hapus sprite sebelumnya untuk menghindari memory leak
            //}
            // Tambahkan (UnityEngine.Sprite) di depan variabelnya
            targetHewan.animalSprite = (UnityEngine.Sprite)snapshotSprite;
            OnAnimalPhotoUpdated?.Invoke();
            Debug.Log($"Slot hewan index {animalSnapshotIndex} berhasil diisi");
        }
        else
        {
            Debug.Log($"Hewan dengan index {animalSnapshotIndex} masih kosong di database");
        }
        OnPhotoReadyToView?.Invoke(false); // Beri tahu UI untuk tutup review snapshot setelah foto disimpan ke jurnal
    }

    public void RetakePhoto()
    {
        //if (snapshotReviewImage.sprite != null)
        //{
        //    Destroy(snapshotReviewImage.sprite.texture); // Hapus snapshot sebelumnya untuk menghindari memory leak
        //    snapshotReviewImage.sprite = null; // Reset gambar review
        //}
        OnPhotoReadyToView?.Invoke(false); // Beri tahu UI untuk tutup review snapshot saat mulai proses pengambilan snapshot baru
        OnPhotoModeReadyToCapture?.Invoke(true); // Mulai proses pengambilan snapshot baru
    }

    public void ClearSnapshot()
    {
        OnPhotoModeReadyToCapture?.Invoke(true); // Beri tahu UI untuk buka mode foto saat mulai proses pengambilan snapshot
        OnPhotoReadyToView?.Invoke(true); // Beri tahu UI untuk tampilkan review snapshot yang baru diambil
    }

}
