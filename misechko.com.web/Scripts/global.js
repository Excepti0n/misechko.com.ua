var mp = window.mp || {};

mp.Lang = (function ($) {
    "use strict";

    var init = function () {
        mp.Lang.SwitchLanguage = function (lang) {
            $.cookie('language', lang, { expires: 365, path: '/' });
            window.location.reload();
        };

        var idd = $('#langdrop').msDropDown();
        idd.on("change", function (res) {
            mp.Lang.SwitchLanguage(this.value);
        });
    };

    return { init: init };
})($);

mp.GlobalLogic = (function ($) {
    "use strict";

    var ready = $(function () {
        mp.Lang.init();
    });
}($));