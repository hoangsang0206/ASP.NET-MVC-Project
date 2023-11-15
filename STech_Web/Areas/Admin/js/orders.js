

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
                        <th>Ngày đặt</th><th>Tổng tiền</th><th>Trạng thái thanh toán</th>
                        <th>Trạng thái</th><th></th></tr>`;
    if (res.length > 0) {
        $('.order-list table tbody').append(strHead);
        for (var i = 0; i < res.length; i++) {
            var date = new Date(res[i].OrderDate);
            var dateFormat = date.toLocaleDateString('en-GB') + ' ' + date.toLocaleTimeString('en-US');

            var statusClass = "order-success";
            if (res[i].PaymentStatus == "Thanh toán thất bại") { statusClass = "order-failed"; }
            else if (res[i].PaymentStatus == "Chờ thanh toán") { statusClass = "order-waiting"; }

            var str = `<tr>
                            <td><div class="order-id">${res[i].OrderID}</div></td>
                            <td><div class="cus-name">${res[i].CustomerName}</div></td>
                            <td><div class="order-date">${dateFormat}</div></td>
                            <td><div class="total-payment">${res[i].TotalPaymentAmout.toLocaleString('vi-VN') + 'đ'}</div></td>
                            <td><div class="order-pstatus d-flex ${statusClass}">${res[i].PaymentStatus} ${res[i].PaymentStatus == 'Chờ thanh toán' ? `&nbsp;<button class='order-btn accept-paid' data-accept-paid="${res[i].OrderID}"><i class='bx bx-check'></i></button>` : ''}</div></td>
                            <td><div class="order-status ${res[i].Status == 'Đã xác nhận' ? 'order-success' : res[i].Status == 'Chờ xác nhận' ? 'order-waiting' : 'order-failed'}">${res[i].Status}</div></td>
                            <td><div class="order-button-box d-flex justify-content-end flex-wrap gap-2">
                                <button class="order-btn order-print-btn" data-print-order="${res[i].OrderID}">In HĐ</button>
                                <button class="order-btn order-detail-btn" data-detail-order="${res[i].OrderID}">Chi tiết</button>
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
       showLoading();
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
   showLoading();
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
       showLoading();
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

//Func append order waiting list
function appendOrderWaitingList(res) {
    var strHead = `<tr> <th>Mã ĐH</th><th>Tên khách hàng</th>
                    <th>Ngày đặt</th><th>Tổng tiền</th><th>Trạng thái thanh toán</th>
                    <th>Trạng thái</th><th></th></tr>`;
    $('.order-waiting-list table tbody').append(strHead);
    if (res.length > 0) {
        for (var i = 0; i < res.length; i++) {
            var date = new Date(res[i].OrderDate);
            var dateFormat = date.toLocaleDateString('en-GB') + ' ' + date.toLocaleTimeString('en-US');

            var statusClass = "order-success";
            if (res[i].PaymentStatus == "Thanh toán thất bại") { statusClass = "order-failed"; }
            else if (res[i].PaymentStatus == "Chờ thanh toán") { statusClass = "order-waiting"; }

            var str = `<tr>
                                    <td><div class="order-id">${res[i].OrderID}</div></td>
                                    <td><div class="cus-name">${res[i].CustomerName}</div></td>
                                    <td><div class="order-date">${dateFormat}</div></td>
                                    <td><div class="total-payment">${res[i].TotalPaymentAmout.toLocaleString('vi-VN') + 'đ'}</div></td>
                                    <td><div class="order-pstatus d-flex ${statusClass}">${res[i].PaymentStatus} ${res[i].PaymentStatus == 'Chờ thanh toán' ? `&nbsp;<button class='order-btn accept-paid' data-accept-paid="${res[i].OrderID}"><i class='bx bx-check'></i></button>` : ''}</div></td>`;
            if (res[i].Status === 'Chờ xác nhận') {
                str += `<td><div class="order-status">
                        <button class="order-btn order-status-accept" data-accept-order="${res[i].OrderID}">Xác nhận</button>
                        <button class="order-btn order-status-refuse" data-refuse-order="${res[i].OrderID}">Hủy</button>
                    </div></td>`;
            }
            else if (res[i].Status === 'Đã xác nhận') {
                str += `<td><div class="order-status order-success">${res[i].Status}</div></td>`
            }
            else if (res[i].Status === 'Đã hủy'){
                str += `<td><div class="order-status order-failed">${res[i].Status}</div></td>`
            }

            str += `<td><div class="order-button-box d-flex justify-content-end flex-wrap gap-2">
                        <button class="order-btn order-detail-btn" data-detail-order="${res[i].OrderID}">Chi tiết</button>
                        <button class="order-btn delete-order-btn" data-del-order="${res[i].OrderID}">Xóa</button>
                    </div></td>
                </tr>`;

            $('.order-waiting-list table tbody').append(str);
        }
    }
}

