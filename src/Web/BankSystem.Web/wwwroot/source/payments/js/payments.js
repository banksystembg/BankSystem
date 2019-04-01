(function () {
    setTimeout(function () {
            let btn = $('#payBtn');
            btn.prop('disabled', false);
            btn.text(payBtnText);
        },
        3000);
})();

$('#payBtn').click(function () {

    function paymentFailed(errorMessage) {
        $('#cancelBtn').removeClass('disabled')
            .removeClass('btn-outline-secondary')
            .addClass('btn-outline-primary')
            .focus();

        btn.addClass('btn-danger');
        btn.html('<i class="fas fa-times"></i> Failed');

        let errAlert = $('#errAlert');
        errAlert.text(errorMessage);
        errAlert.slideDown();
    }

    window.onbeforeunload = function () {
        return "Please do not close this page until your payment is processed.";
    };

    $('#cancelBtn').addClass('disabled');

    let btn = $('#payBtn');

    btn.prop('disabled', true);
    btn.html('<i class="fas fa-circle-notch fa-spin"></i> Processing');

    $.ajax({
        url: '/payments/payasync',
        type: 'post',
        data: $('#paymentForm').serialize()

    })
        .always(function () {
            btn.removeClass('btn-primary');
            window.onbeforeunload = function () {
            };
        })
        .fail(function () {
            paymentFailed('Payment failed. Please try again later.');
        })
        .done(function (response) {

            if (response.success) {

                btn.addClass('btn-success');
                btn.html('<i class="fas fa-check"></i> Success');

                setTimeout(function () {
                        let form = $('<form action="' + response.returnUrl + '" method="post">' +
                            '<input type="hidden" name="data" value="' + response.data + '" />' +
                            '</form>');
                        $('body').append(form);
                        form.submit();
                    },
                    1000);
            } else {
                paymentFailed(response.errorMessage);
            }
        });

    $('#accountPicker').prop('disabled', true);
});