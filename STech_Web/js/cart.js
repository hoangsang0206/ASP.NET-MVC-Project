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
                console.log('them thanh cong')
            }
            else {
                console.log('them that bai')
            }
        }
    })
})