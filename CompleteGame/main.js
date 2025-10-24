// Chạy code sau khi toàn bộ trang đã được tải
document.addEventListener('DOMContentLoaded', function() {

    // --- PHẦN 1: TẢI HEADER VÀ FOOTER TỰ ĐỘNG ---
    const headerPlaceholder = document.querySelector('header.header-placeholder');
    const footerPlaceholder = document.querySelector('footer.footer-placeholder');

    // Tải header.html
    if (headerPlaceholder) {
        fetch('header.html')
            .then(response => response.text())
            .then(data => {
                headerPlaceholder.innerHTML = data;

                // SAU KHI TẢI HEADER XONG, GÁN SỰ KIỆN CHO NÚT HAMBURGER
                const navToggle = document.getElementById('nav-toggle');
                const navMenu = document.querySelector('.nav-menu');

                if (navToggle && navMenu) {
                    navToggle.addEventListener('click', () => {
                        navMenu.classList.toggle('active');
                        navToggle.classList.toggle('is-active');
                    });
                }
            })
            .catch(error => console.error('Error loading header:', error));
    }

    // Tải footer.html
    if (footerPlaceholder) {
        fetch('footer.html')
            .then(response => response.text())
            .then(data => {
                footerPlaceholder.innerHTML = data;
            })
            .catch(error => console.error('Error loading footer:', error));
    }


    // --- PHẦN 2: HIỆU ỨNG HIỆN DẦN KHI CUỘN TRANG (CHO TRANG CHỦ) ---
    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (entry.isIntersecting) {
                entry.target.classList.add('show');
            }
        });
    }, {
        threshold: 0.1
    });

    const hiddenElements = document.querySelectorAll('.hidden');
    hiddenElements.forEach((el) => observer.observe(el));
});