using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Lớp AttackPhase không đổi
[System.Serializable]
public class AttackPhase
{
    public string phaseName;
    public BossAttackType attackType;
    public float duration;
    public float fireRate;
}

public class Enemy : MonoBehaviour
{
    [Header("Loại Kẻ Địch")]
    public MovementType movementType = MovementType.Vertical;

    [Header("Chỉ Số Cơ Bản")]
    public int health = 1;
    public float speed = 3.0f;
    public int collisionDamage = 1;
    public float rotationSpeed = 50.0f;
    public string bossDisplayName = "CHIẾN HẠM ZYGON";

    [Header("Thiết Lập Di Chuyển Roaming (Boss)")]
    public Vector2 roamingAreaMin = new Vector2(-12, -6);
    public Vector2 roamingAreaMax = new Vector2(12, 6);
    public float waitAtDestination = 1.0f;

    [Header("Thiết Lập Bắn")]
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
    public GameObject burnEffectPrefab; // << Kéo prefab BurnVFX vào đây
    private GameObject currentEngineEffect;
    private GameObject activeBurnEffectInstance; // << Để lưu trữ hiệu ứng lửa

    [Header("Thiết Lập Rớt Đồ")]
    public GameObject[] powerUpPrefabs;
    [Range(0, 100)] public float dropChance = 15f;
    
    // Biến nội bộ
    private bool isDead = false;
    private int initialHealth;
    private Vector2 moveDirection;
    private Vector3 nextDestination;
    private bool isMovingToDestination = false;

    private Coroutine activeFreezeCoroutine;
    private Coroutine activeBurnCoroutine;
    // Biến để lưu trữ các chỉ số gốc
    private float originalSpeed;
    private SpriteRenderer spriteRenderer; // Để thay đổi màu sắc


    // ----------------------
   
    void Awake()
    {
        initialHealth = health;
        if (firePoint == null)
        {
            Transform foundPoint = transform.Find("FirePoint");
            if (foundPoint != null) firePoint = foundPoint;
        }
        originalSpeed = speed; // Lưu lại tốc độ gốc khi đối tượng được tạo
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        health = initialHealth;
        isDead = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        fireCooldown = Random.Range(0.5f, fireRate);
        health = initialHealth;
        isDead = false;

        // Thiết lập hướng di chuyển
        switch (movementType)
        {
            case MovementType.Diagonal:
                float randomX = Random.Range(0, 2) == 0 ? -1f : 1f;
                moveDirection = new Vector2(randomX, -1f).normalized;
                break;
            case MovementType.DiagonalRightToLeft:
                moveDirection = new Vector2(-Mathf.Tan(40 * Mathf.Deg2Rad), -1f).normalized;
                break;
            case MovementType.DiagonalLeftToRight:
                moveDirection = new Vector2(Mathf.Tan(40 * Mathf.Deg2Rad), -1f).normalized;
                break;
            case MovementType.MoveUp:
                moveDirection = Vector2.up;
                break;
            case MovementType.HorizontalLeftToRight:
                moveDirection = Vector2.right;
                break;
            case MovementType.HorizontalRightToLeft:
                moveDirection = Vector2.left;
                break;
            case MovementType.Roaming:
                StopAllCoroutines();
                StartCoroutine(RoamingCoroutine());
                break;
        }
        
        speed = originalSpeed;
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        // Dừng các coroutine hiệu ứng còn sót lại từ lần sử dụng trước
        if (activeFreezeCoroutine != null) StopCoroutine(activeFreezeCoroutine);
        if (activeBurnCoroutine != null) StopCoroutine(activeBurnCoroutine);

        if (isBoss)
        {
            if (attackPhases.Count > 0)
            {
                attackPatternCoroutine = StartCoroutine(AttackPatternCoroutine());
            }
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowBossHealthBar(bossDisplayName);
                UIManager.Instance.UpdateBossHealth(health, initialHealth);
            }
        }

        if (engineEffectPrefab != null)
        {
            currentEngineEffect = Instantiate(engineEffectPrefab, transform.position, transform.rotation, transform);
        }
         if (activeBurnEffectInstance != null)
        {
            Destroy(activeBurnEffectInstance);
        }
    }

    // --- HÀM MỚI ĐỂ DỌN DẸP SẠCH SẼ ---
    void OnDisable()
    {
        // Dừng tất cả các coroutine khi đối tượng bị tắt
        StopAllCoroutines();
        
        // Ẩn thanh máu của Boss nếu có
        if (isBoss && UIManager.Instance != null) 
        { 
            UIManager.Instance.HideBossHealthBar(); 
        }

        // Hủy đối tượng hiệu ứng động cơ nếu nó tồn tại
        if (currentEngineEffect != null) 
        { 
            Destroy(currentEngineEffect);
            currentEngineEffect = null; // Đặt lại là null để tránh lỗi
        }
        if (activeBurnEffectInstance != null)
        {
            Destroy(activeBurnEffectInstance);
        }
    }
    // -------------------------------------

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
            case MovementType.DiagonalRightToLeft:
            case MovementType.DiagonalLeftToRight:
            case MovementType.MoveUp:
            case MovementType.HorizontalLeftToRight:
            case MovementType.HorizontalRightToLeft:
                transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
                if (movementType == MovementType.Diagonal)
                {
                    transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
                }
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

        // Xử lý bắn
        if (canShoot && !isBoss)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0)
            {
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
        yield return new WaitForSeconds(2.0f);
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
            case BossAttackType.SingleShot: ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation); break;
            case BossAttackType.DoubleShot: ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position + transform.right * 0.5f, firePoint.rotation); ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position - transform.right * 0.5f, firePoint.rotation); break;
            case BossAttackType.TripleSpread: ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation); ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 20)); ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -20)); break;
            case BossAttackType.QuintupleSpread: ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation); ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 15)); ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -15)); ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 30)); ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -30)); break;
            case BossAttackType.SideCannons: ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 90)); ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -90)); break;
            case BossAttackType.FullFrontalAssault: for (int i = -3; i <= 3; i++) { ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, i * 10)); } break;
            case BossAttackType.CircularBurst: int numberOfBullets = 12; for (int i = 0; i < numberOfBullets; i++) { float angle = i * (360f / numberOfBullets); ObjectPooler.Instance.SpawnFromPool(bulletTag, firePoint.position, Quaternion.Euler(0, 0, angle)); } break;
        }
    }

    // Trong Enemy.cs - Đoạn code này đã đúng và sẽ là nơi duy nhất xử lý va chạm Player-Enemy
