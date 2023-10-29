//--Show button loading -------------------------------------------
function showBtnLoading(button) {
    var btnText = button.text();
    button.empty();
    var loadingStr = `<div class="loadingio-spinner-dual-ring-ekj0ol56kwc">
                        <div class="ldio-gmrbyawnrc">
                            <div></div><div><div></div></div>
                        </div>
                    </div>`;
    button.append(loadingStr);

    return btnText;
}

function resetBtn(button, btnText) {
    button.empty();
    button.text(btnText);
}

//--Register with Ajax --------------------------------------------
$('#re-enter-password').keyup(() => {
    var password = $('#register-password').val();
    var confirmPassword = $('#re-enter-password').val();

    if (password !== confirmPassword) {
        $('#re-enter-password').parent('.form-input-box')
            .css('border', '1px solid #e30019');
    }
    else {
        $('#re-enter-password').parent('.form-input-box')
            .css('border', '1px solid #cfcfcf');
    }
})

$('.register-form').submit((e) => {
    e.preventDefault();
    var userName = $('#register-username').val();
    var password = $('#register-password').val();
    var confirmPassword = $('#re-enter-password').val();
    var email = $('#register-email').val();

    var submitBtn = $(e.target).find('.form-submit-btn');
    var btnText = showBtnLoading(submitBtn);
    $.ajax({
        type: 'POST',
        url: '/account/register',
        data: {
            Username: userName,
            Password: password,
            ConfirmPassword: confirmPassword,
            Email: email
        },
        success: (response) => {
            resetBtn(submitBtn, btnText);
            if (response.success) {
                $('.register .form-error').hide();
                $('.register .form-error').empty();
                $('.register-form').unbind('submit').submit();
            }
            else {
                var str = `<span>
                    <i class="fa-solid fa-circle-exclamation"></i>`
                        + response.error + `</span>`;

                $('.register .form-error').show();
                $('.register .form-error').empty();
                $('.register .form-error').append(str);
            }
        },
        error: (err) => { resetBtn(submitBtn, btnText); }
    })
})



//--Login with Ajax -----------------------------------------------
$('.login-form').submit((e) => {
    e.preventDefault();
    var userName = $('#login-username').val();
    var password = $('#login-password').val();

    var submitBtn = $(e.target).find('.form-submit-btn');
    var btnText = showBtnLoading(submitBtn);
    $.ajax({
        type: 'POST',
        url: '/account/login',
        data: {
            Username: userName,
            Password: password
        },
        success: (response) => {
            resetBtn(submitBtn, btnText);
            if (response.success) {
                $('.login .form-error').hide();
                $('.login .form-error').empty();
                $('.login-form').unbind('submit').submit();
            }
            else {
                var str = `<span>
                    <i class="fa-solid fa-circle-exclamation"></i>`
                    + response.error + `</span>`;

                $('.login .form-error').show();
                $('.login .form-error').empty();
                $('.login .form-error').append(str);
            }
        },
        error: (err) => { resetBtn(submitBtn, btnText); }
    })
})

//--Logout ---------------------------------
$('.logout-confirm-yes').click((e) => {
    var btnText = showBtnLoading($(e.target));
    window.location.href = '/account/logout';
})


//--Update with AJAX ----------------------------------------------
function closeUpdateErr() {
    $('.update-error').empty();
}

$('.user-update-form').submit((e) => {
    e.preventDefault();
    var fullName = $('#UserFulName').val();
    var gender = $('input[name="Gender"]:checked').val();
    var phone = $('#PhoneNumber').val();
    var email = $('#Email').val();
    var dob = $('#DOB').val();
    var address = $('#Address').val();

    var submitBtn = $(e.target).find('.user-form-submit');
    var btnText = showBtnLoading(submitBtn);
    $.ajax({
        type: 'POST',
        url: '/account/update',
        data: {
            UserFullName: fullName,
            Gender: gender,
            PhoneNumber: phone,
            Email: email,
            DOB: dob,
            Address: address
        },
        success: (response) => {
            resetBtn(submitBtn, btnText);
            if (response.success) {
                var str = '<span>Cập nhật thành công.</span>';
                $('.update-error').empty();
                $('.update-error').append(str);

                setTimeout(() => {
                    closeUpdateErr();
                }, 6000)
            }
            else {
                $('.update-error').empty();
                var str = '<span>' + response.error +'</span>'
               
                $('.update-error').append(str);

                setTimeout(() => {
                    closeUpdateErr();
                }, 6000)
            }
        },
        error: (err) => { resetBtn(submitBtn, btnText); }
    })
})

//-Change password -----------------------------------
$('.change-password-form').submit((e) => {
    e.preventDefault();
    var oldPassword = $('#OldPassword').val();
    var newPassword = $('#NewPassword').val();
    var confirmNewPassword = $('#ConfirmNewPassword').val();

    var submitBtn = $(e.target).find('.user-form-submit');
    var btnText = showBtnLoading(submitBtn);
    $.ajax({
        type: 'POST',
        url: '/account/changepassword',
        data: {
            oldPassword: oldPassword,
            newPassword: newPassword,
            confirmNewPassword: confirmNewPassword
        },
        success: (response) => {
            resetBtn(submitBtn, btnText);
            if (response.success) {
                var str = '<span>Đổi mật khẩu thành công.</span>';
                $('.update-error').empty();
                $('.update-error').append(str);

                $('#OldPassword').val('');
                $('#NewPassword').val('');
                $('#ConfirmNewPassword').val('');

                setTimeout(() => {
                    closeUpdateErr();
                }, 6000)
            }
            else {
                $('.update-error').empty();
                var str = `<span>${response.error}</span>`;

                $('.update-error').append(str);

                setTimeout(() => {
                    closeUpdateErr();
                }, 6000)
            }

        },
        error: () => { resetBtn(submitBtn, btnText); }
    })
})

//-----------------------------------------------
function setParentHeight() {
    var childHeight = $('.account-right-box.current').outerHeight(true);
    $('.account-right-side').css('height', childHeight + 'px');
}

function showCard() {
    var idFromUrl = window.location.hash.substring(1);
    if (idFromUrl.length > 0) {
        $('.account-right-box').removeClass('current');
        $('.account-nav-list-item a').removeClass('activeNav');
        $('[data-account-side="' + idFromUrl + '"').addClass('current');
        $('a[href="#' + idFromUrl +'"').addClass('activeNav');
    } 
    setParentHeight();
}

$(document).ready(() => {
    showCard();

    $(window).on('hashchange' ,() => {
        showCard();
    })
})
