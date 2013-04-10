mp.EditorLogicKO = (function ($) {
    "use strict";

    function InlineEditorViewModel(element) {
        var self = this;
        self.element = element;

        self.ShowEditAllowed = true;
        self.ShowSaveAllowed = false;
        self.EditButtonVisible = ko.observable(false);
        self.SaveButtonVisible = ko.observable(false);
        self.ShowEditButton = function() {
            if (self.ShowEditAllowed) {
                self.EditButtonVisible(true);
            }
        };

        self.HideEditButton = function() {
            self.EditButtonVisible(false);
        };
        
        self.ShowSaveButton = function () {
            if (self.ShowSaveAllowed) {
                self.SaveButtonVisible(true);
            }
        };

        self.HideSaveButton = function () {
            self.SaveButtonVisible(false);
        };

        self.EnableEdit = function () {
            self.ShowEditAllowed = false;
            self.HideEditButton();
            var editableBlock = $(element).find('.editable-block')[0];
            $(editableBlock).attr('contenteditable', 'true');
            self.editor = CKEDITOR.inline(editableBlock);
            
            self.editor.on('blur', function () {
                self.ShowSaveAllowed = true;
                self.ShowSaveButton();
            });
        };
        

        self.SaveEditedContent = function () {
            self.ShowSaveAllowed = false;
            self.HideSaveButton();
            $.ajax({
                url: $('#SaveContentUrl').val(),
                global: false,
                type: "POST",
                data: {
                    key: window.location.pathname + '#' + element.id,
                    data: self.editor.getData()
                }
            }).done(function (resp) {
                if (resp.res == "OK") {
                    self.ShowEditAllowed = true;
                } else {
                    alert(resp.message);}
            });

            
        };
    }

    var init = function () {

        window.CKEDITOR_BASEPATH = "/Content/inlineEditor/ckeditor";

        CKEDITOR.on('instanceReady', function (event) {
            event.editor.focus();
        });
        CKEDITOR.on('instanceCreated', function (event) {
            var editor = event.editor,
				element = editor.element;

            editor.on('configLoaded', function () {

                editor.config.language = 'uk';

                editor.config.font_names = editor.config.font_names + ';MyriadPro/MyriadPro-Regular;' + 'MyriadPro Bold/MyriadPro-Bold';

                // Remove unnecessary plugins to make the editor simpler.
                //editor.config.removePlugins = 'colorbutton,find,flash,font,' +
                //    'forms,iframe,image,newpage,removeformat,' +
                //    'smiley,specialchar,stylescombo,templates';

                editor.config.extraPlugins = 'misechkoH1,misechkoH2,misechkoP,radaImg,fastimage';

                // Rearrange the layout of the toolbar.
                editor.config.toolbarGroups = [
                    { name: 'editing', groups: ['basicstyles', 'links'] },
                    { name: 'undo' },
                    { name: 'clipboard', groups: ['selection', 'clipboard'] },
                    { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align'] },
                    '/',
                    { name: 'misechko', items: ['misechkoH1', 'misechkoH2', 'misechkoP'] },
                    { name: 'document', groups: ['mode', 'document', 'doctools'] },
                    { name: 'styles' },
                    { name: 'radacode', items: ['fastimage'] }
                    ];
                });
        });


        $(".editable-wrapper").each(function () {
            ko.applyBindings(new InlineEditorViewModel(this), this);
        });
    };

    return { init: init };
}($));

mp.EditorLogic = (function ($) {
    "use strict";

    var ready = $(function () {
        mp.EditorLogicKO.init();
    });
}($));