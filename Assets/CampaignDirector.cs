using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Lớp EnemyTier vẫn giữ nguyên, rất hữu ích
[System.Serializable]
public class EnemyTier
{
    public string tierName;
    public string[] enemyTags;
    public float minSpawnInterval;
    public float maxSpawnInterval;
}

public class CampaignDirector : MonoBehaviour
{
    [Header("Cài Đặt Các Wave (Màn Chơi)")]
    [Tooltip("Danh sách các wave thông thường, chạy tuần tự.")]
    [SerializeField] private EnemyTier[] waves;
    
    [Tooltip("Thời gian tồn tại của mỗi wave (tính bằng giây).")]
    [SerializeField] private float timePerWave = 10f;

    [Header("Cài Đặt Trùm Cuối (Boss)")]
    [Tooltip("Kéo Prefab của Boss vào đây.")]
    [SerializeField] private GameObject bossPrefab;
    
    [Tooltip("Tạo một GameObject rỗng và kéo vào đây để làm điểm xuất hiện cho Boss.")]
    [SerializeField] private Transform bossSpawnPoint;

    [Header("Giao Diện Người Dùng (UI)")]
    [Tooltip("Kéo đối tượng Text (TextMeshPro) để hiển thị thông báo vào đây.")]
    [SerializeField] private TextMeshProUGUI notificationText;

    // Các biến private để script tự quản lý
    private Camera mainCamera;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    void Start()
    {
        mainCamera = Camera.main;
        InitBounds();
        StartCoroutine(RunCampaign());
    }

    // Coroutine chính điều khiển toàn bộ chiến dịch
    private IEnumerator RunCampaign()
    {
        // --- CHẠY CÁC WAVE THÔNG THƯỜNG ---
        for (int i = 0; i < waves.Length; i++)
        {
            if (notificationText != null)
            {
                notificationText.text = "Wave " + (i + 1);
                yield return new WaitForSeconds(2f); // Chờ 2s cho người chơi đọc
            }

            // Bắt đầu spawn kẻ địch cho wave này
            Coroutine spawnCoroutine = StartCoroutine(SpawnEnemiesInWave(waves[i]));
            
            // Chờ 10 giây
            yield return new WaitForSeconds(timePerWave);

            // Dừng việc spawn kẻ địch của wave hiện tại
            StopCoroutine(spawnCoroutine);
        }

        // --- GIAI ĐOẠN TRÙM CUỐI ---
        
        // Hiển thị cảnh báo
        if (notificationText != null)
        {
            notificationText.text = "WARNING!";
            yield return new WaitForSeconds(2f);
            notificationText.text = "";
        }

        // Tạo ra Boss
        GameObject bossInstance = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);

        // Chờ cho đến khi Boss bị tiêu diệt
        // Điều kiện: Đối tượng bossInstance không còn tồn tại trong game nữa (đã bị Destroy)
        yield return new WaitUntil(() => bossInstance == null);

        // --- KẾT THÚC GAME ---
        if (notificationText != null)
        {
            notificationText.text = "YOU WIN!";
        }
        Debug.Log("Campaign Completed! Player Wins!");
    }

    // Coroutine con, chịu trách nhiệm tạo kẻ địch cho một wave
    private IEnumerator SpawnEnemiesInWave(EnemyTier waveData)
    {
        while (true)
        {
            string randomTag = waveData.enemyTags[Random.Range(0, waveData.enemyTags.Length)];
            float randomX = Random.Range(minBounds.x, maxBounds.x);
            Vector3 spawnPosition = new Vector3(randomX, maxBounds.y, 0);

            GameObject enemyGO = ObjectPooler.Instance.SpawnFromPool(randomTag, spawnPosition, Quaternion.identity);

            if (enemyGO != null)
            {
                Enemy enemyScript = enemyGO.GetComponent<Enemy>();
                if(enemyScript != null)
                {
                    enemyScript.SetMovement(Random.Range(2f, 5f), MovementPattern.StraightDown);
                }
            }
            
            float randomInterval = Random.Range(waveData.minSpawnInterval, waveData.maxSpawnInterval);
            yield return new WaitForSeconds(randomInterval);
        }
    }

    void InitBounds()
    {
        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
        maxBounds.y += 0.5f;
    }
}