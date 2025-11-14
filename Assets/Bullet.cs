using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Chỉ Số Cơ Bản")]
    public float speed = 20f;
    public int damage = 1;

    // --- THÊM MỚI TOÀN BỘ PHẦN NÀY ---
    [Header("Thiết Lập Hiệu Ứng Đặc Biệt")]
    public SpecialEffectType effectType = SpecialEffectType.None;

    [Tooltip("Thời gian hiệu ứng tồn tại (cho Đóng Băng, Đốt Cháy).")]
    public float effectDuration = 3f;

    [Tooltip("Sức mạnh hiệu ứng: Tỉ lệ làm chậm (0-1) cho Băng, Sát thương/giây cho Lửa.")]
    public float effectPotency = 0.5f;

    [Tooltip("Bán kính nổ (cho Đạn Nổ).")]
    public float explosionRadius = 2f;
    [Tooltip("Sát thương lan (cho Đạn Nổ).")]
    public int explosionDamage = 1;
    public string explosionEffectTag = "EnemyExplosion"; // Tag hiệu ứng nổ để gọi từ Pooler
    // ------------------------------------

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        if (rb != null)
        {
            rb.linearVelocity = transform.up * speed;
        }
    }

    // --- HÀM NÀY ĐÃ ĐƯỢC VIẾT LẠI HOÀN TOÀN ---
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        // Kiểm tra xem có va chạm với đối tượng có thể nhận sát thương không
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            // 1. Gây sát thương ban đầu
            enemy.TakeDamage(damage);

            // 2. Kích hoạt hiệu ứng đặc biệt
            if (effectType == SpecialEffectType.Explosion)
            {
                Detonate(); // Xử lý nổ
            }
            else if (effectType != SpecialEffectType.None)
            {
                // Gửi thông tin của viên đạn này cho Enemy để nó tự xử lý hiệu ứng
                enemy.ApplyStatusEffect(this);
            }
            
            // 3. Tắt viên đạn đi
            gameObject.SetActive(false);
        }
    }
    
    // --- HÀM MỚI CHO ĐẠN NỔ ---
    private void Detonate()
    {
        // Tạo hiệu ứng nổ tại vị trí va chạm
        if (!string.IsNullOrEmpty(explosionEffectTag))
        {
            ObjectPooler.Instance.SpawnFromPool(explosionEffectTag, transform.position, Quaternion.identity);
        }

        // Tìm tất cả kẻ địch trong bán kính nổ
        Collider2D[] enemiesInRadius = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D col in enemiesInRadius)
        {
            if (col.TryGetComponent<Enemy>(out Enemy enemy))
            {
                // Gây sát thương lan cho những kẻ địch trúng vụ nổ
                enemy.TakeDamage(explosionDamage);
            }
        }
    }
    // ----------------------------

    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}