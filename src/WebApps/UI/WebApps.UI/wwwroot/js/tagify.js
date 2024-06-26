document.addEventListener('DOMContentLoaded', function () {
    const input = document.querySelector('#tags');
    const tagify = new Tagify(input, {
        whitelist: [], // Load existing tags từ server
        dropdown: {
            maxItems: 20, // số lượng tag gợi ý tối đa
            classname: "tags-look",
            enabled: 0, // hiển thị gợi ý ngay khi focus
            closeOnSelect: false
        }
    });

    // Fetch các tags hiện có từ server bằng jQuery $.get
    $.get('/tags/suggest', { count: 10 }, function (tags) {
        tagify.settings.whitelist = tags;
        tagify.dropdown.show(); // Show the suggestions dropdown
    });

    // Xử lý sự kiện thêm tag mới
    tagify.on('add', function(e) {
        console.log(e.detail); // chứa thông tin tag mới thêm
    });

    tagify.on('remove', function(e) {
        console.log(e.detail); // chứa thông tin tag bị xóa
    });
});
