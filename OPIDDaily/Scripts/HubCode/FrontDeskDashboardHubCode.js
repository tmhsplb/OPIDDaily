$(function () {
    var theHub = $.connection.dailyHub;

    theHub.client.refreshPage = function () {
        var currentPage = jQuery("#dashboardGrid").jqGrid('getGridParam', 'page');
        var url = "GetDashboard?page=pageToken&rows=25"; // @Url.Action("GetDashboard", "FrontDesk", new { page = "pageToken" })";
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

    theHub.client.refreshConversation = function (action) {
        var currentPage = jQuery("#conversationGrid").jqGrid('getGridParam', 'page');
        var url = "GetConversation?page=pageToken&rows=20"; // "@Url.Action("GetDashboard", "BackOffice", new { page = "pageToken" })";
        url = url.replace("pageToken", currentPage);

        if (action == "Open") {
            jQuery("#conversation").removeClass("hideConversation");
        }

        // alert("clientName = " + action);
        // jQuery("#conversationGrid").jqGrid('setGridParam', { caption: action }).trigger("reloadGrid");
        $.ajax({
            url: url,
            cache: false,
            dataType: "json",
            success: function (data) {
                var mygrid = jQuery("#conversationGrid")[0];
                mygrid.addJSONData(data);

            }
        });
    };

    $.connection.hub.start().done(function () {
    });
});