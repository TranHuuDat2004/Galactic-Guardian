using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        // Hàm này được gọi mỗi khi viên đạn được "mượn" từ kho
        // Đây là lúc để bắn nó đi!
        if (rb != null)
        {
            rb.linearVelocity = transform.up * speed;
        }
    }

    void Start()
    {
        rb.linearVelocity = transform.up * speed;
        // Dòng Destroy hẹn giờ không còn cần thiết nữa.
        // Destroy(gameObject, 2f); 
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        if (other.CompareTag("Blue") || other.CompareTag("Black") || other.CompareTag("Orange") || other.CompareTag("Green"))
        {
            // Dòng cũ: Destroy(gameObject);
            // Dòng mới:
            gameObject.SetActive(false); // "Trả" về kho
        }
    }

    void OnBecameInvisible()
    {
        // Dòng cũ: Destroy(gameObject);
        // Dòng mới:
        gameObject.SetActive(false); // "Trả" về kho
    }
}