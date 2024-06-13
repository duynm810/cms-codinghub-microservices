const postsController = function () {
    this.initialize = function () {
        const postId = $('#hid_post_id').val();
        console.log('Initializing postsController with postId:', postId); // Kiểm tra postId
        loadComments(postId);
        registerEvents();
    }

    function loadComments(id) {
        $.get('/posts/GetCommentsByPostId?postId=' + id).done(function (response, statusText, xhr) {
            if (xhr.status === 200) {
                const template = $('#tmpl_comments').html();
                const childrenTemplate = $('#tmpl_children_comments').html();
                if (response) {
                    let html = '';
                    $.each(response.data, function (index, item) {
                        let childrenHtml = '';
                        if (item.Replies && item.Replies.length > 0) {
                            $.each(item.Replies, function (childIndex, childItem) {
                                childrenHtml += Mustache.render(childrenTemplate, {
                                    Id: childItem.id,
                                    Content: childItem.content,
                                    CreatedDate: formatRelativeTime(childItem.createDate),
                                    UserId: childItem.userId
                                });
                            });
                        }
                        html += Mustache.render(template, {
                            childrenHtml: childrenHtml,
                            Id: item.id,
                            Content: item.content,
                            CreatedDate: formatRelativeTime(item.createDate),
                            UserId: item.userId
                        });
                    });
                    $('#comment_list').html(html);
                }
            }
        });
    }

    function formatRelativeTime(fromDate) {
        if (fromDate === undefined)
            fromDate = new Date();
        if (!(fromDate instanceof Date))
            fromDate = new Date(fromDate);

        const msPerMinute = 60 * 1000;
        const msPerHour = msPerMinute * 60;
        const msPerDay = msPerHour * 24;
        const msPerMonth = msPerDay * 30;
        const msPerYear = msPerDay * 365;
        const elapsed = new Date() - fromDate;

        if (elapsed < msPerMinute) {
            return Math.round(elapsed / 1000) + ' giây trước';
        } else if (elapsed < msPerHour) {
            return Math.round(elapsed / msPerMinute) + ' phút trước';
        } else if (elapsed < msPerDay) {
            return Math.round(elapsed / msPerHour) + ' giờ trước';
        } else if (elapsed < msPerMonth) {
            return 'approximately ' + Math.round(elapsed / msPerDay) + ' ngày trước';
        } else if (elapsed < msPerYear) {
            return 'approximately ' + Math.round(elapsed / msPerMonth) + ' tháng trước';
        } else {
            return 'approximately ' + Math.round(elapsed / msPerYear) + ' năm trước';
        }
    }
}