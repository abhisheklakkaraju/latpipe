Lookup = {
    init() {
        if (typeof MvcLookup == 'function') {
            document.querySelectorAll('.mvc-lookup').forEach(element => {
                new MvcLookup(element);
            });
        }
    }
};
