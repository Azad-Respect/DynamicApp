var screenHeight;

$(function () {
    //hide left menu icons
    $(".left-menu-item-icon-div").hide();

    responsibleRightMenu();

    screenHeight = $(window).height();

    browserResize();

    rightMenuHoverAnimations();

    hideUserContainer();

    // temporary
    $(".footer").hide();
})

// right menu items height balance
function responsibleRightMenu() {
    $(".right-menu-item").css("height", (screenHeight / 5) - 5);
    $(".right-menu-item-icon-div").css("transform", "translateY(-50%)");
    $(".right-menu-item-container").css("line-height", (screenHeight / 5) - 5 + "px");
}

// execute when browser size is changed
function browserResize() {
    $(window).resize(function () {
        screenHeight = $(window).height();
        responsibleLeftMenu();
        responsibleRightMenu();
    });
}

// execute when hover on right menu
function rightMenuHoverAnimations() {
    $(".notification-link").hover(function () {
        $(".right-menu-item.notification").stop(true, false).animate({ "opacity": "1" });
        $(".left-menu-item.notification").stop(true, false).animate({ "opacity": "1" });
    }
        , function () {
            $(".right-menu-item.notification").stop(true, false).animate({ "opacity": "0.59" });
            $(".left-menu-item.notification").stop(true, false).animate({ "opacity": "0.49" })
        });

    $(".worklist-link").hover(function () {
        $(".right-menu-item.worklist").stop(true, false).animate({ "opacity": "1" });
        $(".left-menu-item.worklist").stop(true, false).animate({ "opacity": "1" });
    }
        , function () {
            $(".right-menu-item.worklist").stop(true, false).animate({ "opacity": "0.59" });
            $(".left-menu-item.worklist").stop(true, false).animate({ "opacity": "0.49" })
        });

    $(".historical-link").hover(function () {
        $(".right-menu-item.historical").stop(true, false).animate({ "opacity": "1" });
        $(".left-menu-item.historical").stop(true, false).animate({ "opacity": "1" });
    }
        , function () {
            $(".right-menu-item.historical").stop(true, false).animate({ "opacity": "0.59" });
            $(".left-menu-item.historical").stop(true, false).animate({ "opacity": "0.49" })
        });

    $(".settings-link").hover(function () {
        $(".right-menu-item.settings").stop(true, false).animate({ "opacity": "1" });
        $(".left-menu-item.settings").stop(true, false).animate({ "opacity": "1" });
    }
        , function () {
            $(".right-menu-item.settings").stop(true, false).animate({ "opacity": "0.59" });
            $(".left-menu-item.settings").stop(true, false).animate({ "opacity": "0.49" })
        });

    $(".logout-link").hover(function () {
        $(".right-menu-item.logout").stop(true, false).animate({ "opacity": "1" });
        $(".left-menu-item.logout").stop(true, false).animate({ "opacity": "1" });
    }
        , function () {
            $(".right-menu-item.logout").stop(true, false).animate({ "opacity": "0.59" });
            $(".left-menu-item.logout").stop(true, false).animate({ "opacity": "0.49" })
        });
}

// hide layout user container
function hideUserContainer() {
    $(".user-container").hide();
}