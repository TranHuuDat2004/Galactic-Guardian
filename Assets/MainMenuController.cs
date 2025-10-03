using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string worldMapSceneName = "WorldMap";

    // Hàm này sẽ được gọi bởi nút "New Game"
    public void OnNewGameClicked()
    {
        // Xóa tiến trình cũ và đặt lại là màn 1
        GameProgress.ResetProgress();
        GameProgress.SaveLevel(1);
        
        // Chuyển đến bản đồ
        SceneManager.LoadScene(worldMapSceneName);
    }

    // Hàm này sẽ được gọi bởi nút "Continue"
    public void OnContinueClicked()
    {
        // Chỉ cần chuyển đến bản đồ, WorldMapController sẽ tự xử lý
        SceneManager.LoadScene(worldMapSceneName);
    }

    // Hàm này sẽ được gọi bởi nút "Exit"
    public void OnExitClicked()
    {
        Debug.Log("Thoát game!");
        Application.Quit();
    }
}