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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroyed || other == null || other.gameObject == null) return;

        if (other.CompareTag("Bullet"))
        {
            health--;

            if (health <= 0)
            {
                isDestroyed = true;
                Destroy(gameObject);
            }
        }

        if (other.CompareTag("Player"))
        {
            isDestroyed = true;
            Destroy(gameObject);
        }
    }
}
