using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public GameObject formationPrefab;
        public string enemyTagToSpawn;

        [Header("Tùy Chọn Cảnh Báo & Vị Trí")]
        public GameObject warningImagePrefab;
        public Vector3 warningPosition;      // Vị trí của cảnh báo (tọa độ UI)
        public Vector3 enemyStartPosition;   // Vị trí XUẤT PHÁT của đội hình (tọa độ World)
        public float warningDelay = 2.0f;
    }

    [Header("Cài Đặt Màn Chơi")]
    [SerializeField] private Wave[] waves;

    [Header("Thiết Lập UI")]
    [SerializeField] private Canvas mainCanvas; 
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private CanvasGroup waveTextCanvasGroup;
    [SerializeField] private float textDisplayTime = 1.5f;
    [SerializeField] private float textFadeTime = 0.5f;
    
    [Header("Âm Thanh")]
    [SerializeField] private AudioClip levelCompleteSound;
    [SerializeField] private AudioSource backgroundMusicSource;
    private AudioSource audioSource; 

    private int currentWaveIndex = -1;
    private int enemiesAlive = 0;
    private GameObject currentFormationInstance;
    public static WaveManager Instance;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        if (mainCanvas == null) mainCanvas = FindObjectOfType<Canvas>();
    }

    void Start()
    {
        if (waveTextCanvasGroup != null) waveTextCanvasGroup.alpha = 0;
        StartNextWave();
    }

    public void StartNextWave()
    {
        currentWaveIndex++;
        if (currentWaveIndex < waves.Length) StartCoroutine(SpawnWaveCoroutine(waves[currentWaveIndex]));
        else StartCoroutine(ShowWaveTextCoroutine("LEVEL COMPLETE!"));
    }

    // Coroutine đã được đơn giản hóa
    private IEnumerator SpawnWaveCoroutine(Wave wave)
    {
        yield return StartCoroutine(ShowWaveTextCoroutine(wave.waveName));

        // Hiện cảnh báo nếu có
        GameObject warningInstance = null;
        if (wave.warningImagePrefab != null && mainCanvas != null)
        {
            warningInstance = Instantiate(wave.warningImagePrefab, mainCanvas.transform);
            warningInstance.transform.localPosition = wave.warningPosition;
            warningInstance.SetActive(true);
        }

        // Chờ theo thời gian cảnh báo
        yield return new WaitForSeconds(wave.warningDelay);
        
        // Xóa cảnh báo đi
        if (warningInstance != null) Destroy(warningInstance);

        // Tạo đội hình địch ở vị trí BẮT ĐẦU.
        // FormationController sẽ tự động di chuyển nó từ đây.
        if (currentFormationInstance != null) Destroy(currentFormationInstance);
        currentFormationInstance = Instantiate(wave.formationPrefab, wave.enemyStartPosition, Quaternion.identity);

        enemiesAlive = 0;
        foreach (Transform spawnPoint in currentFormationInstance.transform)
        {
            ObjectPooler.Instance.SpawnFromPool(wave.enemyTagToSpawn, spawnPoint.position, spawnPoint.rotation);
            enemiesAlive++;
        }
    }
    
    // Hàm ShowWaveTextCoroutine và OnEnemyDestroyed không thay đổi
    private IEnumerator ShowWaveTextCoroutine(string textToShow)
    {
        if (waveText == null || waveTextCanvasGroup == null) yield break;
        if (textToShow == "LEVEL COMPLETE!")
        {
            if (backgroundMusicSource != null) backgroundMusicSource.Stop();
            if (levelCompleteSound != null && audioSource != null) audioSource.PlayOneShot(levelCompleteSound);
        }
        waveText.text = textToShow;
        float elapsedTime = 0f;
        while (elapsedTime < textFadeTime)
        {
            waveTextCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / textFadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        waveTextCanvasGroup.alpha = 1;
        yield return new WaitForSeconds(textDisplayTime);
        elapsedTime = 0f;
        while (elapsedTime < textFadeTime)
        {
            waveTextCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / textFadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        waveTextCanvasGroup.alpha = 0;
    }

    public void OnEnemyDestroyed()
    {
        enemiesAlive--;
        if (enemiesAlive <= 0) StartNextWave();
    }
}