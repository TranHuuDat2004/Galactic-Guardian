using UnityEngine;

public class ResetPlayerPrefs : MonoBehaviour
{
    [Tooltip("Tick vào ô này và chạy game MỘT LẦN để xóa toàn bộ dữ liệu lưu.")]
    public bool deleteAllPrefs = false;
    
    void Awake()
    {
        if (deleteAllPrefs)
        {
            PlayerPrefs.DeleteAll();
            Debug.LogWarning("ĐÃ XÓA TOÀN BỘ DỮ LIỆU PLAYERPREFS!");
        }
    }
}