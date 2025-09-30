using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f;

    // Không cần hàm OnEnable() nữa

    void Update()
    {
        // Di chuyển viên đạn xuống dưới mỗi frame
        // transform.up của kẻ địch thường là hướng "tiến lên", nên -transform.up là "lùi xuống"
        transform.Translate(-transform.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.TakeDamage(1);
            }
            gameObject.SetActive(false);
        }
    }

    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}