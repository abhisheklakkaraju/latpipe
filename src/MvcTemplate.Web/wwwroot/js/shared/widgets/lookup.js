Lookup = {
    init() {
        if (typeof MvcLookup == "function") {
            for (const element of document.querySelectorAll(".mvc-lookup")) {
                new MvcLookup(element);
            }
        }
    }
};
