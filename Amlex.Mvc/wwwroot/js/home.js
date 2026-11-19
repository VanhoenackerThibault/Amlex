(() => {
    const searchInput = document.getElementById("anx-search-input");
    const table = document.getElementById("anx-acts-table");
    const noResults = document.getElementById("anx-no-results");
    const filters = [...document.querySelectorAll(".anx-filter")];

    if (!table) return;

    const rows = [...table.querySelectorAll("tbody tr")];
    let activeFilter = "all";

    const applyFilters = () => {
        const query = searchInput?.value.trim().toLowerCase() ?? "";
        let visibleCount = 0;

        rows.forEach((row) => {
            const matchesStatus =
                activeFilter === "all" || row.dataset.status === activeFilter;

            const matchesSearch =
                !query || (row.dataset.search ?? "").toLowerCase().includes(query);

            const isVisible = matchesStatus && matchesSearch;
            row.hidden = !isVisible;

            if (isVisible) visibleCount += 1;
        });

        if (noResults) {
            noResults.hidden = visibleCount > 0;
        }
    };

    searchInput?.addEventListener("input", applyFilters);

    filters.forEach((button) => {
        button.addEventListener("click", () => {
            filters.forEach((item) => item.classList.remove("is-active"));
            button.classList.add("is-active");

            activeFilter = button.dataset.filter;
            applyFilters();
        });
    });
})();