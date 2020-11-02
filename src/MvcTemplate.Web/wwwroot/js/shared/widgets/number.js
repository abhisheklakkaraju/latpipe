NumberConverter = {
    init() {
        const parts = new Intl.NumberFormat(document.documentElement.lang).formatToParts(12345.6);

        this.group = new RegExp(`[${parts.find(part => part.type == "group").value}]`, "g")
        this.decimal = new RegExp(`[${parts.find(part => part.type == "decimal").value}]`, "g")
    },
    parse(value) {
        return parseFloat(value.replace(this.group, "").replace(this.decimal, "."));
    },
    format(number, format) {
        const options = Object.assign({
            maximumFractionDigits: 20
        }, format);

        return new Intl.NumberFormat(document.documentElement.lang, options).format(number);
    }
};
