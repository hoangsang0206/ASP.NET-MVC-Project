//--Lazy loading -------------------------------------------------
function loadImg(img) {
    const url = img.getAttribute('lazy-src');

    img.removeAttribute('lazy-src');
    img.setAttribute('src', url);
    img.classList.add('img-loaded');
    //img.parentNode.classList.remove('lazy-loading');
}

function lazyLoading() {
    if ('IntersectionObserver' in window) {
        var lazyImages = document.querySelectorAll('[lazy-src]');
        let observer = new IntersectionObserver((entries => {
            entries.forEach(entry => {
                if (entry.isIntersecting && !entry.target.classList.contains('img-loaded')) {
                    loadImg(entry.target);
                }
            })
        }));

        lazyImages.forEach(img => {
            observer.observe(img);
        });
    }
}

if (window.innerWidth < 768) {
    $('.sidebar').addClass('close');
}

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
    $('.loading').css('display', 'grid');
    reloadCategories();   
    hideLoading();
})

//--AJAX get list product in category --------------------

$('.remove-action-btn').click(() => {
    $('.loading').css('display', 'grid');

    $('.products-cate-list').empty();
    $('.products-cate-list').css('height', 'auto');
    $('input[name="category"]:checked').prop('checked', false);
    var str1 = "Sản phẩm thuộc danh mục";
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

//$('input[name="category"]').on('change', (e) => {
$(document).on('change', 'input[name = "category"]', (e) => {
    var categoryValue = $(e.target).val();

    $('.loading').css('display', 'grid');

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
                var str1 = "Sản phẩm thuộc danh mục - " + responses.length;
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
                            <div class="product-image lazy-loading">
                                <img lazy-src="${responses[i].ImgSrc}" class="product-img">
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
                            <button class="update-product-btn product-hidden-btn">
                                <i class="fa-solid fa-trash"></i>
                            </button>
                            <button class="delete-product-btn product-hidden-btn">
                                <i class="fa-solid fa-screwdriver-wrench"></i>
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
                var str = `<span>
                    <i class="fa-solid fa-circle-exclamation error-icon"></i>
                    ${response.error}
                </span>`;
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

    $.ajax({
        type: 'POST',
        url: '/admin/categories/deletecategory',
        data: {
            CateID: cateID
        },
        success: (response) => {
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
                }, 4000);
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
//Get product list ---------------------------------------------------
//Reload ----------------------------
$('.reload-products').click(() => {
    $('.loading').css('display', 'grid');

    $('.product-list').empty();
    $('.total-result').empty();
    $('.product-list').css('height', '20rem');
    $('#search').val('');
    $('.get-product-by-cate, .get-product-by-brand').prop('selectedIndex', 0);

    hideLoading();
})
//Search ----------------------------
$('.search-products').submit((e) => {
    e.preventDefault();
    var searchText = $('#search').val();

    if (searchText.length > 0) {
        $('.loading').css('display', 'grid');
        $('.get-product-by-cate, .get-product-by-brand').prop('selectedIndex', 0);
        $.ajax({
            type: 'GET',
            url: '/api/products',
            data: {
                name: searchText
            },
            success: (responses) => {
                hideLoading();
                if (responses != null) {
                    $('.product-list').empty();
                    if (responses.length > 14) {
                        $('.product-list').css('height', '40rem');
                    }
                    else if (responses.length <= 0) {
                        $('.product-list').css('height', '20rem');
                    }
                    else {
                        $('.product-list').css('height', 'auto');
                    }

                    var str1 = "Tìm thấy: " + responses.length + " sản phẩm";
                    $('.total-result').empty();
                    $('.total-result').append(str1);

                    for (var i = 0; i <= responses.length; i++) {
                        if (responses[i] != null) {
                            var str2 = `<div class="product-box-container">
                                            <div class="product-box position-relative">
                                                <div class="product-image lazy-loading">
                                                    <img lazy-src="${responses[i].ImgSrc}" class="product-img">
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
                                                <button class="update-product-btn product-hidden-btn">
                                                    <i class="fa-solid fa-trash"></i>
                                                </button>
                                                <button class="delete-product-btn product-hidden-btn">
                                                    <i class="fa-solid fa-screwdriver-wrench"></i>
                                                </button>
                                            </div>
                                        </div>`;

                            $('.product-list').append(str2);
                        }
                    }

                    lazyLoading();
                }
                else {
                    
                }
            },
            error: (err) => {
                hideLoading();
                console.log(err);
            }
        })
    }
})

//Get all product -------------------
$('.get-all-poduct').click(() => {
    $('.loading').css('display', 'grid');
    $('#search').val('');
    $('.get-product-by-cate, .get-product-by-brand').prop('selectedIndex', 0);
    $.ajax({
        type: 'GET',
        url: '/api/products',
        success: (responses) => {
            hideLoading();
            if (responses != null) {
                $('.product-list').empty();
                if (responses.length > 14) {
                    $('.product-list').css('height', '40rem');
                }
                else if (responses.length > 7) {
                    $('.product-list').css('height', '36.5rem');
                }
                else {
                    $('.product-list').css('height', '20rem');
                }

                var str1 = "Tìm thấy: " + responses.length + " sản phẩm";
                $('.total-result').empty();
                $('.total-result').append(str1);

                for (var i = 0; i <= responses.length; i++) {
                    if (responses[i] != null) {
                        var str2 = `<div class="product-box-container">
                                            <div class="product-box position-relative">
                                                <div class="product-image lazy-loading">
                                                    <img lazy-src="${responses[i].ImgSrc}" class="product-img">
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
                                                <button class="update-product-btn product-hidden-btn">
                                                    <i class="fa-solid fa-trash"></i>
                                                </button>
                                                <button class="delete-product-btn product-hidden-btn">
                                                    <i class="fa-solid fa-screwdriver-wrench"></i>
                                                </button>
                                            </div>
                                        </div>`;

                        $('.product-list').append(str2);
                    }
                }

                lazyLoading();
            }
            else {

            }
        },
        error: (err) => {
            hideLoading();
            console.log(err);
        }
    })
})

//Get product by category or by brand
$('#cateID, #brandID').on('change', () => {
    var cateID = $('#cateID').val();
    var brandID = $('#brandID').val();
    $('#search').val('');
    $('.loading').css('display', 'grid');

    $.ajax({
        type: 'GET',
        url: '/api/products',
        data: {
            CateID: cateID,
            BrandID: brandID
        },
        success: (responses) => {
            hideLoading();
            if (responses != null) {
                $('.product-list').empty();
                if (responses.length > 14) {
                    $('.product-list').css('height', '40rem');
                }
                else if (responses.length > 7) {
                    $('.product-list').css('height', '36.5rem');
                }
                else {
                    $('.product-list').css('height', '20rem');
                }

                var str1 = "Tìm thấy: " + responses.length + " sản phẩm";
                $('.total-result').empty();
                $('.total-result').append(str1);

                for (var i = 0; i <= responses.length; i++) {
                    if (responses[i] != null) {
                        var str2 = `<div class="product-box-container">
                                            <div class="product-box position-relative">
                                                <div class="product-image lazy-loading">
                                                    <img lazy-src="${responses[i].ImgSrc}" class="product-img">
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
                                                <button class="update-product-btn product-hidden-btn">
                                                    <i class="fa-solid fa-trash"></i>
                                                </button>
                                                <button class="delete-product-btn product-hidden-btn">
                                                    <i class="fa-solid fa-screwdriver-wrench"></i>
                                                </button>
                                            </div>
                                        </div>`;

                        $('.product-list').append(str2);
                    }
                }

                lazyLoading();
            }
            else {

            }
        },
        error: (err) => {
            hideLoading();
            console.log(err);
        }
    })
})

//Add product --------------------------------------------------------