private void OnTriggerEnter2D(Collider2D other)
{
    if (isDead || other == null) return;

    if (other.CompareTag("Bullet"))
    {
        if (other.TryGetComponent<Bullet>(out Bullet bullet)) { TakeDamage(bullet.damage); }
    }
    else if (other.CompareTag("Player"))
    {
        // Gây sát thương cho Player
        if(other.TryGetComponent<PlayerController>(out PlayerController player)) 
        { 
            player.TakeDamage(collisionDamage); 
        }
        // Sau đó tự hủy
        Die(); 
    }
}

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;
        health -= damageAmount;
        if (isBoss && UIManager.Instance != null) { UIManager.Instance.UpdateBossHealth(health, initialHealth); }
        if (health <= 0) { Die(); }
    }

    public void ApplyStatusEffect(Bullet bullet)
    {
        if (isDead) return;

        switch (bullet.effectType)
        {
            case SpecialEffectType.Freeze:
                // Nếu đang có hiệu ứng đóng băng, dừng nó lại để bắt đầu cái mới
                if (activeFreezeCoroutine != null) StopCoroutine(activeFreezeCoroutine);
                activeFreezeCoroutine = StartCoroutine(FreezeCoroutine(bullet.effectPotency, bullet.effectDuration));
                break;
            case SpecialEffectType.Burn:
                // Tương tự với hiệu ứng đốt cháy
                if (activeBurnCoroutine != null) StopCoroutine(activeBurnCoroutine);
                activeBurnCoroutine = StartCoroutine(BurnCoroutine(bullet.effectPotency, bullet.effectDuration));
                break;
        }
    }
    
     private IEnumerator FreezeCoroutine(float slowMultiplier, float duration)
    {
        speed = originalSpeed * (1f - slowMultiplier); // Làm chậm tốc độ
        if (spriteRenderer != null) spriteRenderer.color = Color.cyan; // Đổi màu thành xanh băng

        yield return new WaitForSeconds(duration);

        // Hồi phục lại trạng thái ban đầu
        speed = originalSpeed;
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        activeFreezeCoroutine = null; // Đánh dấu là đã kết thúc
    }

    // --- COROUTINE CHO HIỆU ỨNG ĐỐT CHÁY ---
    private IEnumerator BurnCoroutine(float damagePerSecond, float duration)
{
    // ... code thay đổi màu sắc và tạo hiệu ứng lửa không đổi ...
    if (spriteRenderer != null) spriteRenderer.color = new Color(1f, 0.5f, 0f);
    if (burnEffectPrefab != null)
    {
        activeBurnEffectInstance = Instantiate(burnEffectPrefab, transform.position, Quaternion.identity, transform);
    }
    
    // ... vòng lặp gây sát thương không đổi ...
    float timer = 0f;
    while (timer < duration)
    {
        TakeDamage(Mathf.CeilToInt(damagePerSecond));
        yield return new WaitForSeconds(1f);
        timer += 1f;
    }
    
    // --- ĐÂY LÀ PHẦN THAY ĐỔI QUAN TRỌNG ---
    // Thay vì hủy ngay lập tức, chúng ta sẽ ra lệnh cho nó tự tắt
    if (activeBurnEffectInstance != null)
    {
        // Lấy component ParticleSystem từ hiệu ứng
        ParticleSystem ps = activeBurnEffectInstance.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            // Ra lệnh cho hệ thống ngừng phát ra các hạt mới
            ps.Stop();
        }
        else
        {
            // Nếu không tìm thấy ParticleSystem (dự phòng), thì mới hủy theo cách cũ
            Destroy(activeBurnEffectInstance);
        }
    }
    // ------------------------------------------
    
    // ... code hồi phục màu sắc và reset coroutine không đổi ...
    if (spriteRenderer != null) spriteRenderer.color = Color.white;
    activeBurnCoroutine = null;
}
    
    

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        TryDropLoot();

        // --- THÊM MỚI Ở ĐÂY ---
    // Gọi SFXManager để phát âm thanh nổ
    if (SFXManager.Instance != null)
    {
        SFXManager.Instance.PlaySound(SFXManager.Instance.enemyExplosionSound);
    }
    // ----------------------


        if (!string.IsNullOrEmpty(explosionTag)) { ObjectPooler.Instance.SpawnFromPool(explosionTag, transform.position, Quaternion.identity); }
        if (WaveManager.Instance != null) { WaveManager.Instance.OnEnemyDestroyed(); }
        
        // Hàm OnDisable sẽ tự động được gọi sau dòng này, giúp dọn dẹp hiệu ứng động cơ
        gameObject.SetActive(false);
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
        
        // Hàm OnDisable sẽ tự động được gọi sau dòng này
        gameObject.SetActive(false);
    }
}