//Get order with Status = "Chờ xác nhận"
$('.reload-orders').click(() => {
   showLoading();
    $('.order-waiting-list table tbody').empty();
    $.ajax({
        type: 'get',
        url: '/api/orders',
        data: {
            type: 'status',
            status: 'Chờ xác nhận'
        },
        success: (res) => {
            hideLoading();
            appendOrderWaitingList(res);

            $.ajax({
                type: 'get',
                url: '/admin/orders/countneworder',
                success: (data) => {
                    if (data.count > 0) {
                        $('.new-order-count').css('display', 'grid');
                        $('.new-order-count').text(data.count);
                    }
                    else {
                        $('.new-order-count').hide();
                    }
                }
            })
        },
        error: () => {  }
    })
})

//Get order with Status = "Đã xác nhận"
$('.get-accept-order').click(() => {
   showLoading();
    $('.order-waiting-list table tbody').empty();
    $.ajax({
        type: 'get',
        url: '/api/orders',
        data: {
            type: 'status',
            status: 'Đã xác nhận'
        },
        success: (res) => {
            hideLoading();
            appendOrderWaitingList(res);
        },
        error: () => { }
    })
})
//Get order with Status = "Đã hủy"
$('.get-refuse-order').click(() => {
   showLoading();
    $('.order-waiting-list table tbody').empty();
    $.ajax({
        type: 'get',
        url: '/api/orders',
        data: {
            type: 'status',
            status: 'Đã hủy'
        },
        success: (res) => {
            hideLoading();
            appendOrderWaitingList(res);
        },
        error: () => { }
    })
})
//Get order with payment status = "Thanh toán thành công"
$('.get-paid-order').click(() => {
   showLoading();
    $('.order-waiting-list table tbody').empty();
    $.ajax({
        type: 'get',
        url: '/api/orders',
        data: {
            type: 'payment-stt',
            status: 'Thanh toán thành công'
        },
        success: (res) => {
            hideLoading();
            appendOrderWaitingList(res);
        },
        error: () => { }
    })
})
//Get order with payment status = "Chờ thanh toán"
$('.get-notpaid-order').click(() => {
   showLoading();
    $('.order-waiting-list table tbody').empty();
    $.ajax({
        type: 'get',
        url: '/api/orders',
        data: {
            type: 'payment-stt',
            status: 'Chờ thanh toán'
        },
        success: (res) => {
            hideLoading();
            appendOrderWaitingList(res);
        },
        error: () => { }
    })
})

//Change order status ------------------
$(document).on('click', '.order-status-accept', (e) => {
    var orderID = $(e.target).data('accept-order');
    if (orderID.length > 0) {
       showLoading();
        $.ajax({
            type: 'post',
            url: '/admin/orders/updatestatus',
            data: {
                orderID: orderID,
                type: 'accept'
            },
            success: () => {
                hideLoading();
                $(e.target).closest('tr').remove();

                $.ajax({
                    type: 'get',
                    url: '/admin/orders/countneworder',
                    success: (data) => {
                        if (data.count > 0) {
                            $('.new-order-count').css('display', 'grid');
                            $('.new-order-count').text(data.count);
                        }
                        else {
                            $('.new-order-count').hide();
                        }
                    }
                })
            },
            error: () => {}
        })
    }
})

$(document).on('click', '.order-status-refuse', (e) => {
    var orderID = $(e.target).data('refuse-order');
    if (orderID.length > 0) {
       showLoading();
        $.ajax({
            type: 'post',
            url: '/admin/orders/updatestatus',
            data: {
                orderID: orderID,
                type: 'refuse'
            },
            success: () => {
                hideLoading();
                $(e.target).closest('tr').remove();

                $.ajax({
                    type: 'get',
                    url: '/admin/orders/countneworder',
                    success: (data) => {
                        if (data.count > 0) {
                            $('.new-order-count').css('display', 'grid');
                            $('.new-order-count').text(data.count);
                        }
                        else {
                            $('.new-order-count').hide();
                        }
                    }
                })
            },
            error: () => { }
        })
    }
})

