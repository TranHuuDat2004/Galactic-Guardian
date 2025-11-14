using UnityEngine;
using UnityEngine.SceneManagement; // Thêm thư viện này để quản lý scene
using UnityEngine.Video;           // Thêm thư viện này để làm việc với VideoPlayer

public class IntroController : MonoBehaviour
{
    // Tên của scene Main Menu, có thể thay đổi trong Inspector
    public string sceneNameToLoad = "SampleScene";

    private VideoPlayer videoPlayer;

    void Awake()
    {
        // Lấy component VideoPlayer từ chính GameObject này (Main Camera)
        videoPlayer = GetComponent<VideoPlayer>();
    }

    void Start()
    {
        // Đăng ký một hàm để được gọi khi video kết thúc
        // loopPointReached là sự kiện được kích hoạt khi video phát đến cuối
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    // Hàm này sẽ được gọi khi sự kiện loopPointReached xảy ra
    void OnVideoFinished(VideoPlayer vp)
    {
        // Load scene Main Menu
        SceneManager.LoadScene(sceneNameToLoad);
    }
}