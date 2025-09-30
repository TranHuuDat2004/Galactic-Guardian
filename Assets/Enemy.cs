using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Loại Kẻ Địch")]
    public MovementType movementType = MovementType.Vertical; // << BIẾN MỚI ĐỂ CHỌN KIỂU DI CHUYỂN

    [Header("Chỉ Số Cơ Bản")]
    public float speed = 3.0f;
    public int health = 1;
    public float rotationSpeed = 50.0f; // Dùng cho kiểu Diagonal

    [Header("Thiết Lập Bắn")]
    public bool canShoot = true;
    public string bulletTag = "EnemyBullet";
    public float fireRate = 2.0f;
    public Transform firePoint;
    private float fireCooldown = 0f;

    [Header("Hiệu Ứng")]
    public string explosionTag = "EnemyExplosion";
    public GameObject engineEffectPrefab;
    private GameObject currentEngineEffect;

    [Header("Thiết Lập Rớt Đồ")]
    public GameObject[] powerUpPrefabs;
    [Range(0, 100)] public float dropChance = 15f;

    // Biến nội bộ
    private bool isDead = false;
    private int initialHealth;
    private Vector2 moveDirection;

    void Awake()
    {
        initialHealth = health;
        if (firePoint == null)
        {
            Transform foundPoint = transform.Find("FirePoint");
            if (foundPoint != null) firePoint = foundPoint;
        }
    }

    void OnEnable()
    {
        health = initialHealth;
        isDead = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        fireCooldown = Random.Range(0.5f, fireRate);
        
        // Thiết lập hướng di chuyển ban đầu dựa trên loại
        if (movementType == MovementType.Diagonal)
        {
            float randomX = Random.Range(0, 2) == 0 ? -1f : 1f;
            moveDirection = new Vector2(randomX, -1f).normalized;
        }

        if (engineEffectPrefab != null)
        {
            currentEngineEffect = Instantiate(engineEffectPrefab, transform.position, transform.rotation, transform);
        }
    }

    void Update()
    {
        if (isDead) return;

        // --- LOGIC DI CHUYỂN MỚI ---
        switch (movementType)
        {
            case MovementType.Vertical:
                transform.Translate(Vector2.down * speed * Time.deltaTime);
                break;
            case MovementType.Diagonal:
                transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
                transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
                break;
            case MovementType.Formation:
                // Không làm gì cả, để cho FormationController điều khiển
                break;
        }
        // -------------------------

        // Logic bắn giữ nguyên
        if (canShoot)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0)
            {
                Shoot();
                fireCooldown = fireRate;
            }
        }
    }
    
    // Tất cả các hàm bên dưới (Shoot, OnTriggerEnter2D, Die, etc.) giữ nguyên y hệt
    // và chắc chắn hoạt động đúng.
    #region Unchanged Methods
    void Shoot()
    {
        if (firePoint == null || string.IsNullOrEmpty(bulletTag)) return;
        ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || other == null) return;
        if (other.CompareTag("Bullet"))
        {
            health--;
            if (health <= 0) Die();
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
        if (!string.IsNullOrEmpty(explosionTag))
        {
            ObjectPooler.Instance.SpawnFromPool(explosionTag, transform.position, Quaternion.identity);
        }
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnEnemyDestroyed();
        }
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        if (currentEngineEffect != null) Destroy(currentEngineEffect);
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
            if (randomPowerUpPrefab != null) Instantiate(randomPowerUpPrefab, transform.position, Quaternion.identity);
        }
    }
    
    public void OnBecameInvisible()
    {
        if (gameObject.activeInHierarchy && !isDead)
        {
            if (WaveManager.Instance != null) WaveManager.Instance.OnEnemyDestroyed();
        }
        if (currentEngineEffect != null) Destroy(currentEngineEffect);
        gameObject.SetActive(false);
    }
    #endregion
}