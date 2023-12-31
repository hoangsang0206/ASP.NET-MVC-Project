﻿//Show overlay when click category button -------------------------------------------
$(".categories-btn").click(() => {
    $(".hidden-menu").toggleClass("showHiddenMenu");

    $(".hidden-menu").click((e) => {
        if ($(e.target).closest('.hidden-menu-box').length <= 0) {
            $(e.target).removeClass("showHiddenMenu");
            $(".overlay").removeClass("showOverlay");
        }      
    });

    $(".overlay").toggleClass("showOverlay");

    $(".overlay").click(() => {
        $(".hidden-menu").removeClass("showHiddenMenu");
        $(".overlay").removeClass("showOverlay");
    });
});

$(".overlay").click(() => {
    $(".overlay").removeClass("showOverlay");
});

//Show scroll to top button ----------------------------------------------------------
const scrollTopBtn = document.querySelector(".to-top-btn");

window.onscroll = function () {
    if (document.body.scrollTop > 200 || document.documentElement.scrollTop > 200) {
        scrollTopBtn.classList.add("showToTopBtn");
    } else {
        scrollTopBtn.classList.remove("showToTopBtn");
    }
}

scrollTopBtn.onclick = function () {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
};

//Show sidebar menu ------------------------------------------------------------------------
function showSidebar() {
    $(".mobile-sidebar").addClass("showMobileSidebar");
    $(".overlay").addClass("showOverlay");
    $(".overlay").click(function () {
        $(".mobile-sidebar").removeClass("showMobileSidebar");
        $("body").removeClass("web-scroll-block");
    });
    $("body").addClass("web-scroll-block");
};

$(".mobile-categories-btn").click(showSidebar);
$(".bottom-nav-show-sidebar").click(showSidebar);
$(".sidebar-close-btn").click(function () {
    $(".mobile-sidebar").removeClass("showMobileSidebar");
    $(".overlay").removeClass("showOverlay");
    $("body").removeClass("web-scroll-block");
});

//Show mobile sidebar menu level 2, 3
var sidebarContents = $('.mobile-sidebar .megamenu-item').toArray();
sidebarContents.forEach(item => {
    var _item = $(item);

    var sidebarChervon = _item.find('.megamenu-item-box .megamenu-chevron');
    sidebarChervon.click(() => {
        _item.children('.megamenu-content').toggleClass('showSidebarContent');
        sidebarChervon.toggleClass('chevronRotate');

        var sidebarContentsLvl2 = _item.children('.megamenu-content').find('.megamenu-content-item').toArray();

        sidebarContentsLvl2.forEach(item1 => {
            var _item1 = $(item1);
            var sidebarChevronLevel2 = _item1.find('.sub-megamenu-box .megamenu-chevron')

            sidebarChevronLevel2.click(() => {
                _item1.children('.megamenu-item-list').toggleClass('showSidebarContent');
                sidebarChevronLevel2.toggleClass('chevronRotate');
            })
        })
    })


})

//Show bottom navigation --------------------------------------------------------------
$(document).ready(function () {
    let preScrollPos = window.scrollY;
    $(window).scroll(() => {
        const currentScrollPos = window.scrollY;
        if (currentScrollPos > preScrollPos) {
            $(".bottom-nav-mobile").addClass("showBottomNav");
        }
        else {
            $(".bottom-nav-mobile").removeClass("showBottomNav");
        }
        preScrollPos = currentScrollPos;
    })
});

//Slick Slider ------------------------------------------------------------------------
$(".sale-slick-slider").slick({
    slidesToShow: 5,
    slidesToScroll: 1,
    infinite: true,
    autoplay: true,
    autoplaySpeed: 2000,
    arrows: true,
    prevArrow: `<button type='button' class='slick-prev slick-arrow'><i class="fa-solid fa-chevron-left"></i></button>`,
    nextArrow: `<button type='button' class='slick-next slick-arrow'><i class="fa-solid fa-chevron-right"></i></button>`,
    dots: true,
    swipeToSlide: true,
    responsive: [
        {
            breakpoint: 767,
            settings: {
                slidesToShow: 2.2,
                autoplay: false,
                infinite: false,
                arrows: false
            }
        }
    ]
});

//Slick Slider for Collection in main page
$(".collection-slick-slider").slick({
    slidesToShow: 5,
    slidesToScroll: 1,
    infinite: false,
    arrows: true,
    swipeToSlide: true,
    prevArrow: `<button type='button' class='slick-prev slick-arrow'><i class="fa-solid fa-chevron-left"></i></button>`,
    nextArrow: `<button type='button' class='slick-next slick-arrow'><i class="fa-solid fa-chevron-right"></i></button>`,
    responsive: [
        {
            breakpoint: 767,
            settings: {
                slidesToShow: 2.2,
                arrows: false
            }
        }
    ]
})

