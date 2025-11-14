using UnityEngine;


[RequireComponent(typeof(ParticleSystem))]
public class SelfDestructParticle : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (ps && !ps.IsAlive())
        {
            // Khi hệ thống hạt đã phát xong, trả nó về Pooler
            // hoặc hủy nó nếu không dùng Pooler cho hiệu ứng.
            gameObject.SetActive(false); // Giả sử bạn sẽ thêm nó vào Pooler
        }
    }
}