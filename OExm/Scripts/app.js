// Global Application JavaScript

// 1. Theme Management
function initTheme() {
    const savedTheme = localStorage.getItem('theme') || 'light';
    if (savedTheme === 'dark') {
        document.documentElement.classList.add('dark');
    } else {
        document.documentElement.classList.remove('dark');
    }
}

function toggleTheme() {
    const isDark = document.documentElement.classList.contains('dark');
    if (isDark) {
        document.documentElement.classList.remove('dark');
        localStorage.setItem('theme', 'light');
    } else {
        document.documentElement.classList.add('dark');
        localStorage.setItem('theme', 'dark');
    }
    updateThemeButtonText();
}

function updateThemeButtonText() {
    const themeBtn = document.getElementById('themeToggleBtn');
    if (themeBtn) {
        const isDark = document.documentElement.classList.contains('dark');
        themeBtn.innerHTML = isDark ? '☀️ Light Mode' : '🌙 Dark Mode';
    }
}

// 2. Navigation Active State
function initNavigation() {
    const path = window.location.pathname.toLowerCase();
    const navItems = document.querySelectorAll('.nav-item');
    
    navItems.forEach(item => {
        const href = item.getAttribute('href');
        if (href) {
            const hrefLower = href.toLowerCase();
            if (path.includes(hrefLower) && hrefLower !== 'default.aspx') {
                item.classList.add('active');
            } else if (path.endsWith('/') || path.endsWith('default.aspx')) {
                if (hrefLower === 'default.aspx') {
                    item.classList.add('active');
                }
            }
        }
    });
}

// 3. Fullscreen Mode Triggers
let examFullscreenActive = false;

function enterExamFullscreen() {
    const element = document.documentElement;
    if (element.requestFullscreen) {
        element.requestFullscreen();
    } else if (element.mozRequestFullScreen) { /* Firefox */
        element.mozRequestFullScreen();
    } else if (element.webkitRequestFullscreen) { /* Chrome, Safari & Opera */
        element.webkitRequestFullscreen();
    } else if (element.msRequestFullscreen) { /* IE/Edge */
        element.msRequestFullscreen();
    }
    
    examFullscreenActive = true;
    const alertBox = document.getElementById('fullscreenAlert');
    if (alertBox) alertBox.style.display = 'none';
}

function initFullscreenListeners() {
    document.addEventListener('fullscreenchange', handleFullscreenChange);
    document.addEventListener('webkitfullscreenchange', handleFullscreenChange);
    document.addEventListener('mozfullscreenchange', handleFullscreenChange);
    document.addEventListener('MSFullscreenChange', handleFullscreenChange);
}

function handleFullscreenChange() {
    const isFullscreen = document.fullscreenElement || 
                         document.webkitFullscreenElement || 
                         document.mozFullScreenElement || 
                         document.msFullscreenElement;
                         
    const alertBox = document.getElementById('fullscreenAlert');
    if (examFullscreenActive && !isFullscreen) {
        // User exited fullscreen during exam! Warn them
        if (alertBox) {
            alertBox.innerHTML = "⚠️ WARNING: Exiting fullscreen mode is flagged as a cheating attempt. Please enter fullscreen again immediately!";
            alertBox.style.display = 'block';
        }
    } else {
        if (alertBox) alertBox.style.display = 'none';
    }
}

// Self-executing initialization on DOMContentLoaded
document.addEventListener('DOMContentLoaded', () => {
    initTheme();
    updateThemeButtonText();
    initNavigation();
    
    const themeBtn = document.getElementById('themeToggleBtn');
    if (themeBtn) {
        themeBtn.addEventListener('click', (e) => {
            e.preventDefault();
            toggleTheme();
        });
    }
});

// Run theme detection immediately to avoid style flickering
initTheme();
