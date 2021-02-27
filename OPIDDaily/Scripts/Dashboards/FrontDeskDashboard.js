var lastServed = 0;
var rowsToColor = [];


$("#dashboardGrid").jqGrid({
    url: "GetDashboard", // "@Url.Action("GetDashboard", "FrontDesk")",
    datatype: "json",
    pageable: true,
    mtype: "Get",
    colNames: ['Id', 'CM', 'Agency', 'Expires', 'Stage', 'C', 'H', 'Last Name', 'First Name', 'Middle Name', 'Birth Name', 'DOB', 'Age', 'ACK', 'XID', 'XBC', 'MSG', 'Notes'],
    colModel: [
        { key: true, hidden: true, name: 'Id', index: 'Id', search: true},
        { key: false, align: 'center', name: 'ServiceTicket', index: 'ServiceTicket', width: 50, editable: true, sortable: true, search: false },
        { key: false, name: 'AgencyName', index: 'AgencyName', width: 150, editable: false, sortable: false, search: true },
        { key: false, align: 'center', name: 'Expiry', index: 'Expiry', formatter: 'date', width: 120, editable: false, sortable: true, search: false },
        { key: false, name: 'Stage', index: 'Stage', width: 100, formatter: rowColorFormatter, editable: true, edittype: 'select', editoptions: { value: { 'Screened': 'Screened', 'CheckedIn': 'CheckedIn', 'Interviewing': 'Interviewing', 'BackOffice': 'BackOffice', 'Done': 'Done' } }, sortable: false, search: false },
        { key: false, name: 'Conversation', index: 'Conversation', width: 35, align: 'center', editable: true, edittype: "checkbox", editoptions: { value: "Y:''" }, sortable: false, search: false },
        { key: false, name: 'HeadOfHousehold', index: 'HeadOfHousehold', width: 35, align: 'center', editable: false, sortable: false, search: false },
        { key: false, name:'LastName', index: 'LastName', width: 150, editable: true, sortable: false, search: true },
        { key: false, name: 'FirstName', index: 'FirstName', width: 150, editable: true, sortable: false, search: true },
        { key: false, name: 'MiddleName', index: 'MiddleName', width: 150, editable: true, sortable: false, search: true },
        { key: false, name: 'BirthName', index: 'BirthName', width: 150, editable: true, sortable: false, search: true },
        // { key: false, align: 'center', name: 'DOB', index: 'DOB', formatter: 'date', width: 120, editable: true, sortable: true, search: false },
        { key: false, align: 'center', name: 'sDOB', index: 'sDOB', width: 120, editable: true, sortable: true, search: true },
        { key: false, align: 'center', name: 'Age', index: 'Age', width: 50, editable: false, sortable: true, search: false },
        { name: 'PND', index: 'PND', align: 'center', width: 50, editable: false, edittype: "checkbox", editoptions: { value: "Y:''" }, search: false },
        { name: 'XID', index: 'XID', align: 'center', width: 50, editable: true, edittype: "checkbox", editoptions: { value: "Y:''" }, search: false },
        { name: 'XBC', index: 'XBC', align: 'center', width: 50, editable: true, edittype: "checkbox", editoptions: { value: "Y:''" }, search: false },
        { key: false, hidden: true, name: 'MSG', index: 'MSG', width: 80, formatter: rowColorFormatter, editable: false, sortable: false, search: false },
        { key: false, name: 'Notes', index: 'Notes', width: 150, editable: true, sortable: false, search: false, edittype: 'textarea', editoptions: { rows: '2', cols: '300' } }
    ],
    pager: '#dashboardPager',
    rowNum: 15,

    onSelectRow: function (nowServing) {
        if (nowServing == null || nowServing == lastServed) {
            // Prevent infinite recursion caused by reloadGrid
            // alert("Prevent infinite recursion");
        } else {
            lastServed = nowServing;

            var dashboard = jQuery("#dashboardGrid"),
               // selRowId = dashboard.jqGrid('getGridParam', 'selrow'),
                hasConversation = dashboard.jqGrid('getCell', nowServing, 'Conversation');

            if (hasConversation == "Y") {
                jQuery("#conversation").removeClass("hideConversation");
            } else {
                jQuery("#conversation").addClass("hideConversation");
            }

            jQuery("#dashboardGrid").jqGrid('setGridParam',
                {
                    postData: { nowConversing: nowServing },
                    url: "NowConversing", // "@Url.Action("NowServing", "FrontDesk")"
                }).trigger('reloadGrid', { fromServer: true });
        }
    },

    height: "100%",
    viewrecords: true,
    loadonce: false,
    loadComplete: function () {
        //  alert("load is Complete");
        jQuery("#dashboardGrid").jqGrid('setSelection', lastServed);
    },

    gridComplete: function () {
        for (var i = 0; i < rowsToColor.length; i++) {
            //  alert("colored row: " + rowsToColor[i].rowId + " " + rowsToColor[i].rowColor);
            $("#" + rowsToColor[i].rowId).css("color", rowsToColor[i].rowColor)
        }
    },

    caption: 'Service Requests Dashboard',
    emptyrecords: 'No records to display',

    jsonReader: {
        root: "rows",
        page: "page",
        total: "total",
        records: "records",
        repeatitems: false,
        id: "Id"
    },
    autowidth: true,
    multiselect: false,

    subGrid: true,
    subGridRowExpanded: function (subgrid_id, row_id) {
        var subgrid_table_id, pager_id;
        subgrid_table_id = subgrid_id + "t";
        pager_id = "p_" + subgrid_table_id;
        $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "' class='scroll'></table><div id='" + pager_id + "' class='scroll'></div>");
        jQuery("#" + subgrid_table_id).jqGrid({
            postData: { id: function () { /* alert("id of expanded row = " + row_id); */ return row_id } },  // the secret to getting the row id in the post!
            url: "GetDependents", // "@Url.Action("GetDependents", "FrontDesk")",
            datatype: "json",
            mtype: 'post',
            editurl: "Dummy", // "@Url.Action("Dummy", "FrontDesk")",
            cellsubmit: 'clientArray',
            colNames: ['Id', 'First Name', 'Middle Name', 'Last Name', 'Birth Name', 'DOB', 'Age', /* 'ACK', */ 'XID', 'XBC', 'Notes'],
            colModel: [
                { key: true, hidden: true, name: 'Id', index: 'Id', editable: true },
                { key: false, name: 'FirstName', index: 'FirstName', width: 100, editable: true },
                { key: false, name: 'MiddleName', index: 'MiddleName', width: 100, editable: true },
                { key: false, name: 'LastName', index: 'LastName', width: 100, editable: true },
                { key: false, name: 'BirthName', index: 'BirthName', width: 100, editable: true, sortable: false, search: false },
               // { key: false, align: 'center', name: 'DOB', index: 'DOB', formatter: 'date', width: 120, editable: true },
                { key: false, align: 'center', name: 'sDOB', index: 'sDOB', width: 120, editable: true, search: false },
                { key: false, align: 'center', name: 'Age', index: 'Age', width: 50, editable: false, sortable: false, search: false },
              //  { name: 'PND', index: 'PND', align: 'center', width: 35, editable: false, edittype: "checkbox", editoptions: { value: "Y:''" } },
                { name: 'XID', index: 'XID', align: 'center', width: 35, editable: true, edittype: "checkbox", editoptions: { value: "Y:''" } },
                { name: 'XBC', index: 'XBC', align: 'center', width: 35, editable: true, edittype: "checkbox", editoptions: { value: "Y:''" } },
                { key: false, name: 'Notes', index: 'Notes', width: 150, sortable: false, editable: true, edittype: 'textarea', editoptions: { rows: '2', cols: '300' } }
            ],
            rowNum: 10,
            pager: pager_id,
            onSelectRow: function (nowServing) {
                // alert("subgrid nowServing = " + nowServing);
                if (nowServing == null || nowServing == lastServed) {
                    // Prevent infinite recursion caused by reloadGrid
                } else {
                    lastServed = nowServing;

                    jQuery("#dashboardGrid").jqGrid('setGridParam',
                        {
                          postData: { nowServing: nowServing },
                          url: "NowServing", // "@Url.Action("NowServing", "FrontDesk")"
                          }).trigger('reloadGrid', { fromServer: true }).jqGrid('setSelection', nowServing, true);
                 }       
            },
            height: '100%',
            // https://stackoverflow.com/questions/3213984/jqgrid-giving-exception-when-json-is-attempted-to-be-loaded
            jsonReader: { repeatitems: false }
        });

        // Insert jquery("#" + subgrid_table_id).jqgrid('navGrid', ...  )  HERE
        jQuery("#" + subgrid_table_id).jqGrid('navGrid', "#" + pager_id, { edit: true, add: false, del: false, search: false, refresh: false },
            {
                zIndex: 100,
                url: "EditDependentClient", //  "@Url.Action("EditDependentClient", "FrontDesk")",
                closeOnEscape: true,
                closeAfterEdit: true,
                recreateForm: true,
                afterComplete: function (response) {
                    if (response.responseText) {
                        //   alert("Edited dependent client: " + response.responseText);
                    }
                }
            }
        )
    } // close subgridRowExpanded 
})

