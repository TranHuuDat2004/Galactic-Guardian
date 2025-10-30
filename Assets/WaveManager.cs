using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Tham Chiếu Trong Scene")]
    [Tooltip("Kéo GameObject cha chứa tất cả các cấu hình DestroyZone vào đây.")]
    public Transform destroyZoneParent; 
    
    [Header("Cài Đặt Chuyển Cảnh")]
    public string worldMapSceneName = "WorldMap";
    public string finalVictorySceneName = "VictoryScene";

    [Header("Thiết Lập Background")]
    public float initialScrollSpeed = 0.01f;
    public float victoryScrollSpeed = 0.1f;

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

    // Biến nội bộ
    private List<Wave> currentLevelWaves;
    private int currentWaveIndex;
    private int enemiesAlive = 0;
    private GameObject currentFormationInstance;
    private GameObject currentBackgroundInstance;
    private GameObject currentActiveDestroyZone;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        if (mainCanvas == null) mainCanvas = FindObjectOfType<Canvas>();
    }

    void Start()
    {
        // Trống, chờ lệnh từ GameManager
    }
    
    public void StartLevel(LevelData levelData)
    {
        if (currentBackgroundInstance != null)
        {
            Destroy(currentBackgroundInstance);
        }

        if (levelData.backgroundPrefab != null)
        {
            currentBackgroundInstance = Instantiate(levelData.backgroundPrefab, Vector3.zero, Quaternion.identity);
        }

        if (backgroundMusicSource != null && levelData.backgroundMusic != null)
        {
            backgroundMusicSource.clip = levelData.backgroundMusic;
            backgroundMusicSource.Play();
        }

        currentLevelWaves = levelData.waves;
        currentWaveIndex = -1;

        if (BackgroundScroller.Instance != null)
        {
            BackgroundScroller.Instance.SetScrollSpeed(initialScrollSpeed);
        }

        StartNextWave();
    }

    public void StartNextWave()
    {
        // Tắt DestroyZone của wave cũ đi
        if (currentActiveDestroyZone != null)
        {
            currentActiveDestroyZone.SetActive(false);
            currentActiveDestroyZone = null; // Reset để đảm bảo
        }

        currentWaveIndex++;
        if (currentWaveIndex < currentLevelWaves.Count)
        {
            StartCoroutine(SpawnWaveCoroutine());
        }
        else
        {
            StartCoroutine(LevelCompleteCoroutine());
        }
    }

    private IEnumerator SpawnWaveCoroutine()
    {
        Wave wave = currentLevelWaves[currentWaveIndex];
        
        // Bước 1: Tìm và kích hoạt DestroyZone dựa trên TÊN
        if (destroyZoneParent != null && !string.IsNullOrEmpty(wave.destroyZoneName))
        {
            Transform zoneTransform = destroyZoneParent.Find(wave.destroyZoneName);
            if (zoneTransform != null)
            {
                currentActiveDestroyZone = zoneTransform.gameObject;
                currentActiveDestroyZone.SetActive(true);
            }
            else
            {
                Debug.LogError("Không tìm thấy DestroyZone có tên: '" + wave.destroyZoneName + "' bên trong " + destroyZoneParent.name);
            }
        }
        
        // Bước 2: Hiện tên wave
        yield return StartCoroutine(ShowWaveTextCoroutine(wave.waveName));

        // Bước 3: Dọn dẹp đội hình cũ
        if (currentFormationInstance != null) Destroy(currentFormationInstance);

        // Bước 4: Hiện cảnh báo nếu có
        GameObject warningInstance = null;
        if (wave.warningImagePrefab != null && mainCanvas != null)
        {
            warningInstance = Instantiate(wave.warningImagePrefab, mainCanvas.transform);
            warningInstance.transform.localPosition = wave.warningPosition;
            warningInstance.SetActive(true);
        }

        yield return new WaitForSeconds(wave.warningDelay);

        if (warningInstance != null) Destroy(warningInstance);

        // Bước 5: Tạo đội hình địch mới
        currentFormationInstance = Instantiate(wave.formationPrefab, wave.enemyStartPosition, Quaternion.identity);

        enemiesAlive = 0;
        foreach (Transform spawnPoint in currentFormationInstance.transform)
        {
            GameObject enemyObj = ObjectPooler.Instance.SpawnFromPool(wave.enemyTagToSpawn, spawnPoint.position, spawnPoint.rotation);
            if(enemyObj != null)
            {
                enemiesAlive++;
            }
        }
    }

    private IEnumerator LevelCompleteCoroutine()
    {
        // Tắt DestroyZone của wave cuối cùng
        if (currentActiveDestroyZone != null)
        {
            currentActiveDestroyZone.SetActive(false);
        }
        
        if (currentFormationInstance != null)
        {
            Destroy(currentFormationInstance);
        }
        
        yield return StartCoroutine(ShowWaveTextCoroutine("LEVEL COMPLETE!"));

        int currentLevel = GameProgress.LoadLevel();
        int nextLevel = currentLevel + 1;
        int totalLevels = 3; // Tạm thời giả định

        if (currentLevel >= totalLevels)
        {
            Debug.Log("CHIẾN THẮNG TOÀN BỘ GAME!");
            yield return new WaitForSeconds(3.0f);
            SceneManager.LoadScene(finalVictorySceneName);
        }
        else
        {
            GameProgress.SaveLevel(nextLevel);
            Debug.Log("Hoàn thành Màn " + currentLevel + ". Lưu tiến trình sang Màn " + nextLevel);
            yield return new WaitForSeconds(3.0f);
            SceneManager.LoadScene(worldMapSceneName);
        }
    }

    private IEnumerator ShowWaveTextCoroutine(string textToShow)
    {
        if (waveText == null || waveTextCanvasGroup == null) yield break;

        if (textToShow == "LEVEL COMPLETE!")
        {
            if (backgroundMusicSource != null) backgroundMusicSource.Stop();
            if (levelCompleteSound != null && audioSource != null) audioSource.PlayOneShot(levelCompleteSound);
            yield return new WaitForSeconds(2.0f);
            if (BackgroundScroller.Instance != null)
            {
                BackgroundScroller.Instance.SetScrollSpeed(victoryScrollSpeed);
            }
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
        if (enemiesAlive <= 0)
        {
            StartNextWave();
        }
    }
}