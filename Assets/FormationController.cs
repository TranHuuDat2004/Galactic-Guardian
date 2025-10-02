using UnityEngine;

public class FormationController : MonoBehaviour
{
    [Header("Di Chuyển Ngang (Side-to-Side)")]
    public float horizontalSpeed = 2.0f;
    public float horizontalWidth = 5.0f;

    [Header("Di Chuyển Dọc (Downward)")]
    public float verticalSpeed = 1.0f; // << TỐC ĐỘ ĐI XUỐNG MỚI

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // 1. Tính toán di chuyển ngang theo hình sin
        float newX = startPosition.x + Mathf.Sin(Time.time * horizontalSpeed) * horizontalWidth;
        
        // 2. Cập nhật vị trí X và giữ nguyên Y, Z ban đầu
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        // 3. Áp dụng di chuyển đi xuống liên tục
        transform.Translate(Vector3.down * verticalSpeed * Time.deltaTime);
    }
}