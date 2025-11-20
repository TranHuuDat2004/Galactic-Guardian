using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    // --- THÊM MỚI Ở ĐÂY: Tạo Singleton ---
    public static BackgroundScroller Instance; // Biến tĩnh để các script khác có thể truy cập
    // ------------------------------------
    
    [Tooltip("Tốc độ cuộn của hình nền.")]
    public float scrollSpeed = 0.01f; // Tốc độ mặc định khi bắt đầu

    private Renderer rend;

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

// Đặt đoạn code này vào hàm Start() của bạn
void Start()
{
    rend = GetComponent<Renderer>();

    // --- PHẦN CODE TỰ ĐỘNG THAY ĐỔI KÍCH THƯỚC BACKGROUND ---
    if (Camera.main == null)
    {
        Debug.LogError("Không tìm thấy Main Camera trong scene!");
        return;
    }

    if (!Camera.main.orthographic)
    {
        Debug.LogWarning("Camera không ở chế độ Orthographic. Đoạn code này được tối ưu cho Orthographic.");
        // Bạn có thể thêm logic cho camera Perspective ở đây nếu cần
        return;
    }
    
    // Lấy chiều cao của khung nhìn camera
    float cameraHeight = Camera.main.orthographicSize * 2;
    // Dựa vào chiều cao và tỷ lệ khung hình để tính ra chiều rộng
    float cameraWidth = cameraHeight * Camera.main.aspect;

    // Lấy kích thước gốc của texture từ material
    Vector2 textureSize = new Vector2(rend.material.mainTexture.width, rend.material.mainTexture.height);
    
    // Tính toán tỷ lệ của texture (rộng / cao)
    float textureAspectRatio = textureSize.x / textureSize.y;

    // Lấy scale hiện tại của đối tượng
    Vector3 newScale = transform.localScale;

    // So sánh tỷ lệ của camera và của texture
    if (Camera.main.aspect >= textureAspectRatio)
    {
        // TRƯỜNG HỢP 1: Màn hình RỘNG hơn hoặc bằng texture (trường hợp của bạn: 3440x1440)
        // -> Ta cần scale background để chiều rộng của nó vừa với chiều rộng màn hình.
        newScale.x = cameraWidth;
        // Scale chiều cao theo tỷ lệ của texture để không bị méo hình.
        newScale.y = cameraWidth / textureAspectRatio; 
    }
    else
    {
        // TRƯỜNG HỢP 2: Màn hình CAO hơn texture
        // -> Ta cần scale background để chiều cao của nó vừa với chiều cao màn hình.
        newScale.y = cameraHeight;
        // Scale chiều rộng theo tỷ lệ của texture.
        newScale.x = cameraHeight * textureAspectRatio;
    }
    
    // Áp dụng scale mới cho đối tượng background
    transform.localScale = newScale;
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