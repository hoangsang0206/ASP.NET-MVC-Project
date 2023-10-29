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