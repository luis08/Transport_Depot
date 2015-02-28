function Utilities() {

}
Utilities.parseCurrency = function (currency) {
  currency = currency.replace("$", "")
  currency = currency.replace(',', '');
  return parseFloat(currency);
};
 
Utilities.formatCurrency = function (number) {
  var number = number.toString(),
    dollars = number.split('.')[0],
    cents = (number.split('.')[1] || '') + '00';
  dollars = dollars.split('').reverse().join('')
        .replace(/(\d{3}(?!$))/g, '$1,')
        .split('').reverse().join('');
  return '$' + dollars + '.' + cents.slice(0, 2);
}

Utilities.formatDate = function (date) {
  var formattedDate = $.datepicker.formatDate('mm/dd/yy', new Date(JSON.parseDateOnly(date)));
  return formattedDate;
}

Utilities.isInt = function (n) {
  return +n === n && !(n % 1);
}

Utilities.getTimeNow = function () {
  var d = new Date();
  var dt = [d.getFullYear(), d.getMonth(), d.getDay()].join("-"),
      tm = [d.getHours(), d.getMinutes()].join(":");
  return tm;
};