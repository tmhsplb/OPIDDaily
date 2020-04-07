$(function () {
    var theHub = $.connection.dailyHub;

    theHub.client.refreshPage = function () {
        var currentPage = jQuery("#clientsGrid").jqGrid('getGridParam', 'page');
        // alert("currentPage = " + currentPage)
        var url = "GetClients?page=pageToken&rows=25"
            // "@Url.Action("GetClients", "FrontDesk", new { page = "pageToken", rows = "rowsToken" })";

        url = url.replace("pageToken", currentPage);
        url = url.replace("rowsToken", 25);
        // alert("url = " + url);

        $.ajax({
            url: url,
            cache: false,
            dataType: "json",
            success: function (data) {
                var mygrid = jQuery("#clientsGrid")[0];
                mygrid.addJSONData(data);
            }
        });
    };

    $.connection.hub.start().done(function () {
    });
});