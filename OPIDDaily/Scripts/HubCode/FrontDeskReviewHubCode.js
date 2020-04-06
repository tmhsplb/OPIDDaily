$(function () {
    var theHub = $.connection.dailyHub;

    theHub.client.refreshPage = function () {
        var currentPage = jQuery("#reviewGrid").jqGrid('getGridParam', 'page');
        var url = "/OPIDDaily/FrontDesk/GetReviewClients?page=pageToken", // "@Url.Action("GetReviewClients", "FrontDesk", new { page = "pageToken" })";
        url = url.replace("pageToken", currentPage);

        $.ajax({
            url: url,
            cache: false,
            dataType: "json",
            success: function (data) {
                var mygrid = jQuery("#reviewGrid")[0];
                mygrid.addJSONData(data);
            }
        });
    };

    $.connection.hub.start().done(function () {
    });
});