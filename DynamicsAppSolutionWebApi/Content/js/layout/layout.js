$(function () {
    $(".footer").css("top", $("body").height() - 50 + "px");
})

function showLoading() {
    $(".loading-container").show().animate({ "opacity": "1" });
} 

function hideLoading() {
    $(".loading-container").animate({ "opacity": "0" }).hide();
}