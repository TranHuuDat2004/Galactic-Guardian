// File: CreditsScroller.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScroller : MonoBehaviour
{
    [Tooltip("Kéo đối tượng Text chứa credits vào đây.")]
    public RectTransform creditsTextTransform;

    [Tooltip("Tốc độ chữ chạy lên.")]
    public float scrollSpeed = 50f;
    
    [Tooltip("Vị trí Y mà khi chữ vượt qua, sẽ tự động quay về menu.")]
    public float endPositionY = 1500f;
    
    [Tooltip("Tên của scene Main Menu.")]
    public string mainMenuSceneName = "MainMenu";

    void Update()
    {
        if (creditsTextTransform == null) return;

        // Di chuyển chữ lên trên
        creditsTextTransform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);

        // Nếu chữ đã chạy hết màn hình, quay về menu
        if (creditsTextTransform.anchoredPosition.y > endPositionY)
        {
            GoToMainMenu();
        }
    }

    // Hàm này để cho nút Back sử dụng
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}