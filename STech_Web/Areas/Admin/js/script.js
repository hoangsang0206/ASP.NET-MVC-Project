$('.toggle').click(() => {
    $('.sidebar').toggleClass('close');
})

// --- Admin login with ajax ----------------------------------------
$('.admin-login-form').submit((e) => {
    e.preventDefault();
    var userName = $('#admin-username').val();
    var password = $('#admin-password').val();

    console.log(userName, password)

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