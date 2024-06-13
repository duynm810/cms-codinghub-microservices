const postsController = function () {
    this.initialize = function () {
        const postId = $('#hid_post_id').val();
        console.log('Initializing postsController with postId:', postId); // Kiểm tra postId
        registerEvents();
        loadComments(postId);
    }

    function loadComments(id) {
        $.get('/posts/GetCommentsByPostId?postId=' + id).done(function (response, statusText, xhr) {
            if (xhr.status === 200) {
                if (response && response.data && response.data.length > 0) {
                    let html = '';

                    $.each(response.data, function (index, item) {
                        let childrenHtml = '';

                        if (item.replies && item.replies.length > 0) {
                            console.log('Item has replies:', item.replies); // Log dữ liệu replies

                            $.each(item.replies, function (childIndex, childItem) {
                                childrenHtml += generateReplyHtml(childItem.id, childItem.content, childItem.createdDate, childItem.userId);
                            });
                        }
                        html += generateCommentHtml(item.id, item.content, item.createdDate, item.userId, childrenHtml);
                    });
                    $('#comment_list').html(html);
                }
            }
        });
    }

    function registerEvents() {
        console.log('Registering events'); // Kiểm tra việc đăng ký sự kiện
        $("#commentform").submit(function (e) {
            e.preventDefault();
            console.log('Comment form submitted'); // Kiểm tra việc submit form

            const form = $(this);
            const url = form.attr('action');
            console.log('Submitting form to URL:', url); // Kiểm tra URL

            $.post(url, form.serialize()).done(function (response) {
                console.log('Form submission successful:', response); // Kiểm tra phản hồi

                const content = $("#txt_new_comment_content").val();
                const newCommentHtml = generateCommentHtml(response.data.id, content, new Date(), $('#hid_current_login_name').val());

                $("#txt_new_comment_content").val('');
                $('#comment_list').prepend(newCommentHtml);
                const numberOfComments = parseInt($('#hid_number_comments').val()) + 1;
                $('#hid_number_comments').val(numberOfComments);
                $('#comments-title').text('Các bình luận (' + numberOfComments + ')');
            });
        });

        $('body').on('click', '.comment-reply-link', function (e) {
            e.preventDefault();
            console.log('Reply link clicked'); // Kiểm tra việc click vào reply link

            const commentId = $(this).data('commentid');
            const replyFormHtml = generateReplyFormHtml(commentId);

            $('#reply_comment_' + commentId).html(replyFormHtml);

            $("#frm_reply_comment_" + commentId).submit(function (e) {
                e.preventDefault();
                console.log('Reply form submitted for commentId:', commentId); // Kiểm tra việc submit form trả lời

                const form = $(this);
                const url = form.attr('action');
                console.log('Submitting reply form to URL:', url); // Kiểm tra URL

                $.post(url, form.serialize()).done(function (response) {
                    console.log('Reply form submission successful:', response); // Kiểm tra phản hồi

                    const content = $("#txt_reply_content_" + commentId).val();
                    const newReplyHtml = generateReplyHtml(response.data.id, content, new Date(), $('#hid_current_login_name').val());

                    $("#txt_reply_content_" + commentId).val('');
                    $('#reply_comment_' + commentId).html('');
                    $('#children_comments_' + commentId).prepend(newReplyHtml);

                    const numberOfComments = parseInt($('#hid_number_comments').val()) + 1;
                    $('#hid_number_comments').val(numberOfComments);
                    $('#comments-title').text('Các bình luận (' + numberOfComments + ')');
                });
            });
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

    function generateCommentHtml(id, content, createdDate, userId, childrenHtml = '') {
        return `
        <div class="single-comment justify-content-between d-flex">
            <div class="user justify-content-between d-flex">
                <div class="thumb">
                    <img src="../imgs/authors/author-2.jpg" alt="">
                </div>
                <div class="desc">
                    <p class="comment">${content}</p>
                    <div class="d-flex justify-content-between">
                        <div class="d-flex align-items-center">
                            <h5><a href="#">${userId}</a></h5>
                            <p class="date">${formatRelativeTime(new Date(createdDate))}</p>
                        </div>
                        <div class="reply-btn">
                            <a href="#" class="btn-reply comment-reply-link" data-commentid="${id}">Reply</a>
                        </div>
                    </div>
                <div id="reply_comment_${id}"></div>
                </div>
            </div>
        </div>
        <ul class="children" id="children_comments_${id}">
            ${childrenHtml}
        </ul>
        `;
    }

    function generateReplyHtml(id, content, createdDate, userId) {
        return `
        <div class="single-comment depth-2 justify-content-between d-flex mt-50">
            <div class="user justify-content-between d-flex">
                <div class="thumb">
                    <img src="../imgs/authors/author.jpg" alt="">
                </div>
                <div class="desc">
                    <p class="comment">${content}</p>
                    <div class="d-flex justify-content-between">
                        <div class="d-flex align-items-center">
                            <h5><a href="#">${userId}</a></h5>
                            <p class="date">${formatRelativeTime(createdDate)}</p>
                        </div>
                        <div class="reply-btn">
                            <a href="#" class="btn-reply comment-reply-link" data-commentid="${id}">Reply</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        `;
    }

    function generateReplyFormHtml(commentId) {
        return `
        <div class="comment-form form-contact rounded bordered">
            <form action="/post/addNewComment" id="frm_reply_comment_${commentId}" class="comment-form" method="post">
                <input type="hidden" name="postId" value="${$('#hid_post_id').val()}" />
                <input type="hidden" name="replyId" value="${commentId}" />
                <div class="messages"></div>
                <div class="row">
                    <div class="column col-md-12">
                        <div class="form-group">
                            <textarea name="content" id="txt_reply_content_${commentId}" class="form-control" rows="4" placeholder="Bạn muốn bình luận gì á..." required="required"></textarea>
                        </div>
                    </div>
                </div>
                <input type="submit" name="submit" id="btn_send_reply" class="btn btn-default" value="Gửi"/>
            </form>
        </div>
        `;
    }
}