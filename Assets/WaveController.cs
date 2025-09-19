using System.Collections;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [Header("Wave Configuration")]
    [SerializeField] private WaveData[] waves; // Kéo các file Wave Data vào đây

    [Header("UI Elements")]
    [SerializeField] private TMPro.TextMeshProUGUI waveText; // Kéo Text hiển thị Wave vào đây

    private int currentWaveIndex = 0;
    
    void Start()
    {
        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        // Chờ 1 chút để game ổn định
        yield return new WaitForSeconds(1f);

        // Lặp qua từng Wave
        foreach (WaveData wave in waves)
        {
            // Hiển thị thông báo Wave mới
            if (waveText != null)
            {
                waveText.text = "Wave " + (currentWaveIndex + 1);
                yield return new WaitForSeconds(2f);
                waveText.text = "";
            }

            // Lặp qua từng đội hình trong Wave
            foreach (FormationStep step in wave.formationSteps)
            {
                // Chờ delay
                yield return new WaitForSeconds(step.delayBefore);

                // Spawn đội hình
                SpawnFormation(step.formationData);
            }
            
            // TODO: Chờ cho đến khi tất cả kẻ địch bị tiêu diệt
            // Đây là một tính năng nâng cao, chúng ta sẽ thêm sau.
            // Hiện tại, wave tiếp theo sẽ bắt đầu sau một khoảng nghỉ.
            yield return new WaitForSeconds(5f); // Nghỉ 5 giây giữa các wave

            currentWaveIndex++;
        }

        // Hoàn thành tất cả các wave
        if (waveText != null) waveText.text = "YOU WIN!";
    }

    private void SpawnFormation(EnemyFormationData data)
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
                Enemy enemyScript = enemyGO.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    // Ra lệnh cho kẻ địch di chuyển theo kịch bản
                    enemyScript.SetMovement(data.moveSpeed, data.movementPattern);
                }
            }

            // Dịch chuyển đến vị trí của kẻ địch tiếp theo
            currentPosition += data.spacing;
        }
    }
}