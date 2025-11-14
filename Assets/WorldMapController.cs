using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldMapController : MonoBehaviour
{
    [Header("Thiết Lập Bản Đồ")]
    public RectTransform playerShipUI;
    public List<LevelData> levels;
    public List<Transform> levelDisplayPositions;

    [Header("Cài Đặt Di Chuyển")]
    public float moveSpeed = 200f;
    public string gameSceneName = "game";
    public string storySceneName = "story"; // Tên của Scene cốt truyện

    void Start()
    {
        if (playerShipUI == null || levels.Count == 0 || levelDisplayPositions.Count == 0) return;

        int currentLevelIndex = GameProgress.LoadLevel() - 1;
        int previousLevelIndex = Mathf.Max(0, currentLevelIndex - 1);

        if (currentLevelIndex >= levelDisplayPositions.Count) currentLevelIndex = levelDisplayPositions.Count - 1;

        playerShipUI.position = levelDisplayPositions[previousLevelIndex].position;
        StartCoroutine(MoveShipCoroutine(currentLevelIndex));
    }

    private IEnumerator MoveShipCoroutine(int destinationIndex)
    {
        Vector3 destination = levelDisplayPositions[destinationIndex].position;
        yield return new WaitForSeconds(1.0f);

        Vector3 direction = (destination - playerShipUI.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        playerShipUI.rotation = Quaternion.Euler(0, 0, angle - 90f);

        while (Vector3.Distance(playerShipUI.position, destination) > 0.1f)
        {
            playerShipUI.position = Vector3.MoveTowards(playerShipUI.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }
        playerShipUI.position = destination;
        yield return new WaitForSeconds(2.0f);

        // Gán LevelData
        GameManager.levelToLoad = levels[destinationIndex];
        // Gửi text cốt truyện cho StoryController
        StoryController.textToDisplay = levels[destinationIndex].storyText;
        
        // --- ĐÂY LÀ DÒNG ĐÃ ĐƯỢC SỬA ---
        // Chuyển sang Scene cốt truyện thay vì vào game trực tiếp
        SceneManager.LoadScene(storySceneName);
        // -------------------------------
    }
    
    // Hàm này đã đúng, không cần sửa
    public void SelectLevel(int levelIndex)
    {
        GameManager.levelToLoad = levels[levelIndex];
        StoryController.textToDisplay = levels[levelIndex].storyText;
        SceneManager.LoadScene(storySceneName);
    }
}