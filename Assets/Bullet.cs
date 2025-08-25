using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f; // Vòng đời của viên đạn (tính bằng giây)
    private Rigidbody2D rb;

    // Awake được gọi trước cả Start, tốt cho việc lấy component
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Gán vận tốc cho viên đạn bay theo hướng nó được tạo ra
        rb.linearVelocity = transform.up * speed;

        // Hủy đối tượng đạn này sau một khoảng thời gian lifeTime
        Destroy(gameObject, lifeTime);
    }
}