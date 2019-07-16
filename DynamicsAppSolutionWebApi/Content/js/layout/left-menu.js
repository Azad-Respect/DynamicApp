var screenHeight;

$(function () {
    screenHeight = $(window).height();
    browserResize();
    responsibleLeftMenu();
})

// execute when browser size is changed
function browserResize() {
    $(window).resize(function () {
        screenHeight = $(window).height();
        responsibleLeftMenu();
    });
}

// left menu items height balance
function responsibleLeftMenu() {
    $(".left-menu-item").css("height", (screenHeight / 5) - 17);
    $(".left-menu-item-icon-div").css("transform", "translateY(-50%)");
}

