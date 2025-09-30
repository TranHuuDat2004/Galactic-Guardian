using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Chỉ Số Cơ Bản")]
    public float speed = 3.0f;
    public int health = 1;
    public bool canMoveIndependently = true; // Cho phép tắt/bật di chuyển tự động


    // --- THÊM MỚI Ở ĐÂY ---
    [Header("Thiết Lập Bắn")]
    public bool canShoot = true; // Cho phép kẻ địch này bắn không?
    public string bulletTag = "EnemyBullet"; // Tag của đạn trong ObjectPooler
    public float fireRate = 2.0f; // Bắn một lần mỗi 2 giây
    public Transform firePoint; // Vị trí đạn được bắn ra
    private float fireCooldown = 0f;
    // ----------------------


    // --- THÊM MỚI Ở ĐÂY ---
    [Header("Hiệu Ứng")]
    public string explosionTag = "EnemyExplosion"; // Tag của hiệu ứng nổ trong ObjectPooler

    // --- THÊM MỚI Ở ĐÂY ---
    public GameObject engineEffectPrefab; // Prefab hiệu ứng động cơ
    private GameObject currentEngineEffect; // Biến để lưu trữ hiệu ứng đã tạo


    // ----------------------


    [Header("Thiết Lập Rớt Đồ (Loot)")]
    public GameObject[] powerUpPrefabs; // Mảng chứa các loại quà có thể rơi ra
    [Range(0, 100)] public float dropChance = 15f;

    private bool isDead = false;
    private int initialHealth;

    void Awake()
    {
        initialHealth = health;
        // Tự tìm firePoint nếu chưa gán
        if (firePoint == null)
        {
            // Giả sử có một đối tượng con tên là "FirePoint"
            Transform foundPoint = transform.Find("FirePoint");
            if (foundPoint != null)
            {
                firePoint = foundPoint;
            }
        }
    }
    void OnEnable()
    {
        health = initialHealth;
        isDead = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        // --- THÊM MỚI Ở ĐÂY ---
        // Reset thời gian hồi chiêu bắn, có thể thêm một chút ngẫu nhiên
        // để các kẻ địch không bắn cùng một lúc
        fireCooldown = Random.Range(0.5f, fireRate);
        // ----------------------


        // --- THÊM MỚI Ở ĐÂY ---
        // Khi kẻ địch được kích hoạt, tạo hiệu ứng động cơ nếu có
        if (engineEffectPrefab != null)
        {
            // Tạo hiệu ứng và đặt nó làm con của kẻ địch để nó di chuyển theo
            currentEngineEffect = Instantiate(engineEffectPrefab, transform.position, transform.rotation, transform);
        }
        // ----------------------
    }

    void Update()
    {
        // Chỉ di chuyển khi còn sống và được phép
        if (!isDead && canMoveIndependently)
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }

         // --- THÊM MỚI Ở ĐÂY: LOGIC BẮN ---
        // Nếu kẻ địch được phép bắn
        if (canShoot)
        {
            // Giảm thời gian hồi chiêu
            fireCooldown -= Time.deltaTime;

            // Nếu đã đến lúc bắn
            if (fireCooldown <= 0)
            {
                Shoot();
                // Reset lại thời gian hồi chiêu
                fireCooldown = fireRate;
            }
        }
        // ---------------------------------
    }

 // --- HÀM MỚI ĐỂ BẮN ---
    void Shoot()
    {
        // Kiểm tra xem có firePoint và tag đạn hợp lệ không
        if (firePoint == null || string.IsNullOrEmpty(bulletTag)) return;

        // Lấy một viên đạn từ pool và bắn ra
        ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation);
    }
    // -------------------------
    
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

    // --- CẬP NHẬT HÀM Die() ---
    private void Die()
    {
        isDead = true;

        TryDropLoot();

        // Gọi hiệu ứng nổ bằng tag đã chỉ định
        if (!string.IsNullOrEmpty(explosionTag))
        {
            ObjectPooler.Instance.SpawnFromPool(explosionTag, transform.position, Quaternion.identity);
        }

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnEnemyDestroyed();
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // --- THÊM MỚI Ở ĐÂY ---
        // Khi kẻ địch chết, hủy hiệu ứng động cơ đi
        if (currentEngineEffect != null)
        {
            Destroy(currentEngineEffect);
        }
        // ----------------------

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

        // --- THÊM MỚI Ở ĐÂY ---
        // Khi kẻ địch bay ra ngoài, cũng hủy hiệu ứng động cơ
        if (currentEngineEffect != null)
        {
            Destroy(currentEngineEffect);
        }
        // ----------------------
        gameObject.SetActive(false);
    }
}