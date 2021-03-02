$("#historyGrid").jqGrid({
    url: "GetVisitHistory", // "@Url.Action("GetVisitHistory", "Superadmin")",
    datatype: "json",
    mtype: "Get",
    colNames: ['Id', 'Date', 'Item', 'Check', 'Status', 'Notes'],
    colModel: [
        { key: true, hidden: true, name: 'Id', index: 'Id' },
        { key: false, align: 'center', name: 'Date', index: 'Date', formatter: 'date', width: 80, editable: true, sortable: true, search: false },
      //  { key: false, name: 'Conversation', index: 'Conversation', width: 35, align: 'center', editable: false, edittype: "checkbox", editoptions: { value: "Y:''" }, sortable: false, search: false }, 
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
    multiselect: false
})

jQuery("#historyGrid").jqGrid('navGrid', '#historyPager', { edit: false, add: false, del: false, search: false, refresh: false });
