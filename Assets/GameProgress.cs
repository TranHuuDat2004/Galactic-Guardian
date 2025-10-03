using UnityEngine;

public static class GameProgress
{
    private const string CurrentLevelKey = "CurrentLevel";

    // Lưu màn chơi hiện tại
    public static void SaveLevel(int level)
    {
        // PlayerPrefs là một cách đơn giản để lưu dữ liệu nhỏ trên máy người chơi
        PlayerPrefs.SetInt(CurrentLevelKey, level);
        PlayerPrefs.Save(); // Đảm bảo dữ liệu được ghi xuống đĩa
    }

    // Tải màn chơi đã lưu
    public static int LoadLevel()
    {
        // Nếu chưa có dữ liệu lưu, mặc định là màn 1
        return PlayerPrefs.GetInt(CurrentLevelKey, 1);
    }

    // (Tùy chọn) Hàm để reset game
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(CurrentLevelKey);
    }
}