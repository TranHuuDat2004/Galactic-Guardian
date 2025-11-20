# 🚀 Galactic Guardian (Vệ Binh Ngân Hà) - A Unity Arcade Shooter Project

Chào mừng đến với **Galactic Guardian**, một dự án game hành động arcade với tiết tấu nhanh, kết hợp giữa sự hồi hộp của việc né tránh và sự thỏa mãn khi quét sạch kẻ thù. Người chơi sẽ điều khiển một phi thuyền tối tân, với nhiệm vụ bảo vệ Trái Đất khỏi một hạm đội xâm lăng ngoài hành tinh và trận mưa thiên thạch dày đặc.

Dự án này bắt đầu như một bài tập làm quen với Unity, nhưng giờ đây đã phát triển thành một tầm nhìn lớn hơn, tập trung vào việc xây dựng các hệ thống gameplay có chiều sâu và trải nghiệm co-op hấp dẫn.

<!-- THAY THẾ BẰNG GIF GAMEPLAY CỦA BẠN KHI CÓ THỂ -->
<!-- Bạn có thể dùng các phần mềm như ScreenToGif để quay lại một đoạn gameplay ngắn -->
![Gameplay GIF](link-to-your-gameplay.gif) 

---

## 🌌 Game Concept & Vision

**Galactic Guardian** không chỉ là một game bắn súng thông thường. Tầm nhìn của dự án là tạo ra một trải nghiệm arcade có thể chơi lại nhiều lần, với các thử thách liên tục thay đổi và đặc biệt tỏa sáng khi chơi cùng với bạn bè.

### Các Tính Năng Cốt Lõi

*   **Lối chơi Arcade kinh điển:** Dễ làm quen nhưng khó thành thạo.
*   **Hệ thống kẻ thù thông minh:** Mỗi kẻ địch có hành vi và chiến thuật riêng, đòi hỏi người chơi phải thích ứng.
*   **Nâng cấp đa dạng:** Các loại đạn độc đáo thay đổi hoàn toàn cách tiếp cận trận chiến.
*   **Các trận đấu trùm hoành tráng:** Những kẻ địch khổng lồ với nhiều giai đoạn tấn công phức tạp.
*   **Chế độ Co-op Online:** Hợp tác cùng một người bạn để bảo vệ ngân hà.

---

<details>
<summary><strong>🤝 Chế Độ Chơi Online: Hợp Tác Bảo Vệ Thiên Hà</strong></summary>
<br>

Phiên bản mở rộng sẽ giới thiệu chế độ chơi online, cho phép người chơi kết nối và chiến đấu cùng bạn bè.

#### **Chế độ Co-op (2 người):**

*   **Nhiệm vụ:** Hai người chơi hợp tác để hoàn thành các màn chơi và đánh bại boss. Độ khó sẽ được điều chỉnh tăng lên để phù hợp với số lượng người chơi.
*   **Cơ chế:** Cả hai phi thuyền sẽ chia sẻ chung một màn hình. Khi một người chơi bị tiêu diệt, người còn lại có thể "hồi sinh" đồng đội bằng cách bay đến vị trí đó và giữ chuột trong vài giây.

#### **Hiệu ứng hợp tác:**

*   **Lá chắn Liên kết:** Khi hai phi thuyền đến gần nhau, chúng có thể kích hoạt một lá chắn chung, giúp cả hai chống lại sát thương trong vài giây.
*   **Kết hợp Sức mạnh:** Kết hợp các loại đạn nâng cấp để tạo ra những hiệu ứng mới mạnh hơn. Ví dụ: một người dùng đạn lửa, người kia dùng đạn điện, khi bắn cùng lúc sẽ tạo ra những vụ nổ lớn gây sát thương trên diện rộng.

</details>

---

<details>
<summary><strong>👾 Hệ Thống Kẻ Thù Nâng Cấp: AI và Kỹ Năng Độc Đáo</strong></summary>
<br>

Mỗi loại quái và boss giờ đây sẽ có mô tả chi tiết và hành vi riêng biệt, tạo ra thử thách đa dạng hơn cho người chơi.

### Quái (Enemies)

1.  **Tàu trinh sát "Mantis":**
    *   **Mô tả:** Một phi cơ nhỏ, tốc độ cao, di chuyển zic-zac khó đoán.
    *   **AI:** Ưu tiên né đạn của người chơi. Thường di chuyển theo nhóm, cố gắng bao vây và xả đạn từ nhiều phía.

