//--Register with Ajax --------------------------------------------
$('.register form-submit-btn').click((e) => {
    var userName = $('#register-username');
    var password = $('#register-password');
    var confirmPassword = $('#re-enter-password');

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
                window.location.href = '/';
            }
            else {
                var str = `<span>Sai tên đăng nhập hoặc mật khẩu</span>`
                $('.login .form-error').show();
                $('.login .form-error').empty();
                $('.login .form-error').append(str);
            }
        }
    })
})