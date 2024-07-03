document.addEventListener('DOMContentLoaded', function () {
    $(document).ready(function () {
        const serverUrl = 'http://localhost:6001'; // Thay thế bằng URL của server của bạn

        // Apply nice-select to all select elements, except elements with class 'ql-header'
        $('select').not('.ql-header').niceSelect();

        const toolbarOptions = [
            // Text formatting (Định dạng văn bản như in đậm, in nghiêng, gạch chân, gạch ngang)
            ['bold', 'italic', 'underline', 'strike'],

            // Headers and block elements (Tiêu đề và phần tử khối như blockquote, code block)
            [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
            ['blockquote', 'code-block'],

            // Lists and checkboxes (Danh sách đánh số, danh sách dấu chấm, danh sách có checkbox)
            [{ 'list': 'ordered' }, { 'list': 'bullet' }, { 'list': 'check' }],

            // Indentation (Tăng/giảm thụt lề)
            [{ 'indent': '-1' }, { 'indent': '+1' }],

            // Text direction (Hướng văn bản (phải sang trái)
            [{ 'direction': 'rtl' }],

            // Size (Kích thước văn bản)
            [{ 'size': ['small', false, 'large', 'huge'] }],

            // Colors (Màu sắc)
            [{ 'color': [] }, { 'background': [] }],

            // Fonts (Phông chữ)
            [{ 'font': [] }],

            // Text alignment (Căn chỉnh văn bản)
            [{ 'align': [] }],

            // Links, media, and formulas (Liên kết, hình ảnh, video, công thức toán học)
            ['link', 'image', 'video', 'formula'],

            // Clean formatting (Loại bỏ định dạng)
            ['clean']
        ];

        // Declare global quill variable
        window.quill = new Quill('#editor', {
            theme: 'snow',
            modules: {
                toolbar: toolbarOptions,
                syntax: true,  // Include syntax module
            },
            placeholder: 'Compose an epic...'
        });

        const contentFromServer = document.getElementById('editor').getAttribute('data-content');
        quill.root.innerHTML = contentFromServer || '';

        // Remove nice-select from Quill elements
        $('.ql-toolbar .nice-select').removeClass('nice-select').removeAttr('style').find('.current, .list').remove();

        // Add event listener for image upload
        quill.getModule('toolbar').addHandler('image', function() {
            selectLocalImage();
        });

        function selectLocalImage() {
            const input = document.createElement('input');
            input.setAttribute('type', 'file');
            input.setAttribute('accept', 'image/*');
            input.click();

            input.onchange = () => {
                const file = input.files[0];
                if (file) {
                    // Create a local URL for the selected image
                    const reader = new FileReader();
                    reader.onload = (e) => {
                        const range = quill.getSelection();
                        quill.insertEmbed(range.index, 'image', e.target.result);

                        // Upload the image to the server
                        const formData = new FormData();
                        formData.append('file', file);
                        formData.append('type', 'posts');

                        $.ajax({
                            url: '/media/upload-image',
                            type: 'POST',
                            data: formData,
                            contentType: false,
                            processData: false,
                            success: function(result) {
                                const imageUrl = `${serverUrl}/${result.data}`; // Thêm serverUrl vào trước URL của hình ảnh

                                // Replace the local URL with the server URL
                                const images = quill.root.querySelectorAll('img');
                                images.forEach(img => {
                                    if (img.src === e.target.result) {
                                        img.src = imageUrl;
                                    }
                                });
                            },
                            error: function(xhr, status, error) {
                                showErrorNotification('Error uploading image:', error);
                            }
                        });
                    };
                    reader.readAsDataURL(file);
                }
            };
        }
    });
});