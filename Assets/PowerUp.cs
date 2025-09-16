using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float fallSpeed = 2f; // Tốc độ rơi xuống

    void Update()
    {
        // Làm cho vật phẩm từ từ rơi xuống
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
    }

    // Tự hủy nếu bay ra khỏi màn hình
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}