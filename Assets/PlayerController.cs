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
    }

    void Start()
    {
        mainCamera = Camera.main;
        InitBounds();
        // ResetState() sẽ được gọi trong OnEnable
    }

    void OnEnable()
    {
        // ResetState chỉ reset các chỉ số logic, không phải trạng thái bất tử
        ResetForNewLife();
    }

    // Reset các chỉ số cơ bản khi bắt đầu một mạng sống mới
    void ResetForNewLife()
    {
        isDestroyed = false;
        // Không reset weaponLevel và currentWeaponType ở đây để giữ nguyên nâng cấp
    }

    // Hàm này được gọi bởi GameManager khi game bắt đầu
    public void InitializePlayer()
    {
        lives = startingLives;
        weaponLevel = 1;
        currentWeaponType = WeaponType.Default;
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

    // Hàm này được gọi bởi GameManager để hồi sinh người chơi
    public void Respawn()
    {
        // Kích hoạt lại GameObject
        gameObject.SetActive(true);
        // Reset các chỉ số cho mạng sống mới
        ResetForNewLife();
        // Bắt đầu coroutine bất tử và nhấp nháy
        StartCoroutine(InvincibilityCoroutine());
    }

    // Coroutine xử lý trạng thái bất tử và hiệu ứng nhấp nháy
    private IEnumerator InvincibilityCoroutine()
    {
        Debug.Log("Player đang trong trạng thái bất tử!");
        isInvincible = true;

        float endTime = Time.time + invincibilityDuration;
        while (Time.time < endTime)
        {
            // Bật/tắt sprite renderer để tạo hiệu ứng nhấp nháy
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f); // Tần suất nhấp nháy
        }

        // Đảm bảo sprite luôn hiện lại cuối cùng và tắt trạng thái bất tử
        spriteRenderer.enabled = true;
        isInvincible = false;
        Debug.Log("Player hết bất tử!");
    }

    // Hàm nhận sát thương đã được viết lại hoàn toàn
    public void TakeDamage(int damageAmount)
    {
        // Nếu đang bất tử, hoặc đã "chết hẳn" (đang chờ game over), thì không nhận sát thương
        if (isInvincible || isDestroyed) return;

        lives -= damageAmount;

        // Tạo hiệu ứng nổ
        if (!string.IsNullOrEmpty(explosionTag))
        {
            ObjectPooler.Instance.SpawnFromPool(explosionTag, transform.position, Quaternion.identity);
        }

        // Tạm ẩn người chơi đi
        gameObject.SetActive(false);

        // Báo cho GameManager biết người chơi vừa mất một mạng
        if (GameManager.Instance != null)
        {
            GameManager.Instance.HandlePlayerDeath();
        }

        // Nếu hết mạng, đánh dấu là đã "chết hẳn"
        if (lives <= 0)
        {
            isDestroyed = true;
        }
    }

    // Các hàm còn lại không cần thay đổi
    // ...
    #region Unchanged Methods
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroyed || other == null) return;

        string tag = other.tag;

        // --- KIỂM TRA VA CHẠM VỚI TẤT CẢ CÁC LOẠI KẺ ĐỊCH ---
        if (tag == "Blue" || tag == "Black" || tag == "Orange" || tag == "Green" || tag == "Meteor" || tag == "Boss")
        {
            // Thử lấy component Enemy từ đối tượng va chạm
            if (other.TryGetComponent<Enemy>(out Enemy enemy))
            {
                // Nhận một lượng sát thương bằng với collisionDamage của kẻ địch đó
                // Nếu là Boss, sẽ là 10. Nếu là địch thường, sẽ là 1.
                TakeDamage(enemy.collisionDamage);
            }
            else
            {
                // Nếu không lấy được component Enemy (dự phòng, ví dụ cho Meteor nếu nó dùng script khác), chỉ trừ 1 máu
                TakeDamage(1);
            }

            // Tắt đối tượng kẻ địch đi
            other.gameObject.SetActive(false);
        }
        // --------------------------------------------------

        else if (tag == "PowerUp")
        {
            if (other.TryGetComponent<PowerUp>(out PowerUp powerUp))
            {
                UpgradeOrChangeWeapon(powerUp.weaponTypeToGive);
            }
            Destroy(other.gameObject);
        }
        else if (tag == "EnemyBullet")
        {
            // Va chạm với đạn địch luôn trừ 1 máu
            TakeDamage(1);
        }
    }

    private void UpgradeOrChangeWeapon(WeaponType newWeaponType)
    {
        if (newWeaponType == WeaponType.UpgradeOnly || newWeaponType == currentWeaponType)
        {
            if (weaponLevel < maxWeaponLevel)
            {
                weaponLevel++;
            }
        }
        else
        {
            currentWeaponType = newWeaponType;
            // weaponLevel = 1; // Giữ nguyên cấp độ khi đổi vũ khí
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