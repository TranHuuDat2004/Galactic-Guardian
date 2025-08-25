using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Mảng chứa tất cả các loại Prefab kẻ địch mà chúng ta muốn tạo ra
    [SerializeField] private GameObject[] enemyPrefabs;

    // Thời gian chờ tối thiểu và tối đa giữa mỗi lần tạo kẻ địch
    [SerializeField] private float minSpawnInterval = 0.5f;
    [SerializeField] private float maxSpawnInterval = 1.5f;

    private Camera mainCamera;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    void Start()
    {
        mainCamera = Camera.main;
        InitBounds();

        // Bắt đầu vòng lặp tạo kẻ địch
        StartCoroutine(SpawnEnemies());
    }

    // Coroutine để tạo kẻ địch liên tục
    private IEnumerator SpawnEnemies()
    {
        // Vòng lặp này sẽ chạy mãi mãi khi game còn diễn ra
        while (true)
        {
            // 1. Chọn ngẫu nhiên một loại kẻ địch từ trong mảng
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyToSpawn = enemyPrefabs[randomIndex];

            // 2. Chọn một vị trí X ngẫu nhiên ở mép trên màn hình
            float randomX = Random.Range(minBounds.x, maxBounds.x);
            Vector3 spawnPosition = new Vector3(randomX, maxBounds.y, 0);

            // 3. Tạo ra kẻ địch tại vị trí đó
            // Quaternion.identity có nghĩa là không xoay (giữ nguyên góc quay của Prefab)
            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);

            // 4. Chờ một khoảng thời gian ngẫu nhiên rồi mới lặp lại
            float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(randomInterval);
        }
    }

    // Hàm tính toán ranh giới (giống hệt trong PlayerController)
    void InitBounds()
    {
        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));

        // Thêm một chút padding để tàu địch không xuất hiện bị lẹm một nửa
        maxBounds.y += 0.5f;
    }
}