using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string worldMapSceneName = "WorldMap";

    [Header("UI Panels")]
    public GameObject mainButtonsPanel;
    public GameObject modeSelectionPanel;
    public GameObject comingSoonPanel;
    
    // Hàm Start chỉ cần quản lý các panel
    void Start()
    {
        // Đảm bảo các panel ở trạng thái đúng khi bắt đầu
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
        if (modeSelectionPanel != null) modeSelectionPanel.SetActive(false);
        if (comingSoonPanel != null) comingSoonPanel.SetActive(false);
    }
    
    public void OnNewGameClicked()
    {
        GameProgress.ResetProgress();
        
        mainButtonsPanel.SetActive(false);
        modeSelectionPanel.SetActive(true);
    }
    
    public void OnContinueClicked()
    {
        mainButtonsPanel.SetActive(false);
        modeSelectionPanel.SetActive(true);
    }
    
    public void OnExitClicked()
    {
        Debug.Log("Thoát game!");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    // Các hàm chuyển cảnh không cần ẩn con trỏ nữa
    public void On1PlayerClicked()
    {
        GameModeManager.NumberOfPlayers = 1;
        SceneManager.LoadScene(worldMapSceneName);
    }

    public void On2PlayerClicked()
    {
        GameModeManager.NumberOfPlayers = 2;
        SceneManager.LoadScene(worldMapSceneName);
    }

    public void OnOnlineClicked()
    {
        comingSoonPanel.SetActive(true);
    }
    
    public void OnBackButtonClicked()
    {
        modeSelectionPanel.SetActive(false);
        mainButtonsPanel.SetActive(true);
    }
    
    public void OnCloseComingSoonClicked()
    {
        comingSoonPanel.SetActive(false);
    }
}