
var lastServed = 0;
var rowsToColor = [];

$("#clientsGrid").jqGrid({
    url: "GetMyClients",  // "@Url.Action("GetMyClients", "CaseManager")",
    datatype: "json",
    pageable: true,
    mtype: "Get",
    colNames: ['Id', 'Intls', 'Added', 'Expires', 'Stage', 'C', 'H', 'Last Name', 'First Name', 'Middle Name', 'Birth Name', 'DOB', 'Age', 'ACK', 'XID', 'XBC', 'MSG', 'Notes'],
    colModel: [
        { key: true, hidden: true, name: 'Id', index: 'Id' },
        { key: false, align: 'center', name: 'ServiceTicket', index: 'ServiceTicket', width: 55, editable: true, sortable: true, search: false },
        { key: false, align: 'center', name: 'ServiceDate', index: 'ServiceDate', formatter: 'date', width: 120, editable: false, sortable: true, search: false },
        { key: false, align: 'center', name: 'Expiry', index: 'Expiry', formatter: 'date', width: 120, editable: false, sortable: true, search: false },
        { key: false, name: 'Stage', index: 'Stage', width: 100, formatter: rowColorFormatter, editable: false, edittype: 'select', editoptions: { value: { 'CheckedIn': 'CheckedIn', 'Interviewing': 'Interviewing', 'BackOffice': 'BackOffice', 'Done': 'Done' } }, sortable: false, search: false },
        { key: false, name: 'Conversation', index: 'Conversation', width: 35, align: 'center', editable: true, edittype: "checkbox", editoptions: { value: "Y:''" }, sortable: false, search: false },
        { key: false, name: 'HeadOfHousehold', index: 'HeadOfHousehold', width: 35, align: 'center', editable: false, sortable: false, search: false },
        { key: false, name: 'LastName', index: 'LastName', width: 150, editable: true, sortable: false, search: false },
        { key: false, name: 'FirstName', index: 'FirstName', width: 150, editable: true, sortable: false, search: false },
        { key: false, name: 'MiddleName', index: 'MiddleName', width: 150, editable: true, sortable: false, search: false },
        { key: false, name: 'BirthName', index: 'BirthName', width: 150, editable: true, sortable: false, search: false },
        { key: false, align: 'center', name: 'DOB', index: 'DOB', formatter: 'date', width: 120, editable: true, sortable: true, search: false },
        { key: false, align: 'center', name: 'Age', index: 'Age', width: 50, editable: false, sortable: false, search: false },
        { name: 'PND', index: 'PND', align: 'center', width: 55, editable: false, edittype: "checkbox", editoptions: { value: "Y:''" }, },
        { name: 'XID', index: 'XID', align: 'center', width: 50, editable: false, edittype: "checkbox", editoptions: { value: "Y:''" } },
        { name: 'XBC', index: 'XBC', align: 'center', width: 50, editable: false, edittype: "checkbox", editoptions: { value: "Y:''" }, },
        { key: false, hidden: true, name: 'MSG', index: 'MSG', width: 80, formatter: rowColorFormatter, editable: false, sortable: false, search: false },
        { key: false, name: 'Notes', index: 'Notes', width: 150, editable: true, sortable: false, search: false, edittype: 'textarea', editoptions: { rows: '2', cols: '300' } }
    ],
    pager: '#clientsPager',
    rowNum: 15,

    onSelectRow: function (nowServing) {
        if (nowServing == null || nowServing == lastServed) {
            // Prevent infinite recursion caused by reloadGrid
            //alert("nowServing is null or nowServing == lastServed!");
           // jQuery("#conversation").addClass("hideConversation");
        } else {
            lastServed = nowServing;

            var clientsGrid = jQuery("#clientsGrid"),
                selRowId = clientsGrid.jqGrid('getGridParam', 'selrow'),
                hasConversation = clientsGrid.jqGrid('getCell', selRowId, 'Conversation');

            if (hasConversation == "Y") {
                jQuery("#conversation").removeClass("hideConversation");
            } else {
                jQuery("#conversation").addClass("hideConversation");
            }

            jQuery("#clientsGrid").jqGrid('setGridParam',
                {
                    postData: { nowServing: nowServing },
                    url: "NowConversing", // "@Url.Action("NowServing", "CaseManager")"
                    }).trigger('reloadGrid', { fromServer: true }).jqGrid('setSelection', nowServing, true);
        }
    },

    height: "100%",
    viewrecords: true,
    loadonce: false,

    gridComplete: function () {
        for (var i = 0; i < rowsToColor.length; i++) {
            //  alert("colored row: " + rowsToColor[i].rowId + " " + rowsToColor[i].rowColor);
            $("#" + rowsToColor[i].rowId).css("color", rowsToColor[i].rowColor)
        }
    },

    caption: 'Clients',
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
            url: "GetDependents",  // "@Url.Action("GetDependents", "CaseManager")",
            datatype: "json",
            mtype: 'post',
            editurl: "Dummy", // "@Url.Action("Dummy", "CaseManager")",
            cellsubmit: 'clientArray',
            colNames: ['Id', 'First Name', 'Middle Name', 'Last Name', 'Birth Name', 'DOB', 'Age', 'ACK', 'XID', 'XBC', 'Notes'],
            colModel: [
                { key: true, hidden: true, name: 'Id', index: 'Id', editable: true },
                { key: false, name: 'FirstName', index: 'FirstName', width: 100, editable: true },
                { key: false, name: 'MiddleName', index: 'MiddleName', width: 100, editable: true },
                { key: false, name: 'LastName', index: 'LastName', width: 100, editable: true },
                { key: false, name: 'BirthName', index: 'BirthName', width: 100, editable: true, sortable: false, search: false },
                { key: false, align: 'center', name: 'DOB', index: 'DOB', formatter: 'date', width: 120, editable: true },
                { key: false, align: 'center', name: 'Age', index: 'Age', width: 50, editable: false, sortable: false, search: false },
                { name: 'PND', index: 'PND', align: 'center', width: 35, editable: false, edittype: "checkbox", editoptions: { value: "Y:''" }, },
                { name: 'XID', index: 'XID', align: 'center', width: 35, editable: false, edittype: "checkbox", editoptions: { value: "Y:''" } },
                { name: 'XBC', index: 'XBC', align: 'center', width: 35, editable: false, edittype: "checkbox", editoptions: { value: "Y:''" }, },
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
                    jQuery("#clientsGrid").jqGrid('setGridParam',
                        {
                            postData: { nowServing: nowServing },
                            url: "NowServing",  // "@Url.Action("NowServing", "CaseManager")"
                             }).trigger('reloadGrid', { fromServer: true }).jqGrid('setSelection', nowServing, true);
                }
            },
            height: '100%',
            // https://stackoverflow.com/questions/3213984/jqgrid-giving-exception-when-json-is-attempted-to-be-loaded
            jsonReader: { repeatitems: false }
        });

        // Insert jquery("#" + subgrid_table_id).jqgrid('navGrid', ...  )  HERE
        jQuery("#" + subgrid_table_id).jqGrid('navGrid', "#" + pager_id, { edit: true, add: true, del: true, search: false, refresh: false },
            {
                zIndex: 100,
                url: "EditDependentClient", // "@Url.Action("EditDependentClient", "CaseManager")",
                closeOnEscape: true,
                closeAfterEdit: true,
                recreateForm: true,
                afterComplete: function (response) {
                    if (response.responseText) {
                        //   alert("Edited dependent client: " + response.responseText);
                    }
                }
            },
            {
                zIndex: 100,
                url: "AddDependentClient?household="+row_id, // "@Url.Action("AddDependentClient", "CaseManager")?household=" + row_id,
                closeOnEscape: true,
                closeAfterAdd: true,
                recreateForm: true,
                afterComplete: function (response) {
                    if (response.responseText) {
                        //   alert("Added dependent client: " + response.responseText);
                    }
                }
            },
            {
                zIndex: 100,
                url: "DeleteDependentClient",  // "@Url.Action("DeleteDependentClient", "CaseManager")",
                closeOnEscape: true,
                closeAfterDelete: true,
                recreateForm: true,
                afterComplete: function (response) {
                    if (response.responseText) {
                        //   alert("Deleted dependent client: " + response.responseText);
                    }
                }
            })
    } // close subgridRowExpanded 
})

