using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;   // thời gian delay giữa mỗi viên đạn
    public int weaponLevel = 1;     // cấp vũ khí (1 = 1 nòng, 2 = 2 nòng, 3 = 3 nòng quạt)

    private Camera mainCamera;
    private Vector2 minBounds;
    private Vector2 maxBounds;
    private bool isDestroyed = false;
    private float fireCooldown = 0f;

    // === THÊM BIẾN NÀY ĐỂ GIỚI HẠN CẤP VŨ KHÍ ===
    private int maxWeaponLevel = 3;

    public int startingLives = 3;
    // Khai báo biến lives này ở đầu script, thay vì dùng private int _lives = 1;
    public int lives;
    void Awake()
    {
        // 1. Nếu firePoint bị mất liên kết trong Inspector, tự tìm lại nó!
        // Chúng ta tìm component Transform của đối tượng con tên là "FirePoint"
        if (firePoint == null)
        {
            Transform foundPoint = transform.Find("FirePoint");
            if (foundPoint != null)
            {
                firePoint = foundPoint;
            }
        }

        // 2. Tương tự, nếu bulletPrefab bị mất, bạn sẽ phải dùng cách khác: 
        // Tìm nó trong thư mục Resources hoặc AssetBundle, nhưng bây giờ chúng ta 
        // cứ dựa vào Inspector cho Prefab, và chỉ tập trung vào FirePoint.

        // Nếu bạn muốn chắc chắn FirePoint không bị mất ngay từ đầu, 
        // bạn có thể đổi 'public Transform firePoint;' thành private 
        // và dùng thuộc tính [SerializeField] để ép người dùng gán nó. 
    }

    void Start()
    {
        mainCamera = Camera.main;
        InitBounds();
        // KHÔNG CẦN gọi ResetState ở đây nữa, vì nó sẽ được gọi khi Enable
    }

    // Thêm hàm này để reset lại tất cả các trạng thái của Player
    void ResetState()
    {
        isDestroyed = false; // Cho phép di chuyển và bắn
        lives = startingLives; // Reset số mạng
        fireCooldown = 0f; // Reset thời gian hồi chiêu

        // Reset các hiệu ứng hình ảnh nếu có (như shield, fire visualization)
        // Ví dụ: 
        // _shieldVisualize.SetActive(false);
        // _Fire1_Visualize.SetActive(false);
        // _Fire2_Visualize.SetActive(false);
        // ... (làm tương tự cho các hiệu ứng khác)

        // Reset lại vị trí và góc quay ban đầu (hoặc vị trí spawn)
        // Ví dụ: transform.position = Vector3.zero;
        //        transform.rotation = Quaternion.identity;
    }

    // Hàm này được gọi mỗi khi GameObject Player được SetActive(true)
    void OnEnable()
    {
        ResetState(); // Khởi tạo lại mọi thứ khi Player được "mượn" từ Pool
    }

    void Update()
    {
        // Dòng này là quan trọng nhất: nếu isDestroyed là true, thì không làm gì cả
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

        switch (weaponLevel)
        {
            case 1: // 1 nòng
                    // Lấy 1 viên đạn từ kho và bắn thẳng
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation);
                break;

            case 2: // 2 nòng song song
                    // Lấy 2 viên đạn từ kho và đặt ở hai vị trí song song
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position + transform.right * 0.3f, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position - transform.right * 0.3f, firePoint.rotation);
                break;

            case 3: // 3 nòng hình quạt
                    // Lấy 3 viên đạn từ kho, giữ nguyên vị trí nhưng thay đổi góc bắn
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 15));
                ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -15));
                break;
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

    // Hàm OnTriggerEnter2D cũ của bạn, CHỈ xử lý trừ máu và kiểm tra Game Over
    private void OnTriggerEnter2D(Collider2D other)
    {
        // THAY THẾ TOÀN BỘ hàm OnTriggerEnter2D cũ bằng hàm này
        if (isDestroyed || other == null || other.gameObject == null) return;

        if (other.CompareTag("Blue") || other.CompareTag("Black") || other.CompareTag("Orange") || other.CompareTag("Green"))
        {
            lives--;
            Debug.Log("Player còn lại: " + lives + " mạng!");

            if (lives <= 0)
            {
                isDestroyed = true; // Đặt cờ isDestroyed là true để dừng Update()
                Debug.Log("GAME OVER! Player đã bị phá hủy.");
                // Thay vì Destroy, chúng ta sẽ TẮT nó đi để trả về pool
                gameObject.SetActive(false);
            }
            // Đừng quên Destroy/SetActive(false) kẻ địch đã va vào mình!
            // Cần thêm logic này để kẻ địch cũng biến mất sau va chạm
            if (other.TryGetComponent<Enemy>(out Enemy enemy))
            {
                // Logic để enemy biến mất (nếu dùng pool thì SetActive(false), nếu không thì Destroy)
                // Vì enemy spawn bằng pool, nên dùng SetActive(false)
                other.gameObject.SetActive(false);
            }
        }

        // --- THÊM LOGIC MỚI: NHẶT POWER-UP ---
        if (other.CompareTag("PowerUp"))
        {
            // Hủy vật phẩm ngay khi nhặt
            Destroy(other.gameObject);

            // Nâng cấp vũ khí
            UpgradeWeapon();
        }
    }

    // --- THÊM HÀM MỚI: NÂNG CẤP VŨ KHÍ ---
    private void UpgradeWeapon()
    {
        // Nếu chưa đạt cấp tối đa thì mới cộng
        if (weaponLevel < maxWeaponLevel)
        {
            weaponLevel++;
        }

        // (Tùy chọn) Chơi một âm thanh nâng cấp
        // audioSource.PlayOneShot(upgradeSound);
    }
}
