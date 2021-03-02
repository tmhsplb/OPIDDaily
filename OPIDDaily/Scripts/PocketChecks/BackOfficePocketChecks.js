 
$("#pocketCheckGrid").jqGrid({
    url: "GetClientPocketChecks", 
    datatype: "json",
    mtype: "Get",
    colNames: ['Id', 'Date', 'Item', 'Check', 'Status', 'Notes'],
    colModel: [
        { key: false, hidden: true, name: 'Id', index: 'Id' },  // Id may not be unique!
        { key: false, align: 'center', name: 'Date', index: 'Date', formatter: 'date', width: 80, editable: true, sortable: true, search: false },
        { key: false, name: 'Item', index: 'Item', width: 80, editable: true, sortable: false, search: false },
        { key: false, name: 'Check', index: 'Check', width: 80, editable: true, sortable: false, search: false },
        { key: false, name: 'Status', index: 'Status', width: 100, editable: true, edittype: 'select', editoptions: { value: { '': '', 'Cleared': 'Cleared', 'Voided': 'Voided', 'Voided/No Reissue': 'Voided/No Reissue', 'Voided/Resissued': 'Voided/Reissued', 'Voided/Replaced': 'Voided/Replaced', 'Scammed Check': 'Scammed Check', 'Used': 'Used', 'Not Used': 'Not Used' } }, sortable: false, search: false },
        { key: false, hidden: false, name: 'Notes', index: 'Notes', width: 150, editable: true, sortable: false, search: false, edittype: 'textarea', editoptions: { rows: '2', columns: '10' } }    
    ],
    pager: '#pocketCheckPager',
    rowNum: 25,

    height: "100%",
    viewrecords: true,
    loadonce: false,

    caption: 'Pocket Checks for ' + GetClientName(),
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
})

jQuery("#pocketCheckGrid").jqGrid('navGrid', '#pocketCheckPager', { edit: true, add: true, del: false, search: false, refresh: false },
    {
        zIndex: 100,
        url: "EditPocketCheck", 
        closeOnEscape: true,
        closeAfterEdit: true,
        recreateForm: true,
        afterComplete: function (response) {
            if (response.responseText) {
                 // alert("BackOffice: " + response.responseText);
            }
        }
    },
    {
        zIndex: 100,
        url: "AddDatedPocketCheck", 
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
        url: "DeletePocketCheck", // "@Url.Action("DeleteVisit", "FrontDesk")",
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