jQuery("#clientsGrid").jqGrid('navGrid', '#clientsPager', { edit: true, add: true, del: true, search: false, refresh: false },
    {
        zIndex: 100,
        url: "EditMyClient", // "@Url.Action("EditClient", "CaseManager")",
        closeOnEscape: true,
        closeAfterEdit: true,
        recreateForm: true,

        afterComplete: function (response) {
            if (response.responseText == "Success") {
                var theHub = $.connection.dailyHub;
                theHub.client.refreshConversation("Open");
            }
        }
    },
    {
        zIndex: 100,
        url: "AddMyClient", // "@Url.Action("AddMyClient", "CaseManager")",
        closeOnEscape: true,
        closeAfterAdd: true,
        recreateForm: true,
        afterComplete: function (response) {
            if (response.responseText) {
                if (response.responseText != "Success") {
                    alert(response.responseText);
                }
            }
        }
    },
    {
        zIndex: 100,
        url: "DeleteMyClient", // "@Url.Action("DeleteMyClient", "CaseManager")",
        closeOnEscape: true,
        closeAfterDelete: true,
        recreateForm: true,
        afterComplete: function (response) {
            if (response.responseText == "Failure") {
                alert("Not permitted to delete a training client!");
            }
        }
    });

// See: https://stackoverflow.com/questions/3908171/jqgrid-change-row-background-color-based-on-condition
function rowColorFormatter(cellValue, options, rowObject) {
    if (cellValue != null && cellValue == "FromAgency") {
        rowsToColor[rowsToColor.length] = { rowId: rowObject.Id, rowColor: "#FF0000" };  // red
    } else if (cellValue != null && (cellValue == "FromOPID" || cellValue == "FromFrontDesk" || cellValue == "FromInterviewer")) {
        // alert("cellValue == FromOPID");
        rowsToColor[rowsToColor.length] = { rowId: rowObject.Id, rowColor: "#00FF00" };  // green
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
    addCaption: "Add Client",
    editCaption: "Edit Client",
    bSubmit: "Submit",
    bCancel: "Cancel",
    bClose: "Close",
    saveData: "Data has been changed! Save changes?",
    bYes: "Yes",
    bNo: "No",
    bExit: "Cancel"
});