jQuery("#dashboardGrid").jqGrid('filterToolbar', { searchOperators: true });
jQuery("#dashboardGrid").jqGrid('navGrid', '#dashboardPager', { edit: true, add: false, del: false, search: false, refresh: true },
    {
        zIndex: 100,
        url: "EditClient", // "@Url.Action("EditClient", "FrontDesk")",
        closeOnEscape: true,
        closeAfterEdit: true,
        recreateForm: true,

        afterComplete: function (response) {
            if (response.responseText == "OpenConversation") {
                var theHub = $.connection.dailyHub;
                theHub.client.openConversation();
            }
        }
    });

// See: https://stackoverflow.com/questions/3908171/jqgrid-change-row-background-color-based-on-condition
function rowColorFormatter(cellValue, options, rowObject) {
    if (cellValue != null && cellValue == "END") {
        // End of conversation. Turn coloring off.
        rowsToColor[rowsToColor.length] = { rowId: rowObject.Id, rowColor: "#000000" };  // black
    } else if (cellValue != null && (cellValue == "FromAgency" || cellValue == "FromOPID" || cellValue == "FromInterviewer")) {
        // alert("cellValue == FromAgency");
        rowsToColor[rowsToColor.length] = { rowId: rowObject.Id, rowColor: "#00FF00" };  // green
    } else if (cellValue != null && cellValue == "FromFrontDesk") {
        // alert("cellValue == FromOPID");
        rowsToColor[rowsToColor.length] = { rowId: rowObject.Id, rowColor: "#FF0000" };  // red
    } else if (cellValue != null && cellValue == "StageChange") {
        // alert("cellValue == FromOPID");
        rowsToColor[rowsToColor.length] = { rowId: rowObject.Id, rowColor: "#0000FF" };  // blue
    }
    return cellValue;
}

// http://www.trirand.com/blog/?page_id=393/help/how-to-use-add-form-dialog-popup-window-set-position
$.extend($.jgrid.edit, {
    recreateForm: true,
    closeAfterAdd: true,
    dataheight: '100%',
    reloadAfterSubmit: true,
    width: 500,
    top: 300,
    left: 450,
    editCaption: "Edit Client",
    bSubmit: "Submit",
    bCancel: "Cancel",
    bClose: "Close",
    saveData: "Data has been changed! Save changes?",
    bYes: "Yes",
    bNo: "No",
    bExit: "Cancel"
});