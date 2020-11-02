Validator = {
    init() {
        Wellidate.default.rules.date.isValid = function () {
            const value = this.normalizeValue();

            return !value || moment(value).isValid();
        };

        Wellidate.default.rules.number.isValid = function () {
            const number = this;
            const scale = parseInt(number.scale);
            const value = number.normalizeValue();
            const precision = parseInt(number.precision);
            const parts = value.split(NumberConverter.decimal);
            let isValid = NumberConverter.parse(value);

            if (isValid && value && precision > 0) {
                number.isValidPrecision = number.digits(parts[0].replace(new RegExp(`^[-+${NumberConverter.group}0]+`), "")) <= precision - (scale || 0);
                isValid = isValid && number.isValidPrecision;
            } else {
                number.isValidPrecision = true;
            }

            if (isValid && parts.length > 1 && scale >= 0) {
                number.isValidScale = number.digits(parts[1].replace(/0+$/, "")) <= scale;
                isValid = isValid && number.isValidScale;
            } else {
                number.isValidScale = true;
            }

            return isValid;
        };

        Wellidate.default.rules.min.isValid = function () {
            const value = this.normalizeValue();

            return !value || parseFloat(this.value) <= NumberConverter.parse(value);
        };

        Wellidate.default.rules.max.isValid = function () {
            const value = this.normalizeValue();

            return !value || NumberConverter.parse(value) <= parseFloat(this.value);
        };

        Wellidate.default.rules.range.isValid = function () {
            const range = this;
            const value = this.normalizeValue();
            const number = NumberConverter.parse(value);

            return !value || (range.min == null || parseFloat(range.min) <= number) && (range.max == null || number <= parseFloat(range.max));
        };

        Wellidate.default.rules.lower.isValid = function () {
            const value = this.normalizeValue();
            const number = NumberConverter.parse(value);

            return !value || number < parseFloat(this.than);
        };

        Wellidate.default.rules.greater.isValid = function () {
            const value = this.normalizeValue();
            const number = NumberConverter.parse(value);

            return !value || parseFloat(this.than) < number;
        };

        document.addEventListener("wellidate-error", e => {
            if (e.target.classList.contains("mvc-lookup-value")) {
                const { wellidate } = e.detail.validatable;
                const { control } = new MvcLookup(e.target);

                control.classList.add(wellidate.inputErrorClass);
                control.classList.remove(wellidate.inputValidClass);
                control.classList.remove(wellidate.inputPendingClass);
            }
        });

        document.addEventListener("wellidate-success", e => {
            if (e.target.classList.contains("mvc-lookup-value")) {
                const { wellidate } = e.detail.validatable;
                const { control } = new MvcLookup(e.target);

                control.classList.add(wellidate.inputValidClass);
                control.classList.remove(wellidate.inputErrorClass);
                control.classList.remove(wellidate.inputPendingClass);
            }
        });

        for (const value of document.querySelectorAll(".mvc-lookup-value.input-validation-error")) {
            new MvcLookup(value).control.classList.add("input-validation-error");
        }

        document.querySelectorAll("form").forEach(form => new Wellidate(form, {
            wasValidatedClass: "validated"
        }));
    }
};
