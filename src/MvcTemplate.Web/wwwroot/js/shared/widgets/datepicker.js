Datepicker = {
    init() {
        if (typeof rome == "function") {
            const dateFormat = moment().locale(document.documentElement.lang)._locale._longDateFormat.L;

            for (const date of document.querySelectorAll(".datepicker")) {
                rome(date, {
                    styles: {
                        container: "rd-container date-container"
                    },
                    monthFormat: "YYYY MMMM",
                    inputFormat: dateFormat,
                    dayFormat: "D",
                    time: false
                });
            }

            for (const date of document.querySelectorAll(".datetimepicker")) {
                rome(date, {
                    styles: {
                        container: "rd-container datetime-container"
                    },
                    inputFormat: `${dateFormat} HH:mm`,
                    monthFormat: "YYYY MMMM",
                    timeInterval: 900,
                    autoClose: false,
                    dayFormat: "D"
                });
            }
        }
    }
};
