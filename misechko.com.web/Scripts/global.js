var RadaCode = window.RadaCode || {};

RadaCode.SwitchLanguage = function(lang) {
    $.cookie('language', lang, { expires: 365, path: '/' });
    window.location.reload();
};

$(function () {
    $("#setRus").click(function () {
        RadaCode.SwitchLanguage('ru');
    });
    $("#setEng").click(function () {
        RadaCode.SwitchLanguage('en');
    });
    $("#setUkr").click(function () {
        RadaCode.SwitchLanguage('uk');
    });
});