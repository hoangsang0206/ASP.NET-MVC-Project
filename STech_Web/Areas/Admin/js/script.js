//--Lazy loading -------------------------------------------------
//function loadImg(img) {
//    const url = img.getAttribute('lazy-src');

//    img.removeAttribute('lazy-src');
//    img.setAttribute('src', url);
//    img.classList.add('img-loaded');
//    //img.parentNode.classList.remove('lazy-loading');
//}

//function lazyLoading() {
//    if ('IntersectionObserver' in window) {
//        var lazyImages = document.querySelectorAll('[lazy-src]');
//        let observer = new IntersectionObserver((entries => {
//            entries.forEach(entry => {
//                if (entry.isIntersecting && !entry.target.classList.contains('img-loaded')) {
//                    loadImg(entry.target);
//                }
//            })
//        }));

//        lazyImages.forEach(img => {
//            observer.observe(img);
//        });
//    }
//}

//--------------------------------------------------------------------

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

$('.dashboard-products').click(() => {
    window.location.href = '/admin/products';
})

//--AJAX get list category --------------------
function reloadCategories() {
    $.ajax({
        type: 'GET',
        url: '/api/categories',
        success: (responses) => {
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
        error: (err) => { console.log(err) }
    })
}

$('.reload-categories').click(() => {
    reloadCategories();
})

//--AJAX get list product in category --------------------

$('.remove-action-btn').click(() => {
    $('.products-cate-list').empty();
    $('input[name="category"]:checked').prop('checked', false);
    var str1 = "Sản phẩm thuộc danh mục";
    $('.products-in-category .box-header h5').empty();
    $('.products-in-category .box-header h5').append(str1);
})

//$('input[name="category"]').on('change', (e) => {
$(document).on('change', 'input[name = "category"]', (e) => {
    var categoryValue = $(e.target).val();
    console.log(categoryValue)

    $.ajax({
        type: 'GET',
        url: '/api/products',
        data: {
            cateID: categoryValue
        },
        success: (responses) => {
            if (responses != null) {
                $('.products-cate-list').empty();
                var str1 = "Sản phẩm thuộc danh mục - " + responses.length;
                $('.products-in-category .box-header h5').empty();
                $('.products-in-category .box-header h5').append(str1);

                for (var i = 0; i <= responses.length; i++) {
                    if (responses[i] != null) {
                        var str2 = `<div class="product-box-container">
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

                        $('.products-cate-list').append(str2);
                    }
                }

                //lazyLoading();
            }
            else {
                var str1 = "Sản phẩm thuộc danh mục";
                $('.products-in-category .box-header h5').empty();
                $('.products-in-category .box-header h5').append(str1);
            }
        },
        error: (err) => {
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

    $.ajax({
        type: 'POST',
        url: '/admin/categories/addcategory',
        data: {
            CateID: cateID,
            CateName: cateName
        },
        success: (response) => {
            if (response.success) {
                $('.add-category-form').unbind('submit').submit();
            }
            else {
                var str = `<span>${response.error}</span>`;
                $('.add-category .form-error').show();
                $('.add-category .form-error').empty();
                $('.add-category .form-error').append(str);
            }
        }
    })
})

//Delete category ----------------------------------------------------
function hideCateNotice() {
    $('.categories-action-notice').hide();
}

$('.delete-category-btn').click(() => {
    var cateID = $('input[name="category"]:checked').val();

    if (cateID == null) {
        var str = `<span>Chọn danh mục muốn xóa từ danh sách phía dưới.</span>`
        $('.categories-action-notice').show();
        $('.categories-action-notice').empty();
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

    $.ajax({
        type: 'POST',
        url: '/admin/categories/deletecategory',
        data: {
            CateID: cateID
        },
        success: (response) => {
            if (response.success) {
                var str = `<span>Xóa thành công.</span>`
                $('.categories-action-notice').show();
                $('.categories-action-notice').empty();
                $('.categories-action-notice').append(str);
                $('.delete-cate-confirm').css('visibility', 'hidden');
                $('.delete-cate-confirm-box').removeClass('show');
                reloadCategories();

                var interval = setInterval(() => {
                    hideCateNotice();
                    clearInterval(interval);
                }, 4000);
            }
            else {
                var str = `<span>${response.err}</span>`
                $('.categories-action-notice').show();
                $('.categories-action-notice').empty();
                $('.categories-action-notice').append(str);
                $('.delete-cate-confirm').css('visibility', 'hidden');
                $('.delete-cate-confirm-box').removeClass('show');

                var interval = setInterval(() => {
                    hideCateNotice();
                    clearInterval(interval);
                }, 4000);
            }
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
        var str = `<span>Vui lòng chọn danh mục muốn sửa từ danh sách</span>`;
        $('.update-category .form-error').show();
        $('.update-category .form-error').empty();
        $('.update-category .form-error').append(str);
        $('.update-category .form-submit-btn').prop('disabled', true);

        return;
    }

    $('#update-category-id').val(cateID);
    $('#update-category-name').val(cateName);
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

    $.ajax({
        type: 'POST',
        url: '/admin/categories/updatecategory',
        data: {
            CateID: cateID,
            CateName: cateName
        },
        success: (response) => {
            if (response.success) {
                $('.update-category-form').unbind('submit').submit();
            }
            else {
                var str = `<span>${response.error}</span>`;
                $('.update-category .form-error').show();
                $('.update-category .form-error').empty();
                $('.update-category .form-error').append(str);
            }
        }
    })
})

//---------Add, update, detele product --------------------------------------
//Add category ---------------------------------------------------