using UnityEngine;

public class CustomCursorController : MonoBehaviour
{
    private RectTransform cursorTransform;

    void Start()
    {
        // Lấy component RectTransform của chính nó
        cursorTransform = GetComponent<RectTransform>();

        // Ẩn con trỏ chuột thật của hệ thống
        Cursor.visible = false;
    }

    void Update()
    {
        // Cập nhật vị trí của Image theo vị trí của con trỏ chuột thật
        cursorTransform.position = Input.mousePosition;
    }

    // Khi đối tượng này bị hủy (ví dụ: chuyển sang Scene game),
    // hãy hiện lại con trỏ chuột thật để tránh bị mất con trỏ.
    private void OnDestroy()
    {
        Cursor.visible = true;
    }
}