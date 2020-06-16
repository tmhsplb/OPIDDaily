
$(function () {
    var theHub = $.connection.dailyHub;

    theHub.client.refreshPage = function () {
        var currentPage = jQuery("#historyGrid").jqGrid('getGridParam', 'page');
        var url = "GetVisitHistory?page=pageToken&rows=15"; 
        url = url.replace("pageToken", currentPage);

        $.ajax({
            url: url,
            cache: false,
            dataType: "json",
            success: function (data) {
                var mygrid = jQuery("#historyGrid")[0];
                mygrid.addJSONData(data);
            }
        });
    };

    $.connection.hub.start().done(function () {
    });
});