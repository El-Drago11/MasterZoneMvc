function StartLoading() {
    $('#LoaderDiv').fadeTo('slow', 1);
    $('#LoaderDiv').append('<div style="background-color: #ffffff;position: fixed;top: 0;left: 0;width: 100%;height: 300%;z-index: 999999999999999;-moz-opacity: 0.4;opacity: 0.4;"><img src="/Content/images/Preloader_grey.gif" style="background-color: #ffffff;position: fixed;top: 40%;width: 5%;left: 46%;"/></div>');
}

function StopLoading() {
    $('#LoaderDiv').fadeOut(800);
}

