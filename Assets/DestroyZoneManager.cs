using UnityEngine;

public class DestroyZoneManager : MonoBehaviour
{
    public static DestroyZoneManager Instance;

    // --- THAY ĐỔI: Không cần [SerializeField] nữa ---
    // Các biến này sẽ được tự động tìm kiếm
    private GameObject topZone;
    private GameObject bottomZone;
    private GameObject leftZone;
    private GameObject rightZone;
    // ---------------------------------------------

    private void Awake()
    {
        Instance = this;

        // --- LOGIC TỰ ĐỘNG TÌM KIẾM ---
        // Script sẽ tìm các GameObject con có tên tương ứng
        // transform.Find("Tên") sẽ tìm con trực tiếp
        Transform topTransform = transform.Find("Top");
        if (topTransform != null) topZone = topTransform.gameObject;
        
        Transform bottomTransform = transform.Find("Bottom");
        if (bottomTransform != null) bottomZone = bottomTransform.gameObject;
        
        Transform leftTransform = transform.Find("Left");
        if (leftTransform != null) leftZone = leftTransform.gameObject;
        
        Transform rightTransform = transform.Find("Right");
        if (rightTransform != null) rightZone = rightTransform.gameObject;
        
        // In ra cảnh báo nếu không tìm thấy để dễ dàng debug
        if (topZone == null) Debug.LogError("DestroyZoneManager không tìm thấy GameObject con tên là 'Top'!");
        if (bottomZone == null) Debug.LogError("DestroyZoneManager không tìm thấy GameObject con tên là 'Bottom'!");
        if (leftZone == null) Debug.LogError("DestroyZoneManager không tìm thấy GameObject con tên là 'Left'!");
        if (rightZone == null) Debug.LogError("DestroyZoneManager không tìm thấy GameObject con tên là 'Right'!");
        // ------------------------------------
    }

    public void ConfigureZones(bool topActive, bool bottomActive, bool leftActive, bool rightActive)
    {
        if (topZone != null) topZone.SetActive(topActive);
        if (bottomZone != null) bottomZone.SetActive(bottomActive);
        if (leftZone != null) leftZone.SetActive(leftActive);
        if (rightZone != null) rightZone.SetActive(rightActive);
    }
}