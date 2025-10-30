using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    // Các file âm thanh bạn muốn quản lý
    public AudioClip playerExplosionSound;
    public AudioClip enemyExplosionSound;
    // Bạn có thể thêm các âm thanh khác ở đây sau này (ví dụ: nhặt power-up)

    // Component "Loa" để phát âm thanh
    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
        
        // Tự động thêm AudioSource nếu chưa có
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Quan trọng: Đảm bảo âm thanh hiệu ứng là 2D và không bị ảnh hưởng bởi vị trí
        audioSource.spatialBlend = 0; 
    }

    // Hàm để phát một âm thanh cụ thể
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            // PlayOneShot cho phép phát nhiều âm thanh chồng lên nhau,
            // rất phù hợp cho các vụ nổ liên tiếp.
            audioSource.PlayOneShot(clip);
        }
    }
}