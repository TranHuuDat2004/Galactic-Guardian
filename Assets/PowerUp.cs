using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // THÊM BIẾN NÀY
    public WeaponType weaponTypeToGive; // Loại vũ khí mà hộp quà này sẽ trao cho người chơi

    public enum PowerUpType
    {
        Weapon,
        Shield
    }
    
     public PowerUpType powerUpType = PowerUpType.Weapon; // Mặc định là quà vũ khí
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