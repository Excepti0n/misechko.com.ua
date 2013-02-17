CKEDITOR.plugins.add('misechkoH1',
{
    init: function (editor) {
        var pluginName = 'misechkoH1';
        editor.ui.addButton('misechkoH1',
            {
                label: 'Добавить заголовок',
                command: 'AddMisechkoH1'
            });
        editor.addCommand('AddMisechkoH1', { exec: function () {
            editor.insertHtml('<div class="block-headline"><div class="light-ltr">А</div><h1>Заголовок</h1></div><p>Абзац под заголовком</p>');
        } });
    }
    

});
