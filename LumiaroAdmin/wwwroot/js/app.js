// Lumiaro Referee Admin - JS Interop

window.lumiaro = {
    // Close sidebar on outside click (mobile)
    closeSidebar: function () {
        const sidebar = document.getElementById('sidebar');
        if (sidebar) sidebar.classList.remove('open');
    },

    // Scroll to top of main content
    scrollToTop: function () {
        const main = document.querySelector('.app-main');
        if (main) main.scrollTo({ top: 0, behavior: 'smooth' });
    },

    // Focus an element by selector
    focusElement: function (selector) {
        const el = document.querySelector(selector);
        if (el) el.focus();
    }
};
