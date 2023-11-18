//--AJAX get list category --------------------
function reloadCategories() {
   showLoading();
    $.ajax({
        type: 'GET',
        url: '/api/categories',
        success: (responses) => {
            hideLoading();
            if (responses != null) {
                $('.categories-list').empty();
                for (var i = 0; i < responses.length; i++) {
                    var str = `
                        <div>
                            <input type="radio" id="category-${i + 1}" name="category" value="${responses[i].CateID}" />
                            <div class="categories-label-box">
                                <label for="category-${i + 1}">${responses[i].CateName}</label>
                            </div>
                        </div>
                    `;

                    $('.categories-list').append(str);
                }
            }
        },
        error: (err) => {
            hideLoading();
            console.log(err)
        }
    })
}

$('.reload-categories').click(() => {
    reloadCategories();
})

//--AJAX get list product in category --------------------

$('.remove-action-btn').click(() => {
    showLoading();

    $('.products-cate-list').empty();
    $('.products-cate-list').css('height', 'auto');
    $('input[name="category"]:checked').prop('checked', false);
    var str1 = "Sản phẩm";
    $('.products-in-category .box-header h5').empty();
    $('.products-in-category .box-header h5').append(str1);

    hideLoading();
})

//Hide loading --
function hideLoading() {
    var interval = setInterval(() => {
        $('.loading').hide();
        clearInterval(interval);
    }, 600);
}

function showLoading() {
    $('.loading').css('display', 'grid');
}

$(document).on('change', 'input[name = "category"]', (e) => {
    var categoryValue = $(e.target).val();

   showLoading();

    $.ajax({
        type: 'GET',
        url: '/api/products',
        data: {
            cateID: categoryValue
        },
        success: (responses) => {
            hideLoading();
            if (responses != null) {
                $('.products-cate-list').empty();
                var str1 = "Sản phẩm - " + responses.length;
                $('.products-in-category .box-header h5').empty();
                $('.products-in-category .box-header h5').append(str1);

                if (responses.length > 14) {
                    $('.products-cate-list').css('height', '40rem');
                }
                else {
                    $('.products-cate-list').css('height', 'auto');
                }

                for (var i = 0; i <= responses.length; i++) {
                    if (responses[i] != null) {
                        var str2 = `<div class="product-box-container">
                        <div class="product-box position-relative">
                        ${responses[i].Quantity <= 0 ? `<div class="product-oot">
                                                    <span>Hết hàng</span>
                                                </div>` : ""}
                            <a href="/admin/product/${responses[i].ProductID}" class="product-link">
                                <div class="product-image lazy-loading">
                                    <img lazy-src="${responses[i].ImgSrc != null ? responses[i].ImgSrc : '/images/no-image.jpg'}" class="product-img">
                                </div>
                            </a>
                            <a href="/admin/product/${responses[i].ProductID}" class="product-link">
                                <div class="product-name">
                                    ${responses[i].ProductName}
                                </div>
                            </a>
                            <div class="product-original-price">
                                ${responses[i].Cost > responses[i].Price ? responses[i].Cost.toLocaleString("vi-VN") + 'đ' :
                                `<span style="visibility: hidden">0</span>`
                            }
                            </div>
                            <div class="product-price d-flex align-items-center">
                                ${responses[i].Price.toLocaleString("vi-VN") + 'đ'}
                            </div>
                            <button class="update-product-btn product-hidden-btn" onclick="window.location.href='/admin/product/${responses[i].ProductID}'">
                                <i class="fa-solid fa-screwdriver-wrench"></i>
                            </button>
                            <button class="delete-product-btn product-hidden-btn" data-product-id="${responses[i].ProductID}">
                                <i class="fa-solid fa-trash"></i>
                            </button>
                        </div>
                    </div>`;

                        $('.products-cate-list').append(str2);
                    }
                }

                lazyLoading();
            }
            else {
                var str1 = "Sản phẩm thuộc danh mục";
                $('.products-in-category .box-header h5').empty();
                $('.products-in-category .box-header h5').append(str1);
            }
        },
        error: (err) => {
            hideLoading();
            console.log(err);
        }
    })
})


//---------Add, update, detele category --------------------------------------
//Add category ---------------------------------------------------
$('.add-category-btn').click(() => {
    $('.add-category').css('visibility', 'visible');
    $('.add-category .form-box').addClass('show');
})

$('.close-add-category').click(() => {
    $('.add-category').css('visibility', 'hidden');
    $('.add-category .form-box').removeClass('show');
})

$('.add-category').click((e) => {
    if ($(e.target).closest('.form-box').length <= 0) {
        $('.add-category').css('visibility', 'hidden');
        $('.add-category .form-box').removeClass('show');
    }
})

$('.add-category-form').submit((e) => {
    e.preventDefault();
    var cateID = $('#category-id').val();
    var cateName = $('#category-name').val();
   showLoading();
    $.ajax({
        type: 'POST',
        url: '/admin/categories/addcategory',
        data: {
            CateID: cateID,
            CateName: cateName
        },
        success: (response) => {
            hideLoading();
            if (response.success) {
                checkInputValid($('#category-id'));
                checkInputValid($('#category-name'));
                var interval = setInterval(() => {
                    showOkForm();
                    $('#category-id').val('');
                    $('#category-name').val('');
                    clearInterval(interval);
                }, 620);
            }
            else {
                var str = `<span>
                    <i class="fa-solid fa-circle-exclamation error-icon"></i>
                    ${response.error}
                </span>`;
                $('.add-category .form-error').show();
                $('.add-category .form-error').empty();
                $('.add-category .form-error').append(str);
            }
        },
        error: () => {
            hideLoading();
        }
    })
})

