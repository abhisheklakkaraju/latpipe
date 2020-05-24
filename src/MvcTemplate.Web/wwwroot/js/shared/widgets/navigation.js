Navigation = {
    init() {
        const maxWidth = 100;
        const navigation = this;

        navigation.element = document.querySelector(".navigation");

        if (navigation.element) {
            navigation.nodes = navigation.element.querySelectorAll("li");
            navigation.search = navigation.element.querySelector("input");

            navigation.search.addEventListener("input", function () {
                navigation.filter(this.value);
            });

            navigation.nodes.forEach(node => {
                node.firstElementChild.addEventListener("click", e => {
                    if (node.firstElementChild.getAttribute("href") != "#") {
                        return;
                    }

                    e.preventDefault();

                    node.classList.toggle("open");

                    if (navigation.element.clientWidth < maxWidth) {
                        [].forEach.call(node.parentElement.children, sibling => {
                            if (sibling != node) {
                                sibling.classList.remove("open");
                            }
                        });
                    }
                });
            });

            window.addEventListener("resize", () => {
                if (navigation.element.clientWidth < maxWidth) {
                    navigation.closeAll();
                }
            });

            window.addEventListener("click", e => {
                if (navigation.element.clientWidth < maxWidth) {
                    let target = e && e.target;

                    while (target && !/navigation/.test(target.className)) {
                        target = target.parentElement;
                    }

                    if (!target && target != window) {
                        navigation.closeAll();
                    }
                }
            });

            if (navigation.element.clientWidth < maxWidth) {
                navigation.closeAll();
            }
        }
    },

    filter(term) {
        this.search.value = term;

        for (const node of this.nodes) {
            node.classList.remove("open");
            node.style.display = "";
        }

        if (term) {
            [].forEach.call(this.element.lastElementChild.children, node => {
                filterNode(node, term.toLowerCase());
            });
        }

        function filterNode(node, search) {
            const text = node.firstElementChild.querySelector(".text").textContent.toLowerCase();
            const match = text.includes(search);

            if (!match && node.children.length > 1) {
                for (const child of node.lastElementChild.children) {
                    if (filterNode(child, search)) {
                        node.classList.add("open");
                    }
                }
            }

            if (!match && !node.classList.contains("open")) {
                node.style.display = "none";
            }

            return match;
        }
    },

    closeAll() {
        for (const node of this.nodes) {
            node.classList.remove("open");
        }
    }
};
