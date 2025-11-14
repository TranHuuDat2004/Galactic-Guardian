using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StoryController : MonoBehaviour
{
    // Biến tĩnh để nhận dữ liệu từ các Scene khác
    public static string textToDisplay;

    [Header("Tham Chiếu UI")]
    public TextMeshProUGUI storyTextComponent;

    [Header("Cài Đặt")]
    public float scrollSpeed = 50f;
    public float delayAfterScroll = 2f; // Chờ 2s sau khi cuộn xong
    public string gameSceneName = "game"; // Tên Scene game chính

    void Start()
    {
        // Kiểm tra xem có text để hiển thị không
        if (string.IsNullOrEmpty(textToDisplay))
        {
            // Nếu không có, bỏ qua và vào game luôn
            LoadGameScene();
            return;
        }

        // Hiển thị text và bắt đầu cuộn
        storyTextComponent.text = textToDisplay;
        StartCoroutine(ScrollStoryCoroutine());
    }

    private IEnumerator ScrollStoryCoroutine()
    {
        // Bắt đầu từ vị trí dưới cùng của màn hình
        storyTextComponent.rectTransform.anchoredPosition = new Vector2(0, -Screen.height);

        // Tính toán vị trí đích (khi text đã cuộn hết lên trên)
        float targetY = Screen.height;

        // Bắt đầu cuộn
        while (storyTextComponent.rectTransform.anchoredPosition.y < targetY)
        {
            storyTextComponent.rectTransform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
            yield return null;
        }

        // Đợi một chút
        yield return new WaitForSeconds(delayAfterScroll);

        // Chuyển sang màn chơi
        LoadGameScene();
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}