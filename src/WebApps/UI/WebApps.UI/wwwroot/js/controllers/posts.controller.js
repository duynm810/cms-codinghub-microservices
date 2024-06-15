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
        console.log('Registering events');

        // Handle submission of new comment form (Xử lý submit form bình luận mới)
        $("#commentform").submit(function (e) {
            e.preventDefault();

            // Check form submisstion (Kiểm tra việc submit form)
            console.log('Comment form submitted');

            const form = $(this);
            const url = form.attr('action');

            // Check url (Kiểm tra URL)
            console.log('Submitting form to URL:', url);

            $.post(url, form.serialize()).done(function (response) {
                // Check response (Kiểm tra phản hồi)
                console.log('Form submission successful:', response);

                // Generate the HTML for the new comment and add it to the comments list (Tạo HTML cho bình luận mới và thêm vào danh sách bình luận)
                const content = $("#txt_new_comment_content").val();
                const newCommentHtml = generateCommentHtml(response.data.id, content, new Date(), $('#hid_current_login_name').val());

                // Reset form and update interface (Reset form và cập nhật giao diện)
                $("#txt_new_comment_content").val('');
                $('#comment_list').prepend(newCommentHtml);
                
                const numberOfComments = parseInt($('#hid_number_comments').val()) + 1;
                $('#hid_number_comments').val(numberOfComments);
                $('#comments-title').text('Các bình luận (' + numberOfComments + ')');
            });
        });

        // Handle when the user clicks the "Reply" button (Xử lý khi người dùng nhấp vào nút "Reply")
        $('body').on('click', '.comment-reply-link', function (e) {
            e.preventDefault();

            // Check for clicking on the reply link (Kiểm tra việc click vào reply link)
            console.log('Reply link clicked');

            // Display comment reply form (Hiển thị form trả lời bình luận)
            const commentId = $(this).data('commentid');
            const replyFormHtml = generateReplyFormHtml(commentId);

            $('#reply_comment_' + commentId).html(replyFormHtml);

            // Set a timeout to hide the reply form if there is no input after 10 seconds (Thiết lập thời gian chờ để ẩn form trả lời nếu không có nhập liệu sau 10 giây)
            let replyTimeout = setTimeout(function() {
                if ($('#txt_reply_content_' + commentId).val().trim() === '') {
                    $('#reply_comment_' + commentId).html('');
                }
            }, 5000); // 10 giây

            // If the user starts typing, cancel the timeout (Nếu người dùng bắt đầu nhập, hủy bỏ thời gian chờ)
            $('#txt_reply_content_' + commentId).on('input', function() {
                clearTimeout(replyTimeout);
            });

            // Nếu người dùng rời khỏi textarea mà không nhập gì, thiết lập thời gian chờ để ẩn form
            $('#txt_reply_content_' + commentId).on('blur', function() {
                replyTimeout = setTimeout(function() {
                    if ($('#txt_reply_content_' + commentId).val().trim() === '') {
                        $('#reply_comment_' + commentId).html('');
                    }
                }, 5000); // 10 giây
            });

            // Nếu người dùng quay lại textarea, hủy bỏ thời gian chờ
            $('#txt_reply_content_' + commentId).on('focus', function() {
                clearTimeout(replyTimeout);
            });

            // Handle submit form to reply to comments (Xử lý submit form trả lời bình luận)
            $("#frm_reply_comment_" + commentId).submit(function (e) {
                e.preventDefault();

                // Check the submission of the answer form (Kiểm tra việc submit form trả lời)
                console.log('Reply form submitted for commentId:', commentId);

                const form = $(this);
                const url = form.attr('action');

                // Check url (Kiểm tra URL)
                console.log('Submitting reply form to URL:', url);

                $.post(url, form.serialize()).done(function (response) {
                    // Kiểm tra phản hồi
                    console.log('Reply form submission successful:', response);

                    const content = $("#txt_reply_content_" + commentId).val();
                    const newReplyHtml = generateReplyHtml(response.data.id, content, new Date(), $('#hid_current_login_name').val());

                    // Reset form and update interface (Reset form và cập nhật giao diện)
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

    function formatRelativeTime(date) {
        const now = new Date();
        const diff = Math.abs(now - date);
        const minutes = Math.floor(diff / 60000);
        if (minutes < 60) {
            return `${minutes} phút trước`;
        }
        const hours = Math.floor(minutes / 60);
        if (hours < 24) {
            return `${hours} giờ trước`;
        }
        const days = Math.floor(hours / 24);
        return `${days} ngày trước`;
    }

    function generateCommentHtml(id, content, createdDate, userId, childrenHtml = '') {
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
                                    <h5><a href="#">${userId}</a></h5>
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

    function generateReplyHtml(id, content, createdDate, userId) {
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
                                    <h5><a href="#">${userId}</a></h5>
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
            <form action="/posts/addNewComment" id="frm_reply_comment_${commentId}" class="comment-form" method="post">
                <input type="hidden" name="postId" value="${$('#hid_post_id').val()}" />
                <input type="hidden" name="replyId" value="${commentId}" />
                <div class="messages"></div>
                <div class="row">
                    <div class="column col-md-12">
                        <div class="form-group d-flex align-items-center">
                            <textarea name="content" id="txt_reply_content_${commentId}" class="form-control" rows="2" placeholder="Xin nhập bình luận..." required="required"></textarea>
                            <button type="submit" id="btn_send_reply" class="btn btn_send_reply ml-2">
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