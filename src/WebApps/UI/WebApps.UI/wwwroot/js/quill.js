document.addEventListener('DOMContentLoaded', function () {
    $(document).ready(function () {
        // Apply nice-select to all select elements, except elements with class 'ql-header'
        $('select').not('.ql-header').niceSelect();

        const toolbarOptions = [
            [{'header': [1, 2, 3, false]}],
            ['bold', 'italic', 'underline'],
            ['link', 'image', 'video'],
            [{'list': 'ordered'}, {'list': 'bullet'}],
            [{'align': []}],
            ['clean']
        ];

        // Declare global quill variable
        window.quill = new Quill('#editor', {
            theme: 'snow',
            modules: {
                toolbar: toolbarOptions
            }
        });

        const contentFromServer = document.getElementById('editor').getAttribute('data-content');
        quill.root.innerHTML = contentFromServer || '';

        // Remove nice-select from Quill elements
        $('.ql-toolbar .nice-select').removeClass('nice-select').removeAttr('style').find('.current, .list').remove();
    });
});
