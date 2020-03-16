numbro.registerLanguage({
    languageTag: "en",
    delimiters: {
        thousands: ",",
        decimal: "."
    },
    abbreviations: {
        thousand: "k",
        million: "m",
        billion: "b",
        trillion: "t"
    },
    ordinal: function (number) {
        const b = number % 10;

        return ~~(number % 100 / 10) === 1 ? "th" : b === 1 ? "st" : b === 2 ? "nd" : b === 3 ? "rd" : "th";
    },
    currency: {
        symbol: "£",
        position: "prefix",
        code: "GBP"
    },
    currencyFormat: {
        thousandSeparated: true,
        totalLength: 4,
        spaceSeparated: true,
        average: true
    },
    formats: {
        fourDigits: {
            totalLength: 4,
            spaceSeparated: true,
            average: true
        },
        fullWithTwoDecimals: {
            output: "currency",
            thousandSeparated: true,
            spaceSeparated: true,
            mantissa: 2
        },
        fullWithTwoDecimalsNoCurrency: {
            mantissa: 2,
            thousandSeparated: true
        },
        fullWithNoDecimals: {
            output: "currency",
            thousandSeparated: true,
            spaceSeparated: true,
            mantissa: 0
        }
    }
});

if (document.documentElement.lang == "en") {
    numbro.setLanguage("en");
}
