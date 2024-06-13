const postsController = function () {
    this.initialize = function () {
        const postId = $('#hid_post_id').val();
        console.log('Initializing postsController with postId:', postId); // Kiểm tra postId
        loadComments(postId);
    }

    function loadComments(id) {
        $.get('/posts/GetCommentsByPostId?postId=' + id).done(function (response, statusText, xhr) {
            if (xhr.status === 200) {
                const template = $('#tmpl_comments').html();
                const childrenTemplate = $('#tmpl_children_comments').html();
                
                if (!template || !childrenTemplate) {
                    console.error('Templates tmpl_comments or tmpl_children_comments not found');
                    return;
                }
                
                if (response && response.data && response.data.length > 0) {
                    let html = '';
                    
                    $.each(response.data, function (index, item) {
                        let childrenHtml = '';
                        
                        if (item.replies && item.replies.length > 0) {
                            console.log('Item has replies:', item.replies); // Log dữ liệu replies
                            
                            $.each(item.replies, function (childIndex, childItem) {
                                childrenHtml += Mustache.render(childrenTemplate, {
                                    Id: childItem.id,
                                    Content: childItem.content,
                                    CreatedDate: formatRelativeTime(childItem.createdDate),
                                    UserId: childItem.userId
                                });
                            });
                        }
                        html += Mustache.render(template, {
                            childrenHtml: childrenHtml,
                            Id: item.id,
                            Content: item.content,
                            CreatedDate: formatRelativeTime(item.createdDate),
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