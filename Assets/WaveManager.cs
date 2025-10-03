using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// KHÔNG CÒN ĐỊNH NGHĨA LỚP "WAVE" Ở ĐÂY NỮA
// Nó đã được định nghĩa trong file LevelData.cs

public class WaveManager : MonoBehaviour
{
    private GameObject currentBackgroundInstance; // Biến mới để lưu background hiện tại
    // KHÔNG CÒN CÁC BIẾN CŨ allWaves và wavesPerLevel
    private List<Wave> currentLevelWaves; // Biến để lưu trữ các wave của màn chơi hiện tại

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
    private int currentWaveIndex;
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
        // Hàm Start sẽ trống, chờ lệnh từ GameManager
    }

    // Hàm này được gọi bởi GameManager để bắt đầu một màn chơi mới
        public void StartLevel(LevelData levelData)
    {
        // 1. Dọn dẹp background của màn chơi cũ (nếu có)
        if (currentBackgroundInstance != null)
        {
            Destroy(currentBackgroundInstance);
        }

        // 2. Tạo ra background mới từ Prefab
        if (levelData.backgroundPrefab != null)
        {
            currentBackgroundInstance = Instantiate(levelData.backgroundPrefab, Vector3.zero, Quaternion.identity);
        }

        // 3. Thiết lập nhạc
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.clip = levelData.backgroundMusic;
            backgroundMusicSource.Play();
        }

        // 4. Bắt đầu các wave
        currentLevelWaves = levelData.waves;
        currentWaveIndex = -1;

        // Tốc độ cuộn giờ sẽ được thiết lập trên chính prefab background,
        // nhưng chúng ta vẫn có thể override nó ở đây nếu muốn.
        if (BackgroundScroller.Instance != null)
        {
            BackgroundScroller.Instance.SetScrollSpeed(initialScrollSpeed);
        }

        StartNextWave();
    }

    public void StartNextWave()
    {
        currentWaveIndex++;
        if (currentWaveIndex < currentLevelWaves.Count)
        {
            StartCoroutine(SpawnWaveCoroutine(currentLevelWaves[currentWaveIndex]));
        }
        else
        {
            // Đã hoàn thành tất cả các wave của màn chơi này
            StartCoroutine(LevelCompleteCoroutine());
        }
    }

    private IEnumerator SpawnWaveCoroutine(Wave wave)
    {
        yield return StartCoroutine(ShowWaveTextCoroutine(wave.waveName));

        if (currentFormationInstance != null) Destroy(currentFormationInstance);

        GameObject warningInstance = null;
        if (wave.warningImagePrefab != null && mainCanvas != null)
        {
            warningInstance = Instantiate(wave.warningImagePrefab, mainCanvas.transform);
            warningInstance.transform.localPosition = wave.warningPosition;
            warningInstance.SetActive(true);
        }

        yield return new WaitForSeconds(wave.warningDelay);

        if (warningInstance != null) Destroy(warningInstance);

        currentFormationInstance = Instantiate(wave.formationPrefab, wave.enemyStartPosition, Quaternion.identity);

        enemiesAlive = 0;
        foreach (Transform spawnPoint in currentFormationInstance.transform)
        {
            ObjectPooler.Instance.SpawnFromPool(wave.enemyTagToSpawn, spawnPoint.position, spawnPoint.rotation);
            enemiesAlive++;
        }
    }

    private IEnumerator LevelCompleteCoroutine()
    {
        yield return StartCoroutine(ShowWaveTextCoroutine("LEVEL COMPLETE!"));

        int currentLevel = GameProgress.LoadLevel();
        // Giả sử tổng số màn là số lượng LevelData bạn có (cần truyền vào từ đâu đó)
        // Cách đơn giản là kiểm tra xem có LevelData cho màn tiếp theo không
        int nextLevel = currentLevel + 1;

        // Cần một cách để biết tổng số màn, ví dụ qua WorldMapController
        // Tạm thời chúng ta sẽ giả sử có 3 màn
        int totalLevels = 3;

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

        // Xử lý logic đặc biệt cho "LEVEL COMPLETE!"
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