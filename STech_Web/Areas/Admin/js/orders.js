

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
                        <th>Ngày đặt</th><th>Tổng tiền</th><th>H.thức thanh toán</th>
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

//--Get order detail



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