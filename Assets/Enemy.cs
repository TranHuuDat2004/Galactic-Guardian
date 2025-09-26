using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Chỉ Số Cơ Bản")]
    public float speed = 3.0f;
    public int health = 1;
    public bool canMoveIndependently = true; // Cho phép tắt/bật di chuyển tự động

    [Header("Thiết Lập Rớt Đồ (Loot)")]
    public GameObject[] powerUpPrefabs; // Mảng chứa các loại quà có thể rơi ra
    [Range(0, 100)] public float dropChance = 15f;

    private bool isDead = false;
    private int initialHealth;
    
    void Awake()
    {
        initialHealth = health;
    }

    void OnEnable()
    {
        health = initialHealth;
        isDead = false;
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    void Update()
    {
        // Chỉ di chuyển khi còn sống và được phép
        if (!isDead && canMoveIndependently)
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
    }
    
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
        
        // Báo cho WaveManager biết một kẻ địch đã bị tiêu diệt
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
            // Chọn ngẫu nhiên một loại quà từ trong danh sách để rơi ra
            int randomIndex = Random.Range(0, powerUpPrefabs.Length);
            GameObject randomPowerUpPrefab = powerUpPrefabs[randomIndex];
            
            if (randomPowerUpPrefab != null)
            {
                Instantiate(randomPowerUpPrefab, transform.position, Quaternion.identity);
            }
        }
    }
    
    // Tự động "trả" về kho nếu bay ra khỏi màn hình
    void OnBecameInvisible()
    {
        // Chỉ tính là "bị tiêu diệt" nếu nó còn sống khi bay ra ngoài
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