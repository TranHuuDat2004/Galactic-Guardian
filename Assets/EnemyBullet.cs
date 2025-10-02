using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f;

    void Update()
    {
        transform.Translate(-transform.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Khi viên đạn va vào BẤT CỨ THỨ GÌ (mà không phải là trigger khác)
        // hoặc cụ thể là Player, nó chỉ cần biến mất.
        // Việc trừ máu đã do Player tự xử lý.
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false); // Trả về pool
        }
    }

    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}