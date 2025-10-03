using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldMapController : MonoBehaviour
{
    [Header("Thiết Lập Bản Đồ")]
    public RectTransform playerShipUI;
    public List<LevelData> levels; // << SỬ DỤNG 'levels' THAY VÌ 'levelPositions'
    public List<Transform> levelDisplayPositions; // Danh sách vị trí của các hành tinh trên bản đồ

    [Header("Cài Đặt Di Chuyển")]
    public float moveSpeed = 200f;
    public string gameSceneName = "game";

    void Start()
    {
        if (playerShipUI == null || levels.Count == 0 || levelDisplayPositions.Count == 0) return;

        int currentLevelIndex = GameProgress.LoadLevel() - 1; // Màn 1 -> index 0
        int previousLevelIndex = Mathf.Max(0, currentLevelIndex - 1);

        if (currentLevelIndex >= levelDisplayPositions.Count) currentLevelIndex = levelDisplayPositions.Count - 1;

        // Đặt phi thuyền ở vị trí màn chơi trước đó
        playerShipUI.position = levelDisplayPositions[previousLevelIndex].position;

        // Bắt đầu di chuyển đến vị trí màn chơi hiện tại
        StartCoroutine(MoveShipCoroutine(currentLevelIndex));
    }

    private IEnumerator MoveShipCoroutine(int destinationIndex)
    {
        Vector3 destination = levelDisplayPositions[destinationIndex].position;
        yield return new WaitForSeconds(1.0f);

        // Xoay phi thuyền
        Vector3 direction = (destination - playerShipUI.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        playerShipUI.rotation = Quaternion.Euler(0, 0, angle - 90f);

        // Di chuyển
        while (Vector3.Distance(playerShipUI.position, destination) > 0.1f)
        {
            playerShipUI.position = Vector3.MoveTowards(playerShipUI.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }
        playerShipUI.position = destination;
        yield return new WaitForSeconds(2.0f);

        // Gán LevelData cần load
        GameManager.levelToLoad = levels[destinationIndex];
        
        // Chuyển cảnh
        SceneManager.LoadScene(gameSceneName);
    }
    
    // Hàm này sẽ được gọi từ các nút bấm trên UI
    public void SelectLevel(int levelIndex)
    {
        // Gán LevelData và chuyển cảnh
        GameManager.levelToLoad = levels[levelIndex];
        SceneManager.LoadScene(gameSceneName);
    }
}