//Delete category ----------------------------------------------------
$('.delete-cate-confirm').click((e) => {
    if ($(e.target).closest('.delete-cate-confirm-box').length <= 0) {
        $('.delete-cate-confirm').css('visibility', 'hidden');
        $('.delete-cate-confirm .delete-cate-confirm-box').removeClass('show');
    }
})
function hideCateNotice() {
    $('.categories-action-notice').hide();
}

function showCateNotice() {
    $('.categories-action-notice').css('display', 'flex');
}

$('.delete-category-btn').click(() => {
    var cateID = $('input[name="category"]:checked').val();

    if (cateID == null) {
        var str = `<span>
            <i class="fa-solid fa-circle-exclamation error-icon"></i>
            Chọn danh mục muốn xóa từ danh sách phía dưới.
        </span>`
        $('.categories-action-notice').empty();
        showCateNotice();
        $('.categories-action-notice').append(str);

        var interval = setInterval(() => {
            hideCateNotice();
            clearInterval(interval);
        }, 4000);

    }
    else {

        $('.delete-cate-confirm').css('visibility', 'visible');
        $('.delete-cate-confirm-box').addClass('show');
    }
})

$('.delete-cate-confirm-no').click(() => {
    $('.delete-cate-confirm').css('visibility', 'hidden');
    $('.delete-cate-confirm-box').removeClass('show');
})

$('.delete-cate-confirm-yes').click(() => {
    var cateID = $('input[name="category"]:checked').val();
   showLoading();
    $.ajax({
        type: 'POST',
        url: '/admin/categories/deletecategory',
        data: {
            CateID: cateID
        },
        success: (response) => {
            hideLoading();
            if (response.success) {
                var str = `<span>Xóa thành công.</span>`
                $('.categories-action-notice').empty();
                showCateNotice();
                $('.categories-action-notice').append(str);
                $('.delete-cate-confirm').css('visibility', 'hidden');
                $('.delete-cate-confirm-box').removeClass('show');
                reloadCategories();

                var interval = setInterval(() => {
                    hideCateNotice();
                    clearInterval(interval);
                }, 6000);
            }
            else {
                var str = `<span>
                    <i class="fa-solid fa-circle-exclamation error-icon"></i>
                    ${response.err}
                    </span>`
                $('.categories-action-notice').empty();
                showCateNotice();
                $('.categories-action-notice').append(str);
                $('.delete-cate-confirm').css('visibility', 'hidden');
                $('.delete-cate-confirm-box').removeClass('show');

                var interval = setInterval(() => {
                    hideCateNotice();
                    clearInterval(interval);
                }, 4000);
            }
        },
        error: () => {
            hideLoading();
        }
    })
})

//Update categories ---------------------------------------------
$('#update-category-id').on('keydown', (e) => {
    e.preventDefault();
})

function setCateValue() {
    var cateID = $('input[name="category"]:checked').val();
    var cateName = $('input[name="category"]:checked ~ .categories-label-box label').text();

    $('#update-category-id').val('');
    $('#update-category-name').val('');
    $('.update-category .form-error').hide();
    $('.update-category .form-error').empty();
    $('.update-category .form-submit-btn').prop('disabled', false);

    if (cateID == null) {
        var str = `<span>
            <i class="fa-solid fa-circle-exclamation error-icon"></i>
            Vui lòng chọn danh mục muốn sửa từ danh sách
        </span>`;
        $('.update-category .form-error').show();
        $('.update-category .form-error').empty();
        $('.update-category .form-error').append(str);
        $('.update-category .form-submit-btn').prop('disabled', true);

        return;
    }

    $('#update-category-id').val(cateID);
    $('#update-category-name').val(cateName);
    checkInputValid($('#update-category-id'));
    checkInputValid($('#update-category-name'));
}

$('.update-category-btn').click(() => {
    $('.update-category').css('visibility', 'visible');
    $('.update-category .form-box').addClass('show');
    setCateValue();
})

$('.close-update-category').click(() => {
    $('.update-category').css('visibility', 'hidden');
    $('.update-category .form-box').removeClass('show');
})

$('.update-category').click((e) => {
    if ($(e.target).closest('.form-box').length <= 0) {
        $('.update-category').css('visibility', 'hidden');
        $('.update-category .form-box').removeClass('show');
    }
})

$('.update-category-form').submit((e) => {
    e.preventDefault();
    var cateID = $('#update-category-id').val();
    var cateName = $('#update-category-name').val();
   showLoading();
    $.ajax({
        type: 'POST',
        url: '/admin/categories/updatecategory',
        data: {
            CateID: cateID,
            CateName: cateName
        },
        success: (response) => {
            hideLoading();
            if (response.success) {
                var interval = setInterval(() => {
                    showUpdateOkForm();
                    clearInterval(interval);
                }, 620);
            }
            else {
                var str = `<span>${response.error}</span>`;
                $('.update-category .form-error').show();
                $('.update-category .form-error').empty();
                $('.update-category .form-error').append(str);
            }
        },
        error: () => {
            hideLoading();
        }
    })
})