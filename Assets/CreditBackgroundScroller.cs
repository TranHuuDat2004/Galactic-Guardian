// File: CreditBackgroundScroller.cs
using UnityEngine;

// Bắt buộc đối tượng phải có component Renderer (như Quad hoặc Sprite Renderer)
[RequireComponent(typeof(Renderer))]
public class CreditBackgroundScroller : MonoBehaviour
{
    [Header("Cài Đặt Background")]
    [Tooltip("Kéo MATERIAL của ảnh nền bạn muốn sử dụng vào đây.")]
    public Material backgroundMaterial; // << Chỗ để bạn kéo Material vào

    [Tooltip("Tốc độ cuộn của hình nền.")]
    public float scrollSpeed = 0.01f;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();

        // 1. Kiểm tra và áp dụng Material
        if (backgroundMaterial != null)
        {
            // Gán Material mà bạn đã kéo vào cho đối tượng này
            rend.material = backgroundMaterial;
        }
        else
        {
            Debug.LogError("Vui lòng kéo một Material vào ô 'Background Material' trên script CreditBackgroundScroller!", this.gameObject);
            return; // Dừng lại nếu chưa có material
        }

        // 2. Tự động thay đổi kích thước để vừa với màn hình
        // Logic này được lấy từ script cũ, đảm bảo background luôn lấp đầy camera
        if (Camera.main != null && Camera.main.orthographic)
        {
            float cameraHeight = Camera.main.orthographicSize * 2;
            float cameraWidth = cameraHeight * Camera.main.aspect;

            float textureRatio = (float)rend.material.mainTexture.width / rend.material.mainTexture.height;

            Vector3 newScale = transform.localScale;

            if (Camera.main.aspect >= textureRatio)
            {
                newScale.x = cameraWidth;
                newScale.y = cameraWidth / textureRatio;
            }
            else
            {
                newScale.y = cameraHeight;
                newScale.x = cameraHeight * textureRatio;
            }
            transform.localScale = newScale;
        }
    }

    void Update()
    {
        // Logic cuộn không thay đổi, vẫn hoạt động hoàn hảo
        float y = Mathf.Repeat(Time.time * scrollSpeed, 1);
        Vector2 offset = new Vector2(0, y);
        rend.material.mainTextureOffset = offset;
    }
}