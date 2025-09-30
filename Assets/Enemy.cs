using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LỚP MỚI ĐỂ ĐỊNH NGHĨA MỘT PHA TẤN CÔNG (Đặt ở ngoài lớp Enemy)
[System.Serializable]
public class AttackPhase
{
    public string phaseName;
    public BossAttackType attackType; // Kiểu tấn công của pha này
    public float duration;            // Pha này kéo dài bao lâu (giây)
    public float fireRate;            // Tốc độ bắn trong pha này
}

public class Enemy : MonoBehaviour
{
    [Header("Loại Kẻ Địch")]
    public MovementType movementType = MovementType.Vertical;

    [Header("Chỉ Số Cơ Bản")]
    public float speed = 3.0f;
    public int health = 1;
    public float rotationSpeed = 50.0f;

    [Header("Thiết Lập Di Chuyển Roaming (Boss)")]
    public Vector2 roamingAreaMin = new Vector2(-12, -6);
    public Vector2 roamingAreaMax = new Vector2(12, 6);
    public float waitAtDestination = 1.0f;

    [Header("Thiết Lập Bắn (Kẻ địch thường)")]
    public bool canShoot = true;
    public string bulletTag = "EnemyBullet";
    public float fireRate = 2.0f;
    public Transform firePoint;
    private float fireCooldown = 0f;
    
    [Header("Thiết Lập Tấn Công Của Boss")]
    public bool isBoss = false;
    public List<AttackPhase> attackPhases;
    private int currentPhaseIndex = -1;
    private Coroutine attackPatternCoroutine;

    [Header("Hiệu Ứng")]
    public string explosionTag = "EnemyExplosion";
    public GameObject engineEffectPrefab;
    private GameObject currentEngineEffect;

    [Header("Thiết Lập Rớt Đồ")]
    public GameObject[] powerUpPrefabs;
    [Range(0, 100)] public float dropChance = 15f;
    
    // Biến nội bộ
    private bool isDead = false;
    private int initialHealth;
    private Vector2 moveDirection;
    private Vector3 nextDestination;
    private bool isMovingToDestination = false;

    void Awake()
    {
        initialHealth = health;
        if (firePoint == null)
        {
            Transform foundPoint = transform.Find("FirePoint");
            if (foundPoint != null) firePoint = foundPoint;
        }
    }

    void OnEnable()
    {
        health = initialHealth;
        isDead = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        // Reset thời gian hồi chiêu cho kẻ địch thường
        fireCooldown = Random.Range(0.5f, fireRate);
        
        // Thiết lập di chuyển ban đầu
        if (movementType == MovementType.Diagonal)
        {
            float randomX = Random.Range(0, 2) == 0 ? -1f : 1f;
            moveDirection = new Vector2(randomX, -1f).normalized;
        }
        else if (movementType == MovementType.Roaming)
        {
            StopAllCoroutines(); 
            StartCoroutine(RoamingCoroutine());
        }
        
        // Khởi động chu trình tấn công nếu là Boss
        if (isBoss && attackPhases.Count > 0)
        {
            attackPatternCoroutine = StartCoroutine(AttackPatternCoroutine());
        }

        if (engineEffectPrefab != null)
        {
            currentEngineEffect = Instantiate(engineEffectPrefab, transform.position, transform.rotation, transform);
        }
    }

    void OnDisable()
    {
        // Dừng tất cả các coroutine khi đối tượng bị tắt để tránh lỗi
        StopAllCoroutines();
        
        if (currentEngineEffect != null)
        {
            Destroy(currentEngineEffect);
            currentEngineEffect = null;
        }
    }

