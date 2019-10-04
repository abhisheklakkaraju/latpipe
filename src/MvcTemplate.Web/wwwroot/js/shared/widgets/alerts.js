Alerts = {
    init() {
        this.element = document.querySelector('.alerts');

        if (this.element) {
            [].forEach.call(this.element.children, this.bind);
        }
    },

    show(alerts) {
        alerts = [].concat(alerts);
        for (let i = 0; i < alerts.length; i++) {
            const alert = document.getElementById(alerts[i].id) || emptyAlert();

            alert.setAttribute('data-timeout', alerts[i].timeout || 0);
            alert.className = `alert alert-${getType(alerts[i].type)}`;
            alert.children[0].innerText = alerts[i].message || '';
            alert.id = alerts[i].id || '';

            this.element.appendChild(alert);
            this.bind(alert);
        }

        function emptyAlert() {
            const alert = document.createElement('div');
            const close = document.createElement('span');
            const message = document.createElement('span');

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
