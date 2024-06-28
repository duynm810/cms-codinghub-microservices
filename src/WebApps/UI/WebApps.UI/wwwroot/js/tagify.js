document.addEventListener('DOMContentLoaded', function () {
    const input = document.querySelector('#tags');

    window.tagify = new Tagify(input, {
        whitelist: [],
        dropdown: {
            maxItems: 20, // Maximum number of suggested tags
            classname: "tags-look",
            enabled: 0, // 0: Show hint immediately upon focus, 1: Show suggestions when start typing 
            closeOnSelect: false
        },
        tagTextProp: 'name', // Display 'name' in the UI
        mapValueToProp: 'name' // Map the value to 'id'
    });

    // Get a list of tag suggestions when the page is loaded (Lấy danh sách gợi ý tags khi trang được tải)
    $.get('/tags/suggest', { count: 10 }, function (response) {
        if (response && response.data && response.data.length > 0) {
            tagify.settings.whitelist = response.data.map(item => ({
                value: item.name,
                name: item.name,
                id: item.id
            }));
        }
    });

    // Xử lý sự kiện nhập liệu gợi ý tags
    tagify.on('input', function(e) {
        const keyword = e.detail.value;

        $.get('/tags/suggest', { keyword: keyword, count: 10 }, function(response) {
            if (response && response.data && response.data.length > 0) {
                tagify.settings.whitelist = response.data.map(item => ({
                    value: item.name,
                    name: item.name,
                    id: item.id
                }));
                tagify.dropdown.show.call(tagify, keyword);
            }
        });
    });

    // Handle event adding a new tag
    tagify.on('add', function(e) {
        const tagName = e.detail.data.value;
        const tagId = e.detail.data.id;
        const tagIndex = tagify.value.findIndex(tag => tag.value === tagName);

        if (tagId) {
            console.log(`Tag "${tagName}" đã tồn tại với ID: ${tagId}`);
            if (tagIndex > -1) {
                tagify.value[tagIndex].id = tagId;
                tagify.value[tagIndex].isExisting = true; // Đánh dấu tag đã tồn tại
            }
        } else {
            console.log(`Tag "${tagName}" là mới và sẽ được tạo.`);
            const slug = createSlug(tagName); // Tạo slug cho tag mới
            if (tagIndex > -1) {
                tagify.value[tagIndex].isExisting = false; // Đánh dấu tag mới
                tagify.value[tagIndex].slug = slug; // Thêm slug vào tag mới
            }
        }
    });

    // Handle event deleting tag (Xử lý xoá nhãn dán)
    tagify.on('remove', function(e) {
        const tagName = e.detail.data.value;
        console.log(`Tag "${tagName}" đã bị xoá.`);
    });

    // Hàm để lấy danh sách các tags hiện tại
    window.getTagsData = function() {
        return tagify.value.map(tag => ({
            id: tag.id || null,  // If id exists, use it, otherwise set to null for new tags
            name: tag.value,
            slug: tag.slug || createSlug(tag.value),
            isExisting: tag.isExisting || false  // Default to false if not set
        }));
    };
});