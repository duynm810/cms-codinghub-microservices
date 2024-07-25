const postsController = function () {
    this.initialize = function () {
        const postId = $('#hid_post_id').val();
        console.log('Initializing postsController with postId:', postId); // Kiểm tra postId
        registerEvents();
        loadComments(postId);
    }

    function loadComments(id) {
        $.get('/comments/get-comments-by-post-id?postId=' + id).done(function (response, statusText, xhr) {
            if (xhr.status === 200) {
                if (response && response.data && response.data.length > 0) {
                    let html = '';

                    $.each(response.data, function (index, item) {
                        let childrenHtml = '';

                        if (item.replies && item.replies.length > 0) {
                            console.log('Item has replies:', item.replies); // Log dữ liệu replies

                            $.each(item.replies, function (childIndex, childItem) {
                                let replyId = childItem.id || '';
                                let replyContent = childItem.content || '';
                                let replyCreatedDate = childItem.createdDate || '';
                                let replyUserFullName = (childItem.user && childItem.user.fullName) || '';
                                childrenHtml += generateReplyHtml(replyId, replyContent, replyCreatedDate, replyUserFullName);
                            });
                        }

                        // Kiểm tra và gán giá trị mặc định nếu thuộc tính không tồn tại
                        let commentId = item.id || '';
                        let commentContent = item.content || '';
                        let commentCreatedDate = item.createdDate || '';
                        let commentUserFullName = (item.user && item.user.fullName) || '';

                        html += generateCommentHtml(commentId, commentContent, commentCreatedDate, commentUserFullName, childrenHtml);
                    });

                    $('#comment_list').html(html);
                }
            }
        });
    }

    function registerEvents() {
        console.log('Registering events');

        // Handle submission of new comment form (Xử lý submit form bình luận mới)
        $("#btn_send_comment").on("click", function (e) {
            e.preventDefault();

            const form = $("#commentForm");
            const url = form.attr('action');
            const postId = form.find("input[name='postId']").val();
            const content = $("#txt_new_comment_content").val();

            const commentData = {
                postId: postId, content: content
            };

            $.ajax({
                type: "POST",
                url: url,
                contentType: "application/json",
                data: JSON.stringify(commentData),
                success: function (response) {
                    console.log('Form submission successful:', response);

                    // Generate the HTML for the new comment and add it to the comments list
                    const currentLoginName = $('#hid_current_login_name').val();
                    const currentUserName = $('#hid_current_user_name').val();
                    const newCommentHtml = generateCommentHtml(response.data.id, content, new Date(), currentLoginName, currentUserName);

                    // Reset form and update interface
                    $("#txt_new_comment_content").val(''); // Clear the content of the input
                    $('#comment_list').append(newCommentHtml);

                    const $hiddenNumberOfComments = $('#hid_number_comments');
                    const numberOfComments = parseInt($hiddenNumberOfComments.val()) + 1;
                    $hiddenNumberOfComments.val(numberOfComments);
                    $('#comments-title').text('Các bình luận (' + numberOfComments + ')');
                },
                error: function (error) {
                    console.error('Error submitting form:', error);
                }
            });
        });

        // Handle when the user clicks the "Reply" button (Xử lý khi người dùng nhấp vào nút "Reply")
        $(document).on('click', '.comment-reply-link', function (e) {
            e.preventDefault();

            // Check for clicking on the reply link (Kiểm tra việc click vào reply link)
            console.log('Reply link clicked');

            // Display comment reply form (Hiển thị form trả lời bình luận)
            const commentId = $(this).data('commentid');
            const replyFormHtml = generateReplyFormHtml(commentId);
            const $replyComment = $('#reply_comment_' + commentId);

            $replyComment.html(replyFormHtml);

            // Handle submit form to reply to comments (Xử lý submit form trả lời bình luận)
            $("#frm_reply_comment_" + commentId).on('submit', function (e) {
                e.preventDefault();

                // Check the submission of the answer form (Kiểm tra việc submit form trả lời)
                console.log('Reply form submitted for commentId:', commentId);

                const form = $(this);
                const url = form.attr('action') + "?parentId=" + commentId;
                const $replyContent = $('#txt_reply_content_' + commentId);
                const $childrenComments = $('#children_comments_' + commentId);
                const $hiddenNumberOfComments = $('#hid_number_comments');

                // Check url (Kiểm tra URL)
                console.log('Submitting reply form to URL:', url);

                $.ajax({
                    type: 'POST', 
                    url: url, 
                    contentType: 'application/json', 
                    data: JSON.stringify({
                        postId: form.find("input[name='postId']").val(),
                        content: $replyContent.val()
                    }), success: function (response) {
                        // Kiểm tra phản hồi
                        console.log('Reply form submission successful:', response);

                        const content = $replyContent.val();
                        const currentLoginName = $('#hid_current_login_name').val();
                        const newReplyHtml = generateReplyHtml(response.data.id, content, new Date(), currentLoginName);

                        // Reset form and update interface (Reset form và cập nhật giao diện)
                        $replyContent.val('');
                        $replyComment.html('');
                        $childrenComments.append(newReplyHtml); // Thêm bình luận mới vào cuối danh sách con

                        const numberOfComments = parseInt($hiddenNumberOfComments.val()) + 1;
                        $hiddenNumberOfComments.val(numberOfComments);
                        $('#comments-title').text('Các bình luận (' + numberOfComments + ')');
                    }, error: function (error) {
                        console.error('Error submitting reply form:', error);
                    }
                });
            });
        });
    }

    function formatRelativeTime(date) {
        const now = new Date();
        const diff = Math.abs(now - date);
        const minutes = Math.floor(diff / 60000);
        if (minutes < 60) {
            return `${minutes} minute ago`;
        }
        const hours = Math.floor(minutes / 60);
        if (hours < 24) {
            return `${hours} hour ago`;
        }
        const days = Math.floor(hours / 24);
        return `${days} day ago`;
    }

    function generateCommentHtml(id, content, createdDate, fullName, childrenHtml = '') {
        const authorUrl = "/author/" + '';
        return `
            <li class="comment-item">
                <div class="single-comment justify-content-between d-flex">
                    <div class="user justify-content-between d-flex">
                        <div class="thumb">
                            <img src="../imgs/authors/author-2.jpg" alt="">
                        </div>
                        <div class="desc">
                            <p class="comment">${content}</p>
                            <div class="d-flex justify-content-between align-items-center info-row">
                                <div class="d-flex align-items-center">
                                    <h5><a href="${authorUrl}">${fullName}</a></h5>
                                    <p class="date">${formatRelativeTime(new Date(createdDate))}</p>
                                </div>
                                <div class="reply-btn">
                                    <a href="#" class="btn-reply comment-reply-link" data-commentid="${id}">
                                        <i class="fas fa-reply"></i>
                                    </a>
                                </div>
                            </div>
                            <div id="reply_comment_${id}"></div>
                        </div>
                    </div>
                </div>
                <ul class="children" id="children_comments_${id}">
                    ${childrenHtml}
                </ul>
            </li>
        `;
    }

    function generateReplyHtml(id, content, createdDate, fullName) {
        const authorUrl = "/author/" + '';
        return `
            <li class="comment-item">
                <div class="single-comment depth-2 justify-content-between d-flex mt-30">
                    <div class="user justify-content-between d-flex">
                        <div class="thumb">
                            <img src="../imgs/authors/author.jpg" alt="">
                        </div>
                        <div class="desc">
                            <p class="comment">${content}</p>
                            <div class="d-flex justify-content-between align-items-center info-row">
                                <div class="d-flex align-items-center">
                                    <h5><a href="${authorUrl}">${fullName}</a></h5>
                                    <p class="date">${formatRelativeTime(new Date(createdDate))}</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </li>
        `;
    }

    function generateReplyFormHtml(commentId) {
        return `
            <div class="comment-form form-contact rounded bordered">
                <form action="/comments/reply-to-comment" id="frm_reply_comment_${commentId}" class="comment-form" method="post">
                    <input type="hidden" name="postId" value="${$('#hid_post_id').val()}" />
                    <input type="hidden" name="parentId" value="${commentId}" />
                    <div class="messages"></div>
                    <div class="row">
                        <div class="column col-md-12">
                            <div class="d-flex align-items-center">
                                <textarea name="content" id="txt_reply_content_${commentId}" class="form-control" rows="2" placeholder="Please enter a comment..." required="required"></textarea>
                                <button type="submit" class="btn btn_send_reply ml-2">
                                   <i class="fas fa-paper-plane"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        `;
    }
}