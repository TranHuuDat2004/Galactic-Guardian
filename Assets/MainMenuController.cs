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


    // --- THÊM MỚI Ở ĐÂY ---
    [Header("Tùy Chỉnh Cursor")]
    public Texture2D cursorSprite; // Kéo ảnh con trỏ đã import vào đây
    public Vector2 cursorHotspot = Vector2.zero; // "Điểm nóng" của con trỏ (mặc định là góc trên bên trái)
    // ----------------------

    void Start()
    {

        // --- LOGIC CURSOR ĐÃ ĐƯỢC NÂNG CẤP ---
        // Ẩn con trỏ mặc định đi nếu đang trong Editor để không bị chồng chéo
        #if UNITY_EDITOR
        Cursor.visible = true; // Trong Editor, vẫn hiện con trỏ hệ thống để dễ debug
        #else
        Cursor.visible = false; // Khi build game, ẩn con trỏ hệ thống đi
        #endif

        // Thiết lập con trỏ tùy chỉnh
        if (cursorSprite != null)
        {
            Cursor.SetCursor(cursorSprite, cursorHotspot, CursorMode.Auto);
        }
        // -------------------------------------

        // Đảm bảo các panel ở trạng thái đúng khi bắt đầu
        mainButtonsPanel.SetActive(true);
        modeSelectionPanel.SetActive(false);
        comingSoonPanel.SetActive(false);
    }

// Khi thoát khỏi Scene này, trả lại con trỏ mặc định (tùy chọn nhưng nên có)
    private void OnDestroy()
    {
        // Reset con trỏ về mặc định
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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

    // Nếu đang chạy trong Unity Editor
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    // Nếu đang chạy trong bản build
    #else
        Application.Quit();
    #endif
}

    // --- CÁC HÀM MỚI CHO CÁC NÚT BẤM ---

    // Được gọi bởi nút "1 NGƯỜI CHƠI"
    public void On1PlayerClicked()
    {
        GameModeManager.NumberOfPlayers = 1;
         // --- THÊM DÒNG NÀY ---
    Cursor.visible = false; // Ẩn con trỏ ngay trước khi tải màn chơi
    // ----------------------

        SceneManager.LoadScene(worldMapSceneName);
    }

    // Được gọi bởi nút "2 NGƯỜI CHƠI"
    public void On2PlayerClicked()
    {
        GameModeManager.NumberOfPlayers = 2;
         // --- THÊM DÒNG NÀY ---
    Cursor.visible = false; // Ẩn con trỏ ngay trước khi tải màn chơi
    // ----------------------
    
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