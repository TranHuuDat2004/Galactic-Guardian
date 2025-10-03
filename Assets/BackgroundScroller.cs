using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    // --- THÊM MỚI Ở ĐÂY: Tạo Singleton ---
    public static BackgroundScroller Instance; // Biến tĩnh để các script khác có thể truy cập
    // ------------------------------------
    
    [Tooltip("Tốc độ cuộn của hình nền.")]
    public float scrollSpeed = 0.01f; // Tốc độ mặc định khi bắt đầu

    private Renderer rend;

    // --- BỎ BIẾN savedOffset ĐI, KHÔNG CẦN NỮA ---

    void Awake()
    {
        // --- THÊM MỚI Ở ĐÂY: Thiết lập Singleton ---
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Đảm bảo chỉ có 1 BackgroundScroller duy nhất
        }
        // ------------------------------------------
    }

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        // Tính toán và áp dụng độ dịch chuyển
        // Không có gì thay đổi trong hàm này
        float y = Mathf.Repeat(Time.time * scrollSpeed, 1);
        Vector2 offset = new Vector2(0, y);
        rend.material.mainTextureOffset = offset;
    }

    // --- HÀM MỚI ĐỂ CÁC SCRIPT KHÁC CÓ THỂ GỌI ---
    public void SetScrollSpeed(float newSpeed)
    {
        scrollSpeed = newSpeed;
    }
    // -------------------------------------------
}