using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TypingManager : MonoBehaviour
{
    [Header("Settings")]
    public string targetWord = "GEMINI";
    [SerializeField] private float waitingTime = 0.1f;

    [Header("UI References")]
    public TextMeshProUGUI displayText;
    public GameObject bgPrefab; // Prefab kotak Image tadi
    public Transform bgParent;  // Objek kosong sebagai penampung

    [Header("Colors")]
    public Color colorIdle = new Color(1, 1, 1, 0.1f); // Putih transparan
    public Color colorCorrect = Color.green;
    public Color colorWrong = Color.red;

    private int currentIndex = 0;
    private bool canType = false;
    private int[] letterStatus; // 0: Idle, 1: Benar, 2: Salah
    private List<Image> spawnedBgs = new List<Image>();

    private void Awake()
    {
        letterStatus = new int[targetWord.Length];
        displayText.text = targetWord;

        // Paksa TextMeshPro untuk menghitung posisi huruf di awal
        displayText.ForceMeshUpdate();

        SetupBackgrounds();
    }

    void SetupBackgrounds()
    {
        // Bersihkan jika ada sisa
        foreach (var bg in spawnedBgs) Destroy(bg.gameObject);
        spawnedBgs.Clear();

        for (int i = 0; i < targetWord.Length; i++)
        {
            GameObject newBg = Instantiate(bgPrefab, bgParent);
            Image img = newBg.GetComponent<Image>();
            img.color = colorIdle;
            spawnedBgs.Add(img);

            // Ambil posisi huruf i dari TextMeshPro
            TMP_CharacterInfo charInfo = displayText.textInfo.characterInfo[i];

            // Tentukan posisi tengah huruf
            Vector3 charMidPos = (charInfo.bottomLeft + charInfo.topRight) / 2f;

            // Pindahkan kotak background ke posisi huruf tersebut
            newBg.transform.localPosition = charMidPos;
        }
    }

    private void OnEnable()
    {
        canType = false;
        StartCoroutine(OnConnected());
    }

    private void OnDisable()
    {
        if (Keyboard.current != null)
            Keyboard.current.onTextInput -= OnTyped;
    }

    private IEnumerator OnConnected()
    {
        yield return new WaitForSeconds(waitingTime);
        if (Keyboard.current != null)
            Keyboard.current.onTextInput += OnTyped;
        canType = true;
    }

    private void Update()
    {
        if (canType && Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            HandleBackspace();
        }
    }

    private void OnTyped(char ch)
    {
        if (!canType || currentIndex >= targetWord.Length) return;

        if (ch == targetWord[currentIndex]) letterStatus[currentIndex] = 1;
        else letterStatus[currentIndex] = 2;

        UpdateVisuals();
        currentIndex++;

        if (currentIndex == targetWord.Length) Debug.Log("Selesai!");
    }

    private void HandleBackspace()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            letterStatus[currentIndex] = 0;
            UpdateVisuals();
        }
    }

    void UpdateVisuals()
    {
        for (int i = 0; i < spawnedBgs.Count; i++)
        {
            if (letterStatus[i] == 1) spawnedBgs[i].color = colorCorrect;
            else if (letterStatus[i] == 2) spawnedBgs[i].color = colorWrong;
            else spawnedBgs[i].color = colorIdle;
        }
    }
}