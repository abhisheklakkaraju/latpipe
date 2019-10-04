Grid = {
    init() {
        if (typeof MvcGrid == 'function') {
            MvcGridNumberFilter.prototype.isValid = function (value) {
                return value == '' || !isNaN(numbro(value).value());
            };

            document.querySelectorAll('.mvc-grid').forEach(element => {
                new MvcGrid(element);
            });
        }
    }
};
