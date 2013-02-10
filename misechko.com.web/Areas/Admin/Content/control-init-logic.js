$(function () {

    var UsersInitialized = false;

    $("#control-area").tabs({
        select: function (event, ui) {
            switch(ui.index)
            {
                case 1:
                    if (!UsersInitialized) {
                        UsersView.Init();
                        UsersInitialized = true;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }
    });
})