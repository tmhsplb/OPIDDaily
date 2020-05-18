$(function () {
    var theHub = $.connection.dailyHub;

    theHub.client.refreshPage = function () {
        var currentPage = jQuery("#clientsGrid").jqGrid('getGridParam', 'page');
        // alert("currentPage = " + currentPage)
        var url = "GetMyClients?page=pageToken&rows=25",
        // @Url.Action("GetMyClients", "CaseManager", new { page = "pageToken", rows = "rowsToken" }) ";

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