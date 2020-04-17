
$("#historyGrid").jqGrid({
    url: "GetVisitHistory", // "@Url.Action("GetVisitHistory", "FrontDesk")",
    datatype: "json",
    mtype: "Get",
    colNames: ['Id', 'Date', 'Item', 'Check', 'Status', 'Notes'],
    colModel: [
        { key: true, hidden: true, name: 'Id', index: 'Id' },
        { key: false, align: 'center', name: 'Date', index: 'Date', formatter: 'date', width: 80, editable: true, sortable: true, search: false },
        { key: false, name: 'Item', index: 'Item', width: 80, editable: true, sortable: false, search: false },
        { key: false, name: 'Check', index: 'Check', width: 80, editable: true, sortable: false, search: false },
        { key: false, name: 'Status', index: 'Status', width: 100, editable: true, edittype: 'select', editoptions: { value: { '': '', 'Cleared': 'Cleared', 'Voided': 'Voided', 'Voided/No Reissue': 'Voided/No Reissue', 'Voided/Resissued': 'Voided/Reissued', 'Voided/Replaced': 'Voided/Replaced', 'Used': 'Used', 'Not Used': 'Not Ussed' } }, sortable: false, search: false },
        { key: false, name: 'Notes', index: 'Notes', width: 150, editable: true, sortable: false, search: false, edittype: 'textarea', editoptions: { rows: '2', columns: '10' } }
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
    multiselect: false
})

jQuery("#historyGrid").jqGrid('navGrid', '#historyPager', { edit: true, add: true, del: true, search: false, refresh: false },
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
        url: "AddVisit", // "@Url.Action("AddVisit", "FrontDesk")",
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
        url: "DeleteVisit", // "@Url.Action("DeleteVisit", "FrontDesk")",
        closeOnEscape: true,
        closeAfterDelete: true,
        recreateForm: true,
        afterComplete: function (response) {
            if (response.responseText) {
                //   alert(response.responseText);
            }
        }
    });

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