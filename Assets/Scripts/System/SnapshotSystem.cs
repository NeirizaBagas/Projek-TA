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

    [Header("Photo Size Settings")]
    [SerializeField] private int photoWidth = 412;
    [SerializeField] private int photoHeight = 350;

    [Header("Snapshot Review")]
    [SerializeField] private Image snapshotReviewImage;
    private Sprite snapshotSprite;

    [Header("Flashlight Settings")]
    [SerializeField] private Light flashlight;
    [SerializeField] private float flashTime = 0.5f;

    [Header("Photo Fade Effect")]
    [SerializeField] private Animator fadingAnimation;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask animalLayer;
    [SerializeField] private float detectionRange = 25f;
    private Transform playerCamera;
    private bool isAnimalInView = false;

    public static event Action<bool> OnPhotoModeReadyToCapture;
    public static event Action<bool> OnPhotoReadyToView;
    public static event Action OnAnimalPhotoUpdated;

    private void Start()
    {
        playerCamera = Camera.main.transform;
    }

    private void OnEnable()
    {
        //ItemManager.OnAnimalPhotoRequested += ChangeAnimalPhotoIndex;
    }

    private void OnDisable()
    {
        //ItemManager.OnAnimalPhotoRequested -= ChangeAnimalPhotoIndex;
    }

    private void LateUpdate()
    {


    }

    public void CaptureSnapshot() // Dipanggil dari skrip interacttoobject saat player menekan tombol foto(klik kanan mouse)
    {
        isAnimalInView = TryDetectAnimal(out int detectedAnimalIndex);

        if (isAnimalInView)
        {
            animalSnapshotIndex = detectedAnimalIndex;
            Debug.Log($"Hewan terdeteksi dengan index {animalSnapshotIndex}. Mulai proses pengambilan snapshot...");
        }
        else
        {
            Debug.Log("Tidak ada hewan yang terdeteksi. Mulai proses pengambilan snapshot kosong...");
            animalSnapshotIndex = -1; // Set
        }

        StartCoroutine(TakeSnapshot());
    }

    private bool TryDetectAnimal(out int index)
    {
        index = -1;

        if (Physics.SphereCast(playerCamera.position, detectionRadius, playerCamera.transform.forward, out RaycastHit hit, detectionRange, animalLayer))
        {
            AnimalPhotoTarget target = hit.collider.GetComponent<AnimalPhotoTarget>();
            if (target != null)
            {
                index = target.animalID;
                Debug.Log($"Hewan terdeteksi dengan index {index}");
                return true;
            }
        }
        Debug.Log("Tidak ada hewan yang terdeteksi dalam jangkauan.");
        return false;
    }

    IEnumerator TakeSnapshot() // Coroutine untuk ngambil snapshot setelah frame selesai dirender
    {
        isCapturingPhoto = true; // Set flag untuk menandakan proses pengambilan snapshot sedang berlangsung
        OnPhotoModeReadyToCapture?.Invoke(false); // Beri tahu UI untuk tutup mode foto saat mulai proses pengambilan snapshot
        yield return new WaitForEndOfFrame(); // Tunggu hingga frame selesai untuk memastikan semua sudah dirender

        int startX = (Screen.width - photoWidth) / 2 ;
        int startY = (Screen.height - photoHeight) / 2;

        Texture2D currentSnapshot = new Texture2D(photoWidth, photoHeight, TextureFormat.RGB24, false);
        Rect regionToRead = new Rect(startX, startY, photoWidth, photoHeight);
        currentSnapshot.ReadPixels(regionToRead, 0, 0);
        currentSnapshot.Apply();

        SavingPhoto(currentSnapshot);
        isCapturingPhoto = false; // Reset flag setelah proses pengambilan snapshot selesai
    }

    //public void ChangeAnimalPhotoIndex(int index)
    //{
    //    animalSnapshotIndex = index;
    //}

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
        AudioManager.Instance.PlaySFX(1); // Mainkan SFX kamera (asumsi index 1 adalah suara kamera)
        yield return new WaitForSeconds(flashTime);
        flashlight.enabled = false;
    }

    public void SavePhotoToJournal()
    {
        if (!isAnimalInView || animalSnapshotIndex < 0)
        {
            Debug.Log("Tidak ada hewan yang terdeteksi atau index hewan tidak valid. Foto kosong tidak akan disimpan ke jurnal.");

            OnPhotoReadyToView?.Invoke(false); // Beri tahu UI untuk tutup review snapshot setelah foto disimpan ke jurnal
            OnPhotoModeReadyToCapture?.Invoke(false); // Beri tahu UI untuk tutup mode foto setelah foto disimpan ke jurnal
            ItemManager.Instance.ResetExclusiveItemState(); // Reset state item eksklusif setelah menyimpan foto ke jurnal
            return;
        }

        SODataHewan targetHewan = journalDatabase.animalDatabase[animalSnapshotIndex];

        if (targetHewan != null)
        {
            snapshotSprite.name = "Snapshot_Review " + targetHewan.animalName; // Beri nama agar mudah diidentifikasi
            targetHewan.animalSprite = (UnityEngine.Sprite)snapshotSprite;
            OnAnimalPhotoUpdated?.Invoke();
            Debug.Log("FOTO BERHASIL DISIMPAN KE JURNAL: " + targetHewan.animalName);
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

    private void OnDrawGizmosSelected()
    {
        // Pastikan kamera tidak null agar tidak error saat di editor
        if (playerCamera == null) return;

        // Ubah warna gizmo agar mudah dilihat (misal: kuning transparan)
        Gizmos.color = Color.yellow;

        // Tentukan titik awal dan titik akhir tembakan
        Vector3 startPos = playerCamera.position;
        Vector3 endPos = startPos + (playerCamera.forward * detectionRange);

        // 1. Gambar bola di titik awal dan titik maksimal
        Gizmos.DrawWireSphere(startPos, detectionRadius);
        Gizmos.DrawWireSphere(endPos, detectionRadius);

        // 2. Gambar garis tengah (seperti laser)
        Gizmos.DrawLine(startPos, endPos);

        // 3. Gambar garis luar penutup tabung (atas, bawah, kiri, kanan)
        Vector3 up = playerCamera.up * detectionRadius;
        Vector3 right = playerCamera.right * detectionRadius;

        Gizmos.DrawLine(startPos + up, endPos + up);       // Garis atas
        Gizmos.DrawLine(startPos - up, endPos - up);       // Garis bawah
        Gizmos.DrawLine(startPos + right, endPos + right); // Garis kanan
        Gizmos.DrawLine(startPos - right, endPos - right); // Garis kiri
    }

}
