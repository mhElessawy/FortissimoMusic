// Navbar scroll effect
window.addEventListener('scroll', function () {
    const nav = document.getElementById('mainNav');
    if (nav) {
        if (window.scrollY > 50) {
            nav.classList.add('scrolled');
        } else {
            nav.classList.remove('scrolled');
        }
    }
});

// Set active nav link
(function () {
    const path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.navbar-nav .nav-link').forEach(link => {
        const href = link.getAttribute('href')?.toLowerCase() || '';
        if (href && href !== '/' && path.includes(href.replace(/^\//, ''))) {
            link.classList.add('active');
        } else if (href === '/' && path === '/') {
            link.classList.add('active');
        }
    });
})();

// Animate elements on scroll
function revealOnScroll() {
    const elements = document.querySelectorAll('.vision-card, .event-card, .video-card, .stat-item');
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, { threshold: 0.1 });

    elements.forEach(el => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(20px)';
        el.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(el);
    });
}

document.addEventListener('DOMContentLoaded', revealOnScroll);

// Hero carousel initialization
document.addEventListener('DOMContentLoaded', function () {
    var carouselEl = document.getElementById('heroCarousel');
    if (carouselEl && typeof bootstrap !== 'undefined') {
        var carousel = new bootstrap.Carousel(carouselEl, {
            interval: 5000,
            ride: 'carousel',
            wrap: true,
            touch: true
        });

        var prevBtn = carouselEl.querySelector('.carousel-control-prev');
        var nextBtn = carouselEl.querySelector('.carousel-control-next');

        if (prevBtn) {
            prevBtn.addEventListener('click', function (e) {
                e.preventDefault();
                e.stopPropagation();
                carousel.prev();
            });
        }
        if (nextBtn) {
            nextBtn.addEventListener('click', function (e) {
                e.preventDefault();
                e.stopPropagation();
                carousel.next();
            });
        }
    }
});
