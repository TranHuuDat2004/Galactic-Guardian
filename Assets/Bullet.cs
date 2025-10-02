using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    // --- THÊM BIẾN NÀY ---
    public int damage = 1; // Lượng sát thương mà viên đạn này gây ra
    // ----------------------
    
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

    // Hàm OnTriggerEnter2D không cần thay đổi, vì logic gây sát thương
    // sẽ được xử lý bên phía Enemy.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        string tag = other.tag;
        if (tag == "Blue" || tag == "Black" || tag == "Orange" || tag == "Green" || tag == "Boss"  || tag == "Meteor")
        {
            gameObject.SetActive(false);
        }
    }

    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}