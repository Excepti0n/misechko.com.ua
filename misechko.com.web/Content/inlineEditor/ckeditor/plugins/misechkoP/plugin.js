CKEDITOR.plugins.add('misechkoP',
{
    init: function (editor) {
        var pluginName = 'misechkoP';
        editor.ui.addButton('misechkoP',
            {
                label: 'Добавить абзац',
                command: 'AddMisechkoP'
            });
        editor.addCommand('AddMisechkoP', { exec: function () {
            editor.insertHtml('</br><p>Текст абзаца</p>');
        } });
    }
});
