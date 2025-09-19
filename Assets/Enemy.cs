using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Chỉ Số Cơ Bản")]
    public int health = 1;

    [Header("Thiết Lập Rớt Đồ (Loot)")]
    public GameObject powerUpPrefab;
    [Range(0, 100)] public float dropChance = 15f;

    // Các biến private để điều khiển trạng thái và di chuyển
    private float speed;
    private Vector2 moveDirection;
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
        GetComponent<Collider2D>().enabled = true;
    }

    // =================================================================
    // === THÊM LẠI TOÀN BỘ HÀM NÀY VÀO SCRIPT ENEMY.CS CỦA BẠN ===
    // =================================================================
    public void SetMovement(float newSpeed, MovementPattern pattern)
    {
        this.speed = newSpeed;

        // Mặc định xoay đối tượng về hướng ban đầu
        transform.rotation = Quaternion.identity;

        switch (pattern)
        {
            case MovementPattern.StraightDown:
                moveDirection = Vector2.down;
                break;
            case MovementPattern.FromLeftCorner:
                // Hướng từ trên-trái xuống dưới-phải
                moveDirection = new Vector2(1, -1).normalized;
                break;
            case MovementPattern.FromRightCorner:
                // Hướng từ trên-phải xuống dưới-trái
                moveDirection = new Vector2(-1, -1).normalized;
                break;
            case MovementPattern.MeteorFall:
                // Rơi nghiêng 45 độ từ phải sang trái
                moveDirection = new Vector2(-1, -1).normalized;
                transform.rotation = Quaternion.Euler(0, 0, 45f); // Xoay thiên thạch cho đúng hướng rơi
                break;
        }
    }

    void Update()
    {
        if (!isDead)
        {
            // Sử dụng Space.World để đảm bảo di chuyển đúng hướng toàn cục
            // bất kể đối tượng có bị xoay hay không (quan trọng cho thiên thạch)
            transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || other == null) return;
        if (other.CompareTag("Bullet")) Die(); // Đơn giản hóa: đạn thường giết địch ngay
        if (other.CompareTag("Player")) Die();
    }


    private void Die()
    {
        isDead = true;

        // THÊM BƯỚC KIỂM TRA NÀY
        // Trước khi báo cáo, hãy kiểm tra xem WaveManager.Instance có tồn tại không
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.EnemyDestroyed(this.gameObject);
        }
        else
        {
            // (Tùy chọn) Ghi log để cảnh báo nếu không tìm thấy WaveManager
            Debug.LogWarning("WaveManager.Instance not found. Cannot report enemy destruction.");
        }

        TryDropLoot();
        gameObject.SetActive(false);
    }


    private void TryDropLoot()
    {
        if (powerUpPrefab == null) return;
        if (Random.Range(0f, 100f) <= dropChance)
        {
            Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        }
    }

    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}