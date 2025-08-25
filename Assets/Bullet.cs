using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.linearVelocity = transform.up * speed;
        // Dòng Destroy hẹn giờ không còn cần thiết nữa.
        // Destroy(gameObject, 2f); 
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng va chạm có còn tồn tại không
        if (other == null) return;

        // Nếu va chạm với kẻ địch, tự hủy ngay
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

    // Hàm này sẽ tự động được gọi khi viên đạn bay ra khỏi màn hình
    void OnBecameInvisible()
    {
        // Hủy viên đạn
        Destroy(gameObject);
    }
}