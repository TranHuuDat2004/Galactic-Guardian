using UnityEngine;

// Đổi tên lớp thành "Meteor"
public class Meteor : MonoBehaviour
{
    [Header("Chỉ Số Cơ Bản")]
    public float speed = 5.0f; // Bạn có thể đặt tốc độ khác cho thiên thạch
    public int health = 1;
    public float rotationSpeed = 50.0f; // Thêm tốc độ xoay

    [Header("Hiệu Ứng")]
    public string explosionTag = "MeteorExplosion"; // Tag hiệu ứng nổ riêng cho thiên thạch

    [Header("Thiết Lập Rớt Đồ (Loot)")]
    public GameObject[] powerUpPrefabs;
    [Range(0, 100)] public float dropChance = 15f;

    // Biến nội bộ
    private bool isDead = false;
    private int initialHealth;
    private Vector2 moveDirection; // Biến để lưu hướng di chuyển

    void Awake()
    {
        initialHealth = health;
    }

    void OnEnable()
    {
        health = initialHealth;
        isDead = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
        
        // Chọn ngẫu nhiên hướng di chuyển chéo khi được kích hoạt
        float randomX = Random.Range(0, 2) == 0 ? -1f : 1f;
        moveDirection = new Vector2(randomX, -1f).normalized;
    }

    void Update()
    {
        if (isDead) return;

        // --- ĐÂY LÀ PHẦN THAY ĐỔI DUY NHẤT TRONG LOGIC ---
        // Di chuyển theo đường chéo
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        // Tự xoay
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
    
    // CÁC HÀM BÊN DƯỚI ĐƯỢC GIỮ NGUYÊN HOÀN TOÀN TỪ ENEMY.CS
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || other == null) return;

        if (other.CompareTag("Bullet"))
        {
            health--;
            if (health <= 0)
            {
                Die();
            }
        }
        else if (other.CompareTag("Player"))
        {
            Die();
        }
    }
    
    private void Die()
    {
        isDead = true;

        TryDropLoot();

        // Gọi hiệu ứng nổ bằng tag đã chỉ định
        if (!string.IsNullOrEmpty(explosionTag))
        {
            ObjectPooler.Instance.SpawnFromPool(explosionTag, transform.position, Quaternion.identity);
        }

        // Báo cáo về cho WaveManager
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnEnemyDestroyed();
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        gameObject.SetActive(false);
    }

    private void TryDropLoot()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;

        float randomChance = Random.Range(0f, 100f);

        if (randomChance <= dropChance)
        {
            int randomIndex = Random.Range(0, powerUpPrefabs.Length);
            GameObject randomPowerUpPrefab = powerUpPrefabs[randomIndex];

            if (randomPowerUpPrefab != null)
            {
                Instantiate(randomPowerUpPrefab, transform.position, Quaternion.identity);
            }
        }
    }
    
    void OnBecameInvisible()
    {
        if (gameObject.activeInHierarchy && !isDead)
        {
            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.OnEnemyDestroyed();
            }
        }
        
        gameObject.SetActive(false);
    }
}