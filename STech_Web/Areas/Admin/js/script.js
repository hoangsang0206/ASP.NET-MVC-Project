$('.toggle').click(() => {
    $('.sidebar').toggleClass('close');
})

// --- Admin login with ajax ----------------------------------------
$('.admin-login-form').submit((e) => {
    e.preventDefault();
    var userName = $('#admin-username').val();
    var password = $('#admin-password').val();

    $.ajax({
        type: 'POST',
        url: '/account/login',
        data: {
            Username: userName,
            Password: password
        },
        success: (response) => {
            if (response.success) {
                $('.form-error').hide();
                $('.form-error').empty();
                $('.admin-login-form').unbind('submit').submit();
                window.location.href = response.redirectUrl;
            }
            else {
                var str = '<ul>';
                $.each(response.errors, (index, value) => {
                    str += `<li>
                        <i class="fa-solid fa-circle-exclamation" ></i>`
                        + value + '</li>';
                })

                str += '</li>';

                $('.form-error').show();
                $('.form-error').empty();
                $('.form-error').append(str);
            }
        },
        error: (err) => { console.log(err) }
    })
})

//-------------------------------------------------------
$('.dashboard-categories').click(() => {
    window.location.href = '/admin/categories';
})

//--AJAX get list brand in category --------------------
$('.reload-brands').click(() => {
    $.ajax({
        beforeSend: () => {
            $('.categories-list .loading').css('display', 'grid');
        },
        type: 'GET',
        url: '/api/categories',
        success: (responses) => {
            if (responses != null) {
                $('.categories-list').empty();
                for (var i = 0; i < responses.length; i++) {
                    var str = `
                        <div>
                            <input type="radio" id="category-${i+1}" name="category" value="${responses[i].CateID}" />
                            <div class="categories-label-box">
                                <label for="category-${i+1}">${responses[i].CateName}</label>
                            </div>
                        </div>
                    `;

                    $('.categories-list').append(str);
                }
            }
        },
        complete: () => {
            $('.categories-list .loading').hide();
        },
        error: (err) => { console.log(err) }
    })
})

//--AJAX get list product in category --------------------

$('.remove-action-btn').click(() => {
    $('.products-cate-list').empty();
    $('input[name="category"]:checked').prop('checked', false);
})

$('input[name="category"]').on('change', (e) => {
    var categoryValue = $(e.target).val();

    $.ajax({
        beforeSend: () => {
            $('.products-cate-list .loading').css('display', 'grid');
        },
        type: 'GET',
        url: '/api/products',
        data: {
            cateID: categoryValue
        },
        success: (responses) => {
            if (responses != null) {
                $('.products-cate-list').empty();
                for (var i = 0; i <= responses.length; i++) {
                    if (responses[i] != null) {
                        var str = `<div class="product-box-container">
                            <div class="product-box">
                                <div class="product-image">
                                    <img src="${responses[i].ImgSrc}" class="product-img">
                                </div>
                                <div class="product-name">
                                    ${responses[i].ProductName}
                                </div>
                                <div class="product-original-price">
                                    ${responses[i].Cost !== 0 ? responses[i].Cost.toLocaleString("vi-VN") + 'đ' :
                                        `<span style="visibility: hidden">0</span>`
                                    }
                                </div>
                                <div class="product-price d-flex align-items-center">
                                    ${responses[i].Price.toLocaleString("vi-VN") + 'đ'}
                                </div>
                            </div>
                        </div>`;

                        $('.products-cate-list').append(str);
                    }
                }
            }
        },
        complete: () => {
            $('.products-cate-list .loading').hide();
        },
        error: (err) => {
            console.log(err);
        }
    })
})