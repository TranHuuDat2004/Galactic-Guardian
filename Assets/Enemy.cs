using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3.0f;
    public int health = 1;
    private bool isDestroyed = false;

    void Update()
    {
        if (!isDestroyed)
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
    }

    public int maxHealth = 1; // Thêm biến này để lưu máu tối đa

    void OnEnable()
    {
        // Hàm này được gọi mỗi khi GameObject được SetActive(true)
        // Đây là nơi hoàn hảo để reset trạng thái của kẻ địch!
        health = maxHealth;
        isDestroyed = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Log 1: Kiểm tra va chạm ban đầu (Dòng này đã hoạt động)
        Debug.Log("Enemy va chạm với " + other.name + ", có tag là: '" + other.tag + "'");

        if (isDestroyed || other == null) return;

        // Bây giờ, chúng ta kiểm tra điều kiện so sánh Tag
        if (other.CompareTag("Bullet"))
        {
            // Log 2: Nếu code chạy vào đây, nghĩa là nó đã xác nhận va chạm là Đạn!
            Debug.Log("XÁC NHẬN va chạm là Bullet! Bắt đầu trừ máu.");

            health--;

            // Log 3: Kiểm tra xem máu còn lại bao nhiêu
            Debug.Log("Máu của Enemy còn lại: " + health);

            if (health <= 0)
            {
                // Log 4: Nếu code vào đây, nghĩa là Enemy đã hết máu và sẽ bị tắt
                Debug.Log("HẾT MÁU! Tắt Enemy.");

                isDestroyed = true;
                gameObject.SetActive(false);
            }
        }

        if (other.CompareTag("Player"))
        {
            isDestroyed = true;
            // Phá hủy KẺ ĐỊCH, chứ không phải người chơi
            gameObject.SetActive(false);
        }
    }
}