//Slick Slider for Brands Collection
$(".brand-collections").slick({
    slidesToShow: 7,
    slidesToScroll: 1,
    dots: false,
    infinite: true,
    arrows: false,
    autoplay: true,
    autoplaySpeed: 1500,
    swipeToSlide: true,
    responsive: [
        {
            breakpoint: 767,
            settings: {
                slidesToShow: 3
            }
        }
    ]
})

//Show footer column content -----------------------------------------------------------
var footerColArr = $(".footer-col").toArray();
footerColArr.forEach(item => {
    var _item = $(item);
    _item.children(".footer-title").click(() => {
        _item.toggleClass("activeFooter");
    });
});

//Lazy loading image
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

$(document).ready(lazyLoading);


//$(document).click(function (e) {
//    var target = e.target;
//    while (target && target.tagName !== 'A') {
//        target = target.parentNode;
//    }
//    if (target.getAttribute('href') === '#') {
//        e.preventDefault();
//        window.location.href = "/error/notfound";
//    }
//})


 //---Show order dropdown content---------------------------
$(".sort-dropdown-btn").click(() => {
    $(".sort-dropdown-content").toggleClass("showDropdown");
    $(".sort-dropdown-content").click(() => {
        $(".sort-dropdown-content").removeClass("showDropdown");
    })
})

//----------------------------------------------------------
//---Save search history to Local Storage-------------------
$(document).ready(function () {
    $('.search-form').submit(() => {
        var searchText = $('#search').val();

        var searchHistory = JSON.parse(localStorage.getItem('searchHistory')) || [];

        if (searchHistory.length > 13) {
            searchHistory.shift();
        }

        if (searchText.length > 0 || searchText != null) {
            searchHistory.push(searchText);
        }

        localStorage.setItem('searchHistory', JSON.stringify(searchHistory));
    })
})

//Show search width 100% in mobile
$(document).ready(function () {
    $('#search').on('click', () => {
        const windowWidth = $(window).width();
        if (windowWidth <= 768) {
            $('.header-box').hide();
            $('.header-btn').hide();
            $('body').css('overflow', 'hidden');
            $('.search').css('width', '100%');
            $('.close-search').show();
            $('.search-history').show();
        }

        $('.close-search').click(() => {
            $('.header-box').css('display', 'flex');
            $('.header-btn').show();
            $('body').css('overflow', 'scroll');
            $('.search').css('width', '60%');
            $('.close-search').hide();
            $('.ajax-search-autocomplete').hide();
            $('.search-history').hide();
        })

        var searchHistory = JSON.parse(localStorage.getItem('searchHistory')) || [];

        $('.search-history-list').empty();
        if (searchHistory.length > 0) {
            $('.search-history').css('padding', '0 0 1rem 0');
            $('.search-history').children('h4').hide();

            for (var i = 0; i < searchHistory.length; i++) {
                $('.search-history-list').append(`<a href="/search/${searchHistory[i]}">`
                    + `<li class="search-history-list-item">`
                    + searchHistory[i] + '</li>' + '</a>');
            }
        }
        else {
            $('.search-history').children('h4').show();
        }

        $('.clear-search-history').click(() => {
            $('.search-history-list').empty();
            localStorage.removeItem('searchHistory');
            $('.search-history').css('padding', '1rem');
            $('.search-history').children('h4').show();
        })
    })

})

//---AJAX---------------------------------------------------

//---AJAX autocomplete search----------------------------
$("#search").keyup(function () {
    var searchText = $(this).val();
    if (searchText.length > 0) {
        $.ajax({
            url: '/api/collections',
            type: 'GET',
            data: {
                id: searchText
            },
            success: function (responses) {
                var maxItems = window.innerWidth <= 768 ? 25 : 6

                $('.ajax-search-autocomplete').show();
                $('.ajax-search-autocomplete').empty();

                if (responses == null || responses.length <= 0) {
                    $('.ajax-search-autocomplete').hide();
                    $('.ajax-search-autocomplete').empty();
                }
                else {
                    for (let i = 0; i <= responses.length && i < maxItems; i++) {
                        const product = responses[i];
                        if (product != null) {
                            const strHTML = `<a href="/product/${product.ProductID}"> 
                                    <div class="ajax-search-item d-flex justify-content-between align-items-center">
                                        <div class="ajax-search-item-info">
                                            <div class="ajax-search-item-name d-flex align-items-center">
                                                <h3>${product.ProductName}</h3>
                                            </div>
                                            <div class="ajax-search-item-price d-flex align-items-center">
                                                <h3>${product.Price.toLocaleString("vi-VN") + 'đ'}</h3>
                                                <h4>${product.Cost !== 0 ? product.Cost.toLocaleString("vi-VN") + 'đ' : ''}</h4>
                                            </div>
                                        </div>
                                        <div class="ajax-search-item-image">
                                            <img src="${product.ImgSrc}" alt="" />
                                        </div>
                                    </div>
                                </a>`;

                            $('.ajax-search-autocomplete').append(strHTML);
                        }
                    }
                }
            },
            error: () => {
                $('.ajax-search-autocomplete').hide();
            }
        })
    }
    else {
        $('.ajax-search-autocomplete').hide();
    }

    $(document).on('click', (e) => {
        if (window.innerWidth > 768) {
            var ajaxSearch = $('.ajax-search-autocomplete');
            if (!$(e.target).closest('.ajax-search-autocomplete').length) {
                ajaxSearch.hide();
            }
        }
    })
})


