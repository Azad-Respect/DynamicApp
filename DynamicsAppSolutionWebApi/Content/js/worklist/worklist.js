$(function () {
    getWorkTypes();

})

// Invoke GetWorkTypes webmethod
function getWorkTypes() {
    $.ajax({
        type: "POST",
        url: "Worklist/GetWorkTypes",
        data: JSON.stringify({
            'userName': 'RSNETDEVeloper',
            'password': 'bltx6RlWSVW6xmY9KNshMw==',
            'domain': 'kbt.local',
            'companies': '-1,AKN,BLT,DAT,KBT,SBT,SNT,TST,TTR,URT',
            'utcTime': '2019-07-18T14:05:16',
            'signatureValue': 'MTdlOWExMmY4MDZkZWYxMmQzZjg4NDJjMzA3M2I1NTM='
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "",
        success: function (list) {
            createWorkTypeGrid(list);

            $(".tile").hover(
                function () {
                    $(this).find(".tile-back-div").stop(true, false).animate({ "opacity": "0.8" });
                    $(this).find(".tile-title").stop(true, false).css({ "color": "#3c0140" });
                    $(this).find(".tile-count").stop(true, false).css({ "color": "#3c0140" });
                }
                , function () {
                    $(this).find(".tile-back-div").stop(true, false).animate({ "opacity": "0.4" });
                    $(this).find(".tile-title").stop(true, false).css({ "color": "rgb(255, 204, 0)" });
                    $(this).find(".tile-count").stop(true, false).css({ "color": "rgb(255, 204, 0)" });
                });
        },
        error: function () {
            alert("GetWorkTypes method error");
        }
    });
}

// Create worktype grid at runtime
function createWorkTypeGrid(list) {
    for (var i = 0; i < list.length; i++) {
        $(".tile-container").append(
            "<div class='tile'>" +
            "<div class='tile-back-div'></div>" +
            "<div class='tile-title'>" + list[i].WorkTypeName + "</div>" +
            "<div class='tile-count'>" + list[i].WorkTypeCount + "</div>" +
            "</div> ");
    }
}


