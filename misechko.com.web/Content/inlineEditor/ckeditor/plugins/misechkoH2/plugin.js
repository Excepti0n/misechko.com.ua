CKEDITOR.plugins.add('misechkoH2',
{
    init: function (editor) {
        var pluginName = 'misechkoH2';
        editor.ui.addButton('misechkoH2',
            {
                label: 'Добавить подзаголовок',
                command: 'AddMisechkoH2'
            });
        editor.addCommand('AddMisechkoH2', { exec: function () {
            editor.insertHtml('<h2>Заголовок второго уровня</h2>');
        } });
    }
    

});
