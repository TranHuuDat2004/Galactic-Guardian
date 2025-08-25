using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3.0f; // Tốc độ di chuyển của gà

    // Update is called once per frame
    void Update()
    {
        // Di chuyển con gà xuống dưới
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    // Hàm này sẽ được gọi khi có một đối tượng khác va chạm vào nó
    // (Vì chúng ta đã đặt Collider là "Is Trigger")
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng va chạm có phải là viên đạn không
        if (other.CompareTag("Bullet"))
        {
            // Nếu đúng, hủy cả hai đối tượng: gà và đạn
            Destroy(other.gameObject); // Hủy viên đạn
            Destroy(gameObject);       // Hủy chính con gà này
        }
    }
}