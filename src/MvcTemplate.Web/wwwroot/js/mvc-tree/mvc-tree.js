class MvcTree {
    constructor(element, options) {
        const tree = this;
        element = tree.closestTree(element);
        if (element.dataset.id) {
            return MvcTree.instances[parseInt(element.dataset.id)].set(options || {});
        }

        tree.values = {};
        tree.element = element;
        tree.element.dataset.id = MvcTree.instances.length;
        tree.ids = tree.element.querySelector('.mvc-tree-ids');
        tree.view = tree.element.querySelector('.mvc-tree-view');
        tree.readonly = element.classList.contains('mvc-tree-readonly');

        [].forEach.call(tree.ids.children, input => {
            tree.values[input.value] = input;
        });

        [].forEach.call(tree.view.children, branch => {
            tree.update(branch, true);
        });

        MvcTree.instances.push(tree);
        tree.set(options || {});
        tree.bind();
    }

    closestTree(element) {
        let tree = element;

        if (!tree) {
            throw new Error('Tree element was not specified.');
        }

        while (tree.parentElement && !tree.classList.contains('mvc-tree')) {
            tree = tree.parentElement;
        }

        if (tree == document) {
            throw new Error('Tree can only be created from within mvc-tree structure.');
        }

        return tree;
    }
    set({ readonly }) {
        const tree = this;

        tree.readonly = readonly == null ? tree.readonly : readonly;

        if (tree.readonly) {
            tree.element.classList.add('mvc-tree-readonly');
        } else {
            tree.element.classList.remove('mvc-tree-readonly');
        }
    }

    uncheck(branch) {
        let parent = branch.parentElement.parentElement;

        this.uncheckNode(branch);

        branch.querySelectorAll('li').forEach(node => {
            this.uncheckNode(node);
        });

        while (parent.tagName == 'LI') {
            this.update(parent);

            parent = parent.parentElement.parentElement;
        }
    }
    check(branch) {
        let parent = branch.parentElement.parentElement;

        this.checkNode(branch);

        branch.querySelectorAll('li').forEach(node => {
            this.checkNode(node);
        });

        while (parent.tagName == 'LI') {
            this.update(parent);

            parent = parent.parentElement.parentElement;
        }
    }

    update(branch, recursive) {
        if (branch.lastElementChild.tagName == 'UL') {
            let checked = 0;
            let unchecked = 0;
            const children = branch.lastElementChild.children;

            [].forEach.call(children, node => {
                const states = recursive ? this.update(node, recursive) : node.classList;

                if (states.contains('mvc-tree-undetermined')) {
                } else if (states.contains('mvc-tree-checked')) {
                    checked++;
                } else {
                    unchecked++;
                }
            });

            if (children.length == unchecked) {
                branch.classList.remove('mvc-tree-checked');
                branch.classList.remove('mvc-tree-undetermined');
            } else if (children.length == checked) {
                branch.classList.add('mvc-tree-checked');
                branch.classList.remove('mvc-tree-undetermined');
            } else {
                branch.classList.add('mvc-tree-undetermined');
            }
        }

        return branch.classList;
    }
    uncheckNode(node) {
        node.classList.remove('mvc-tree-checked');
        node.classList.remove('mvc-tree-undetermined');

        if (node.dataset.id && this.values[node.dataset.id]) {
            this.ids.removeChild(this.values[node.dataset.id]);

            delete this.values[node.dataset.id];
        }
    }
    checkNode(node) {
        node.classList.add('mvc-tree-checked');
        node.classList.remove('mvc-tree-undetermined');

        if (node.dataset.id && !this.values[node.dataset.id]) {
            const input = document.createElement('input');
            input.name = this.element.dataset.for;
            input.value = node.dataset.id;
            input.type = 'hidden';

            this.values[node.dataset.id] = input;
            this.ids.appendChild(input);
        }
    }

    collapse(branch) {
        branch.classList.add('mvc-tree-collapsed');
    }
    expand(branch) {
        branch.classList.remove('mvc-tree-collapsed');
    }

    bind() {
        const tree = this;

        tree.element.querySelectorAll('a').forEach(node => {
            node.addEventListener('click', function (e) {
                e.preventDefault();

                if (!tree.readonly) {
                    const branch = this.parentElement;
                    if (branch.classList.contains('mvc-tree-checked')) {
                        tree.uncheck(branch);
                    } else {
                        tree.check(branch);
                    }
                }
            });
        });

        tree.element.querySelectorAll('.mvc-tree-branch > i').forEach(branch => {
            branch.addEventListener('click', function () {
                const parent = this.parentElement;
                if (parent.classList.contains('mvc-tree-collapsed')) {
                    tree.expand(parent);
                } else {
                    tree.collapse(parent);
                }
            });
        });
    }
}

MvcTree.instances = [];
