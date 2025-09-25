// DÒNG NÀY LÀ QUAN TRỌNG NHẤT VÀ PHẢI NẰM Ở TRÊN CÙNG
using UnityEngine; 

public class FormationController : MonoBehaviour
{
    public float speed = 2.0f;
    public float width = 5.0f; // Độ rộng di chuyển qua lại
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Di chuyển đội hình theo hình sin qua lại
        float newX = startPosition.x + Mathf.Sin(Time.time * speed) * width;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}