
$(function () {
    var theHub = $.connection.dailyHub;

    theHub.client.refreshPage = function () {
        var currentPage = jQuery("#dashboardGrid").jqGrid('getGridParam', 'page');
        var url = "GetDashboard?page=pageToken"; 
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

    theHub.client.refreshConversation = function (nowConversing, messageCnt) {
        var currentPage = jQuery("#conversationGrid").jqGrid('getGridParam', 'page');
        var url = "GetConversation?page=pageToken&nowConversing="+nowConversing;  
        url = url.replace("pageToken", currentPage);

        if (messageCnt > 0) {
            jQuery("#messages").removeClass("hideMessages");
        } else {
            jQuery("#messages").addClass("hideMessages");
        }

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