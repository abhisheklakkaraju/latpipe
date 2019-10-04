Datepicker = {
    init() {
        if (typeof rome == 'function') {
            const dateFormat = moment().locale(document.documentElement.lang)._locale._longDateFormat.L;

            document.querySelectorAll('.datepicker').forEach(date => {
                rome(date, {
                    styles: {
                        container: 'rd-container date-container'
                    },
                    monthFormat: 'YYYY MMMM',
                    inputFormat: dateFormat,
                    dayFormat: 'D',
                    time: false
                });
            });

            document.querySelectorAll('.datetimepicker').forEach(date => {
                rome(date, {
                    styles: {
                        container: 'rd-container datetime-container'
                    },
                    inputFormat: `${dateFormat} HH:mm`,
                    monthFormat: 'YYYY MMMM',
                    timeInterval: 900,
                    autoClose: false,
                    dayFormat: 'D'
                });
            });
        }
    }
};
