using UnityEngine;

/// <summary>
/// Script này chứa các ID (dạng số) của các tham số trong Animator Controller.
/// Việc chuyển đổi tên tham số (dạng chữ) sang ID một lần duy nhất lúc bắt đầu
/// sẽ giúp cải thiện hiệu năng so với việc dùng tên chữ mỗi frame.
/// </summary>
public class AnimatorIDs : MonoBehaviour
{
    // Biến để lưu ID của tham số "Speed"
    public int speedFloat;

    // Biến để lưu ID của tham số "Direction"
    public int directionFloat;

    void Awake()
    {
        // Hàm Awake() được gọi khi script được tải.
        // Chúng ta thực hiện việc chuyển đổi ở đây để đảm bảo các ID đã sẵn sàng
        // trước khi các script khác cần dùng đến chúng trong hàm Start().

        // Chuyển đổi chuỗi "Speed" thành một ID số nguyên và lưu lại.
        speedFloat = Animator.StringToHash("Speed");

        // Chuyển đổi chuỗi "Direction" thành một ID số nguyên và lưu lại.
        directionFloat = Animator.StringToHash("Direction");
    }
}