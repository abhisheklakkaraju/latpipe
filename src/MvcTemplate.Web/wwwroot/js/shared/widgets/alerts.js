Alerts = {
    init() {
        this.element = document.querySelector('.alerts');

        if (this.element) {
            [].forEach.call(this.element.children, this.bind);
        }
    },

    show(alerts) {
        [].concat(alerts).forEach(alert => {
            const element = document.getElementById(alert.id) || emptyAlert();

            element.setAttribute('data-timeout', alert.timeout || 0);
            element.className = `alert alert-${getType(alert.type)}`;
            element.children[0].innerText = alert.message || '';
            element.id = alert.id || '';

            this.element.appendChild(element);
            this.bind(element);
        });

        function emptyAlert() {
            const message = document.createElement('span');
            const close = document.createElement('span');
            const alert = document.createElement('div');

            close.innerHTML = '&#x00D7;';
            close.className = 'close';

            alert.append(message);
            alert.append(close);

            return alert;
        }

        function getType(id) {
            switch (id) {
                case 0:
                    return 'danger';
                case 1:
                    return 'warning';
                case 2:
                    return 'info';
                case 3:
                    return 'success';
                default:
                    return id;
            }
        }
    },
    bind(alert) {
        if (alert.dataset.timeout > 0) {
            setTimeout(() => {
                alert.querySelector('.close').click();
            }, alert.dataset.timeout);
        }
    },
    close(id) {
        document.querySelector(`#${id} .close`).click();
    },
    closeAll() {
        this.element.querySelectorAll('.close').forEach(close => {
            close.click();
        });
    },

    clear() {
        this.element.innerHTML = '';
    }
};
