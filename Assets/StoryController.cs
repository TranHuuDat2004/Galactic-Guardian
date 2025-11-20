using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StoryController : MonoBehaviour
{
    public static string textToDisplay;

    [Header("Tham Chiếu UI")]
    public TextMeshProUGUI storyTextComponent;

    [Header("Cài Đặt")]
    public float scrollSpeed = 50f;
    public float delayAfterScroll = 2f;
    public string gameSceneName = "game";

    // --- THÊM MỚI Ở ĐÂY ---
    private bool canSkip = false; // Cờ để tránh người chơi vô tình skip ngay lập tức
    private bool isSkipping = false; // Cờ để đảm bảo hàm skip chỉ được gọi 1 lần
    private Coroutine scrollingCoroutine; // Để lưu trữ coroutine đang chạy
    // ----------------------

    void Start()
    {
        if (string.IsNullOrEmpty(textToDisplay))
        {
            LoadGameScene();
            return;
        }

        storyTextComponent.text = textToDisplay;
        // Lưu lại coroutine để có thể dừng nó lại
        scrollingCoroutine = StartCoroutine(ScrollStoryCoroutine());
    }

    // --- THÊM HÀM UPDATE() ---
    void Update()
    {
        // Nếu người chơi nhấn bất kỳ phím nào hoặc click chuột
        if (canSkip && Input.anyKeyDown)
        {
            SkipStory();
        }
    }
    // -------------------------

    private IEnumerator ScrollStoryCoroutine()
    {
        // --- THÊM MỚI: Bật cờ cho phép skip sau một khoảng trễ ngắn ---
        yield return new WaitForSeconds(0.5f); // Chờ nửa giây để tránh skip nhầm
        canSkip = true;
        // -----------------------------------------------------------

        

        storyTextComponent.rectTransform.anchoredPosition = new Vector2(0, -Screen.height);
        float targetY = Screen.height;

        while (storyTextComponent.rectTransform.anchoredPosition.y < targetY)
        {
            storyTextComponent.rectTransform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(delayAfterScroll);
        LoadGameScene();
    }

    // --- HÀM MỚI DÀNH CHO NÚT BẤM VÀ INPUT ---
    public void SkipStory()
    {
        // Nếu chưa skip, thì thực hiện skip
        if (!isSkipping)
        {
            isSkipping = true;
            Debug.Log("Đã bỏ qua cốt truyện!");

            // Dừng coroutine đang cuộn chữ lại
            if (scrollingCoroutine != null)
            {
                StopCoroutine(scrollingCoroutine);
            }
            
            // Tải màn chơi ngay lập tức
            LoadGameScene();
        }
    }
    // -------------------------------------------

    void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}