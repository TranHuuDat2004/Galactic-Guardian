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
    public GameObject player2LivesUI_Container; // Kéo cụm UI mạng của P2 vào đây
    public TextMeshProUGUI livesText_P2;

    [Header("UI Kỹ Năng Co-op")]
    public GameObject coopSkillUI_Container; // << BIẾN BỊ THIẾU
    public Image coopSkillFill;

 // --- THÊM MỚI Ở ĐÂY ---
    [Header("UI Tạm Dừng")]
    public GameObject pauseMenuUI; // Kéo PauseMenu_Panel vào đây

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Ẩn các UI không cần thiết khi bắt đầu
        if (healthBarUI != null) healthBarUI.SetActive(false);
        
        // Chỉ hiện UI của Player 2 và Co-op Skill nếu chơi 2 người
        if (GameModeManager.NumberOfPlayers == 1)
        {
            if (player2LivesUI_Container != null) player2LivesUI_Container.SetActive(false);
            if (coopSkillUI_Container != null) coopSkillUI_Container.SetActive(false);
        }
        else // Chơi 2 người
        {
            if (player2LivesUI_Container != null) player2LivesUI_Container.SetActive(true);
            if (coopSkillUI_Container != null) coopSkillUI_Container.SetActive(true);
        }
    }
    
    // --- CÁC HÀM MỚI ĐỂ ĐIỀU KHIỂN PAUSE MENU ---
    public void ShowPauseMenu()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
    }

    public void HidePauseMenu()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }
    // ---------------------------------------------


    // --- HÀM CẬP NHẬT COOLDOWN ĐÃ ĐỔI TÊN BIẾN ---
    public void UpdateCoopSkillCooldown(float fillAmount)
    {
        if (coopSkillFill != null)
        {
            coopSkillFill.fillAmount = fillAmount;
        }
    }
    // ------------------------------------------

    public void UpdateLives(int currentLives)
    {
        if (livesText != null)
        {
            livesText.text = "x " + currentLives;
        }
    }

    public void UpdateLives_P2(int currentLives)
    {
        if (livesText_P2 != null)
        {
            livesText_P2.text = "x " + currentLives;
        }
    }

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