    void Update()
    {
        if (isDead) return;

        // Xử lý di chuyển
        switch (movementType)
        {
            case MovementType.Vertical:
                transform.Translate(Vector2.down * speed * Time.deltaTime);
                break;
            case MovementType.Diagonal:
                transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
                transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
                break;
            case MovementType.Formation:
                break;
            case MovementType.Roaming:
                if (isMovingToDestination)
                {
                    transform.position = Vector3.MoveTowards(transform.position, nextDestination, speed * Time.deltaTime);
                }
                break;
        }

        // Xử lý bắn cho KẺ ĐỊCH THƯỜNG (không phải Boss)
        if (canShoot && !isBoss)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0)
            {
                // Kẻ địch thường chỉ có 1 kiểu bắn đơn giản
                Shoot(BossAttackType.SingleShot); 
                fireCooldown = fireRate;
            }
        }
    }
    
    private IEnumerator RoamingCoroutine()
    {
        while (!isDead)
        {
            float randomX = Random.Range(roamingAreaMin.x, roamingAreaMax.x);
            float randomY = Random.Range(roamingAreaMin.y, roamingAreaMax.y);
            nextDestination = new Vector3(randomX, randomY, 0);
            
            isMovingToDestination = true;

            while (Vector3.Distance(transform.position, nextDestination) > 0.1f)
            {
                if (isDead) yield break; 
                yield return null;
            }

            isMovingToDestination = false;
            yield return new WaitForSeconds(waitAtDestination);
        }
    }

    private IEnumerator AttackPatternCoroutine()
    {
        yield return new WaitForSeconds(2.0f); // Chờ 2 giây trước khi bắt đầu

        while (!isDead)
        {
            currentPhaseIndex = (currentPhaseIndex + 1) % attackPhases.Count;
            AttackPhase currentPhase = attackPhases[currentPhaseIndex];
            
            float phaseTimer = 0f;
            float shotTimer = 0f;

            while (phaseTimer < currentPhase.duration)
            {
                if (isDead) yield break;

                shotTimer -= Time.deltaTime;
                if (shotTimer <= 0)
                {
                    Shoot(currentPhase.attackType);
                    shotTimer = currentPhase.fireRate;
                }

                phaseTimer += Time.deltaTime;
                yield return null;
            }
        }
    }
    
    void Shoot(BossAttackType attackType)
    {
        if (firePoint == null || string.IsNullOrEmpty(bulletTag)) return;

        switch (attackType)
        {
            case BossAttackType.SingleShot:
                ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation);
                break;
            case BossAttackType.DoubleShot:
                ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position + transform.right * 0.5f, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position - transform.right * 0.5f, firePoint.rotation);
                break;
            case BossAttackType.TripleSpread:
                ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 20));
                ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -20));
                break;
            case BossAttackType.QuintupleSpread:
                ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation);
                ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 15));
                ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -15));
                ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 30));
                ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -30));
                break;
            case BossAttackType.SideCannons:
                ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 90));
                ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -90));
                break;
            case BossAttackType.FullFrontalAssault:
                for (int i = -3; i <= 3; i++)
                {
                    ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, i * 10));
                }
                break;
            case BossAttackType.CircularBurst:
                int numberOfBullets = 12;
                for (int i = 0; i < numberOfBullets; i++)
                {
                    float angle = i * (360f / numberOfBullets);
                    ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, Quaternion.Euler(0, 0, angle));
                }
                break;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || other == null) return;
        if (other.CompareTag("Bullet"))
        {
            if (other.TryGetComponent<Bullet>(out Bullet bullet))
            {
                TakeDamage(bullet.damage);
            }
        }
        else if (other.CompareTag("Player"))
        {
            Die();
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;
        health -= damageAmount;
        if (health <= 0) Die();
    }
    
    private void Die()
    {
        isDead = true;
        
        TryDropLoot();

        if (!string.IsNullOrEmpty(explosionTag))
        {
            ObjectPooler.Instance.SpawnFromPool(explosionTag, transform.position, Quaternion.identity);
        }

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnEnemyDestroyed();
        }
        
        gameObject.SetActive(false); // Sẽ tự động gọi OnDisable
    }

    private void TryDropLoot()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;
        float randomChance = Random.Range(0f, 100f);
        if (randomChance <= dropChance)
        {
            int randomIndex = Random.Range(0, powerUpPrefabs.Length);
            GameObject randomPowerUpPrefab = powerUpPrefabs[randomIndex];
            if (randomPowerUpPrefab != null) Instantiate(randomPowerUpPrefab, transform.position, Quaternion.identity);
        }
    }
    
    public void OnBecameInvisible()
    {
        if (gameObject.activeInHierarchy && !isDead)
        {
            if (WaveManager.Instance != null) WaveManager.Instance.OnEnemyDestroyed();
        }
        gameObject.SetActive(false); // Sẽ tự động gọi OnDisable
    }
}