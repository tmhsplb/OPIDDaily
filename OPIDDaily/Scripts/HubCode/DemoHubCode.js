$(function () {
    var theHub = $.connection.dailyHub;

    theHub.client.refreshPage = function () {
        var currentPage = jQuery("#dashboardGrid").jqGrid('getGridParam', 'page');
        var url = "GetDemoDashboard?page=pageToken" // "@Url.Action("GetDemoDashboard", "Superadmin", new { page = "pageToken" })";
        url = url.replace("pageToken", currentPage);

        $.ajax({
            url: url,
            cache: false,
            dataType: "json",
            success: function (data) {
                var mygrid = jQuery("#dashboardGrid")[0];
                mygrid.addJSONData(data);
            }
        });
    };

    $.connection.hub.start().done(function () {
    });
});