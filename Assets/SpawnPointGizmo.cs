using UnityEngine;

public class SpawnPointGizmo : MonoBehaviour
{
    // Bạn có thể thay đổi màu và kích thước trong Inspector
    public Color gizmoColor = Color.yellow;
    public float gizmoRadius = 0.3f;

    // Hàm này chỉ chạy trong Editor, không chạy trong game thật
    private void OnDrawGizmos()
    {
        // Đặt màu cho Gizmo
        Gizmos.color = gizmoColor;

        // Vẽ một hình cầu tại vị trí của GameObject này
        Gizmos.DrawSphere(transform.position, gizmoRadius);
    }
}