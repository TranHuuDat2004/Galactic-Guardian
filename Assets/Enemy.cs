using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Chỉ Số Cơ Bản")]
    public float speed = 3.0f;
    public int health = 1;

    [Header("Thiết Lập Rớt Đồ (Loot)")]
    public GameObject powerUpPrefab; // Kéo Prefab Power-up vào đây trong Inspector
    [Range(0, 100)] public float dropChance = 15f; // Tỉ lệ % rớt đồ (ví dụ: 15%)

    private bool isDead = false;
    private int initialHealth; // Biến để lưu trữ máu ban đầu

    void Awake()
    {
        // Lưu lại lượng máu ban đầu khi game mới bắt đầu
        initialHealth = health;
    }

    void OnEnable()
    {
        // Hàm này được gọi mỗi khi đối tượng được kích hoạt (lấy ra từ pool)
        // Reset lại tất cả các trạng thái về ban đầu
        health = initialHealth;
        isDead = false;

        // Bật lại collider để có thể va chạm, phòng trường hợp nó đã bị tắt
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    void Update()
    {
        // Chỉ di chuyển khi còn sống
        if (!isDead)
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Nếu đã chết hoặc vật va chạm không tồn tại, bỏ qua
        if (isDead || other == null) return;

        // Xử lý va chạm với đạn
        if (other.CompareTag("Bullet"))
        {
            health--;
            if (health <= 0)
            {
                Die();
            }
        }

        // Xử lý va chạm với người chơi
        if (other.CompareTag("Player"))
        {
            Die();
        }
    }

    // Hàm xử lý tất cả logic khi kẻ địch "chết"
    private void Die()
    {
        isDead = true;

        // Thử rớt vật phẩm
        TryDropLoot();

        // (Tùy chọn) Tạo hiệu ứng nổ tại đây nếu bạn có
        // ObjectPooler.Instance.SpawnFromPool("Explosion", transform.position, Quaternion.identity);

        // Tắt collider đi ngay lập tức để không gây ra thêm va chạm nào nữa
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // --- THÊM DÒNG NÀY VÀO ĐÂY ---
        // Báo cho WaveManager biết rằng một kẻ địch đã bị tiêu diệt
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnEnemyDestroyed();
        }
        // ----------------------------

        // "Trả" kẻ địch về kho chứa (pool)
        gameObject.SetActive(false);
    }

    // Hàm kiểm tra tỉ lệ và tạo ra vật phẩm
    private void TryDropLoot()
    {
        // Nếu không có prefab vật phẩm được gán, bỏ qua
        if (powerUpPrefab == null) return;

        // Tung một con số ngẫu nhiên từ 0 đến 100
        float randomChance = Random.Range(0f, 100f);

        // Nếu con số ngẫu nhiên nhỏ hơn hoặc bằng tỉ lệ rớt đồ
        if (randomChance <= dropChance)
        {
            // Tạo ra vật phẩm tại vị trí của kẻ địch
            Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        }
    }

    // Tương tự, nếu bạn muốn kẻ địch bay ra khỏi màn hình cũng tính là hết wave
    void OnBecameInvisible()
    {
        // --- BẠN CŨNG CÓ THỂ THÊM VÀO ĐÂY (TÙY CHỌN) ---
        if (gameObject.activeInHierarchy && !isDead) // Chỉ gọi nếu nó còn sống và đang active
        {
            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.OnEnemyDestroyed();
            }
        }
        // ----------------------------------------------------
        gameObject.SetActive(false);
    }
}