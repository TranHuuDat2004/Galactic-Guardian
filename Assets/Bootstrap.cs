using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    void Start()
    {
        // Tải Scene Menu chính ngay sau khi các Manager đã được khởi tạo
        SceneManager.LoadScene("MainMenu"); // Thay "MainMenu" bằng tên Scene Menu của bạn
    }
}