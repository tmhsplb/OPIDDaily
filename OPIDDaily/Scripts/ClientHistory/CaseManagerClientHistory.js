var rowsToColor = [];

$("#historyGrid").jqGrid({
    url: "GetVisitHistory", // "@Url.Action("GetVisitHistory", "CaseManager")",
    datatype: "json",
    mtype: "Get",
    colNames: ['Id', 'Date', 'Item', 'Check', 'Status', 'Sender', 'Notes'],
    colModel: [
        { key: true, hidden: true, name: 'Id', index: 'Id' },
        { key: false, align: 'center', name: 'Date', index: 'Date', formatter: 'date', width: 80, editable: false, sortable: true, search: false },
     //   { key: false, name: 'Conversation', index: 'Conversation', width: 35, align: 'center', editable: false, edittype: "checkbox", editoptions: { value: "Y:''" }, sortable: false, search: false },
        { key: false, name: 'Item', index: 'Item', width: 80, editable: true, sortable: false, search: false },
        { key: false, name: 'Check', index: 'Check', width: 80, editable: true, sortable: false, search: false },
        { key: false, name: 'Status', index: 'Status', width: 100, editable: true, edittype: 'select', editoptions: { value: { '': '', 'Cleared': 'Cleared', 'Voided': 'Voided', 'Voided/No Reissue': 'Voided/No Reissue', 'Voided/Resissued': 'Voided/Reissued', 'Voided/Replaced': 'Voided/Replaced', 'Used': 'Used', 'Not Used': 'Not Ussed' } }, sortable: false, search: false },
        { key: false, hidden: true, name: 'Sender', index: 'Sender', formatter: rowColorFormatter, editable: false, sortable: false, search: false },
        { key: false, hidden: false, name: 'Notes', index: 'Notes', width: 150, editable: true, sortable: false, search: false, edittype: 'textarea', editoptions: { rows: '2', cols: '300' } }
    ],
    pager: '#historyPager',
    rowNum: 25,
    height: "100%",
    viewrecords: true,
    loadonce: false,

    gridComplete: function () {
        for (var i = 0; i < rowsToColor.length; i++) {
          //  alert("colored row: " + rowsToColor[i].rowId + " " + rowsToColor[i].rowColor);
            $("#" + rowsToColor[i].rowId).css("color", rowsToColor[i].rowColor)
        }
    },

    caption: 'Previous visits by ' + GetClientName(),
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
        // alert("row_id = " +  row_id);
        var subgrid_table_id, pager_id;
        subgrid_table_id = subgrid_id + "t";
        pager_id = "p_" + subgrid_table_id;
        $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "' class='scroll'></table><div id='" + pager_id + "' class='scroll'></div>");
        jQuery("#" + subgrid_table_id).jqGrid({
            postData: { id: function () { /* alert("id of expanded row = " + row_id); */ return row_id } },
            url: "GetVisitNotes", // "@Url.Action("GetVisitNotes", "BackOffice")", 
            datatype: "json",
            mtype: 'post',
            editurl: "Dummy",  // "@Url.Action("Dummy", "FrontDesk")", 
            cellsubmit: 'clientArray',
            colNames: ['Id', 'Date', 'From', 'Note'],
            colModel: [
                { key: true, hidden: true, name: 'Id', index: 'Id', editable: true },
                { key: false, align: 'center', name: 'Date', index: 'Date', formatter: 'date', width: 80, editable: false, sortable: true, search: false },
                { key: false, name: 'From', index: 'From', width: 80, editable: true, sortable: false, search: false },
                { key: false, name: 'Note', index: 'Note', width: 750, sortable: false, editable: true, edittype: 'textarea', editoptions: { rows: '2', cols: '300' } }
            ],
            rowNum: 10,
            pager: pager_id,
            height: '100%',
            // https://stackoverflow.com/questions/3213984/jqgrid-giving-exception-when-json-is-attempted-to-be-loaded
            jsonReader: { repeatitems: false }
        });

        // Insert jquery("#" + subgrid_table_id).jqgrid('navGrid', ...  )  HERE
        jQuery("#" + subgrid_table_id).jqGrid('navGrid', "#" + pager_id, { edit: true, add: true, del: true, search: false, refresh: false },
            {
                zIndex: 100,
                url: "EditVisitNote", //"@Url.Action("EditDependentClient", "FrontDesk")",
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
                url: "AddVisitNote?vid=" + row_id + "&sender=Agency",
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
                url: "DeleteVisitNote", 
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

jQuery("#historyGrid").jqGrid('navGrid', '#historyPager', { edit: true, add: false, del: false, search: false, refresh: false },
{
        zIndex: 100,
        url: "EditVisit",  
        closeOnEscape: true,
        closeAfterEdit: true,
        recreateForm: true,
        afterComplete: function (response) {
            if (response.responseText) {
                //   alert("Edited dependent client: " + response.responseText);
            }
        }
 })
      

// See: https://stackoverflow.com/questions/3908171/jqgrid-change-row-background-color-based-on-condition
function rowColorFormatter(cellValue, options, rowObject) {
    if (cellValue != null && cellValue == "FromAgency") {
        rowsToColor[rowsToColor.length] = { rowId: rowObject.Id, rowColor: "#FF0000" };  // red
    } else if (cellValue != null && cellValue == "FromOPID") {
        rowsToColor[rowsToColor.length] = { rowId: rowObject.Id, rowColor: "#00FF00" };  // green
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