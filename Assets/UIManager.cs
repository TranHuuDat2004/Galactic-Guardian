using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton để dễ dàng truy cập

    [Header("Thanh Máu Boss")]
    public GameObject healthBarUI;
    public Image healthBarFill;
    public TextMeshProUGUI bossNameText;

    private void Awake()
    {
        Instance = this;
    }

    // Hàm để hiện thanh máu
    public void ShowBossHealthBar(string bossName)
    {
        if (healthBarUI != null)
        {
            if (bossNameText != null) bossNameText.text = bossName;
            healthBarUI.SetActive(true);
        }
    }

    // Hàm để ẩn thanh máu
    public void HideBossHealthBar()
    {
        if (healthBarUI != null)
        {
            healthBarUI.SetActive(false);
        }
    }

    // Hàm để cập nhật giá trị thanh máu
    public void UpdateBossHealth(float currentHealth, float maxHealth)
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}