// Chạy code sau khi toàn bộ trang đã được tải
document.addEventListener('DOMContentLoaded', function () {

    // 1. Hiệu ứng hiện dần khi cuộn trang
    // ------------------------------------
    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (entry.isIntersecting) {
                entry.target.classList.add('show');
            } else {
                // Bạn có thể bỏ comment dòng dưới nếu muốn hiệu ứng lặp lại mỗi khi cuộn lên/xuống
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
const homeSection = document.getElementById('home');

if (homeSection) {
    // ---- Cấu hình cho ảnh SAO ----
    const stars = document.createElement('div');
    stars.className = 'parallax-bg';
    stars.style.backgroundImage = "url('img/stars-background.jpg')";
    
    // THÊM 2 DÒNG NÀY ĐỂ SỬA LỖI LẶP ẢNH
    stars.style.backgroundRepeat = 'no-repeat';
    stars.style.backgroundSize = 'cover';
    
    stars.style.position = 'absolute';
    stars.style.top = 0;
    stars.style.left = 0;
    stars.style.width = '100%';
    stars.style.height = '100%';
    stars.style.zIndex = '-2';

    // ---- Cấu hình cho ảnh TINH VÂN ----
    const nebula = document.createElement('div');
    nebula.className = 'parallax-bg';
    nebula.style.backgroundImage = "url('img/nebula-background.jpg')";

    // THÊM 2 DÒNG NÀY ĐỂ SỬA LỖI LẶP ẢNH
    nebula.style.backgroundRepeat = 'no-repeat';
    nebula.style.backgroundSize = 'cover';

    nebula.style.position = 'absolute';
    nebula.style.top = 0;
    nebula.style.left = 0;
    nebula.style.width = '100%';
    nebula.style.height = '100%';
    nebula.style.zIndex = '-1';
    nebula.style.opacity = '0.6';

    // ---- Thêm ảnh vào trang và gán hiệu ứng cuộn ----
    homeSection.prepend(nebula);
    homeSection.prepend(stars);

    window.addEventListener('scroll', function () {
        let scrollValue = window.scrollY;
        stars.style.transform = 'translateY(' + scrollValue * 0.5 + 'px)';
        nebula.style.transform = 'translateY(' + scrollValue * 0.2 + 'px)';
    });
}

    // 3. CODE MỚI: Xử lý cho menu di động
    // ------------------------------------
    const navToggle = document.getElementById('nav-toggle');
    const navMenu = document.querySelector('.nav-menu');

    if (navToggle && navMenu) {
        navToggle.addEventListener('click', () => {
            navMenu.classList.toggle('active');
            navToggle.classList.toggle('is-active');
        });

        // Đóng menu khi click vào một link (để chuyển trang)
        document.querySelectorAll('.nav-link').forEach(link => {
            link.addEventListener('click', () => {
                if (navMenu.classList.contains('active')) {
                    navMenu.classList.remove('active');
                    navToggle.classList.remove('is-active');
                }
            });
        });
    }
});