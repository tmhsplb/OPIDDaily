var lastServed = 0;
 
$("#clientsGrid").jqGrid({
    url: "GetServiceDateClients", // "@Url.Action("GetServiceDateClients", "Superadmin")",
    datatype: "json",
    pageable: true,
    mtype: "Get",
    colNames: ['Id', 'CM', 'Expiry', 'Stage', 'Last Name', 'First Name', 'Middle Name', 'Birth Name', 'DOB', 'Age', 'ACK', 'XID', 'XBC', 'Notes'],
    colModel: [
    { key: true, hidden: true, name: 'Id', index: 'Id' },
    { key: false, align: 'center', name: 'ServiceTicket', index: 'ServiceTicket', width: 50, editable: true, sortable: true, search: false },
    { key: false, align: 'center', name: 'Expiry', index: 'Expiry', formatter: 'date', width: 120, editable: false, sortable: true, search: false },
    { key: false, name: 'Stage', index: 'Stage', width: 100, editable: true, edittype: 'select', editoptions: { value: { 'Screened': 'Screened', 'CheckedIn': 'CheckedIn', 'Interviewing': 'Interviewing', 'Interviewed': 'Interviewed', 'BackOffice': 'BackOffice', 'Done': 'Done' } }, sortable: false, search: false },
    { key: false, name: 'LastName', index: 'LastName', width: 150, editable: true, sortable: false, search: false },
    { key: false, name: 'FirstName', index: 'FirstName', width: 150, editable: true, sortable: false, search: false },
    { key: false, name: 'MiddleName', index: 'MiddleName', width: 150, editable: true, sortable: false, search: false },
    { key: false, name: 'BirthName', index: 'BirthName', width: 150, editable: true, sortable: false, search: false },
    { key: false, align: 'center', name: 'DOB', index: 'DOB', formatter: 'date', width: 100, editable: true, sortable: true, search: false },
    { key: false, align: 'center', name: 'Age', index: 'Age', width: 50, editable: false, sortable: false, search: false },
     
    { name: 'PND', index: 'PND', align: 'center', width: 50, editable: true, edittype: "checkbox", editoptions: { value: "Y:''" }, },
    { name: 'XID', index: 'XID', align: 'center', width: 50, editable: true, edittype: "checkbox", editoptions: { value: "Y:''" } },
    { name: 'XBC', index: 'XBC', align: 'center', width: 50, editable: true, edittype: "checkbox", editoptions: { value: "Y:''" }, },
    { key: false, name: 'Notes', index: 'Notes', width: 150, editable: true, sortable: false, search: false, edittype: 'textarea', editoptions: { rows: '2', cols: '300' } }
        ],
    pager: '#clientsPager',
    rowNum: 25,

        onSelectRow: function (nowServing) {
            if (nowServing == null || nowServing == lastServed) {
                // Prevent infinite recursion caused by reloadGrid
                //alert("nowServing is null or nowServing == lastServed!");
            } else {
                lastServed = nowServing;
                jQuery("#clientsGrid").jqGrid('setGridParam',
                    {
                        postData: { nowServing: nowServing },
                        url: "NowServing" // "@Url.Action("NowServing", "Superadmin")"
                    }).trigger('reloadGrid', { fromServer: true }).jqGrid('setSelection', nowServing, true);
            }
        },

    height: "100%",
    viewrecords: true,
    loadonce: false,

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
    multiselect: false
    })

jQuery("#clientsGrid").jqGrid('navGrid', '#clientsPager', { edit: false, add: false, del: false, search: false, refresh: false });

 
