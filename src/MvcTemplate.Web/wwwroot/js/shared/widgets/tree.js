Tree = {
    init() {
        if (typeof MvcTree == 'function') {
            document.querySelectorAll('.mvc-tree').forEach(element => {
                new MvcTree(element);
            });
        }
    }
};
