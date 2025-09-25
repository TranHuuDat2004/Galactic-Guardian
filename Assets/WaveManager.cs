// WaveManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public GameObject formationPrefab; // Kéo Prefab đội hình vào đây
        public string enemyTagToSpawn;     // Loại kẻ địch cho wave này (ví dụ: "Blue")
        public float delayBeforeStart = 2.0f; // Thời gian chờ trước khi wave bắt đầu
    }

    [Header("Cài Đặt Màn Chơi")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private TextMeshProUGUI waveText;

    private int currentWaveIndex = -1;
    private int enemiesAlive = 0;
    private GameObject currentFormationInstance; // Biến để lưu đội hình hiện tại

    // --- Singleton Pattern để dễ dàng truy cập từ các script khác ---
    public static WaveManager Instance;

    void Awake()
    {
        Instance = this;
    }
    // -----------------------------------------------------------------

    void Start()
    {
        // Khi game bắt đầu, gọi wave đầu tiên NGAY LẬP TỨC
        // KHÔNG dùng vòng lặp hay timer ở đây
        StartNextWave();
    }

    public void StartNextWave()
    {
        currentWaveIndex++;

        // Kiểm tra xem còn wave nào trong danh sách không
        if (currentWaveIndex < waves.Length)
        {
            StartCoroutine(SpawnWaveCoroutine(waves[currentWaveIndex]));
        }
        else
        {
            // Đã hoàn thành tất cả các wave!
            if (waveText != null) waveText.text = "LEVEL COMPLETE!";
            Debug.Log("Bạn đã thắng!");
            // Kích hoạt logic qua màn, tính sao, v.v.
        }
    }

    private IEnumerator SpawnWaveCoroutine(Wave wave)
    {
        if (waveText != null) waveText.text = wave.waveName;
        
        // Chờ một khoảng thời gian trước khi spawn
        yield return new WaitForSeconds(wave.delayBeforeStart);

        // Hủy đội hình cũ đi nếu nó còn tồn tại
        if (currentFormationInstance != null)
        {
            Destroy(currentFormationInstance);
        }

        // Tạo đội hình mới
        currentFormationInstance = Instantiate(wave.formationPrefab, transform.position, Quaternion.identity);

        // Reset biến đếm và bắt đầu đếm lại
        enemiesAlive = 0;
        foreach (Transform spawnPoint in currentFormationInstance.transform)
        {
            // Quan trọng: Chỉ spawn vào các vị trí con (Spawn Points)
            if (spawnPoint.CompareTag("Untagged")) // Giả sử các spawn point không có tag đặc biệt
            {
                 ObjectPooler.Instance.SpawnFromPool(wave.enemyTagToSpawn, spawnPoint.position, spawnPoint.rotation);
                 enemiesAlive++;
            }
        }
        
        Debug.Log("Bắt đầu " + wave.waveName + " với " + enemiesAlive + " kẻ địch.");
    }

    // HÀM QUAN TRỌNG NHẤT: Kẻ địch sẽ gọi hàm này khi chúng bị phá hủy
    public void OnEnemyDestroyed()
    {
        enemiesAlive--;
        Debug.Log("Một kẻ địch đã bị tiêu diệt, còn lại: " + enemiesAlive);

        // Nếu không còn kẻ địch nào sống sót
        if (enemiesAlive <= 0)
        {
            Debug.Log("Đã tiêu diệt hết địch! Chuẩn bị wave tiếp theo...");
            // Bắt đầu wave tiếp theo
            StartNextWave();
        }
    }
}