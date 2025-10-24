// Chạy code sau khi toàn bộ trang đã được tải
document.addEventListener('DOMContentLoaded', function() {

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


    // 2. Hiệu ứng Parallax cho Header (Giữ nguyên)
    // ---------------------------------
    const header = document.getElementById('home');
    if (header) {
        const stars = document.createElement('div');
        stars.className = 'parallax-bg';
        stars.style.backgroundImage = "url('https://i.imgur.com/k2o2y3X.png')";
        stars.style.position = 'absolute';
        stars.style.top = 0;
        stars.style.left = 0;
        stars.style.width = '100%';
        stars.style.height = '100%';
        stars.style.zIndex = '-2';
        
        const nebula = document.createElement('div');
        nebula.className = 'parallax-bg';
        nebula.style.backgroundImage = "url('https://i.imgur.com/uGNA28H.png')";
        nebula.style.position = 'absolute';
        nebula.style.top = 0;
        nebula.style.left = 0;
        nebula.style.width = '100%';
        nebula.style.height = '100%';
        nebula.style.zIndex = '-1';
        nebula.style.opacity = '0.6';

        header.prepend(nebula);
        header.prepend(stars);

        window.addEventListener('scroll', function() {
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