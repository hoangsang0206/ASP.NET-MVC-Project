//Update quantity of item in cart to header
function updateCartCount() {
    $.ajax({
        type: 'POST',
        url: '/cart/cartcount',
        success: (data) => {
            $('.cart-count').empty();
            $('.cart-count').append(data.count);
        },
        error: () => {
            $('.cart-count').empty();
            $('.cart-count').append(0);
        }
    })
}

$(document).ready(() => {
    updateCartCount();
})

//-Add product to cart ------------------------------------
$('.buy-action-btn').click(() => {
    var productID = $('.buy-action-btn').data('product-id');
    console.log(productID);

    $.ajax({
        type: 'POST',
        url: '/cart/addtocart',
        data: {
            ProductID: productID,
            Quantity: 1
        },
        success: (respone) => {
            if (respone.success) {
                updateCartCount();
            }
        }
    })
})

//-------------------------------
$('.not-logged-in').click(() => {
    $('.login').css('visibility', 'visible');
    $('.login .form-container').addClass('showForm');
})

//---------------------------------
$(document).ready(() => {
    var cartFormInput = $('.cart-form input').toArray();
    var addressVal;
    cartFormInput.forEach((input) => {
        checkInputValid($(input));
    })

    if ($('#cod-method').is(':checked')) {
        $('.input-ship-info').show();
    }

    $('input[name="shipmethod"]').on('change', () => {
        if ($('#cod-method').is(':checked')) {
            $('.input-ship-info').show();
        }
        else {
            $('.input-ship-info').hide();
        }
    })
})

//---------------------------------
function activeCartStep(step1, step2, step3, step4) {
    $(step1).addClass('step-active');
    $(step2).addClass('step-active');
    $(step3).addClass('step-active');
    $(step4).addClass('step-active');
}

function disAciveStep(step2, step3, step4) {
    $(step2).removeClass('step-active');
    $(step3).removeClass('step-active');
    $(step4).removeClass('step-active');
}

function showCartInfo() {
    var idFromUrl = window.location.hash.substring(1);
    if (idFromUrl.length > 0) {
        hideCartInfo();
        $('#' + idFromUrl).addClass('form-current');

        disAciveStep('.step-2', '.step-3', '.step-4');

        if (idFromUrl == 'cart-order-box') {
            activeCartStep('.step-1', '.step-2');
        }
        if (idFromUrl == 'cart-payment-box') {
            activeCartStep('.step-1', '.step-2', '.step-3');
        }
        if (idFromUrl == 'cart-success-box') {
            activeCartStep('.step-1', '.step-2', '.step-3', '.step-4');
        }
    }
}

function hideCartInfo() {
    var cartInfoList = $('.cart-info').toArray();
    cartInfoList.forEach((item) => {
        $(item).removeClass('form-current');
    })
}

$(document).ready(() => {
    showCartInfo();

    $(window).on('hashchange', () => {
        showCartInfo();
    })
})