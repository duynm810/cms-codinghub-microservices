document.addEventListener('DOMContentLoaded', function () {
    $(document).ready(function () {
        const serverUrl = 'http://localhost:6001'; // Thay thế bằng URL của server của bạn

        let currentImages = new Set();
        
        // Apply nice-select to all select elements, except elements with class 'ql-header'
        $('select').not('.ql-header').niceSelect();

        const toolbarOptions = [
            // Text formatting (Định dạng văn bản như in đậm, in nghiêng, gạch chân, gạch ngang)
            ['bold', 'italic', 'underline', 'strike'],

            // Headers and block elements (Tiêu đề và phần tử khối như blockquote, code block)
            [{'header': [1, 2, 3, 4, 5, 6, false]}],
            ['blockquote', 'code-block'],

            // Lists and checkboxes (Danh sách đánh số, danh sách dấu chấm, danh sách có checkbox)
            [{'list': 'ordered'}, {'list': 'bullet'}, {'list': 'check'}],

            // Indentation (Tăng/giảm thụt lề)
            [{'indent': '-1'}, {'indent': '+1'}],

            // Text direction (Hướng văn bản (phải sang trái)
            [{'direction': 'rtl'}],

            // Size (Kích thước văn bản)
            [{'size': ['small', false, 'large', 'huge']}],

            // Colors (Màu sắc)
            [{'color': []}, {'background': []}],

            // Fonts (Phông chữ)
            [{'font': []}],

            // Text alignment (Căn chỉnh văn bản)
            [{'align': []}],

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
                syntax: true,
                imageDropAndPaste: {
                    handler: imageHandler,
                },
            },
            placeholder: 'Compose an epic...'
        });

        const contentFromServer = document.getElementById('editor').getAttribute('data-content');
        quill.root.innerHTML = contentFromServer || '';

        $('.ql-toolbar .nice-select').removeClass('nice-select').removeAttr('style').find('.current, .list').remove();

        const ImageData = QuillImageDropAndPaste.ImageData;
        quill.getModule('toolbar').addHandler('image', function (clicked) {
            if (clicked) {
                let fileInput = this.container.querySelector('input.ql-image[type=file]')
                if (fileInput == null) {
                    fileInput = document.createElement('input')
                    fileInput.setAttribute('type', 'file')
                    fileInput.setAttribute(
                        'accept',
                        'image/png, image/gif, image/jpeg, image/bmp, image/x-icon'
                    )
                    fileInput.classList.add('ql-image')
                    fileInput.addEventListener('change', function (e) {
                        const files = e.target.files
                        let file
                        if (files.length > 0) {
                            file = files[0]
                            const type = file.type
                            const reader = new FileReader()
                            reader.onload = (e) => {
                                const dataUrl = e.target.result
                                imageHandler(dataUrl, type, new ImageData(dataUrl, type, generateRandomFileName(type)));
                                fileInput.value = ''
                            }
                            reader.readAsDataURL(file)
                        }
                    })
                    this.container.appendChild(fileInput);
                }
                fileInput.click()
            }
        })

        function imageHandler(imageDataUrl, type, imageData) {
            imageData
                .minify({
                    maxWidth: 1024,
                    maxHeight: 1024,
                    quality: 0.9,
                })
                .then((miniImageData) => {
                    const blob = miniImageData.toBlob();
                    const file = miniImageData.toFile(generateRandomFileName(type));

                    // Upload the image to the server
                    const formData = new FormData();
                    formData.append('file', file);
                    formData.append('type', 'posts');

                    $.ajax({
                        url: `/media/upload-image`,
                        type: 'POST',
                        data: formData,
                        contentType: false,
                        processData: false,
                        success: function (result) {
                            const imageUrl = result.data;

                            // Chèn URL ảnh từ server vào editor
                            const range = quill.getSelection();
                            quill.insertEmbed(range.index, 'image', `${serverUrl}/${imageUrl}`);
                            updateCurrentImages();
                        },
                        error: function (xhr, status, error) {
                            showErrorNotification('Error uploading image:', error);
                        }
                    });
                });
        }
        
        quill.on('text-change', function(delta, oldDelta, source) {
            if (source === 'user') {
                const newImages = new Set();
                const images = document.querySelectorAll('img');
                images.forEach(img => {
                    newImages.add(img.src);
                });

                const deletedImages = [];
                currentImages.forEach(image => {
                    if (!newImages.has(image)) {
                        deletedImages.push(image);
                    }
                });

                deletedImages.forEach(fullImageUrl => {
                    let imagePath = new URL(fullImageUrl).pathname;
                    imagePath = imagePath.replace(/^\/+/, '');

                    // Gọi API để xóa ảnh trên server
                    $.ajax({
                        url: `/media/delete-image/${encodeURIComponent(imagePath)}`,
                        type: 'DELETE',
                        contentType: 'application/json',
                        success: function (result) {
                            console.log(`Image deleted: ${imagePath}`);
                        },
                        error: function (xhr, status, error) {
                            showErrorNotification('Error deleting image:', error);
                        }
                    });
                });

                currentImages = newImages;
            }
        });

        function updateCurrentImages() {
            const images = document.querySelectorAll('img');
            currentImages = new Set();
            images.forEach(img => {
                currentImages.add(img.src);
            });
        }

        function generateRandomFileName(type) {
            const randomString = Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);
            const extension = type.split('/')[1];
            return `${randomString}.${extension}`;
        }

        updateCurrentImages();
    });
});