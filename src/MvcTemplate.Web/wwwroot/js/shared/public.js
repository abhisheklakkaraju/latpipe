// Widgets init
(function () {
    Validator.init();
    Alerts.init();
})();

// Input focus binding
(function () {
    const invalid = document.querySelector(".input-validation-error[type=text]:not([readonly]):not(.datepicker):not(.datetimepicker)");

    if (invalid) {
        invalid.setSelectionRange(0, invalid.value.length);
        invalid.focus();
    } else {
        const input = document.querySelector("input[type=text]:not([readonly]):not(.datepicker):not(.datetimepicker)");

        if (input) {
            input.setSelectionRange(0, input.value.length);
            input.focus();
        }
    }
})();
