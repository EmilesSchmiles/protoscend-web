// ============================================================
// PROTOSCEND — app.js
// ============================================================

(function () {
    'use strict';

    const SPLASH_DURATION = 5000;

    function dismissSplash() {
        const splash = document.getElementById('splash-screen');
        const app = document.getElementById('app');
        if (!splash) return;
        splash.classList.add('exiting');
        setTimeout(() => {
            splash.style.display = 'none';
            if (app) app.style.display = '';
        }, 800);
    }

    setTimeout(dismissSplash, SPLASH_DURATION);

    // ─── NAVBAR ───────────────────────────────────────────────
    function initNavbar() {
        const navbar = document.querySelector('.navbar');
        if (!navbar) return;
        const onScroll = () => {
            navbar.classList.toggle('scrolled', window.scrollY > 20);
        };
        window.addEventListener('scroll', onScroll, { passive: true });
        onScroll();
    }

    // ─── MOBILE NAV ───────────────────────────────────────────
    function initMobileNav() {
        const btn = document.getElementById('nav-hamburger-btn');
        const menu = document.getElementById('nav-mobile-menu');
        if (!btn || !menu) return;

        // Remove old listener by cloning
        const newBtn = btn.cloneNode(true);
        btn.parentNode.replaceChild(newBtn, btn);

        newBtn.addEventListener('click', () => {
            menu.classList.toggle('open');
            newBtn.classList.toggle('active');
        });
        menu.querySelectorAll('a').forEach(link => {
            link.addEventListener('click', () => {
                menu.classList.remove('open');
                newBtn.classList.remove('active');
            });
        });
    }

    // ─── SCROLL REVEAL ────────────────────────────────────────
    function initScrollReveal() {
        // Reset all previously revealed elements so they re-animate
        document.querySelectorAll('.reveal.visible').forEach(el => {
            el.classList.remove('visible');
        });

        const elements = document.querySelectorAll('.reveal');
        if (!elements.length) return;

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('visible');
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.12, rootMargin: '0px 0px -40px 0px' });

        elements.forEach(el => observer.observe(el));
    }

    // ─── HEX TILES ────────────────────────────────────────────
    function initHexTiles() {
        const tiles = document.querySelectorAll('.hex-tile');
        if (!tiles.length) return;
        function activateRandom() {
            const idx = Math.floor(Math.random() * tiles.length);
            tiles[idx].classList.add('active');
            setTimeout(() => tiles[idx].classList.remove('active'), 800);
        }
        setInterval(activateRandom, 300);
    }

    // ─── COUNTERS ─────────────────────────────────────────────
    function animateCounter(el) {
        const target = parseInt(el.dataset.target, 10);
        const suffix = el.dataset.suffix || '';
        const duration = 1500;
        const start = performance.now();
        function step(now) {
            const progress = Math.min((now - start) / duration, 1);
            const eased = 1 - Math.pow(1 - progress, 3);
            el.textContent = Math.round(eased * target) + suffix;
            if (progress < 1) requestAnimationFrame(step);
        }
        requestAnimationFrame(step);
    }

    function initCounters() {
        const counters = document.querySelectorAll('[data-target]');
        if (!counters.length) return;
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    animateCounter(entry.target);
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.5 });
        counters.forEach(c => observer.observe(c));
    }

    // ─── CONTACT FORM ─────────────────────────────────────────
    function initContactForm() {
        const form = document.getElementById('contact-form');
        if (!form) return;
        form.addEventListener('submit', (e) => {
            e.preventDefault();
            showToast("MESSAGE SENT — We'll be in touch shortly.");
            form.reset();
        });
    }

    // ─── TOAST ────────────────────────────────────────────────
    function showToast(message) {
        let toast = document.querySelector('.toast');
        if (!toast) {
            toast = document.createElement('div');
            toast.className = 'toast';
            document.body.appendChild(toast);
        }
        toast.textContent = message;
        toast.classList.add('show');
        setTimeout(() => toast.classList.remove('show'), 4000);
    }

    // ─── SMOOTH SCROLL ────────────────────────────────────────
    function initSmoothScroll() {
        document.querySelectorAll('a[href^="#"]').forEach(link => {
            link.addEventListener('click', (e) => {
                const id = link.getAttribute('href').slice(1);
                const target = document.getElementById(id);
                if (target) {
                    e.preventDefault();
                    target.scrollIntoView({ behavior: 'smooth', block: 'start' });
                }
            });
        });
    }

    // ─── BACKGROUND LINES ─────────────────────────────────────
    function injectBgLines() {
        if (document.querySelector('.bg-line')) return;
        ['l1', 'l2', 'l3', 'l4', 'l5'].forEach(cls => {
            const el = document.createElement('div');
            el.className = `bg-line ${cls}`;
            document.body.appendChild(el);
        });
    }

    // ─── SCROLL TO SECTION (called from Blazor after nav) ─────
    function scrollToSection(id) {
        setTimeout(() => {
            const el = document.getElementById(id);
            if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }, 100);
    }

    // ─── FULL INIT ────────────────────────────────────────────
    function init() {
        injectBgLines();
        initNavbar();
        initMobileNav();
        initScrollReveal();
        initHexTiles();
        initCounters();
        initContactForm();
        initSmoothScroll();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    window.protoscendInit = init;
    window.showToast = showToast;
    window.scrollToSection = scrollToSection;

})();