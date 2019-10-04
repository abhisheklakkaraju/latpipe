Navigation = {
    init() {
        const navigation = this;

        navigation.element = document.querySelector('.navigation');
        navigation.nodes = navigation.element.querySelectorAll('li');
        navigation.activeNodes = navigation.element.querySelectorAll('.has-active');

        if (navigation.element) {
            navigation.search = navigation.element.querySelector('input');

            navigation.search.addEventListener('input', function () {
                navigation.filter(this.value);
            });

            [].map.call(navigation.nodes, node => node)
                .filter(node => node.classList.contains('submenu'))
                .forEach(submenu => {
                    submenu.firstElementChild.addEventListener('click', e => {
                        e.preventDefault();

                        submenu.classList.toggle('open');

                        if (navigation.element.clientWidth < 100) {
                            [].forEach.call(submenu.parentElement.children, sibling => {
                                if (sibling != submenu) {
                                    sibling.classList.remove('open');
                                }
                            });
                        }
                    });
                });

            window.addEventListener('resize', () => {
                if (navigation.element.clientWidth < 100) {
                    navigation.closeAll();
                }
            });

            window.addEventListener('click', e => {
                if (navigation.element.clientWidth < 100) {
                    let target = e && e.target;

                    while (target && !/navigation/.test(target.className)) {
                        target = target.parentElement;
                    }

                    if (!target && target != window) {
                        navigation.closeAll();
                    }
                }
            });

            if (navigation.element.clientWidth < 100) {
                navigation.closeAll();
            }
        }
    },

    filter(term) {
        this.search.value = term;
        term = term.toLowerCase();

        this.nodes.forEach(node => {
            node.classList.remove('open');
            node.style.display = '';
        });

        if (term) {
            [].forEach.call(this.element.lastElementChild.children, node => {
                filterNode(node, term);
            });
        } else {
            this.activeNodes.forEach(node => {
                node.classList.add('open');
            });
        }

        function filterNode(element, term) {
            const text = element.firstElementChild.querySelector('.text').textContent.toLowerCase();
            const matches = text.includes(term);

            if (element.classList.contains('submenu')) {
                const children = element.lastElementChild.children;

                for (let i = 0; i < children.length; i++) {
                    if (filterNode(children[i], term)) {
                        element.classList.add('open');
                    }
                }
            }

            if (!matches && !element.classList.contains('open')) {
                element.style.display = 'none';
            }

            return matches;
        }
    },

    closeAll() {
        this.nodes.forEach(node => {
            node.classList.remove('open');
        });
    }
};
