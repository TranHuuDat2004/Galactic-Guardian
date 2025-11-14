using UnityEngine;
using UnityEngine.SceneManagement; // Bắt buộc phải có để quản lý scene

public class TimedSceneLoader : MonoBehaviour
{
    // Tên của scene sẽ được load, có thể chỉnh trong Inspector
    public string sceneNameToLoad = "MainMenu";

    // Thời gian chờ (tính bằng giây) trước khi chuyển cảnh
    public float delayInSeconds = 4f;

    // Start is called before the first frame update
    void Start()
    {
        // Gọi hàm "LoadMainMenuScene" sau một khoảng thời gian delayInSeconds
        Invoke("LoadMainMenuScene", delayInSeconds);
    }

    // Hàm để load scene
    void LoadMainMenuScene()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }
}