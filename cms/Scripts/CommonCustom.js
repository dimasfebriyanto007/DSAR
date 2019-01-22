setTimeout(function () {
    $("body").append("<div id='blokade' style='position: fixed; width: 100%; height: 100%; z-index: 100; background-color: rgba(0,0,0,0.5); top: 0;'>aaaa</div>");
    //window.location.href = "/Account/LogOff";
    $("#blokade").click(function () {
        window.location.href = "/Account/LogOff";
    });
}, 900000);



