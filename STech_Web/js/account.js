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
    if (password !== confirmPassword) {
        var str = `<span>Xác nhận mật khẩu không đúng</span>`
        $('.register .form-error').show();
        $('.register .form-error').empty();
        $('.register .form-error').append(str);
    }
    else {
        $.ajax({
            type: 'POST',
            url: '/account/register',
            data: {
                Username: userName,
                Password: password,
                Email: email
            },
            success: (response) => {
                if (response.success) {
                    $('.register .form-error').hide();
                    $('.register .form-error').empty();
                    $('.register-form').unbind('submit').submit();
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

                    $('.register .form-error').show();
                    $('.register .form-error').empty();
                    $('.register .form-error').append(str);
                }
            },
            error: (err) => { console.log(err) }
        })
    }
})



//--Login with Ajax -----------------------------------------------
$('.login-form').submit((e) => {
    e.preventDefault();
    var userName = $('#login-username').val();
    var password = $('#login-password').val();

    $.ajax({
        type: 'POST',
        url: '/account/login',
        data: {
            Username: userName,
            Password: password
        },
        success: (response) => {
            if (response.success) {
                $('.login .form-error').hide();
                $('.login .form-error').empty();
                $('.login-form').unbind('submit').submit();
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

                $('.login .form-error').show();
                $('.login .form-error').empty();
                $('.login .form-error').append(str);
            }
        },
        error: (err) => { console.log(err) }
    })
})


//--Update with AJAX ----------------------------------------------
$('.user-update-form').submit((e) => {
    e.preventDefault();
    var fullName = $('#UserFulName').val();
    var gender = $('input[name="Gender"]:checked').val();
    var phone = $('#PhoneNumber').val();
    var email = $('#Email').val();
    var dob = $('#DOB').val();
    var userID = $('#userID').val();

    var isExcuted = false;
    function closeUpdateErr() {   
        if (isExcuted == false) {
            $('.update-error').empty();
        }

        isExcuted = true;
    }

    $.ajax({
        type: 'POST',
        url: '/account/update',
        data: {
            UserFullName: fullName,
            Gender: gender,
            PhoneNumber: phone,
            Email: email,
            DOB: dob,
            userID: userID
        },
        success: (response) => {
            if (response.success) {
                var str = '<span>Cập nhật thành công.</span>';
                $('.update-error').empty();
                $('.update-error').append(str);

                setInterval(closeUpdateErr, 7000);
                isExcuted = false;
            }
            else {
                $('.update-error').empty();
                var str = '<span>';
                $.each(response.errors, (index, value) => {
                    str += value;
                })
                str += '</span>'
               
                $('.update-error').append(str);

                setInterval(closeUpdateErr, 7000);
                isExcuted = false;
            }
        },
        error: (err) => { console.log(err) }
    })
})