document.addEventListener('DOMContentLoaded', function () {
    const input = document.querySelector('#tags');

    const newTagsData = [];
    const existingTagsData = [];
    
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

    // Handle event adding a new tag (Xử lý sự kiện thêm tag mới)
    tagify.on('add', function(e) {
        const tagName = e.detail.data.value;

        // Check if the tag already exists (Kiểm tra xem tag đã tồn tại chưa)
        $.get('/tags/check', { name: tagName }, function(response) {
            if (response && response.data) {
                console.log(`Tag "${tagName}" đã tồn tại với ID: ${response.data.id}`);
                const tagIndex = tagify.value.findIndex(tag => tag.value === tagName);
                if (tagIndex > -1) {
                    tagify.value[tagIndex].id = response.data.id;
                    existingTagsData.push({
                        id: response.data.id,
                        name: tagName
                    });
                }
            } else {
                console.log(`Tag "${tagName}" là mới và sẽ được tạo.`);
                newTagsData.push({
                    id: null,
                    name: tagName
                });
            }
        });
    });

    tagify.on('remove', function(e) {
        console.log(e.detail); // Contains removed tag information
    });

    // Hàm để lấy danh sách các tags đã tồn tại và mới
    window.getTagsData = function() {
        return {
            existingTags: existingTagsData,
            newTags: newTagsData
        };
    };
});
