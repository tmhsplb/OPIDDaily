$(function () {
    var theHub = $.connection.dailyHub;

    theHub.client.refreshPage = function () {
        var currentPage = jQuery("#clientsGrid").jqGrid('getGridParam', 'page');
        // alert("currentPage = " + currentPage)
        var url = "GetMyClients?page=pageToken",
        // @Url.Action("GetMyClients", "CaseManager", new { page = "pageToken", rows = "rowsToken" }) ";

        url = url.replace("pageToken", currentPage);
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

    theHub.client.refreshConversation = function (nowServing) {
        var currentPage = jQuery("#conversationGrid").jqGrid('getGridParam', 'page');
        var url = "GetConversation?page=pageToken&nowServing="+nowServing; // "@Url.Action("GetDashboard", "BackOffice", new { page = "pageToken" })";
        url = url.replace("pageToken", currentPage);

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

    theHub.client.openConversation = function () {
        var currentPage = jQuery("#conversationGrid").jqGrid('getGridParam', 'page');
        var url = "GetConversation?page=pageToken"
        url = url.replace("pageToken", currentPage);

        jQuery("#conversation").removeClass("hideConversation");

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