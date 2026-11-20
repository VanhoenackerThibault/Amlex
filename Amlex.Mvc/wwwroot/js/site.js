/* ================================================================
   site.js — Amlex global scripts
   ================================================================ */

(() => {
    "use strict";

    /* ── Scroll-shadow on header ──────────────────────────────── */
    const header = document.querySelector(".amx-header");

    if (header) {
        const toggleScrolled = () => {
            header.classList.toggle("is-scrolled", window.scrollY > 8);
        };

        window.addEventListener("scroll", toggleScrolled, { passive: true });
        toggleScrolled(); // run once on load
    }

    /* ── Mobile nav toggle ────────────────────────────────────── */
    const toggle = document.getElementById("amx-mobile-toggle");
    const mobileNav = document.getElementById("amx-mobile-nav");

    if (toggle && mobileNav) {
        toggle.addEventListener("click", () => {
            const isOpen = mobileNav.classList.toggle("is-open");
            toggle.setAttribute("aria-expanded", isOpen);
        });

        // Close drawer when any link inside is clicked
        mobileNav.querySelectorAll("a, button[type='submit']").forEach((el) => {
            el.addEventListener("click", () => {
                mobileNav.classList.remove("is-open");
                toggle.setAttribute("aria-expanded", "false");
            });
        });

        // Close on Escape
        document.addEventListener("keydown", (e) => {
            if (e.key === "Escape" && mobileNav.classList.contains("is-open")) {
                mobileNav.classList.remove("is-open");
                toggle.setAttribute("aria-expanded", "false");
                toggle.focus();
            }
        });
    }
})();