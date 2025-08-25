using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f; // Tốc độ di chuyển của tàu
    public GameObject bulletPrefab; // "Khuôn" đạn để bắn ra
    public Transform firePoint; // Vị trí nòng súng nơi đạn sẽ xuất hiện

    // Update is called once per frame
    void Update()
    {
        // 1. XỬ LÝ DI CHUYỂN
        // Lấy giá trị đầu vào từ trục ngang (phím A, D hoặc mũi tên trái, phải)
        float horizontalInput = Input.GetAxis("Horizontal");

        // Di chuyển tàu dựa trên đầu vào
        // Vector3.right là di chuyển theo trục x
        // Time.deltaTime giúp di chuyển mượt mà, không phụ thuộc vào tốc độ máy
        transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime);


        // 2. XỬ LÝ BẮN ĐẠN
        // Nếu người chơi nhấn nút Space (phím cách)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    // Hàm để bắn đạn
    void Shoot()
    {
        // Tạo ra một bản sao của bulletPrefab tại vị trí và hướng của firePoint
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}