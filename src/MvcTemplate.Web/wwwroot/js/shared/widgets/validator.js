Validator = {
    init() {
        Wellidate.prototype.rules.date.isValid = function () {
            return !this.element.value || moment(this.element.value).isValid();
        };

        const isValid = Wellidate.prototype.rules.number.isValid;
        Wellidate.prototype.rules.number.isValid = function () {
            return !this.element.value || isValid.call(this, numbro(this.element.value).value());
        };

        Wellidate.prototype.rules.min.isValid = function () {
            return !this.element.value || this.min <= numbro(this.element.value).value();
        };

        Wellidate.prototype.rules.max.isValid = function () {
            return !this.element.value || numbro(this.element.value).value() <= this.max;
        };

        Wellidate.prototype.rules.range.isValid = function () {
            const range = this;
            const value = numbro(this.element.value).value();

            return !range.element.value || (range.min == null || range.min <= value) && (range.max == null || value <= range.max);
        };

        Wellidate.prototype.rules.greater.isValid = function () {
            const value = numbro(this.element.value).value();

            return !this.element.value || this.min <= value && value <= this.max;
        };

        document.addEventListener('wellidate-error', e => {
            if (event.target.classList.contains('mvc-lookup-value')) {
                const wellidate = e.detail.validatable.wellidate;
                const control = new MvcLookup(event.target).control;

                control.classList.add(wellidate.inputErrorClass);
                control.classList.remove(wellidate.inputValidClass);
                control.classList.remove(wellidate.inputPendingClass);
            }
        });

        document.addEventListener('wellidate-success', e => {
            if (event.target.classList.contains('mvc-lookup-value')) {
                const wellidate = e.detail.validatable.wellidate;
                const control = new MvcLookup(event.target).control;

                control.classList.add(wellidate.inputValidClass);
                control.classList.remove(wellidate.inputErrorClass);
                control.classList.remove(wellidate.inputPendingClass);
            }
        });

        document.querySelectorAll('.mvc-lookup-value.input-validation-error').forEach(value => {
            new MvcLookup(value).control.classList.add('input-validation-error');
        });

        document.querySelectorAll('form').forEach(form => new Wellidate(form, {
            wasValidatedClass: 'validated'
        }));
    }
};
