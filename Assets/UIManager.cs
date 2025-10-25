using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Thanh Máu Boss")]
    public GameObject healthBarUI;
    public Image healthBarFill;
    public TextMeshProUGUI bossNameText;

    [Header("UI Mạng Sống Player 1")]
    public TextMeshProUGUI livesText;

    [Header("UI Mạng Sống Player 2")]
    public GameObject player2LivesUI_Container;
    public TextMeshProUGUI livesText_P2;

    [Header("UI Kỹ Năng Co-op")]
    public GameObject coopSkillUI_Container; // << DÙNG GAMEOBJECT CHA
    public Image coopSkillFill;

    private void Awake()
    {
        Instance = this;

        // --- CẬP NHẬT LOGIC Ở ĐÂY ---
        // Kiểm tra xem có phải chế độ 1 người chơi không
        bool isSinglePlayer = (GameModeManager.NumberOfPlayers < 2);
        
        // Ẩn UI của Player 2 nếu là 1 người chơi
        if (player2LivesUI_Container != null)
        {
            player2LivesUI_Container.SetActive(!isSinglePlayer); // Sẽ là false nếu 1 người, true nếu 2 người
        }

        // Ẩn UI kỹ năng co-op nếu là 1 người chơi
        if (coopSkillUI_Container != null)
        {
            coopSkillUI_Container.SetActive(!isSinglePlayer); // Tương tự
        }
        // -----------------------------
    }

    // Hàm cập nhật mạng sống P1
    public void UpdateLives(int currentLives)
    {
        if (livesText != null)
        {
            livesText.text = "x " + currentLives;
        }
    }

    // Hàm cập nhật mạng sống P2
    public void UpdateLives_P2(int currentLives)
    {
        if (livesText_P2 != null)
        {
            livesText_P2.text = "x " + currentLives;
        }
    }
    
    // Hàm cập nhật UI hồi chiêu kỹ năng co-op
    public void UpdateCoopSkillCooldown(float timer, float cooldown)
    {
        if (coopSkillFill != null)
        {
            if (cooldown > 0 && timer > 0)
            {
                // Hiển thị tiến trình hồi chiêu
                coopSkillFill.fillAmount = timer / cooldown;
            }
            else
            {
                // Khi sẵn sàng, thanh fill đầy
                coopSkillFill.fillAmount = 1; 
            }
        }
    }

    // Các hàm của Boss không thay đổi
    public void ShowBossHealthBar(string bossName)
    {
        if (healthBarUI != null)
        {
            if (bossNameText != null) bossNameText.text = bossName;
            healthBarUI.SetActive(true);
        }
    }

    public void HideBossHealthBar()
    {
        if (healthBarUI != null)
        {
            healthBarUI.SetActive(false);
        }
    }
    
    public void UpdateBossHealth(float currentHealth, float maxHealth)
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}