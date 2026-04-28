using UnityEngine;

public class ItemVisualHandler : MonoBehaviour
{
    [SerializeField] private GameObject myVisualObject; // Masukkan objek senter/kamera 3D-nya ke sini
    [SerializeField] private bool isPersistent; // Centang untuk Senter & Defuse Kit
    [SerializeField] private MeshRenderer[] allRenderer;

    private bool isEquipped = false;

    private void Awake()
    {
        //allRenderer = myVisualObject.GetComponentsInChildren<Renderer>(true);
        SetAllVisualsState(false); // Mulai dengan semua visual dimatikan
    }

    private void SetAllVisualsState(bool state)
    {
        foreach (var renderer in allRenderer)
        {
            renderer.enabled = state;
        } 
    }

    // --- Dipanggil oleh onHoverEnterEvent di UI ---
    public void ShowPreview()
    {
        SetAllVisualsState(true);
        Debug.Log("Preview Shown for " + gameObject.name);
    }

    // --- Dipanggil oleh onHoverExitEvent di UI ---
    public void HidePreview()
    {
        // Kalau item ini persistent dan lagi di-equip, jangan dimatikan!
        if (isPersistent && isEquipped) return;

        SetAllVisualsState(false);
    }

    // --- Dipanggil oleh onClickAction di UI ---
    public void ToggleEquipItem()
    {
        isEquipped = !isEquipped;

        // Paksa visual mengikuti status isEquipped saat diklik
        SetAllVisualsState(isEquipped);
    }

    // --- Khusus untuk Jurnal/Kamera (Item Non-Persistent) yang memicu UI Lain ---
    public void ForceHide()
    {
        isEquipped = false;
        SetAllVisualsState(false);
    }
}