//--Change order status to "Thanh toán thành công"
$(document).on('click', '.accept-paid', (e) => {
    var orderID = $(e.target).data('accept-paid');
    if (orderID.length > 0) {
        $('.payment-acception-confirm').css('visibility', 'visible');
        $('.payment-acception').addClass('show');

        $('.cancel-acception').click(() => {
            $('.payment-acception-confirm').css('visibility', 'hidden');
            $('.payment-acception').removeClass('show');
        })

        $('.confirm-acception').click(() => {
            $.ajax({
                type: 'post',
                url: '/admin/orders/acceptpaid',
                data: { orderID: orderID },
                success: (res) => {
                    if (res.success) {
                        $('.payment-acception-confirm').css('visibility', 'hidden');
                        $('.payment-acception').removeClass('show');
                    }
                }
            })
        })
    }
})

//--Get order detail ---------------------------------------
$('.close-order-info').click(() => {
    $('.order-infomation-wrapper').css('visibility', 'hidden');
    $('.order-infomation-box').removeClass('show');
})

$(document).on('click', '.order-detail-btn', (e) => {
    var orderID = $(e.target).data('detail-order');
    if (orderID.length > 0) {
       showLoading();
        $.ajax({
            tpe: 'get',
            url: '/api/orders',
            data: { id: orderID },
            success: (data) => {
                var date = new Date(data.OrderDate);
                var dateFormat = date.toLocaleDateString('en-GB') + ' ' + date.toLocaleTimeString('en-US');

                $('.order-info-header').text('Đơn hàng - ' + data.OrderID)
                $('.order-info-date').text(dateFormat);
                $('.order-info-payment').text(data.PaymentMethod);
                $('.order-info-ship').text(data.ShipMethod);
                $('.order-info-note').text(data.Note);
                $('.order-info-total').text(data.TotalPrice.toLocaleString('vi-VN') + 'đ');
                $('.order-info-ship-total').text(data.DeliveryFee.toLocaleString('vi-VN') + 'đ');
                $('.order-info-totalpay').text(data.TotalPaymentAmout.toLocaleString('vi-VN') + 'đ');  
                $('.order-info-pstatus').text(data.PaymentStatus);
                $('.order-info-status').text(data.Status);

                //Get list product in order
                $.ajax({
                    type: 'get',
                    url: '/api/orders',
                    data: { orderID: data.OrderID },
                    success: (data1) => {
                        hideLoading();
                        $('.order-products-info table tbody').empty();
                        var strH = ` <tr>
                                    <th>Mã sản phẩm</th>
                                    <th>Tên sản phẩm</th>
                                    <th>Giá bán</th>
                                    <th>Số lượng</th>
                                    <th>Thành tiền</th>
                                </tr>`;
                        var str = ``;
                        if (data1.length > 0) {
                            for (var i = 0; i < data1.length; i++) {
                                str += `<tr>
                                            <td>${data1[i].Product.ProductID}</td>
                                            <td>${data1[i].Product.ProductName}</td>
                                            <td>${data1[i].Product.Price.toLocaleString('vi-VN') + 'đ'}</td>
                                             <td>${data1[i].Quantity}</td>
                                            <td class="fw-bold">${(data1[i].Product.Price * data1[i].Quantity).toLocaleString('vi-VN') + 'đ'}</td>
                                        </tr>`;
                            }

                            $('.order-info-cnt').text('Số sản phẩm - ' + data1.length);
                            $('.order-products-info table tbody').append(strH + str);
                            setTimeout(() => {
                                $('.order-infomation-wrapper').css('visibility', 'visible');
                                $('.order-infomation-box').addClass('show');
                            }, 500)
                        }
                    }
                })

                //Get customer info
                $.ajax({
                    type: 'get',
                    url: '/api/users',
                    data: { customerID: data.CustomerID },
                    success: (data2) => {
                        $('.order-cus-id').text(data2.CustomerID);
                        $('.order-cus-name').text(data2.CustomerName);
                        $('.order-cus-phone').text(data2.Phone);
                        $('.order-cus-email').text(data2.Email);
                        $('.order-cus-address').text(data2.Address);
                    }
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
            $.ajax({
                type: 'post',
                url: '/admin/orders/deleteorder',
                data: {
                    orderID: orderID
                },
                success: (response) => {
                    if (response.success) {
                        $('.complete-delete-notice').css('visibility', 'visible');
                        $('.complete-notice-box').addClass('showForm');
                        $('.delete-order-confirm').css('visibility', 'hidden');
                        $('.delete-order-confirm .delete-confirm-box').removeClass('show');
                    }
                },
                error: () => { console.log('Không thể xóa đơn hàng') }
            })
        }
    })
})
