using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f; // Tốc độ bay của đạn
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        // Lấy component Rigidbody2D từ chính đối tượng đạn
        rb = GetComponent<Rigidbody2D>();
        // Làm cho viên đạn bay thẳng lên trên ngay khi được tạo ra
        rb.linearVelocity = transform.up * speed;
    }
}