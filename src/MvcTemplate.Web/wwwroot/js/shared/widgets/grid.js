Grid = {
    init() {
        if (typeof MvcGrid == "function") {
            MvcGridNumberFilter.prototype.isValid = function (value) {
                return !value || !isNaN(numbro(value).value());
            };

            for (const element of document.querySelectorAll(".mvc-grid")) {
                new MvcGrid(element);
            }
        }
    }
};
