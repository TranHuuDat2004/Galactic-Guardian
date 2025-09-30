using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Cần cho UI
using TMPro;      // Cần cho TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton

    [Header("Thiết Lập Player")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    public float respawnDelay = 1.0f; // Thời gian chờ trước khi hồi sinh
    private PlayerController currentPlayer;

    [Header("Thiết Lập UI")]
    public GameObject gameOverUI; // Kéo GameObject chứa màn hình Game Over vào đây
    
    [Header("Âm Thanh")]
    public AudioClip gameOverSound;
    private AudioSource audioSource;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Ẩn màn hình Game Over khi bắt đầu
        if (gameOverUI != null) gameOverUI.SetActive(false);
        
        // Bắt đầu game bằng cách tạo Player
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        GameObject playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        currentPlayer = playerInstance.GetComponent<PlayerController>();
    }

    // Hàm này sẽ được gọi từ PlayerController khi nó hết mạng
    public void HandlePlayerDeath()
    {
        // Kiểm tra xem Player còn mạng không
        if (currentPlayer.lives > 0)
        {
            // Nếu còn, bắt đầu Coroutine hồi sinh
            StartCoroutine(RespawnPlayerCoroutine());
        }
        else
        {
            // Nếu hết mạng, kích hoạt Game Over
            GameOver();
        }
    }

    private IEnumerator RespawnPlayerCoroutine()
    {
        // Chờ một khoảng thời gian
        yield return new WaitForSeconds(respawnDelay);

        // Kích hoạt lại Player
        if (currentPlayer != null)
        {
            currentPlayer.gameObject.transform.position = playerSpawnPoint.position;
            currentPlayer.gameObject.transform.rotation = playerSpawnPoint.rotation;
            currentPlayer.Respawn(); // Gọi hàm đặc biệt để reset trạng thái
        }
    }

    void GameOver()
    {
        Debug.Log("GAME OVER!");

        // Hiện UI Game Over
        if (gameOverUI != null) gameOverUI.SetActive(true);
        
        // Phát âm thanh Game Over
        if (gameOverSound != null && audioSource != null)
        {
            // Dừng nhạc nền trước
            // (Bạn có thể thêm tham chiếu đến AudioSource nhạc nền ở đây nếu muốn)
            audioSource.PlayOneShot(gameOverSound);
        }

        // (Tùy chọn) Dừng thời gian
        Time.timeScale = 0f;
    }
}