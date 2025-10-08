using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static LevelData levelToLoad;
    public static GameManager Instance;

    [Header("Thiết Lập Player")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    public float respawnDelay = 1.0f;
    private PlayerController currentPlayer;
    
    // --- VỊ TRÍ ĐỂ THÊM THÔNG TIN CHO PLAYER 2 ---
    // Ví dụ, bạn có thể thêm các dòng sau khi đã có prefab cho Player 2
    // public GameObject player2Prefab;
    // public Transform player2SpawnPoint;
    // private PlayerController currentPlayer2;
    // ---------------------------------------------
     [Header("Thiết Lập Player 2 (Co-op)")]
    public GameObject player2Prefab; // Prefab cho Player 2, có thể dùng chung với Player 1
    public Transform player2SpawnPoint; // Vị trí xuất hiện của Player 2
    private PlayerController currentPlayer2;


    [Header("Thiết Lập UI")]
    public GameObject gameOverUI;

    [Header("Âm Thanh")]
    public AudioSource backgroundMusicSource;
    public AudioClip gameOverSound;
    private AudioSource audioSource;

    [Header("Chuyển Cảnh")]
    public string menuSceneName = "MainMenu";
    public float delayBeforeLoadScene = 8.0f;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

     void Start()
    {
        // --- CẬP NHẬT LOGIC TẠO PLAYER Ở ĐÂY ---
        // Sử dụng thông tin từ GameModeManager
        SpawnPlayers();
        // ---------------------------------------
        
        if (levelToLoad != null && WaveManager.Instance != null)
        {
            WaveManager.Instance.StartLevel(levelToLoad);
        }
        else
        {
            Debug.LogError("LỖI: Không có LevelData để tải!");
        }
    }

    // --- THAY THẾ HÀM SpawnPlayer() BẰNG HÀM MỚI NÀY ---
    void SpawnPlayers()
    {
        // Tạo Player 1
        GameObject playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        currentPlayer = playerInstance.GetComponent<PlayerController>();
        if (currentPlayer != null)
        {
            currentPlayer.InitializePlayer();
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateLives(currentPlayer.lives);
            }
        }

        // Tạo Player 2 nếu cần
         if (GameModeManager.NumberOfPlayers == 2)
    {
        if (player2Prefab != null && player2SpawnPoint != null)
        {
            GameObject player2Instance = Instantiate(player2Prefab, player2SpawnPoint.position, player2SpawnPoint.rotation);
            currentPlayer2 = player2Instance.GetComponent<PlayerController>();
            if (currentPlayer2 != null)
            {
                currentPlayer2.InitializePlayer();

                // --- THÊM DÒNG NÀY ĐỂ KHỞI TẠO UI CHO PLAYER 2 ---
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.UpdateLives_P2(currentPlayer2.lives);
                }
                // ----------------------------------------------------
            }
        }
        // ...
    }
    }


     public void HandlePlayerDeath(PlayerController playerWhoDied)
    {
        // Kiểm tra xem người chơi đó có còn mạng để hồi sinh không
        if (playerWhoDied.lives > 0)
        {
            // Gọi coroutine hồi sinh và truyền vào người chơi cần hồi sinh
            StartCoroutine(RespawnPlayerCoroutine(playerWhoDied));
        }
        else
        {
            // Người chơi này đã hết mạng. Kiểm tra xem game đã kết thúc chưa.
            CheckForGameOver();
        }
    }

     private IEnumerator RespawnPlayerCoroutine(PlayerController playerToRespawn)
    {
        yield return new WaitForSeconds(respawnDelay);

        // Xác định đúng vị trí hồi sinh cho từng người chơi
        Transform spawnPoint;
        if (playerToRespawn == currentPlayer)
        {
            spawnPoint = playerSpawnPoint;
        }
        else
        {
            spawnPoint = player2SpawnPoint;
        }

        // Hồi sinh người chơi
        if (playerToRespawn != null)
        {
            playerToRespawn.transform.position = spawnPoint.position;
            playerToRespawn.transform.rotation = spawnPoint.rotation;
            playerToRespawn.Respawn(); // Gọi hàm Respawn của chính người chơi đó
        }
    }
    
    void CheckForGameOver()
    {
        // Trường hợp 1: Chơi 1 người và người đó hết mạng
        if (GameModeManager.NumberOfPlayers == 1 && currentPlayer.lives <= 0)
        {
            GameOver();
            return;
        }

        // Trường hợp 2: Chơi 2 người và CẢ HAI đều hết mạng
        if (GameModeManager.NumberOfPlayers == 2 && currentPlayer.lives <= 0 && currentPlayer2.lives <= 0)
        {
            GameOver();
            return;
        }
        
        Debug.Log("Một người chơi đã hết mạng, nhưng game vẫn tiếp tục.");
    }

    void GameOver()
    {
        Debug.Log("GAME OVER!");
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop();
        }
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        if (gameOverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }
        StartCoroutine(LoadMenuAfterDelay());
    }

    private IEnumerator LoadMenuAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoadScene);
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}