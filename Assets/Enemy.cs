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
        if (isDestroyed || other == null || other.gameObject == null) return;

        if (other.CompareTag("Bullet"))
        {
            health--;

            if (health <= 0)
            {
                isDestroyed = true;
                // Dòng cũ: Destroy(gameObject);
                // Dòng mới:
                gameObject.SetActive(false); // "Trả" về kho
            }
        }

        if (other.CompareTag("Player"))
        {
            isDestroyed = true;
            // Dòng cũ: Destroy(gameObject);
            // Dòng mới:
            gameObject.SetActive(false); // "Trả" về kho
        }
    }
}
