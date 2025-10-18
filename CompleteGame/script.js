// --- JAVASCRIPT CHO CÁC HIỆU ỨNG TRANG WEB ---

// Chạy code sau khi toàn bộ trang đã được tải
document.addEventListener('DOMContentLoaded', function() {

    // 1. Hiệu ứng hiện dần khi cuộn trang
    // ------------------------------------
    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (entry.isIntersecting) {
                entry.target.classList.add('show');
            } else {
                // Bạn có thể bỏ bình luận dòng dưới nếu muốn hiệu ứng lặp lại mỗi khi cuộn lên/xuống
                // entry.target.classList.remove('show'); 
            }
        });
    }, {
        threshold: 0.1 // Kích hoạt khi 10% của phần tử hiện ra
    });

    const hiddenElements = document.querySelectorAll('.hidden');
    hiddenElements.forEach((el) => observer.observe(el));


    // 2. Hiệu ứng Parallax cho Header
    // ---------------------------------
    const header = document.getElementById('home');
    // Chỉ thêm hiệu ứng này nếu có header (để tránh lỗi ở các trang con)
    if (header) {
        // Chúng ta cần tạo các layer parallax bằng JavaScript để code HTML gọn gàng hơn
        const stars = document.createElement('div');
        stars.className = 'parallax-bg';
        stars.id = 'bg-stars';
        stars.style.backgroundImage = "url('https://i.imgur.com/k2o2y3X.png')"; // URL ảnh sao
        
        const nebula = document.createElement('div');
        nebula.className = 'parallax-bg';
        nebula.id = 'bg-nebula';
        nebula.style.backgroundImage = "url('https://i.imgur.com/uGNA28H.png')"; // URL ảnh tinh vân
        nebula.style.opacity = '0.6';

        header.prepend(nebula); // Thêm nebula vào trước
        header.prepend(stars);  // Thêm sao vào trước cùng (để nó nằm sau nebula)

        window.addEventListener('scroll', function() {
            let scrollValue = window.scrollY;
            // Di chuyển các lớp nền với tốc độ khác nhau
            stars.style.transform = 'translateY(' + scrollValue * 0.5 + 'px)';
            nebula.style.transform = 'translateY(' + scrollValue * 0.2 + 'px)';
        });
    }
});