using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    private Camera mainCamera;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    void Start()
    {
        mainCamera = Camera.main;
        InitBounds();
    }

    void Update()
    {
        MoveAndRotate();

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void MoveAndRotate()
    {
        // 1. Lấy vị trí thế giới của chuột
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 2. Tạo ra một "vị trí mục tiêu" (target) và giới hạn nó ngay lập tức
        Vector3 targetPosition = mouseWorldPosition;
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        targetPosition.z = 0; // Đảm bảo luôn ở mặt phẳng 2D

        // 3. DI CHUYỂN: Gán vị trí của tàu bằng vị trí mục tiêu đã được giới hạn
        // Cách này đảm bảo độ nhạy 1:1 và tàu sẽ "dính" vào con trỏ chuột
        transform.position = targetPosition;

        // 4. XOAY: Tính toán hướng xoay DỰA TRÊN VỊ TRÍ MỤC TIÊU ĐÃ GIỚI HẠN
        // chứ không phải vị trí chuột gốc nữa.
        Vector3 direction = targetPosition - transform.position;

        // Chỉ xoay khi có một hướng di chuyển rõ ràng (tránh bị giật khi chuột đứng yên trên tàu)
        if (direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // Giữ lại -90f vì sprite hướng lên
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    void InitBounds()
    {
        // Lấy kích thước của sprite để tính toán padding một cách tự động
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return; // Thoát nếu không có SpriteRenderer

        Vector2 spriteSize = sr.bounds.size;

        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));

        // Tự động thêm padding dựa trên một nửa kích thước của sprite
        minBounds.x += spriteSize.x / 2;
        maxBounds.x -= spriteSize.x / 2;
        minBounds.y += spriteSize.y / 2;
        maxBounds.y -= spriteSize.y / 2;
    }

    // === THÊM HÀM MỚI VÀO ĐÂY ===
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem có va chạm với đối tượng có tag "Enemy" không
        if (other.CompareTag("Enemy"))
        {
            // Hủy kẻ địch khi va chạm
            Destroy(other.gameObject);

            // Hủy chính người chơi
            // Trong tương lai, bạn có thể thay thế dòng này bằng việc trừ máu
            // hoặc kích hoạt màn hình Game Over.
            Destroy(gameObject);
        }
    }
}