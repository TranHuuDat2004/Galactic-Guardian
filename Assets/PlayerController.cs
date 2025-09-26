using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class BulletMapping
    {
        public WeaponType weaponType;
        public string bulletTag;
    }

    [Header("Thiết Lập Vũ Khí")]
    public List<BulletMapping> bulletMappings;
    public Transform firePoint;
    public float fireRate = 0.2f;
    [Range(1, 10)] public int weaponLevel = 1;
    private int maxWeaponLevel = 10;
    private WeaponType currentWeaponType = WeaponType.Default;

    [Header("Thiết Lập Sinh Tồn")]
    public int startingLives = 3;
    public int lives;

    [Header("Âm thanh")]
    public AudioClip shootSound;
    private AudioSource audioSource;
    
    // ... các biến nội bộ khác ...
    private Camera mainCamera;
    private Vector2 minBounds, maxBounds;
    private bool isDestroyed = false;
    private float fireCooldown = 0f;

    void Awake()
    {
        if (firePoint == null) firePoint = transform.Find("FirePoint");
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        mainCamera = Camera.main;
        InitBounds();
    }
    
    void OnEnable()
    {
        ResetState();
    }
    
    void ResetState()
    {
        isDestroyed = false;
        lives = startingLives;
        fireCooldown = 0f;
        currentWeaponType = WeaponType.Default;
        weaponLevel = 1;
    }

    void Update()
    {
        if (isDestroyed) return;
        MoveAndRotate();
        if (Input.GetMouseButton(0) && fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = fireRate;
        }
        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
        }
    }
    
    // --- HÀM UPGRADEORCHANGEWEAPON() ĐÃ ĐƯỢC THAY ĐỔI HOÀN TOÀN ---
    private void UpgradeOrChangeWeapon(WeaponType newWeaponType)
    {
        // TRƯỜNG HỢP 1: Nhặt được hộp quà "Chỉ Nâng Cấp" (tia sét)
        if (newWeaponType == WeaponType.UpgradeOnly)
        {
            // Chỉ cần tăng cấp độ vũ khí hiện tại
            if (weaponLevel < maxWeaponLevel)
            {
                weaponLevel++;
                Debug.Log("Nâng cấp vũ khí! Cấp độ hiện tại: " + weaponLevel);
            }
        }
        // TRƯỜNG HỢP 2: Nhặt được hộp quà cùng loại với vũ khí đang dùng
        else if (newWeaponType == currentWeaponType)
        {
            // Cũng tăng cấp độ vũ khí
            if (weaponLevel < maxWeaponLevel)
            {
                weaponLevel++;
                Debug.Log("Nâng cấp vũ khí! Cấp độ hiện tại: " + weaponLevel);
            }
        }
        // TRƯỜNG HỢP 3: Nhặt được hộp quà khác loại
        else
        {
            // Đổi sang loại vũ khí mới VÀ GIỮ NGUYÊN CẤP ĐỘ
            currentWeaponType = newWeaponType;
            Debug.Log("Đổi vũ khí thành: " + currentWeaponType + " ở cấp độ " + weaponLevel);
        }
    }
    
    // Tất cả các hàm khác không cần thay đổi
    // Shoot(), GetBulletTagForCurrentWeapon(), OnTriggerEnter2D(), TakeDamage(), ...
    #region Unchanged Methods 
    void Shoot()
    {
        if (firePoint == null) return;

        string bulletTagToSpawn = GetBulletTagForCurrentWeapon();
        if (string.IsNullOrEmpty(bulletTagToSpawn)) return;

        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        switch (weaponLevel)
        {
            case 1:
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation);
                break;
            case 2:
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position + transform.right * 0.3f, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position - transform.right * 0.3f, firePoint.rotation);
                break;
            case 3:
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 15));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -15));
                break;
            case 4:
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 15));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -15));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 30));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -30));
                break;
            case 5:
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position + transform.right * 0.2f, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position - transform.right * 0.2f, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 25));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -25));
                break;
            case 6:
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position + transform.right * 0.2f, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position - transform.right * 0.2f, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 20));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -20));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 40));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -40));
                break;
            case 7:
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 15));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -15));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 30));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -30));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 90));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -90));
                break;
            case 8:
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position + transform.right * 0.2f, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position - transform.right * 0.2f, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 20));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -20));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 40));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -40));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 90));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -90));
                break;
            case 9:
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 10));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -10));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 20));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -20));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 30));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -30));
                break;
            case 10:
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 20));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -20));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 45));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -45));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 90));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -90));
                ObjectPooler.Instance.SpawnFromPool(bulletTagToSpawn, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 180));
                break;
            default:
                goto case 10;
        }
    }
    
    void MoveAndRotate()
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPosition = mouseWorldPosition;
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        targetPosition.z = 0;
        transform.position = targetPosition;
        Vector3 direction = targetPosition - transform.position;
        if (direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    private string GetBulletTagForCurrentWeapon()
    {
        foreach (var mapping in bulletMappings)
        {
            if (mapping.weaponType == currentWeaponType)
            {
                return mapping.bulletTag;
            }
        }
        Debug.LogError("Không tìm thấy tag đạn cho loại vũ khí: " + currentWeaponType);
        return null;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroyed || other == null) return;

        string tag = other.tag;
        if (tag == "Blue" || tag == "Black" || tag == "Orange" || tag == "Green" || tag == "Meteor")
        {
            TakeDamage(1);
            other.gameObject.SetActive(false);
        }
        else if (tag == "PowerUp")
        {
            if (other.TryGetComponent<PowerUp>(out PowerUp powerUp))
            {
                UpgradeOrChangeWeapon(powerUp.weaponTypeToGive);
            }
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDestroyed) return;
        lives -= damageAmount;
        if (lives <= 0)
        {
            isDestroyed = true;
            gameObject.SetActive(false);
        }
    }

    void InitBounds()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;
        Vector2 spriteSize = sr.bounds.size;
        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
        minBounds.x += spriteSize.x / 2;
        maxBounds.x -= spriteSize.x / 2;
        minBounds.y += spriteSize.y / 2;
        maxBounds.y -= spriteSize.y / 2;
    }
    #endregion
}