// Dán toàn bộ nội dung này vào file PlayerController.cs

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;
    public int weaponLevel = 1;
    private int maxWeaponLevel = 10;

    // --- THÊM MỚI Ở ĐÂY ---
    [Header("Âm thanh")]
    public AudioClip shootSound; // << Biến để kéo file âm thanh bắn vào
    private AudioSource audioSource; // << Biến để điều khiển component "Loa"
    // ----------------------

    private Camera mainCamera;
    private Vector2 minBounds;
    private Vector2 maxBounds;
    private bool isDestroyed = false;
    private float fireCooldown = 0f;

    public int startingLives = 3;
    public int lives;

    void Awake()
    {
        if (firePoint == null)
        {
            Transform foundPoint = transform.Find("FirePoint");
            if (foundPoint != null)
            {
                firePoint = foundPoint;
            }
        }

        // --- THÊM MỚI Ở ĐÂY ---
        // Lấy component Audio Source để có thể điều khiển nó
        audioSource = GetComponent<AudioSource>();
        // ----------------------
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
    }

    void Update()
    {
        if (isDestroyed) return;

        MoveAndRotate();

        if (Input.GetMouseButton(0))
        {
            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = fireRate;
            }
        }

        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;
    }

    void MoveAndRotate()
    {
        // ... (Hàm này không thay đổi)
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

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Phát âm thanh bắn
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        switch (weaponLevel)
        {
            case 1: // 1 nòng
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation);
                break;

            case 2: // 2 nòng song song
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position + transform.right * 0.3f, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position - transform.right * 0.3f, firePoint.rotation);
                break;

            case 3: // 3 nòng hình quạt
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 15));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -15));
                break;

            // --- CÁC CẤP ĐỘ MỚI BẮT ĐẦU TỪ ĐÂY ---

            case 4: // Cấp 4: Ngôi Sao 5 Cánh (5 nòng hình quạt)
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 15));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -15));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 30));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -30));
                break;

            case 5: // Cấp 5: Mũi Tên Hủy Diệt (2 thẳng, 2 chéo)
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position + transform.right * 0.2f, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position - transform.right * 0.2f, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 25));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -25));
                break;

            case 6: // Cấp 6: Loạt Đạn Tên Lửa (2 thẳng, 4 chéo)
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position + transform.right * 0.2f, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position - transform.right * 0.2f, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 20));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -20));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 40));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -40));
                break;

            case 7: // Cấp 7: Pháo Đài Bay (5 nòng trước + 2 bên)
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 15));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -15));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 30));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -30));
                // Súng hai bên
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 90));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -90));
                break;

            case 8: // Cấp 8: Bão Đạn (Kết hợp cấp 6 và súng hai bên)
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position + transform.right * 0.2f, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position - transform.right * 0.2f, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 20));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -20));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 40));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -40));
                // Súng hai bên
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 90));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -90));
                break;

            case 9: // Cấp 9: Vòng Cung Hủy Diệt (7 nòng quạt rộng)
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 10));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -10));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 20));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -20));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 30));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -30));
                break;

            case 10: // Cấp 10: Bất Khả Xâm Phạm (Bắn mọi hướng)
                     // Phía trước
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 20));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -20));
                // Chéo
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 45));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -45));
                // Hai bên
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 90));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -90));
                // Phía sau
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 180));
                break;

            // Thêm một trường hợp default để nếu weaponLevel lớn hơn 10, nó sẽ bắn như cấp 10
            default:
                goto case 10;
        }
    }

    void InitBounds()
    {
        // ... (Hàm này không thay đổi)
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroyed || other == null || other.gameObject == null) return;

        if (other.CompareTag("Blue") || other.CompareTag("Black") || other.CompareTag("Orange") || other.CompareTag("Green"))
        {
            TakeDamage(1); // Gọi hàm TakeDamage để xử lý va chạm
            if (other.TryGetComponent<Enemy>(out Enemy enemy))
            {
                other.gameObject.SetActive(false);
            }
        }

        if (other.CompareTag("PowerUp"))
        {
            Destroy(other.gameObject);
            UpgradeWeapon();
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

    private void UpgradeWeapon()
    {
        if (weaponLevel < maxWeaponLevel)
        {
            weaponLevel++;
        }
    }
}