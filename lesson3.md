# Nhật Ký Học Tập - Bài 3: Nâng Cấp Hiệu Ứng (VFX) và Tối Ưu Hóa (Object Pooling)

Buổi học hôm nay tập trung vào việc nâng cấp phần nhìn của game bằng cách tích hợp một gói hiệu ứng hình ảnh (VFX) chuyên nghiệp và chuyển đổi hệ thống sinh sản đối tượng sang Object Pooling để tối ưu hóa hiệu năng.

## 1. Tích Hợp Hiệu Ứng Đạn VFX (Visual Effects)

Game ban đầu sử dụng sprite tĩnh làm đạn, trông khá đơn điệu. Sau khi đầu tư gói asset "Unique Projectiles", mình đã học được quy trình chuyển đổi một Prefab VFX (vốn là 3D) để sử dụng trong game 2D.

### Quy trình "Phẫu Thuật" Prefab VFX:

Đây là các bước bắt buộc phải thực hiện trên một bản sao (Duplicate) của Prefab VFX gốc để không làm hỏng tài sản gốc.

1.  **Gỡ Bỏ Các Component Không Tương Thích:**
    *   **Xóa Script Gốc:** Gỡ bỏ component script điều khiển do tác giả cung cấp (ví dụ: `ProjectileMoveScript`).
    *   **Xóa Component Vật Lý 3D:** Gỡ bỏ các component `Rigidbody` và `Box Collider` (phiên bản 3D, không có chữ "2D").

2.  **Thêm Các Component 2D Tương Thích:**
    *   **`Rigidbody 2D`:** Thêm component này để đạn có thể tương tác với hệ thống vật lý 2D.
        *   **Cài đặt quan trọng:** Đặt **Gravity Scale = 0** để đạn không bị rơi xuống do trọng lực.
    *   **`Collider 2D`:** Thêm một component va chạm 2D (ví dụ `Box Collider 2D` hoặc `Capsule Collider 2D`).
        *   **Cài đặt quan trọng:** Phải **tích vào ô `Is Trigger`** để nó có thể kích hoạt hàm `OnTriggerEnter2D` mà không gây ra các va chạm vật lý (đẩy nhau).
    *   **Thêm Script `Bullet.cs`:** Gắn "bộ não" của chính mình vào để điều khiển logic di chuyển và va chạm.

3.  **Thiết Lập Tag:**
    *   **Cài đặt quan trọng:** Gán **Tag** cho prefab đạn là **`Bullet`**. Đây là bước cực kỳ **hay quên** nhưng lại tối quan trọng, vì nếu thiếu, logic `other.CompareTag("Bullet")` trong script `Enemy` sẽ không bao giờ đúng và kẻ địch sẽ trở nên "bất tử".

## 2. Xử Lý Các Vấn Đề Đồ Họa Của VFX

Khi làm việc với Particle System (3D) trong một dự án 2D URP, một vài lỗi đồ họa đã xảy ra.

*   **Vấn đề:** Hiệu ứng đạn bị hình nền che mất, chỉ hiện ra khi bay gần kẻ địch.
*   **Nguyên nhân:** Xung đột về thứ tự hiển thị giữa hệ thống `Renderer` 3D của Particle System và `Sprite Renderer` 2D.
*   **Giải pháp - Sorting Layers:**
    1.  Tạo một **Sorting Layer** mới tên là `FX`.
    2.  Trong component `Particle System` của prefab đạn, vào mục `Renderer`.
    3.  Thiết lập **Sorting Layer** thành `FX` để đảm bảo nó luôn được vẽ lên trên tất cả các lớp khác.

## 3. Tối Ưu Hóa Bằng Object Pooling

Để tránh việc `Instantiate` và `Destroy` liên tục gây sụt giảm hiệu năng, mình đã xây dựng một hệ thống Object Pooling.

### Cấu Trúc Hệ Thống:
*   **`ObjectPooler.cs`:** Đóng vai trò là "nhà máy", chịu trách nhiệm tạo sẵn (pre-warm) và quản lý các "kho" (pools) chứa đối tượng.
    *   Sử dụng **Singleton Pattern** (`public static ObjectPooler Instance;`) để có thể được gọi dễ dàng từ bất kỳ script nào khác.
    *   Sử dụng **Dictionary** để lưu trữ các kho, giúp truy xuất đối tượng bằng **Tag (string)** một cách nhanh chóng.
    *   Logic khởi tạo kho phải được đặt trong hàm `Awake()` để đảm bảo "nhà máy" luôn sẵn sàng trước khi có "đơn đặt hàng".
*   **`EnemySpawner.cs`:** Đóng vai trò là "khách hàng".
    *   Thay vì dùng `Instantiate`, nó sẽ gọi `ObjectPooler.Instance.SpawnFromPool(tag, ...)` để yêu cầu một đối tượng từ kho.
    *   Cần phải đồng bộ hóa danh sách **`Tags`** trong Inspector của Spawner với các **`Tags`** đã được định nghĩa trong `ObjectPooler`.

### Cập Nhật Logic Script: "Tắt" Thay Vì "Hủy"
*   Để配合 Object Pooling, tất cả các lệnh `Destroy(gameObject)` trong `Bullet.cs` và `Enemy.cs` được thay thế bằng `gameObject.SetActive(false)`. Thao tác này sẽ "trả" đối tượng về kho thay vì phá hủy nó.
*   Hàm `OnEnable()` được thêm vào các script để reset lại trạng thái của đối tượng mỗi khi nó được "mượn" ra khỏi kho.

## 4. Bài Học Rút Ra
*   Luôn kiểm tra kỹ các component trên Prefab. Việc thêm nhầm component 3D vào môi trường 2D (và ngược lại) là một lỗi rất phổ biến.
*   **Tagging is King:** Gán Tag là một bước đơn giản nhưng cực kỳ quan trọng. Quên gán Tag có thể dẫn đến những lỗi logic khó tìm.
*   Hiểu rõ vòng đời script (`Awake()` -> `OnEnable()` -> `Start()`) là chìa khóa để tránh các lỗi `NullReferenceException` khi các script phụ thuộc lẫn nhau.