using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(VideoPlayer))]
public class IntroVideoManager : MonoBehaviour
{
    // Danh sách các video có thể được chọn
    public List<string> videoFileNames;
    
    // Scene tiếp theo
    public string nextSceneName = "MainMenu";

    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        // Đăng ký sự kiện: Khi video kết thúc, gọi hàm LoadNextScene
        videoPlayer.loopPointReached += LoadNextScene;
        
        // Bắt đầu phát một video ngẫu nhiên
        PlayRandomVideo();
    }

    void PlayRandomVideo()
    {
        // Kiểm tra xem có video nào trong danh sách không
        if (videoFileNames == null || videoFileNames.Count == 0)
        {
            Debug.LogWarning("Không có video nào trong danh sách, chuyển cảnh ngay.");
            LoadNextScene(videoPlayer); // Truyền tham số để khớp với sự kiện
            return;
        }

        // Chọn ngẫu nhiên một video từ danh sách
        int randomIndex = Random.Range(0, videoFileNames.Count);
        string randomVideoName = videoFileNames[randomIndex];

        // Thiết lập URL và phát
        videoPlayer.source = VideoSource.Url;
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, randomVideoName);
        videoPlayer.url = videoPath;
        videoPlayer.Play();
        
        Debug.Log("Đang phát video ngẫu nhiên: " + randomVideoName);
    }

    // Hàm này được gọi khi video kết thúc
    // Nó cần có một tham số VideoPlayer để khớp với sự kiện loopPointReached
    void LoadNextScene(VideoPlayer vp)
    {
        // Hủy đăng ký sự kiện để tránh lỗi khi tải scene mới
        vp.loopPointReached -= LoadNextScene;
        // Tải Scene tiếp theo
        SceneManager.LoadScene(nextSceneName);
    }
}