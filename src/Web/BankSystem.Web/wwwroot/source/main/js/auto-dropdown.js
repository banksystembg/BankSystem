$('li.auto-dropdown').hover(function () {
    $(this).find('.dropdown-menu').stop(true, true).delay(200).slideDown();
},
    function () {
        $(this).find('.dropdown-menu').stop(true, true).delay(200).slideUp();
    });
