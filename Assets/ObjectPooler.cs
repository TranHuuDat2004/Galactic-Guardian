using System.Collections.Generic;
using UnityEngine;

// Đây là một lớp tiện ích để cấu hình từng loại object trong Inspector
[System.Serializable]
public class Pool
{
    public string tag;          // Tên để gọi ra, ví dụ: "Bullet", "Enemy"
    public GameObject prefab;   // Prefab của đối tượng
    public int size;            // Số lượng đối tượng tạo sẵn
}

public class ObjectPooler : MonoBehaviour
{
    // Tạo một phiên bản tĩnh (Singleton) để có thể truy cập từ mọi nơi
    public static ObjectPooler Instance;

    public List<Pool> pools; // Danh sách các kho chứa sẽ được cấu hình trong Inspector
    
    // Dictionary để lưu trữ các kho, truy cập bằng tag (string)
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        // Thiết lập Singleton
        Instance = this;

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // Duyệt qua tất cả các "Pool" đã được cấu hình trong Inspector
        foreach (Pool pool in pools)
        {
            // Tạo một hàng đợi (Queue) để chứa các đối tượng
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // Tạo sẵn các đối tượng và đưa vào hàng đợi
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false); // TẮT chúng đi ngay lập tức
                objectPool.Enqueue(obj); // Thêm vào hàng đợi
            }

            // Thêm hàng đợi vừa tạo vào Dictionary với Tag tương ứng
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    void Start()
    {
        
    }

    // Hàm để "mượn" một đối tượng từ kho
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        // Lấy một đối tượng từ đầu hàng đợi
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // BẬT nó lên
        objectToSpawn.SetActive(true);
        // Di chuyển nó đến vị trí và góc quay mong muốn
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Trả đối tượng lại vào cuối hàng đợi để tái sử dụng
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}