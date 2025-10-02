using UnityEngine;

public class DestroyOnContact : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem vật va chạm có phải là Enemy hoặc Meteor không
        // Chúng ta dùng chung script Enemy.cs cho cả hai
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            // Gọi trực tiếp hàm OnBecameInvisible() của đối tượng đó.
            // Hàm này đã chứa logic báo cáo cho WaveManager và tắt đối tượng.
            enemy.OnBecameInvisible();
        }
        // Bạn cũng có thể thêm logic để hủy đạn của địch nếu muốn
        else if (other.CompareTag("EnemyBullet"))
        {
            other.gameObject.SetActive(false);
        }
    }
}