// Widgets init
(function () {
    NumberConverter.init();
    Datepicker.init();
    Navigation.init();
    Validator.init();
    Alerts.init();
    Header.init();
    Lookup.init();
    Grid.init();
    Tree.init();
})();

// Read only binding
(function () {
    for (const widget of document.querySelectorAll(".widget-box.readonly")) {
        for (const element of widget.querySelectorAll(".mvc-lookup")) {
            new MvcLookup(element, { readonly: true });
        }

        for (const element of widget.querySelectorAll(".mvc-tree")) {
            new MvcTree(element, { readonly: true });
        }

        for (const textarea of widget.querySelectorAll("textarea")) {
            textarea.readOnly = true;
            textarea.tabIndex = -1;
        }

        for (const input of widget.querySelectorAll("input")) {
            input.readOnly = true;
            input.tabIndex = -1;
        }
    }

    window.addEventListener("click", e => {
        if (e.target && e.target.readOnly) {
            e.preventDefault();
        }
    });
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
