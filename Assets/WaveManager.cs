using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Thêm dòng này để sử dụng TextMeshPro

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance; // Singleton để các script khác có thể tham chiếu

    [Header("Cài Đặt Màn Chơi")]
    public WaveData[] waves; // Kéo các file Wave_1, Wave_2, Wave_3 vào đây
    public TextMeshProUGUI waveText; // Kéo Text hiển thị tên Wave vào đây

    private int currentWaveIndex = -1;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartNextWave();
    }

    public void StartNextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("YOU WIN!"); // Hết màn chơi
            waveText.text = "YOU WIN!";
            return;
        }

        StartCoroutine(SpawnCurrentWave());
    }

    private IEnumerator SpawnCurrentWave()
    {
        WaveData currentWave = waves[currentWaveIndex];
        if (waveText != null)
        {
            waveText.text = "Wave " + (currentWaveIndex + 1);
            yield return new WaitForSeconds(2f);
            waveText.text = "";
        }

        // Lặp qua tất cả các đội hình trong wave
        foreach (FormationStep step in currentWave.formationSteps)
        {
            // Chờ delay trước khi spawn
            yield return new WaitForSeconds(step.delayBefore);

            // Xóa danh sách địch cũ trước khi spawn đội hình mới
            activeEnemies.Clear();

            // Spawn đội hình
            // Chúng ta không cần phân biệt Meteor nữa vì logic di chuyển đã nằm trong Enemy.cs
            SpawnFormation(step.formationData);

            // Chờ cho đến khi tất cả kẻ địch trong đội hình này bị tiêu diệt
            yield return new WaitUntil(() => activeEnemies.Count == 0);
        }

        // Sau khi hoàn thành tất cả đội hình trong wave, bắt đầu wave tiếp theo
        StartNextWave();
    }

    private void SpawnFormation(EnemyFormationData data) // Đổi tên biến cho rõ ràng
    {
        if (data == null)
        {
            Debug.LogError("Formation Data is missing!");
            return;
        }

        Vector2 currentPosition = data.startPosition;
        for (int i = 0; i < data.numberOfEnemies; i++)
        {
            // Lấy Tag từ tên của Prefab
            string enemyTag = data.enemyPrefab.name;
            GameObject enemyGO = ObjectPooler.Instance.SpawnFromPool(enemyTag, currentPosition, Quaternion.identity);

            if (enemyGO != null)
            {
                activeEnemies.Add(enemyGO); // Thêm vào danh sách theo dõi

                // === THÊM LẠI PHẦN QUAN TRỌNG NÀY ===
                Enemy enemyScript = enemyGO.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    // Ra lệnh cho kẻ địch di chuyển theo kịch bản
                    enemyScript.SetMovement(data.moveSpeed, data.movementPattern);
                }
                // ===================================
            }

            currentPosition += data.spacing;
        }
    }


    private void SpawnMeteorFormation(EnemyFormationData formation)
    {
        // ... (Code spawn thiên thạch ngẫu nhiên sẽ được thêm vào sau) ...
        // Tạm thời chúng ta cũng spawn theo đội hình cho đơn giản
        SpawnFormation(formation);
    }

    // Kẻ địch sẽ gọi hàm này trước khi bị hủy
    public void EnemyDestroyed(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }
}