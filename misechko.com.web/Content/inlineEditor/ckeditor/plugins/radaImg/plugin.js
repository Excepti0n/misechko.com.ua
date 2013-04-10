CKEDITOR.plugins.add('radaImg',
{
    init: function (editor) {
        var pluginName = 'radaImg';
        editor.ui.addButton('radaImg',
            {
                label: 'Добавить изображение',
                command: 'AddRadaImg'
            });
        editor.addCommand('AddRadaImg', {
            exec: function () {
                editor.insertHtml('<div class="block-headline"><div class="light-ltr">А</div><h1>Заголовок</h1></div><p>Абзац под заголовком</p>');
            }
        });
    }


});