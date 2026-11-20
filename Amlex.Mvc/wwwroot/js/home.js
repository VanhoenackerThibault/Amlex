/* ================================================================
   home.js — Amlex home page
   Live search + status filter for the legal acts table
   ================================================================ */

(() => {
    "use strict";

    const table = document.getElementById("amx-acts-table");
    const noResults = document.getElementById("amx-no-results");
    const searchEl = document.getElementById("amx-search-input");
    const filters = [...document.querySelectorAll(".amx-filter")];

    if (!table) return;

    const rows = [...table.querySelectorAll("tbody tr")];
    let activeFilter = "all";

    /* ── Core filter function ─────────────────────────────────── */
    const applyFilters = () => {
        const query = (searchEl?.value ?? "").trim().toLowerCase();
        let visible = 0;

        rows.forEach((row) => {
            const statusMatch =
                activeFilter === "all" || row.dataset.status === activeFilter;

            const searchMatch =
                !query ||
                (row.dataset.search ?? "").toLowerCase().includes(query);

            const show = statusMatch && searchMatch;
            row.hidden = !show;
            if (show) visible++;
        });

        if (noResults) noResults.hidden = visible > 0;
    };

    /* ── Wire up search ───────────────────────────────────────── */
    if (searchEl) {
        searchEl.addEventListener("input", applyFilters);
    }

    /* ── Wire up filter buttons ───────────────────────────────── */
    filters.forEach((btn) => {
        btn.addEventListener("click", () => {
            filters.forEach((b) => b.classList.remove("is-active"));
            btn.classList.add("is-active");
            activeFilter = btn.dataset.filter;
            applyFilters();
        });
    });
})();