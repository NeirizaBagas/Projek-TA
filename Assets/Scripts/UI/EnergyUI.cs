using UnityEngine;
using UnityEngine.UI;

public class EnergyUI : MonoBehaviour
{
    [SerializeField] private Slider energySlider; // Referensi ke Slider UI untuk menampilkan energi

    private void OnEnable()
    {
        EnergySystem.OnEnergyChanged += UpdateEnergyUI; // Subscribe ke event perubahan energi
    }

    private void OnDisable()
    {
        EnergySystem.OnEnergyChanged -= UpdateEnergyUI; // Unsubscribe dari event saat tidak diperlukan
    }

    private void UpdateEnergyUI(float persentage)
    {
        energySlider.value = persentage; // Update nilai slider berdasarkan persentase energi
    }
}
