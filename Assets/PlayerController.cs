using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Lớp nội bộ để ánh xạ vũ khí
    [System.Serializable]
    public class BulletMapping
    {
        public WeaponType weaponType;
        public string bulletTag;
    }

     [Header("Thiết Lập Điều Khiển")]
    [Tooltip("Đặt là 1 cho Player 1 (Chuột), 2 cho Player 2 (Bàn phím)")]
    public int playerID = 1; // Mặc định là Player 1
    public float keyboardMoveSpeed = 8f; // Tốc độ di chuyển bằng bàn phím

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
    public bool isInvincible = false; // Trạng thái bất tử
    public float invincibilityDuration = 2.0f; // Thời gian bất tử sau khi hồi sinh

// --- THÊM MỚI Ở ĐÂY ---
    [Header("Thiết Lập Khiên")]
    public GameObject shieldVisual; // Kéo GameObject hình ảnh của khiên vào đây
    private bool isShieldActive = false;
    // ----------------------

    [Header("Hiệu Ứng")]
    public string explosionTag = "PlayerExplosion";

    [Header("Âm thanh")]
    public AudioClip shootSound;
    private AudioSource audioSource;

    // Biến nội bộ
    private Camera mainCamera;
    private Vector2 minBounds, maxBounds;
    private bool isDestroyed = false; // Dùng để kiểm soát trạng thái "chết hẳn"
    private float fireCooldown = 0f;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        if (firePoint == null) firePoint = transform.Find("FirePoint");
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
         if (shieldVisual != null) shieldVisual.SetActive(false);
    }

    void Start()
    {
        mainCamera = Camera.main;
        InitBounds();
    }

    void OnEnable()
    {
        ResetForNewLife();
    }

    void ResetForNewLife()
    {
        isDestroyed = false;
    }

    public void InitializePlayer()
    {
        lives = startingLives;
        weaponLevel = 1;
        currentWeaponType = WeaponType.Default;
    }

     void Update()
    {
        if (isDestroyed) return;

        if (playerID == 1)
        {
            // Player 1 dùng chuột
            MoveAndRotate();
            if (Input.GetMouseButton(0) && fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = fireRate;
            }
        }
        else // Player 2 dùng bàn phím
        {
            MoveWithKeyboard();

            // --- ĐÂY LÀ DÒNG BẠN CẦN THAY ĐỔI ---
            // Đã đổi từ KeyCode.RightControl thành KeyCode.Space
            if (Input.GetKey(KeyCode.Space) && fireCooldown <= 0f) 
            {
                Shoot();
                fireCooldown = fireRate;
            }
            // ------------------------------------
        }


        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
        }
    }

    void MoveWithKeyboard()
    {
        float moveX = Input.GetAxis("Horizontal"); 
        float moveY = Input.GetAxis("Vertical");   

        Vector3 moveDirection = new Vector3(moveX, moveY, 0).normalized;
        Vector3 targetPosition = transform.position + moveDirection * keyboardMoveSpeed * Time.deltaTime;
        
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);

        transform.position = targetPosition;
        
        if (moveDirection.sqrMagnitude > 0.01f)
        {
           transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        ResetForNewLife();
        StartCoroutine(InvincibilityCoroutine());
    }

    private IEnumerator InvincibilityCoroutine()
    {
        Debug.Log("Player đang trong trạng thái bất tử!");
        isInvincible = true;

        float endTime = Time.time + invincibilityDuration;
        while (Time.time < endTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
        }

        spriteRenderer.enabled = true;
        isInvincible = false;
        Debug.Log("Player hết bất tử!");
    }

    public void TakeDamage(int damageAmount)
    {
         // Nếu khiên đang bật, không nhận sát thương
        if (isShieldActive || isInvincible || isDestroyed) return;
        // --------------------

        // Phần còn lại của hàm giữ nguyên
        lives -= damageAmount;

        if (UIManager.Instance != null)
        {
            if (playerID == 1) UIManager.Instance.UpdateLives(lives);
            else UIManager.Instance.UpdateLives_P2(lives);
        }

        Debug.Log("Player " + playerID + " bị trúng đạn! Còn lại: " + lives + " mạng!");

        if (!string.IsNullOrEmpty(explosionTag))
        {
            ObjectPooler.Instance.SpawnFromPool(explosionTag, transform.position, Quaternion.identity);
        }

        gameObject.SetActive(false);
        GameManager.Instance.HandlePlayerDeath(this);

        if (lives <= 0)
        {
            isDestroyed = true;
            Debug.Log("Player " + playerID + " đã hết mạng!");
        }
    }
    
    // --- HÀM MỚI ĐỂ KÍCH HOẠT KHIÊN ---
    public void ActivateShield(float duration)
    {
        // Nếu khiên chưa được kích hoạt, hãy bắt đầu coroutine
        if (!isShieldActive)
        {
            StartCoroutine(ShieldCoroutine(duration));
        }
    }

    private IEnumerator ShieldCoroutine(float duration)
    {
        Debug.Log("Player " + playerID + " đã bật khiên!");
        isShieldActive = true;
        if (shieldVisual != null) shieldVisual.SetActive(true);

        // Chờ hết thời gian của khiên
        yield return new WaitForSeconds(duration);

        // Tắt khiên
        isShieldActive = false;
        if (shieldVisual != null) shieldVisual.SetActive(false);
        Debug.Log("Player " + playerID + " đã hết khiên!");
    }
    // ------------------------------------


    #region Unchanged Methods
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroyed || other == null) return;

        string tag = other.tag;

        if (tag == "Blue" || tag == "Black" || tag == "Orange" || tag == "Green" || tag == "Meteor" || tag == "Boss")
        {
            if (other.TryGetComponent<Enemy>(out Enemy enemy))
            {
                TakeDamage(enemy.collisionDamage);
            }
            else
            {
                TakeDamage(1);
            }
            other.gameObject.SetActive(false);
        }
       else if (tag == "PowerUp")
    {
        if (other.TryGetComponent<PowerUp>(out PowerUp powerUp))
        {
            // Kiểm tra xem đây là loại quà gì
            switch (powerUp.powerUpType)
            {
                case PowerUp.PowerUpType.Weapon:
                    UpgradeOrChangeWeapon(powerUp.weaponTypeToGive);
                    break;
                case PowerUp.PowerUpType.Shield:
                    ActivateShield(5f); // Kích hoạt khiên trong 5 giây
                    break;
            }
        }
        Destroy(other.gameObject);
    }
        else if (tag == "EnemyBullet")
        {
            TakeDamage(1);
        }
    }

// Trong PlayerController.cs
private void UpgradeOrChangeWeapon(WeaponType newWeaponType)
{
    // --- THÊM ĐIỀU KIỆN MỚI Ở ĐẦU ---
    if (newWeaponType == WeaponType.Shield)
    {
        // Kích hoạt khiên với thời gian cố định, ví dụ 5 giây
        ActivateShield(5.0f);
    }
    // --------------------------------
    else if (newWeaponType == WeaponType.UpgradeOnly || newWeaponType == currentWeaponType)
    {
        if (weaponLevel < maxWeaponLevel) weaponLevel++;
    }
    else
    {
        currentWeaponType = newWeaponType;
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
        return null;
    }

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

    void InitBounds()
    {
        if (spriteRenderer == null) return;
        Vector2 spriteSize = spriteRenderer.bounds.size;
        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
        minBounds.x += spriteSize.x / 2;
        maxBounds.x -= spriteSize.x / 2;
        minBounds.y += spriteSize.y / 2;
        maxBounds.y -= spriteSize.y / 2;
    }
    #endregion
}