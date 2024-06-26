document.addEventListener('DOMContentLoaded', function () {
    $(document).ready(function () {
        // Apply nice-select to all select elements, except elements with class 'ql-header'
        $('select').not('.ql-header').niceSelect();

        // Sau khi khởi tạo Quill, loại bỏ nice-select trên các phần tử của Quill
        const toolbarOptions = [
            [{'header': [1, 2, 3, false]}],
            ['bold', 'italic', 'underline'],
            ['link', 'image', 'video'],
            [{'list': 'ordered'}, {'list': 'bullet'}],
            [{'align': []}],
            ['clean']
        ];

        const quill = new Quill('#editor', {
            theme: 'snow',
            modules: {
                toolbar: toolbarOptions
            }
        });

        // Listen for form submission to copy content from Quill to textarea
        document.querySelector('form').onsubmit = function () {
            document.querySelector('textarea[name="Content"]').value = quill.root.innerHTML;
        };

        // Loại bỏ nice-select khỏi các phần tử của Quill
        $('.ql-toolbar .nice-select').removeClass('nice-select').removeAttr('style').find('.current, .list').remove();
    });
});
