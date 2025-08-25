using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3.0f;
    public int health = 1;

    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            health--;

            if (health <= 0)
            {
                Destroy(gameObject);
                // SAU KHI HỦY, THOÁT NGAY LẬP TỨC KHỎI HÀM NÀY
                return;
            }
        }
    }
}