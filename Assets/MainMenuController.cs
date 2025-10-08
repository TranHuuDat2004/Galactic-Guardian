using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string worldMapSceneName = "WorldMap";

    // --- THÊM MỚI Ở ĐÂY ---
    [Header("UI Panels")]
    public GameObject mainButtonsPanel;
    public GameObject modeSelectionPanel;
    public GameObject comingSoonPanel;
    // ----------------------

    void Start()
    {
        // Đảm bảo các panel ở trạng thái đúng khi bắt đầu
        mainButtonsPanel.SetActive(true);
        modeSelectionPanel.SetActive(false);
        comingSoonPanel.SetActive(false);
    }

    // Sửa hàm này: Thay vì vào game, nó sẽ hiện panel chọn chế độ
    public void OnNewGameClicked()
    {
        GameProgress.ResetProgress();
        GameProgress.SaveLevel(1);

        // Hiện panel chọn chế độ
        mainButtonsPanel.SetActive(false);
        modeSelectionPanel.SetActive(true);
    }

    // Sửa hàm này: Tương tự, hiện panel chọn chế độ
    public void OnContinueClicked()
    {
        // Hiện panel chọn chế độ
        mainButtonsPanel.SetActive(false);
        modeSelectionPanel.SetActive(true);
    }

    // Hàm này giữ nguyên
    public void OnExitClicked()
    {
        Debug.Log("Thoát game!");
        Application.Quit();
    }

    // --- CÁC HÀM MỚI CHO CÁC NÚT BẤM ---

    // Được gọi bởi nút "1 NGƯỜI CHƠI"
    public void On1PlayerClicked()
    {
        GameModeManager.NumberOfPlayers = 1;
        SceneManager.LoadScene(worldMapSceneName);
    }

    // Được gọi bởi nút "2 NGƯỜI CHƠI"
    public void On2PlayerClicked()
    {
        GameModeManager.NumberOfPlayers = 2;
        SceneManager.LoadScene(worldMapSceneName);
    }

    // Được gọi bởi nút "ONLINE"
    public void OnOnlineClicked()
    {
        comingSoonPanel.SetActive(true);
    }

    // Được gọi bởi nút "QUAY LẠI" trong ModeSelectionPanel
    public void OnBackButtonClicked()
    {
        modeSelectionPanel.SetActive(false);
        mainButtonsPanel.SetActive(true);
    }

    // Được gọi bởi nút "ĐÓNG" trong ComingSoonPanel
    public void OnCloseComingSoonClicked()
    {
        comingSoonPanel.SetActive(false);
    }
    // ------------------------------------
}