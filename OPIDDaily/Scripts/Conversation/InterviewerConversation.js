

$("#conversationGrid").jqGrid({
    url: "GetConversation", 
    datatype: "json",
    mtype: "Get",
    colNames: ['Id', 'Date', 'From', 'To', 'Msg'],
    colModel: [
        { key: true, hidden: true, name: 'Id', index: 'Id' },
        { key: false, align: 'center', name: 'Date', index: 'Date', formatter: 'date', width: 80, editable: false, sortable: true, search: false },
        { key: false, align: 'center', name: 'From', index: 'From', width: 80, editable: true, sortable: false, search: false },
        { key: false, align: 'center', name: 'To', index: 'To', width: 80, editable: true, sortable: false, search: false },
        { key: false, hidden: false, name: 'Msg', index: 'Msg', width: 850, editable: true, sortable: false, search: false, edittype: 'textarea', editoptions: { rows: '2', columns: '10' } }
    ],
    pager: '#conversationPager',
    rowNum: 10,

    height: "100%",
    viewrecords: true,
    loadonce: false,

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


jQuery("#conversationGrid").jqGrid('navGrid', '#conversationPager', { edit: true, add: true, del: false, search: false, refresh: false },
    {
        zIndex: 100,
        url: "EditTextMsg",  
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
        url: "AddTextMsg?sender=Interviewer", 
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
    });
