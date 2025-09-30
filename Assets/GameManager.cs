using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton

    [Header("Thiết Lập Player")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    public float respawnDelay = 1.0f;
    private PlayerController currentPlayer;

    [Header("Thiết Lập UI")]
    public GameObject gameOverUI;

    [Header("Âm Thanh")]
    public AudioSource backgroundMusicSource; // << BIẾN BỊ THIẾU ĐÃ ĐƯỢC THÊM LẠI
    public AudioClip gameOverSound;
    private AudioSource audioSource; // Dùng để phát âm thanh của riêng GameManager

    [Header("Chuyển Cảnh")]
    public string menuSceneName = "MainMenu";
    public float delayBeforeLoadScene = 8.0f;

    void Awake()
    {
        Instance = this;
        // Lấy AudioSource trên chính GameObject này
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        SpawnPlayer();
    }
    
    void SpawnPlayer()
    {
        GameObject playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        currentPlayer = playerInstance.GetComponent<PlayerController>();

        // Khởi tạo số mạng cho Player khi game bắt đầu
        if (currentPlayer != null)
        {
            // Thay vì currentPlayer.lives = currentPlayer.startingLives;
            // chúng ta sẽ gọi một hàm khởi tạo để code gọn gàng hơn
            currentPlayer.InitializePlayer();
        }
    }
    
    public void HandlePlayerDeath()
    {
        if (currentPlayer == null) return;

        // Kiểm tra xem Player còn mạng không (sau khi đã bị trừ)
        if (currentPlayer.lives > 0)
        {
            StartCoroutine(RespawnPlayerCoroutine());
        }
        else
        {
            GameOver();
        }
    }

    private IEnumerator RespawnPlayerCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (currentPlayer != null)
        {
            // Di chuyển Player về vị trí hồi sinh trước khi kích hoạt lại
            currentPlayer.transform.position = playerSpawnPoint.position;
            currentPlayer.transform.rotation = playerSpawnPoint.rotation;
            
            // Gọi hàm Respawn để kích hoạt và bắt đầu trạng thái bất tử
            currentPlayer.Respawn();
        }
    }
    
    void GameOver()
    {
        Debug.Log("GAME OVER!");

        // 1. Dừng nhạc nền
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop();
        }

        // 2. Hiện UI Game Over
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        
        // 3. Phát âm thanh Game Over
        if (gameOverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }
        
        // 4. Bắt đầu đếm ngược để quay về Menu
        StartCoroutine(LoadMenuAfterDelay());
    }
    
    private IEnumerator LoadMenuAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoadScene);

        // Reset lại Time.timeScale về bình thường trước khi chuyển cảnh
        Time.timeScale = 1f; 

        // Tải lại Scene Menu
        SceneManager.LoadScene(menuSceneName);
    }
}