using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Lớp EnemyTier không cần thay đổi
[System.Serializable]
public class EnemyTier
{
    public string tierName;
    public string[] enemyTags;
    public float minSpawnInterval;
    public float maxSpawnInterval;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Cài Đặt Các Cấp Độ")]
    [SerializeField] private EnemyTier[] enemyTiers; 
    [SerializeField] private float timePerTier = 15f; 

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI levelText; 

    // Các biến private
    private Camera mainCamera;
    private Vector2 minBounds;
    private Vector2 maxBounds;
    private int currentTierIndex = 0;

    void Start()
    {
        mainCamera = Camera.main;
        InitBounds();
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            EnemyTier currentTier = enemyTiers[currentTierIndex];
            
            if (levelText != null)
            {
                if (currentTierIndex == enemyTiers.Length - 1)
                    levelText.text = "Endless Mode";
                else
                    levelText.text = "Level " + (currentTierIndex + 1);
            }

            Coroutine spawnCoroutine = StartCoroutine(SpawnEnemiesInTier(currentTier));
            yield return new WaitForSeconds(timePerTier);
            StopCoroutine(spawnCoroutine);

            if (currentTierIndex < enemyTiers.Length - 1)
            {
                currentTierIndex++;
            }
        }
    }

    private IEnumerator SpawnEnemiesInTier(EnemyTier tier)
    {
        while (true)
        {
            if (tier.enemyTags.Length == 0)
            {
                yield return null;
                continue;
            }

            string randomTag = tier.enemyTags[Random.Range(0, tier.enemyTags.Length)];
            float randomX = Random.Range(minBounds.x, maxBounds.x);
            Vector3 spawnPosition = new Vector3(randomX, maxBounds.y, 0);

            // --- THAY ĐỔI DUY NHẤT NẰM Ở ĐÂY ---
            // Chỉ cần spawn ra là đủ. Kẻ địch sẽ tự di chuyển.
            ObjectPooler.Instance.SpawnFromPool(randomTag, spawnPosition, Quaternion.identity);
            // Chúng ta không cần lấy component Enemy và gọi SetMovement nữa.
            // ------------------------------------
            
            float randomInterval = Random.Range(tier.minSpawnInterval, tier.maxSpawnInterval);
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