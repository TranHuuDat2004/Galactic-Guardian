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

    void Start()
    {
        mainCamera = Camera.main;
        InitBounds();
    }

    void Update()
    {
        if (isDestroyed) return;

        MoveAndRotate();

        // Bắn liên tục khi giữ chuột trái
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
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                break;

            case 2: // 2 nòng song song
                Instantiate(bulletPrefab, firePoint.position + transform.right * 0.3f, firePoint.rotation);
                Instantiate(bulletPrefab, firePoint.position - transform.right * 0.3f, firePoint.rotation);
                break;

            case 3: // 3 nòng hình quạt
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 15));
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -15));
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroyed || other == null || other.gameObject == null) return;

        if (other.CompareTag("Enemy"))
        {
            isDestroyed = true;
            Destroy(gameObject);
        }
    }
}
