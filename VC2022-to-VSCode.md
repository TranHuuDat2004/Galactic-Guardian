Bạn chỉ cần làm theo 2 phần chính sau:
1.  **Phần 1:** Chỉ cho Unity biết nơi để mở VS Code.
2.  **Phần 2 (Rất quan trọng):** Cài đặt các công cụ cần thiết để VS Code "hiểu" được code Unity và có thể gợi ý code (IntelliSense).

---

### Phần 1: Thay đổi Trình Soạn Thảo Mặc Định

Đây là cách bạn "bảo" Unity hãy dùng VS Code mỗi khi bạn nhấp đúp vào một file script.

1.  Trong Unity Editor, mở menu **Edit** trên thanh công cụ trên cùng.
2.  Chọn **Preferences...** (ở gần cuối danh sách).
    *   *Lưu ý: Nếu bạn dùng macOS, đường dẫn sẽ là **Unity -> Settings...***
3.  Một cửa sổ mới sẽ hiện ra. Ở danh sách bên trái, hãy chọn tab **External Tools**.
4.  Ở mục **External Script Editor**, bạn sẽ thấy một danh sách thả xuống (dropdown). Hãy nhấp vào nó.
5.  **Chọn Visual Studio Code** từ danh sách.
    *   **Nếu không thấy VS Code trong danh sách?** Đừng lo, hãy chọn mục **Browse...**. Một cửa sổ file sẽ hiện ra, bạn hãy tìm đến nơi đã cài đặt VS Code và chọn file thực thi của nó. Trên Windows, đường dẫn mặc định thường là:
        `C:\Users\[Tên người dùng]\AppData\Local\Programs\Microsoft VS Code\Code.exe`
6.  (Tùy chọn nhưng nên làm) Sau khi chọn xong, nhấn vào nút **Regenerate project files** ở ngay bên dưới. Nút này sẽ đảm bảo các file cấu hình của project được cập nhật cho VS Code.



Bây giờ, khi bạn nhấp đúp vào một file C# trong Unity, nó sẽ tự động mở bằng VS Code. Nhưng khoan, chúng ta vẫn chưa xong!

---

### Phần 2: Để VS Code "Hiểu" Được Unity (Fix Lỗi Gợi Ý Code)

Nếu bạn chỉ làm Phần 1, VS Code sẽ chỉ là một trình soạn thảo văn bản thông thường. Nó sẽ không tự động gợi ý code (`transform`, `GetComponent`, `Vector3`...) và sẽ báo lỗi lung tung. Để sửa lỗi này, bạn cần cài đặt gói công cụ của Unity.

1.  Trong Unity Editor, mở menu **Window** trên thanh công cụ.
2.  Chọn **Package Manager**.
3.  Trong cửa sổ Package Manager, ở góc trên bên trái, hãy nhấp vào nút **"Packages: In Project"** và đổi nó thành **"Packages: Unity Registry"**. Việc này để hiển thị tất cả các gói chính thức của Unity.
4.  Sử dụng thanh tìm kiếm ở góc trên bên phải, gõ vào **`Visual Studio Code Editor`**.
5.  Chọn gói có tên **"Visual Studio Code Editor"** từ kết quả tìm kiếm.
6.  Ở góc dưới bên phải cửa sổ, nhấn nút **Install** (hoặc **Update** nếu nó đã được cài nhưng là phiên bản cũ).
7.  Chờ Unity cài đặt xong.



### Bước cuối cùng: Cài Extension C# trong VS Code

Để hoàn tất, bạn cần đảm bảo VS Code có công cụ để làm việc với ngôn ngữ C#.

1.  Mở **Visual Studio Code**.
2.  Nhấn vào biểu tượng **Extensions** ở thanh công cụ bên trái (trông giống 4 ô vuông).
3.  Trong thanh tìm kiếm, gõ **`C#`**.
4.  Tìm extension có tên **"C#"** của nhà phát hành **Microsoft** và nhấn **Install**.

