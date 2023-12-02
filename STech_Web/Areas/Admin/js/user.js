$('.create-customer-box').submit((e) => {
    e.preventDefault();
    let name = $(e.target).find('#create-cusName').val();
    let phone = $(e.target).find('#create-cusPhone').val();
    let address = $(e.target).find('#create-cusAddress').val();
    let gender = $(e.target).find('input[name="create-cusGender"]:checked').val();
    let dob = $(e.target).find('#create-cusDOB').val();
    let email = $(e.target).find('#create-cusEmail').val();

    if (name.length > 0 && phone.length > 0 && address.length > 0) {
        showLoading();
        $.ajax({
            type: 'post',
            url: '/admin/users/createcustomer',
            data: {
                'CustomerName': name,
                'Phone': phone,
                'Address': address,
                'Gender': gender,
                'DoB': dob,
                'Email': email
            },
            success: (response) => {
                hideLoading();
                if (response.success) {
                    $(e.target).find('.form-error').hide();
                    $(e.target).find('.form-error').empty();
                }
                else {
                    var str = `<span>
                    <i class="fa-solid fa-circle-exclamation"></i>`
                        + response.error + `</span>`;

                    $(e.target).find('.form-error').show();
                    $(e.target).find('.form-error').empty();
                    $(e.target).find('.form-error').append(str);
                }

            },
            error: () => {
                var str = `<span>
                    <i class="fa-solid fa-circle-exclamation"></i>Đã xảy ra lỗi.</span>`;

                $(e.target).find('.form-error').show();
                $(e.target).find('.form-error').empty();
                $(e.target).find('.form-error').append(str);
            }
        })
    }

    $(e.target).find('.form-submit-reset').click(() => {
        $(e.target).find('.form-error').hide();
        $(e.target).find('.form-error').empty();
    })
})