//---Show password ------------------------------------------------------
var passwordInputArr = $('input[name="Password"], input[name="ConfirmPassword"]').toArray();

passwordInputArr.forEach(item => {
    var _item = $(item);
    var togglePassword = _item.siblings('.toggle-password');

    _item.focus(() => {
        togglePassword.css('display', 'grid');
        togglePassword.click(() => {
            if (_item.attr('type') === 'password') {
                _item.attr('type', 'text');
                togglePassword.addClass('hiddenEyeSlash');
            }
            else {
                _item.attr('type', 'password');
                togglePassword.removeClass('hiddenEyeSlash');
            }
        })
    })

    _item.blur(() => {

        _item.attr('type', 'password');
        togglePassword.removeClass('hiddenEyeSlash');

        if (_item.val().length <= 0) {
            togglePassword.css('display', 'none');
        }
        else {
            togglePassword.css('display', 'grid');
        }
    })
});


//--Show form ----------------------------------------------------------------
$('.action-login-btn').click(() => {
    $('.login').css('visibility', 'visible');
    $('.login .form-container').addClass('showForm');
})

$('.action-register-btn').click(() => {
    $('.register').css('visibility', 'visible');
    $('.register .form-container').addClass('showForm');
})

$('.login-btn .login-link').click(() => {
    $('.login').css('visibility', 'visible');
    $('.login .form-container').addClass('showForm');
})

$('.bottom-nav-account').click(() => {
    $('.login').css('visibility', 'visible');
    $('.login .form-container').addClass('showForm');
})

//-----

$('.login-info-logout, .account-logout').on('click', () => {
    $('.logout-confirm').css('visibility', 'visible');
    $('.logout-confirm-box').addClass('showLogoutConfirm');
})

$('.logout-confirm').click((e) => {
    if (!(e.target).closest('.logout-confirm-box')) {
        $(e.target).css('visibility', 'hidden');
        $('.logout-confirm-box').removeClass('showLogoutConfirm');
    }
})

$('.logout-confirm-no').click(() => {
    $('.logout-confirm').css('visibility', 'hidden');
    $('.logout-confirm-box').removeClass('showLogoutConfirm');
})

//---
var formArr = [$('.login'), $('.register'), $('.register'), $('.forgot-password'), $('.reset-password')];

formArr.forEach(form => {
    var _form = $(form);
    _form.on('click', (e) => {
        if ($(e.target).closest('.form-container').length <= 0) {
            $(e.target).css('visibility', 'hidden');
            $('.form-container').removeClass('showForm');
        }
    })

    //--CLose form -----
    _form.find('.close-form').click(() => {
        _form.css('visibility', 'hidden');
        _form.find('.form-container').removeClass('showForm');
    })
})

 //------------------------
$('.register-now-link').click(() => {
    $('.login').css('visibility', 'hidden');
    $('.login .form-container').removeClass('showForm');
    $('.register').css('visibility', 'visible');
    $('.register .form-container').addClass('showForm');
    
}) 

$('.login-now-link').click(() => {
    $('.register').css('visibility', 'hidden');
    $('.register .form-container').removeClass('showForm');
    $('.login').css('visibility', 'visible');
    $('.login .form-container').addClass('showForm');
})

$('.forgot-password-link').click(() => {
    $('.login').css('visibility', 'hidden');
    $('.login .form-container').removeClass('showForm');
    $('.forgot-password').css('visibility', 'visible');
    $('.forgot-password .form-container').addClass('showForm');
})

$('.back-to-login-link').click(() => {
    $('.forgot-password').css('visibility', 'hidden');
    $('.forgot-password .form-container').removeClass('showForm');
    $('.login').css('visibility', 'visible');
    $('.login .form-container').addClass('showForm');
})

$('.forgot-password-form').on('submit', (e) => {
    e.preventDefault();
    $('.forgot-password').css('visibility', 'hidden');
    $('.forgot-password .form-container').removeClass('showForm');
    $('.reset-password').css('visibility', 'visible');
    $('.reset-password .form-container').addClass('showForm');
})