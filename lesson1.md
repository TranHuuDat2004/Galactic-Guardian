# Nhật Ký Học Tập - Bài 1: Khởi Tạo và Thiết Lập Nhân Vật

Hôm nay là buổi học đầu tiên, tập trung vào việc cài đặt, tìm hiểu giao diện và tạo ra đối tượng người chơi cơ bản.

## 1. Những Điều Đã Học Được

### Unity Hub & Cài Đặt
*   **Unity Hub** là công cụ trung tâm để quản lý các phiên bản Unity Editor và tất cả các dự án.
*   Khi cài đặt Editor, nên chọn phiên bản **LTS (Long-Term Support)** vì nó ổn định nhất.
*   Cần cài đặt thêm module **Visual Studio Community** để lập trình C#.

### Giao Diện Unity Editor
*   **Scene View:** "Xưởng làm việc", nơi sắp xếp, di chuyển các đối tượng.
*   **Game View:** Cửa sổ xem trước trò chơi sẽ trông như thế nào khi chạy.
*   **Hierarchy:** Liệt kê tất cả các đối tượng (GameObjects) đang có trong màn chơi.
*   **Project:** "Kho chứa", nơi quản lý tất cả tài nguyên (hình ảnh, âm thanh, code...).
*   **Inspector:** Hiển thị và cho phép chỉnh sửa thuộc tính của đối tượng đang được chọn.

### Quản Lý Tài Nguyên (Assets)
*   Phân biệt được sự khác nhau cơ bản giữa **tài sản 2D và 3D**.
    *   **2D:** Dùng **Sprite Renderer** để hiển thị.
    *   **3D:** Dùng **Mesh Renderer** và **Mesh Filter**.
*   Học được cách **import tài nguyên từ bên ngoài** (ví dụ: Kenney.nl) vào Unity: Tải về -> Giải nén -> Kéo thả thư mục vào cửa sổ **Project**.
*   **Cấu hình hình ảnh thành Sprite:** Sau khi import, phải chọn hình ảnh, vào **Inspector** và đổi **Texture Type** thành **`Sprite (2D and UI)`** rồi nhấn **Apply**. Đây là bước bắt buộc để Unity hiểu đây là hình ảnh 2D.

### GameObject và Components
*   Mọi thứ trong game đều là một **GameObject**.
*   GameObject chỉ là một cái "thùng chứa" rỗng. Các **Component** được thêm vào sẽ quyết định hành vi và hình dạng của nó.
*   Đã tạo thành công GameObject `Player` với các component thiết yếu:
    *   **Transform:** Luôn có sẵn, quyết định vị trí, vòng quay và kích thước.
    *   **Sprite Renderer:** Để hiển thị hình ảnh của con tàu.
    *   **Rigidbody 2D:** Để GameObject có thể tương tác với hệ thống vật lý (di chuyển, va chạm). Đã học cách set **Gravity Scale = 0** để tàu không bị rơi.
    *   **Box Collider 2D:** Tạo ra một "vùng va chạm" hình hộp xung quanh đối tượng.
    *   **Script (PlayerController):** Gắn "bộ não" được lập trình bằng C# vào để điều khiển.

## 2. Vấn Đề Gặp Phải & Cách Giải Quyết

*   **Vấn đề:** Tải nhầm gói tài sản 3D, khiến con tàu hiển thị không đúng trong dự án 2D.
*   **Giải quyết:** Nhận biết qua các component `Mesh Renderer`. Đã tìm và tải lại gói tài sản 2D phù hợp (dạng file PNG, có component `Sprite Renderer`).

## 3. Bước Tiếp Theo

*   Tạo đối tượng viên đạn và kẻ thù (gà).
*   Tìm hiểu về **Prefabs** để có thể tái sử dụng các đối tượng này.
*   Hoàn thiện code trong `PlayerController` để thực hiện chức năng bắn đạn.
