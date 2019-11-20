// Widgets init
(function () {
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
    document.querySelectorAll('.widget-box.readonly').forEach(widget => {
        widget.querySelectorAll('.mvc-lookup').forEach(element => {
            new MvcLookup(element, { readonly: true });
        });

        widget.querySelectorAll('.mvc-tree').forEach(element => {
            new MvcTree(element, { readonly: true });
        });

        widget.querySelectorAll('textarea').forEach(textarea => {
            textarea.readOnly = true;
            textarea.tabIndex = -1;
        });

        widget.querySelectorAll('input').forEach(input => {
            input.readOnly = true;
            input.tabIndex = -1;
        });
    });

    window.addEventListener('click', e => {
        if (e.target && e.target.readOnly) {
            e.preventDefault();
        }
    });
})();

// Input focus binding
(function () {
    const invalid = document.querySelector('.input-validation-error[type=text]:not([readonly]):not(.datepicker):not(.datetimepicker)');

    if (invalid) {
        invalid.setSelectionRange(0, invalid.value.length);
        invalid.focus();
    } else {
        const input = document.querySelector('input[type=text]:not([readonly]):not(.datepicker):not(.datetimepicker)');

        if (input) {
            input.setSelectionRange(0, input.value.length);
            input.focus();
        }
    }
})();
