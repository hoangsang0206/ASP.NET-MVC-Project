$(document).ready(() => {
    var filterBtn = $('.filter-btn').toArray();
    filterBtn.forEach(item => {
        var _item = $(item);
        var filterHead = _item.children('.filter-head');
        var filterContent = _item.children('.filter-contents');

        filterHead.click(() => {
            filterContent.toggleClass('show');
        })
    })
})