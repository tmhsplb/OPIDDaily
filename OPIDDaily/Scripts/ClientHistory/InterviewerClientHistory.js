$("#historyGrid").jqGrid({
    url:"GetVisitHistory", // "@Url.Action("GetHistory", "Interviewer")",
    datatype: "json",
    mtype: "Get",
    colNames: ['Id', 'Date', 'Item', 'Check', 'Status', 'Notes'],
    colModel: [
        { key: false, hidden: true, name: 'Id', index: 'Id' },  // Id may not be unique
        { key: false, align: 'center', name: 'Date', index: 'Date', formatter: 'date', width: 80, editable: false, sortable: true, search: false },
        { key: false, name: 'Item', index: 'Item', width: 80, editable: true, sortable: false, search: false },
        { key: false, name: 'Check', index: 'Check', width: 80, editable: true, sortable: false, search: false },
        { key: false, name: 'Status', index: 'Status', width: 100, editable: true, edittype: 'select', editoptions: { value: { '': '', 'Cleared': 'Cleared', 'Voided': 'Voided', 'Voided/No Reissue': 'Voided/No Reissue', 'Voided/Resissued': 'Voided/Reissued', 'Voided/Replaced': 'Voided/Replaced' } }, sortable: false, search: false },
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
    multiselect: false
})

jQuery("#historyGrid").jqGrid('navGrid', '#historyPager', { edit: true, add: true, del: true, search: false, refresh: false },
    {
        zIndex: 100,
        url: "EditVisit",
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
        url: "AddPocketVisit", // "@Url.Action("AddVisit", "FrontDesk")",
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