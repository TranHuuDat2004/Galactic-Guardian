using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // THAY ĐỔI: Thay vì mảng GameObject, chúng ta dùng mảng string chứa các Tag
    // Bạn sẽ điền các Tag này trong Inspector (ví dụ: "EnemyBlack", "EnemyBlue"...)
    [SerializeField] private string[] enemyTags; 

    [SerializeField] private float minSpawnInterval = 0.5f;
    [SerializeField] private float maxSpawnInterval = 1.5f;

    private Camera mainCamera;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    void Start()
    {
        mainCamera = Camera.main;
        InitBounds();
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // THAY ĐỔI 1: Kiểm tra xem mảng tag có rỗng không để tránh lỗi
            if (enemyTags.Length == 0)
            {
                Debug.LogWarning("Enemy Tags array in Spawner is empty!");
                // Chờ 1 giây rồi thử lại, tránh vòng lặp vô hạn gây treo máy
                yield return new WaitForSeconds(1f); 
                continue; // Bỏ qua lần lặp này và bắt đầu lại vòng lặp
            }
            
            // 1. Chọn ngẫu nhiên một Tag từ trong mảng
            string randomTag = enemyTags[Random.Range(0, enemyTags.Length)];

            // 2. Chọn một vị trí X ngẫu nhiên
            float randomX = Random.Range(minBounds.x, maxBounds.x);
            Vector3 spawnPosition = new Vector3(randomX, maxBounds.y, 0);

            // 3. THAY ĐỔI 2: Sử dụng Tag ngẫu nhiên vừa chọn để gọi Object Pooler
            ObjectPooler.Instance.SpawnFromPool(randomTag, spawnPosition, Quaternion.identity);

            // 4. Chờ một khoảng thời gian ngẫu nhiên
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