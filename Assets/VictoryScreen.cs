using UnityEngine;
using UnityEngine.SceneManagement; // QUAN TRỌNG: Cần có để xử lý Scene

public class VictoryScreenController : MonoBehaviour
{
    // Tên của Scene Menu Chính. Bạn có thể thay đổi trong Inspector.
    public string mainMenuSceneName = "MainMenu"; 

// --- THÊM HÀM MỚI Ở ĐÂY ---
    void Start()
    {
        // Luôn luôn hiện con trỏ chuột khi vào màn hình chiến thắng
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    // ---------------------------
    
    // Hàm này sẽ được gọi khi người chơi bấm vào nút
    public void GoToMainMenu()
    {
        Debug.Log("Đang quay về Menu Chính...");
        SceneManager.LoadScene(mainMenuSceneName);
    }
}