2.  **Tàu hạng nặng "Crusher":**
    *   **Mô tả:** Tàu địch lớn, bọc giáp dày. Tốc độ di chuyển chậm nhưng hỏa lực mạnh.
    *   **AI:** Cố định vị trí và bắn những viên đạn lớn. Khi máu giảm xuống dưới 50%, nó sẽ phóng ra các tàu con tự sát.

3.  **Kẻ gây nhiễu "Specter":**
    *   **Mô tả:** Một chiếc tàu địch gần như vô hình. Không tấn công trực tiếp.
    *   **AI:** Khi bị tiêu diệt, nó sẽ tạo ra một vùng nhiễu sóng làm chậm tốc độ di chuyển và tốc độ bắn của người chơi.

### Trùm (Boss)

1.  **"Oblivion Core" (Lõi Hủy Diệt):**
    *   **Mô tả:** Một khối cầu kim loại khổng lồ được bảo vệ bởi một lá chắn năng lượng xoay quanh.
    *   **AI & Kiểu tấn công:**
        *   **Giai đoạn 1:** Phóng ra các tia laser xoay tròn.
        *   **Giai đoạn 2:** Lá chắn tạm thời biến mất, bắn những chùm đạn laze theo hình quạt.
        *   **Giai đoạn 3 (dưới 25% máu):** Hấp thụ thiên thạch để tạo lá chắn. Người chơi phải phá hủy các thiên thạch trước.

</details>

---

<details>
<summary><strong>✨ Hệ Thống Hiệu Ứng Bắn Đạn (Power-ups)</strong></summary>
<br>

Các Power-ups giờ đây sẽ có các hiệu ứng đặc biệt và hình ảnh trực quan hơn.

1.  **Đạn Phân Mảnh (Scatter Shot):**
    *   **Mô tả:** Bắn ra nhiều viên đạn nhỏ tỏa ra theo hình quạt, quét sạch nhiều kẻ thù nhỏ cùng lúc.
    *   **Hiệu ứng hình ảnh:** Hiệu ứng bắn tạo ra các vệt sáng nhỏ, khi trúng địch sẽ nổ ra một luồng sáng chói mắt.

2.  **Tia Laze Hủy Diệt (Laser Beam):**
    *   **Mô tả:** Bắn ra một tia laze mạnh, xuyên thủng mọi kẻ thù trên đường đi của nó.
    *   **Hiệu ứng hình ảnh:** Một dải ánh sáng liên tục, sắc nét. Kẻ địch khi bị chiếu vào sẽ bốc cháy dần trước khi nổ tung.

3.  **Sấm Sét Tích Hợp (Lightning Burst):**
    *   **Mô tả:** Bắn ra một viên đạn điện, tạo ra một luồng sét nhảy sang các kẻ thù gần đó.
    *   **Hiệu ứng hình ảnh:** Một tia sét xanh tím lóe sáng, tạo ra một chuỗi điện giật đẹp mắt giữa các mục tiêu.

</details>

---

## 🛠️ Công Nghệ Sử Dụng

*   **Game Engine:** Unity (Phiên bản 2022.3 LTS hoặc mới hơn)
*   **Ngôn ngữ lập trình:** C#
*   **IDE:** Microsoft Visual Studio Community

## 📊 Tình Trạng Dự Án

*   **Hiện tại:** Đã hoàn thành các cơ chế cốt lõi của một game bắn súng arcade:
    *   Điều khiển tàu vũ trụ.
    *   Hệ thống bắn đạn cơ bản.
    *   Xử lý va chạm.
    *   Tạo kẻ thù di chuyển theo đội hình đơn giản.
*   **Lộ trình phát triển (Roadmap):**
    *   [ ] Triển khai hệ thống kẻ thù nâng cao với AI độc đáo.
    *   [ ] Xây dựng 3 loại Power-ups với hiệu ứng hình ảnh riêng biệt.
    *   [ ] Thiết kế và lập trình trận đấu boss "Oblivion Core".
    *   [ ] Nghiên cứu và tích hợp networking cho chế độ Co-op (sử dụng Mirror hoặc Photon).
    *   [ ] Cải thiện UI/UX và thêm hiệu ứng âm thanh.

## ✍️ Tác giả

*   **Trần Hữu Đạt** - [GitHub của tôi](https://github.com/TranHuuDat2004)
