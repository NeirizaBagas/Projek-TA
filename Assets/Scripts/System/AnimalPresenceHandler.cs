using UnityEngine;

public class AnimalPresenceHandler : MonoBehaviour
{
    [Tooltip("Tentukan ini hewan apa supaya tidak salah merespons event")]
    [SerializeField] private AnimalType myAnimalType;

    private void OnEnable()
    {
        // Berlangganan (subscribe) ke event dari TrapScheduleManager
        TrapScheduleManager.OnAnimalLost += HandleAnimalLost;
        TrapScheduleManager.OnAnimalSafe += HandleAnimalSafe;
    }

    private void OnDisable()
    {
        // Wajib cabut langganan (unsubscribe) agar tidak error saat objek hancur
        TrapScheduleManager.OnAnimalLost -= HandleAnimalLost;
        TrapScheduleManager.OnAnimalSafe -= HandleAnimalSafe;
    }

    private void HandleAnimalLost(AnimalType lostAnimal)
    {
        // Cek dulu, apakah pengumuman yang hilang itu adalah SAYA?
        if (lostAnimal == myAnimalType)
        {
            Debug.Log($"{myAnimalType} menghilang dari map!");

            // Matikan NPC Hewan ini dari dunia game (dianggap sudah diburu)
            gameObject.SetActive(false);
        }
    }

    private void HandleAnimalSafe(AnimalType safeAnimal)
    {
        if (safeAnimal == myAnimalType)
        {
            Debug.Log($"{myAnimalType} sekarang aman dari pemburu!");

            // (Opsional) Mungkin kamu mau mengubah state FSM hewan ini jadi "Relax"
            // atau memutar partikel efek daun-daun bahagia?
        }
    }
}
