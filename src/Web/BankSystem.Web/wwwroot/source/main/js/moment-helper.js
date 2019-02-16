function getFriendlyDate(momentTime) {
    return momentTime.calendar(null,
        {
            sameDay: '[Today at] HH:mm',
            nextDay: '[Tomorrow at] HH:mm',
            nextWeek: 'dddd [at] HH:mm',
            lastDay: '[Yesterday at] HH:mm',
            lastWeek: '[Last] dddd [at] HH:mm',
            sameElse: 'Do MMM YYYY [at] HH:mm'
        });
}

function getFormattedDate(momentTime) {
    return momentTime.format('Do MMM YYYY HH:mm:ss');
}

$('.auto-friendly-date').each(function () {
    this.innerHTML = getFriendlyDate(moment.utc(this.innerHTML).local());
});

$('.auto-format-date').each(function () {
    this.innerHTML = getFormattedDate(moment.utc(this.innerHTML).local());
});