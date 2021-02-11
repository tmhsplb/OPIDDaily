
$("#historyGrid").jqGrid({
    url: "GetVisitHistory", // "@Url.Action("GetVisitHistory", "FrontDesk")",
    datatype: "json",
    mtype: "Get",
    colNames: ['Id', 'Date', 'C', 'Item', 'Check', 'Status', 'Notes'],
    colModel: [
        { key: true, hidden: true, name: 'Id', index: 'Id' },
        { key: false, align: 'center', name: 'Date', index: 'Date', formatter: 'date', width: 80, editable: true, sortable: true, search: false },
        { key: false, name: 'Conversation', index: 'Conversation', width: 35, align: 'center', editable: false, edittype: "checkbox", editoptions: { value: "Y:''" }, sortable: false, search: false },
        { key: false, name: 'Item', index: 'Item', width: 80, editable: true, sortable: false, search: false },
        { key: false, name: 'Check', index: 'Check', width: 80, editable: true, sortable: false, search: false },
        { key: false, name: 'Status', index: 'Status', width: 100, editable: true, edittype: 'select', editoptions: { value: { '': '', 'Cleared': 'Cleared', 'Voided': 'Voided', 'Voided/No Reissue': 'Voided/No Reissue', 'Voided/Resissued': 'Voided/Reissued', 'Voided/Replaced': 'Voided/Replaced', 'Used': 'Used', 'Not Used': 'Not Used' } }, sortable: false, search: false },
        { key: false, name: 'Notes', index: 'Notes', width: 150, editable: true, sortable: false, search: false, edittype: 'textarea', editoptions: { rows: '2', cols: '300' } }
    ],
    pager: '#historyPager',
    rowNum: 25,
    height: "100%",
    viewrecords: true,
    loadonce: false,
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
                { key: true, hidden: true, name: 'Id', index: 'Id' },
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
                url: "EditVisitNote",
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
                url: "AddVisitNote?vid=" + row_id + "&sender=OPID",
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

jQuery("#historyGrid").jqGrid('navGrid', '#historyPager', { edit: true, add: true, del: false, search: false, refresh: false },
    {
        zIndex: 100,
        url: "EditVisit", // "@Url.Action("EditVisit", "FrontDesk")",
        closeOnEscape: true,
        closeAfterEdit: true,
        recreateForm: true,
        afterComplete: function (response) {
            if (response.responseText) {
                //  alert("FrontDesk: " + response.responseText);
            }
        }
    },
    {
        zIndex: 100,
        url: "AddPocketCheck",
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
    }
    /*
    {
        zIndex: 100,
        url: "DeletePocketCheck", 
        closeOnEscape: true,
        closeAfterDelete: true,
        recreateForm: true,
        afterComplete: function (response) {
            if (response.responseText) {
                //   alert(response.responseText);
            }
        }
    }
    */
    );

// http://www.trirand.com/blog/?page_id=393/help/how-to-use-add-form-dialog-popup-window-set-position
$.extend($.jgrid.edit, {
    recreateForm: true,
    closeAfterAdd: true,
    dataheight: '100%',
    reloadAfterSubmit: true,
    width: 500,
    top: 300,
    left: 150,
    addCaption: "Add Visit",
    editCaption: "Edit Visit",
    bSubmit: "Submit",
    bCancel: "Cancel",
    bClose: "Close",
    saveData: "Data has been changed! Save changes?",
    bYes: "Yes",
    bNo: "No",
    bExit: "Cancel"
});