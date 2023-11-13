

//----Orders page --------------------------------------------------------
$('.dropdown-search-main').click(() => {
    $('.dropdown-search-list').toggleClass('show');
})

$('.order-search-value').click((e) => {
    var text = $(e.target).text();
    var value = $(e.target).data('search-select');

    $('.dropdown-search-selected').text(text);
    $('.dropdown-search-selected').data('search', value);
    $('.dropdown-search-list').removeClass('show');
})

//--Search orders

function appendOrderList(res) {
    var strHead = `<tr> <th>Mã ĐH</th><th>Tên khách hàng</th>
                        <th>Ngày đặt</th><th>Tổng tiền</th><th>H.thức TT</th>
                        <th>Trạng thái thanh toán</th><th></th></tr>`;
    if (res.length > 0) {
        $('.order-list table tbody').append(strHead);
        for (var i = 0; i < res.length; i++) {
            var date = new Date(res[i].OrderDate);
            var dateFormat = date.toLocaleDateString('en-GB') + ' ' + date.toLocaleTimeString('en-US');

            var statusClass = "order-success";
            if (res[i].Status == "Thanh toán thất bại.") { statusClass = "order-failed"; }
            else if (res[i].Status == "Chờ thanh toán") { statusClass = "order-waiting"; }

            var str = `<tr>
                            <td><div class="order-id">${res[i].OrderID}</div></td>
                            <td><div class="cus-name">${res[i].CustomerName}</div></td>
                            <td><div class="order-date">${dateFormat}</div></td>
                            <td><div class="total-payment">${res[i].TotalPaymentAmout.toLocaleString('vi-VN') + 'đ'}</div></td>
                            <td><div class="order-payment">${res[i].PaymentMethod}</div></td>
                            <td><div class="order-status ${statusClass}">${res[i].Status}</div></td>
                            <td><div class="order-button-box d-flex justify-content-end flex-wrap gap-2">
                                <button class="order-btn order-print-btn" data-print-order="${res[i].OrderID}">In HĐ</button>
                                <button class="order-btn order-detail-btn" data-detail-order="${res[i].OrderID}">Chi tiết</button>
                                <button class="order-btn order-update-btn" data-update-order="${res[i].OrderID}">Sửa</button>
                                <button class="order-btn delete-order-btn" data-del-order="${res[i].OrderID}">Xóa</button>
                            </div></td>
                        </tr>`;

            $('.order-list table tbody').append(str);
        }
    }
    else {
        $('.order-list table tbody').append(strHead);
    }
}

$('.search-orders').submit((e) => {
    e.preventDefault();
    var searchType = $('.dropdown-search-selected').data('search');
    var searchVal = $('#search-orders').val();
    if (searchType.length > 0 && searchVal.length > 0) {
        $('.loading').css('display', 'grid');
        $('.order-list table tbody').empty();
        $.ajax({
            type: 'get',
            url: '/api/orders',
            data: {
                searchType: searchType,
                searchValue: searchVal
            },
            success: (res) => {
                hideLoading();
                appendOrderList(res);
            },
            error: (err) => { console.log(err) }
        })
    }
})

//---Get all orders -----
$('.get-all-order').click(() => {
    $('.loading').css('display', 'grid');
    $('.order-list table tbody').empty();
    $.ajax({
        type: 'get',
        url: '/api/orders',
        success: (res) => {
            hideLoading();
            appendOrderList(res);
        },
        error: (err) => { console.log(err) }
    })
})

//--Search orders by date range
$('.search-by-date-btn').click(() => {
    var dateFrom = $('#date-from').val();
    var dateTo = $('#date-to').val();

    if (dateFrom.length > 0 && dateTo.length > 0) {
        $('.loading').css('display', 'grid');
        $('.order-list table tbody').empty();
        $.ajax({
            type: 'get',
            url: '/api/orders',
            data: {
                dateFrom: dateFrom,
                dateTo: dateTo
            },
            success: (res) => {
                hideLoading();
                appendOrderList(res);
            },
            error: (err) => {
                console.log(err);
            }
        })
    }
})

//--Get order detail ------------------------
$('.close-order-info').click(() => {
    $('.order-infomation-wrapper').css('visibility', 'hidden');
    $('.order-infomation-box').removeClass('show');
})

$(document).on('click', '.order-detail-btn', (e) => {
    var orderID = $(e.target).data('detail-order');
    if (orderID.length > 0) {
        $('.loading').css('display', 'grid');
        $.ajax({
            tpe: 'get',
            url: '/api/orders',
            data: { id: orderID },
            success: (data) => {
                var date = new Date(data.OrderDate);
                var dateFormat = date.toLocaleDateString('en-GB') + ' ' + date.toLocaleTimeString('en-US');

                $('.order-info-header').text('Chi tiết đơn hàng - ' + data.OrderID)
                $('.order-info-id').text(data.OrderID);
                $('.order-info-date').text(dateFormat);
                $('.order-info-payment').text(data.PaymentMethod);
                $('.order-info-ship').text(data.ShipMethod);
                $('.order-info-note').text(data.Note);
                $('.order-info-total').text(data.TotalPrice.toLocaleString('vi-VN') + 'đ');
                $('.order-info-ship-total').text(data.DeliveryFee.toLocaleString('vi-VN') + 'đ');
                $('.order-info-totalpay').text(data.TotalPaymentAmout.toLocaleString('vi-VN') + 'đ');  
                $('.order-info-status').text(data.Status);

                $.ajax({
                    type: 'get',
                    url: '/api/orders',
                    data: { orderID: data.OrderID },
                    success: (data1) => {
                        hideLoading();
                        $('.order-products-info table tbody').empty();
                        var strH = ` <tr>
                                    <th>Số lượng</th>
                                    <th>Mã sản phẩm</th>
                                    <th>Tên sản phẩm</th>
                                    <th>Giá bán</th>
                                    <th>Thành tiền</th>
                                </tr>`;
                        var str = ``;
                        if (data1.length > 0) {
                            for (var i = 0; i < data1.length; i++) {
                                str += `<tr>
                                            <td>${data1[i].Quantity}</td>
                                            <td>${data1[i].Product.ProductID}</td>
                                            <td>${data1[i].Product.ProductName}</td>
                                            <td>${data1[i].Product.Price.toLocaleString('vi-VN') + 'đ'}</td>
                                            <td class="fw-bold">${(data1[i].Product.Price * data1[i].Quantity).toLocaleString('vi-VN') + 'đ'}</td>
                                        </tr>`;
                            }

                            $('.order-info-cnt').text('Số sản phẩm - ' + data1.length);
                            $('.order-infomation-wrapper').css('visibility', 'visible');
                            $('.order-infomation-box').addClass('show');
                            $('.order-products-info table tbody').append(strH + str);
                        }
                    },
                    error: () => {  }
                })
            },
            error: () => { console.log('Error') }
        })
    }
})

//--Delete order
$(document).on('click', '.delete-order-btn', (e) => {
    $('.delete-order-confirm').css('visibility', 'visible');
    $('.delete-order-confirm .delete-confirm-box').addClass('show');
    //----------
    $('.cancel-delete').click(() => {
        $('.delete-order-confirm').css('visibility', 'hidden');
        $('.delete-order-confirm .delete-confirm-box').removeClass('show');
    })

    $('.delete-order-confirm .confirm-delete-order').click(() => {
        var orderID = $(e.target).data('del-order');
        if (orderID.length > 0) {

        }
    })
})