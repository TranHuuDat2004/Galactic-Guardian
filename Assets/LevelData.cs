// Trong file LevelData.cs
using UnityEngine;
using System.Collections.Generic;

// Định nghĩa lớp Wave ở đây để các script khác có thể dùng
[System.Serializable]
public class Wave
{
    public string waveName;
    public GameObject formationPrefab;
    public string enemyTagToSpawn;

    [Header("Tùy Chọn Cảnh Báo & Vị Trí")]
    public GameObject warningImagePrefab;
    public Vector3 warningPosition;
    public Vector3 enemyStartPosition;
    public float warningDelay = 2.0f;

 [Header("Thiết Lập Destroy Zone")]
    [Tooltip("Gõ vào TÊN của GameObject cấu hình DestroyZone trong Hierarchy.")]
    public string destroyZoneName; // << CHỈ CẦN DÙNG STRING
}

// Dòng này cho phép bạn tạo file asset từ script
[CreateAssetMenu(fileName = "New Level", menuName = "Game Data/Level")]
public class LevelData : ScriptableObject
{
    [Header("Thông Tin Màn Chơi")]
    public string levelName;

    [Header("Cốt Truyện")]
    [Tooltip("Nội dung cốt truyện sẽ hiển thị trước khi bắt đầu màn chơi này.")]
    [TextArea(10, 20)] // Giúp ô nhập liệu trong Inspector lớn hơn, dễ gõ hơn
    public string storyText;
    
    [Header("Thiết Lập Chung")]
    public GameObject backgroundPrefab; // << THAY ĐỔI Ở ĐÂY
    public AudioClip backgroundMusic; // Nhạc nền cho màn này
    
    [Header("Danh Sách Các Wave")]
    public List<Wave> waves; // Danh sách các wave của riêng màn này
}