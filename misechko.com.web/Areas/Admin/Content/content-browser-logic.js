$(function () {
    
    $('#content-fileupload').attr("action", $("#FileHandlerUrl").val());

    $('#content-browser-area').corner("right");
    $('#enabler').corner("right");

    var toogleContentBrowser = function() {

        var $browserDiv = $('#content-browser-area')    ;

        if ($browserDiv.hasClass('expanded')) {
            $browserDiv.removeClass('expanded');
            $browserDiv.addClass('collapsed');
        } else {
            $browserDiv.addClass('expanded');
            $browserDiv.removeClass('collapsed');
        }
    };

    $('#content-browser-area').on('click', '#enabler', toogleContentBrowser);

    // Initialize the jQuery File Upload widget:
    $('#content-fileupload').fileupload();
    $('#content-fileupload').fileupload('option', {
        filesContainer: $('#content-fileupload .files'),
        formData: { what: 'oblig-content' }
    });

    var tbody = $('#content-fileupload .files');
    
    // Load existing files:
    $.getJSON($('#content-fileupload').prop('action'), function (files) {
        var fu = $('#content-fileupload').data('fileupload');
        fu._adjustMaxNumberOfFiles(-files.length);

        var rows = fu._renderDownload(files);
        tbody.append(rows);

        fu._forceReflow(rows);
        fu._transition(rows);
    });
    // Open download dialogs via iframes,
    // to prevent aborting current uploads:
    $('#content-fileupload .files a:not([target^=_blank])').live('click', function (e) {
        e.preventDefault();
        $('<iframe style="display:none;"></iframe>')
            .prop('src', this.href)
            .appendTo('body');
    });

});