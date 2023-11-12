﻿//--Lazy loading -------------------------------------------------
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



//---------Add, update, detele product --------------------------------------
//Get product list ---------------------------------------------------
//Reload ----------------------------
$('.reload-products').click(() => {
    $('.loading').css('display', 'grid');
    hideLoading();
    $('.product-list').empty();
    $('.total-result').empty();
    $('.product-list').css('height', '27rem');
    $('#search').val('');
    $('.get-product-by-cate, .get-product-by-brand').prop('selectedIndex', 0);
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
                        $('.product-list').css('height', '27rem');
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
                                                <a href="/admin/product/${responses[i].ProductID}" class="product-link">
                                                    <div class="product-image lazy-loading">
                                                        <img lazy-src="${responses[i].ImgSrc}" class="product-img">
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
$('.get-all-product').click(() => {
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
                                                <a href="/admin/product/${responses[i].ProductID}" class="product-link">
                                                    <div class="product-image lazy-loading">
                                                        <img lazy-src="${responses[i].ImgSrc}" class="product-img">
                                                    </div>
                                                </a>
                                                <a href="/admin/product/${responses[i].ProductID}" class="product-link">
                                                    <div class="product-name">
                                                        ${responses[i].ProductName}
                                                    </div>
                                                </a>
                                                <div class="product-original-price">
                                                    ${responses[i].Cost > responses[i].Price ? responses[i].Cost.toLocaleString("vi-VN") + 'đ' :
                                `<span style="visibility: hidden">0</span>`}
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
                                                <a href="/admin/product/${responses[i].ProductID}" class="product-link">
                                                    <div class="product-image lazy-loading">
                                                        <img lazy-src="${responses[i].ImgSrc}" class="product-img">
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
//Check quantity > 0
$('#add-product-quantity').keyup(() => {
    var inputVal = $('#add-product-quantity').val();
    if (isNaN(inputVal)) {
        var str = `<span>
            <i class="fa-solid fa-circle-exclamation error-icon"></i>
            Số lượng không hợp lệ.
        </span>`;
        $('.add-product .form-error').empty();
        $('.add-product .form-error').show();
        $('.add-product .form-error').append(str);
    } else if (inputVal < 0) {
        var str = `<span>
            <i class="fa-solid fa-circle-exclamation error-icon"></i>
            Số lượng không được nhỏ hơn 0.
        </span>`;
        $('.add-product .form-error').empty();
        $('.add-product .form-error').show();
        $('.add-product .form-error').append(str);
    }
    else {
        $('.add-product .form-error').empty();
        $('.add-product .form-error').hide();
    }
})

//--Show/hide form product--------------------
$('.add-product-btn').click(() => {
    $('.add-product').css('visibility', 'visible');
    $('.product-form-container').addClass('showForm');
})

$('.close-product-form').click(() => {
    $('.add-product').css('visibility', 'hidden');
    $('.product-form-container').removeClass('showForm');
})

function showOkForm() {
    $('.complete-notice').css('visibility', 'visible');
    $('.complete-notice .complete-notice-box').addClass('showForm');

    $('.complete-notice-box button').click(() => {
        $('.complete-notice').css('visibility', 'hidden');
        $('.complete-notice .complete-notice-box').removeClass('showForm');
    })
}

function showUpdateOkForm() {
    $('.complete-update-notice').css('visibility', 'visible');
    $('.complete-update-notice .complete-notice-box').addClass('showForm');

    $('.complete-notice-box button').click(() => {
        $('.complete-update-notice').css('visibility', 'hidden');
        $('.complete-update-notice .complete-notice-box').removeClass('showForm');
    })
}

//Update image box
function updateImgBox(input, imgbox) {
    input.mouseleave(() => {
        var imgVal = input.val();
        if (imgVal.length > 0) {
            imgbox.attr('src', imgVal);
        }
        else {
            imgbox.attr('src', '/images/no-image.jpg');
        }
    })
}

$(document).ready(() => {
    updateImgBox($('#ImgSrc'), $('.p-img-1'));
    updateImgBox($('#add-product-image-1'), $('.p-img-2'));
    updateImgBox($('#add-product-image-2'), $('.p-img-3'));
    updateImgBox($('#add-product-image-3'), $('.p-img-4'));
    updateImgBox($('#add-product-image-4'), $('.p-img-5'));
    updateImgBox($('#add-product-image-5'), $('.p-img-6'));
})

$(document).ready(() => {
    var imageList = $('.product-detail .product-detail-image img').toArray();
    updateImgBox($('.product-detail #ImgSrc'), $(imageList[0]));
    updateImgBox($('.product-detail #add-product-image-1'), $(imageList[1]));
    updateImgBox($('.product-detail #add-product-image-2'), $(imageList[2]));
    updateImgBox($('.product-detail #add-product-image-3'), $(imageList[3]));
    updateImgBox($('.product-detail #add-product-image-4'), $(imageList[4]));
    updateImgBox($('.product-detail #add-product-image-5'), $(imageList[5]));
})

//Add product --------------------------------
$('.add-product-form').submit((e) => {
    var productID = $('.add-product #ProductID').val();
    var productName = $('.add-product #ProductName').val();
    var productCost = $('.add-product #Cost').val();
    var productPrice = $('.add-product #Price').val();
    var cateID = $('.add-product #CateID').val();
    var brandID = $('.add-product #BrandID').val();
    var imgSrc = $('.add-product #ImgSrc').val();
    var warranty = $('.add-product #Warranty').val();
    var quantity = $('.add-product #add-product-quantity').val();
    var imgSrc1 = $('.add-product #add-product-image-1').val();
    var imgSrc2 = $('.add-product #add-product-image-2').val();
    var imgSrc3 = $('.add-product #add-product-image-3').val();
    var imgSrc4 = $('.add-product #add-product-image-4').val();
    e.preventDefault();

    //Clear input value
    function clearInputVal() {
        const inputFormArr = $('.add-product input').toArray();
        inputFormArr.forEach((input) => {
            $(input).val('');
            checkInputValid($(input));
        })

        const selectArr = $('.add-product select').toArray();
        selectArr.forEach((select) => {
            $(select).prop('selectedIndex', 0);
        })
    }
    $('.loading').css('display', 'grid');
    $.ajax({
        type: 'POST',
        url: '/admin/products/addproduct',
        data: {
            'ProductID': productID,
            'ProductName': productName,
            'Cost': productCost,
            'Price': productPrice,
            'CateID': cateID,
            'BrandID': brandID,
            'ImgSrc': imgSrc,
            'Warranty': warranty,
            quantity: quantity,
            imgSrc1: imgSrc1,
            imgSrc2: imgSrc2,
            imgSrc3: imgSrc3,
            imgSrc4: imgSrc4
        },
        success: (responses) => {
            hideLoading();
            if (responses.success) {
                $('.add-product .form-error').empty();
                $('.add-product .form-error').hide();
                var interval = setInterval(() => {
                    clearInputVal();
                    showOkForm();
                    clearInterval(interval);
                }, 620);
            }
            else {
                var str = `<span>
                        <i class="fa-solid fa-circle-exclamation error-icon"></i>
                        ${responses.error}
                    </span>`;
                $('.add-product .form-error').empty();
                $('.add-product .form-error').show();
                $('.add-product .form-error').append(str);

                var interval = setInterval(() => {
                    $('.add-product .form-error').empty();
                    $('.add-product .form-error').hide();
                    clearInterval(interval)
                }, 8000)
            }
        },
        error: (err) => {
            console.log(err);
            hideLoading();
        }
    })
})

//----
$('.add-product-form').on('reset', () => {
    const inputFormArr = $('.add-product input').toArray();
    inputFormArr.forEach((input) => {
        $(input).val('');
        checkInputValid($(input));
    })

    const selectArr = $('.add-product select').toArray();
    selectArr.forEach((select) => {
        $(select).prop('selectedIndex', 0);
    })
})

//-------
$(document).ready(() => {
    var productDetailInput = $('.product-detail-form input').toArray();
    productDetailInput.forEach((input) => {
        checkInputValid($(input));
    })
})

//Update product --------------------------------
$('.product-detail-form #ProductID').on('keydown', (e) => {
    e.preventDefault();
})

$('.product-detail-form').submit((e) => {
    e.preventDefault();
    var productID = $('.product-detail-form #ProductID').val();
    var productName = $('.product-detail-form #ProductName').val();
    var productCost = $('.product-detail-form #Cost').val();
    var productPrice = $('.product-detail-form #Price').val();
    var cateID = $('.product-detail-form #CateID').val();
    var brandID = $('.product-detail-form #BrandID').val();
    var imgSrc = $('.product-detail-form #ImgSrc').val();
    var warranty = $('.product-detail-form #Warranty').val();
    var quantity = $('.product-detail-form #add-product-quantity').val();
    var imgSrc1 = $('.product-detail-form #add-product-image-1').val();
    var imgSrc2 = $('.product-detail-form #add-product-image-2').val();
    var imgSrc3 = $('.product-detail-form #add-product-image-3').val();
    var imgSrc4 = $('.product-detail-form #add-product-image-4').val();
    $('.loading').css('display', 'grid');
    $.ajax({
        type: 'POST',
        url: '/admin/products/updateproduct',
        data: {
            'ProductID': productID,
            'ProductName': productName,
            'Cost': productCost,
            'Price': productPrice,
            'CateID': cateID,
            'BrandID': brandID,
            'ImgSrc': imgSrc,
            'Warranty': warranty,
            quantity: quantity,
            imgSrc1: imgSrc1,
            imgSrc2: imgSrc2,
            imgSrc3: imgSrc3,
            imgSrc4: imgSrc4
        },
        success: (responses) => {
            hideLoading();
            if (responses.success) {
                $('.product-detail .form-error').empty();
                $('.product-detail .form-error').hide();
                var interval = setInterval(() => {
                    showUpdateOkForm();
                    clearInterval(interval);
                }, 620);
            }
            else {
                var str = `<span>
                        <i class="fa-solid fa-circle-exclamation error-icon"></i>
                        ${responses.error}
                    </span>`;
                $('.product-detail .form-error').empty();
                $('.product-detail .form-error').show();
                $('.product-detail .form-error').append(str);

                var interval = setInterval(() => {
                    $('.add-product .form-error').empty();
                    $('.add-product .form-error').hide();
                    clearInterval(interval)
                }, 8000)
            }
        },
        error: () => {
            hideLoading();
        }
    })
})

//Delete product --------------------------------
$(document).on('click ', '.delete-product-btn', (e) => {
    const productID = $(e.target).data('product-id');
    //----------
    $('.delete-product-confirm').css('visibility', 'visible');
    $('.delete-product-confirm .delete-confirm-box').addClass('show');
    //----------
    $('.cancel-delete').click(() => {
        $('.delete-product-confirm').css('visibility', 'hidden');
        $('.delete-product-confirm .delete-confirm-box').removeClass('show');
    })
    //----------
    $('.confirm-delete').click(() => {
        //----------
        $('.loading').css('display', 'grid');
        $.ajax({
            type: 'POST',
            url: '/admin/products/deleteproduct',
            data: {
                productID: productID
            },
            success: (res) => {
                hideLoading();
                if (res.success) {
                    var interval = setInterval(() => {
                        $('.complete-delete-notice').css('visibility', 'visible');
                        $('.complete-delete-notice .complete-notice-box').addClass('showForm');
                        clearInterval(interval);
                    }, 620);
                }
            },
            error: () => {
                hideLoading();
            }
        })

        $('.delete-product-confirm').css('visibility', 'hidden');
        $('.delete-product-confirm .delete-confirm-box').removeClass('show');
    })
})

//-----
$('.product-delete-frm-btn').click(() => {
    var productID = $('#ProductID').val();
    $.ajax({
        type: 'POST',
        url: '/admin/products/deleteproduct',
        data: {
            productID: productID
        },
        success: (res) => {
            window.location.href = '/admin/products'
        },
        error: () => {
        }
    })
})
//-----

$('.delete-product-confirm').click((e) => {
    if ($(e.target).closest('.delete-confirm-box').length <= 0) {
        $('.delete-product-confirm').css('visibility', 'hidden');
        $('.delete-product-confirm .delete-confirm-box').removeClass('show');
    }
})

$('.complete-delete-notice .complete-notice-box button').click(() => {
    $('.complete-delete-notice').css('visibility', 'hidden');
    $('.complete-delete-notice .complete-notice-box').removeClass('showForm');
})

$('.complete-delete-notice').click((e) => {
    if ($(e.target).closest('.complete-notice-box').length <= 0) {
        $('.complete-delete-notice').css('visibility', 'hidden');
        $('.complete-delete-notice .complete-notice-box').removeClass('showForm');
    }
})

//--Delete all product in one category ------------------------------------
$('.delete-product-in-cate-confirm').click((e) => {
    if ($(e.target).closest('.delete-confirm-box').length <= 0) {
        $('.delete-product-in-cate-confirm').css('visibility', 'hidden');
        $('.delete-product-in-cate-confirm .delete-confirm-box').removeClass('show');
    }
})

$('.delete-product-in-cate-btn').click(() => {
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

        $('.delete-product-in-cate-confirm').css('visibility', 'visible');
        $('.delete-product-in-cate-confirm .delete-confirm-box').addClass('show');
    }
})

$('.delete-product-in-cate-confirm .cancel-delete').click(() => {
    $('.delete-product-in-cate-confirm').css('visibility', 'hidden');
    $('.delete-product-in-cate-confirm .delete-confirm-box').removeClass('show');
})

$('.delete-product-in-cate-confirm .confirm-delete').click(() => {
    var cateID = $('input[name="category"]:checked').val();

    $('.loading').css('display', 'grid');
    $.ajax({
        type: 'POST',
        url: '/admin/products/deleteallincategory',
        data: {
            cateID: cateID
        },
        success: (response) => {
            hideLoading();
            if (response.success) {
                var interval = setInterval(() => {
                    $('.complete-delete-notice').css('visibility', 'visible');
                    $('.complete-delete-notice .complete-notice-box').addClass('showForm');
                    clearInterval(interval);
                }, 620);
            }
            else {
                var str = `<span>
                    <i class="fa-solid fa-circle-exclamation error-icon"></i>
                    ${response.error}
                    </span>`
                $('.categories-action-notice').empty();
                showCateNotice();
                $('.categories-action-notice').append(str);
                $('.delete-cate-confirm').css('visibility', 'hidden');
                $('.delete-cate-confirm-box').removeClass('show');

                var interval = setInterval(() => {
                    hideCateNotice();
                    clearInterval(interval);
                }, 6000);
            }
        },
        error: () => {
            hideLoading();
        }
    })

    $('.delete-product-in-cate-confirm').css('visibility', 'hidden');
    $('.delete-product-in-cate-confirm .delete-confirm-box').removeClass('show');
})