﻿//---------------------------------

//Kiểm tra thông tin người đặt hàng 
$('.order-info-summary').click(() => {
    var gender = $('input[name="gender"]:checked').val();
    var name = $('#UserFullName').val();
    var phone = $('#PhoneNumber').val();
    var shipMethod = $('input[name="shipmethod"]:checked').val();
    var address = $('#ship-address').val();
    var note = $('#cart-note').val();

    $.ajax({
        type: 'Post',
        url: '/order/checkorderinfo',
        data: {
            gender: gender,
            customerName: name,
            customerPhone: phone,
            shipMethod: shipMethod,
            address: address,
            note: note
        },
        success: (res) => {
            if (res.success) {       
                $.ajax({
                    type: 'POST',
                    url: '/account/update',
                    data: {
                        UserFullName: name,
                        Gender: gender,
                        PhoneNumber: phone,
                        Address: address
                    },
                    success: (response) => {
                        if (response.success) {
                            window.location.href = '/order/orderinfo';
                        }
                    },
                    error: () => { }
                })
            }
            else {
                $('.cart-info .form-error').empty();
                $('.cart-info .form-error').show();
                var str = `<span>
                    <i class="fa-solid fa-circle-exclamation"></i>`
                    + res.error + `</span>`;
                $('.cart-info .form-error').append(str);

                setTimeout(function () {
                    $('.cart-info .form-error').hide();
                }, 8000)
            }
        },
        error: () => {  }
    })
})