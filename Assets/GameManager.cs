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

    // --- BIẾN MỚI ĐỂ THEO DÕI TRẠNG THÁI DỪNG GAME ---
    private bool isPaused = false;

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


     [Header("Thiết Lập Khiên Co-op")]
    [Tooltip("Khoảng cách tối đa để 2 người chơi kích hoạt khiên.")]
    public float proximityDistance = 2.0f;
    [Tooltip("Thời gian khiên tồn tại.")]
    public float proximityShieldDuration = 5.0f;
    [Tooltip("Thời gian hồi chiêu của kỹ năng.")]
    public float proximityShieldCooldown = 30.0f;

    private float proximityCooldownTimer = 0f;
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
                // Khi bắt đầu game, đảm bảo thời gian chạy bình thường và ẩn các UI
        Time.timeScale = 1f; 
        Cursor.visible = false;
        
        if (gameOverUI != null) gameOverUI.SetActive(false);
        // Ẩn cả Pause Menu khi bắt đầu
        if (UIManager.Instance != null) UIManager.Instance.HidePauseMenu();

        // --- DÒNG QUAN TRỌNG NHẤT ---
    // Luôn luôn ẩn con trỏ chuột khi vào màn chơi
    Cursor.visible = false;
    // ---------------------------

    if (gameOverUI != null)
    {
        gameOverUI.SetActive(false);
    }

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
         if (GameModeManager.NumberOfPlayers == 2)
        {
            proximityCooldownTimer = proximityShieldCooldown; 
            // Hoặc bạn có thể đặt một thời gian ngắn hơn, ví dụ 5.0f
            proximityCooldownTimer = 5.0f;
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

    void Update()
    {
        // Kiểm tra xem người chơi có nhấn nút tạm dừng không
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            // Nếu game đang chạy, thì tạm dừng
            if (!isPaused)
            {
                TogglePause();
            }
            // Nếu game đang tạm dừng, thì tiếp tục
            else
            {
                ResumeGame();
            }
        }

        // Chỉ chạy logic này ở chế độ 2 người chơi
        if (GameModeManager.NumberOfPlayers == 2)
        {
            HandleProximityShield();
        }
    }
    
    // --- HÀM MỚI ĐỂ NÚT BẤM VÀ PHÍM CÙNG SỬ DỤNG ---
    public void TogglePause()
    {
        // isPaused là biến bool chúng ta đã tạo trước đó
        isPaused = !isPaused; // Đảo ngược trạng thái

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }
    // ---------------------------------------------
    
    public void PauseGame()
    {
        isPaused = true; // Đảm bảo trạng thái luôn đúng
        Time.timeScale = 0f;
        if (UIManager.Instance != null) UIManager.Instance.ShowPauseMenu();
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false; // Đảm bảo trạng thái luôn đúng
        Time.timeScale = 1f;
        if (UIManager.Instance != null) UIManager.Instance.HidePauseMenu();
        Cursor.visible = false;
    }

    public void RestartLevel()
    {
        // Reset lại Time.timeScale trước khi tải lại Scene
        Time.timeScale = 1f;
        // Tải lại Scene game hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
    // ---------------------------------------------
    
    void HandleProximityShield()
    {
        // Đếm ngược thời gian hồi chiêu
        if (proximityCooldownTimer > 0)
        {
            proximityCooldownTimer -= Time.deltaTime;

            // --- CẬP NHẬT UI COOLDOWN Ở ĐÂY ---
            // Tính toán tỉ lệ fill amount (thời gian còn lại / tổng thời gian)
            float fillAmount = proximityCooldownTimer / proximityShieldCooldown;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateCoopSkillCooldown(fillAmount);
            }
            // ------------------------------------


            return; // Đang trong thời gian hồi, không làm gì cả
        }

// Nếu đã hồi chiêu xong, đảm bảo fill amount là 0
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCoopSkillCooldown(0);
        }
        
        // Kiểm tra xem cả 2 người chơi có tồn tại và đang hoạt động không
        if (currentPlayer != null && currentPlayer2 != null &&
            currentPlayer.gameObject.activeInHierarchy && currentPlayer2.gameObject.activeInHierarchy)
        {
            // Tính khoảng cách giữa 2 người chơi
            float distance = Vector3.Distance(currentPlayer.transform.position, currentPlayer2.transform.position);

            // Nếu khoảng cách đủ gần
            if (distance <= proximityDistance)
            {
                Debug.Log("Kích hoạt khiên co-op!");

                // Kích hoạt khiên cho cả hai
                currentPlayer.ActivateShield(proximityShieldDuration);
                currentPlayer2.ActivateShield(proximityShieldDuration);

                // Bắt đầu đếm ngược thời gian hồi chiêu
                proximityCooldownTimer = proximityShieldCooldown;
            }
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

         // --- THÊM DÒNG NÀY VÀO ---
    // Hiện lại con trỏ chuột để người chơi có thể tương tác với UI Game Over
    Cursor.visible = true;
    // -------------------------


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

      private void OnDrawGizmos()
    {
        // Chỉ vẽ khi game đang chạy và ở chế độ 2 người
        if (!Application.isPlaying || GameModeManager.NumberOfPlayers < 2)
        {
            return;
        }

        // Đảm bảo cả 2 player đều tồn tại
        if (currentPlayer != null && currentPlayer2 != null)
        {
            // Vẽ một đường thẳng nối 2 player
            Gizmos.color = Color.white;
            Gizmos.DrawLine(currentPlayer.transform.position, currentPlayer2.transform.position);

            // Vẽ một vòng tròn xung quanh mỗi player với bán kính là proximityDistance
            // Vòng tròn này thể hiện "vùng kích hoạt"
            if (proximityCooldownTimer <= 0)
            {
                // Màu xanh lá cây khi kỹ năng sẵn sàng
                Gizmos.color = Color.green;
            }
            else
            {
                // Màu đỏ khi kỹ năng đang hồi
                Gizmos.color = Color.red;
            }
            
            Gizmos.DrawWireSphere(currentPlayer.transform.position, proximityDistance);
            Gizmos.DrawWireSphere(currentPlayer2.transform.position, proximityDistance);
        }
    }
}