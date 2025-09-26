using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // THÊM BIẾN NÀY
    public WeaponType weaponTypeToGive; // Loại vũ khí mà hộp quà này sẽ trao cho người chơi

    public float fallSpeed = 2f;

    void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}