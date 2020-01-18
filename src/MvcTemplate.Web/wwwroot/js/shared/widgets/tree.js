Tree = {
    init() {
        if (typeof MvcTree == "function") {
            for (const element of document.querySelectorAll(".mvc-tree")) {
                new MvcTree(element);
            }
        }
    }
};
