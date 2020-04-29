var lastServed = 0;
 
$("#clientsGrid").jqGrid({
    url: "GetDemoDashboard", // "@Url.Action("GetDemoDashboard", "Superadmin")",
    datatype: "json",
    pageable: true,
    mtype: "Get",
    colNames: ['Id', 'Agency', 'Added', 'Expires', 'H', 'Last Name', 'First Name', 'Middle Name', 'Birth Name', 'DOB', 'Age'],
    colModel: [
        { key: true, hidden: true, name: 'Id', index: 'Id' },
        { key: false, name: 'AgencyName', index: 'AgencyName', width: 150, editable: false, sortable: false, search: false },
        { key: false, align: 'center', name: 'ServiceDate', index: 'ServiceDate', formatter: 'date', width: 120, editable: true, sortable: true, search: false },
        { key: false, align: 'center', name: 'Expiry', index: 'Expiry', formatter: 'date', width: 120, editable: false, sortable: true, search: false },
        { key: false, name: 'HeadOfHousehold', index: 'HeadOfHousehold', width: 35, align: 'center', editable: false, sortable: false, search: false },
        { key: false, name: 'LastName', index: 'LastName', width: 150, editable: true, sortable: false, search: false },
        { key: false, name: 'FirstName', index: 'FirstName', width: 150, editable: true, sortable: false, search: false },
        { key: false, name: 'MiddleName', index: 'MiddleName', width: 150, editable: true, sortable: false, search: false },
        { key: false, name: 'BirthName', index: 'BirthName', width: 150, editable: true, sortable: false, search: false },
        { key: false, align: 'center', name: 'DOB', index: 'DOB', formatter: 'date', width: 120, editable: true, sortable: true, search: false },
        { key: false, align: 'center', name: 'Age', index: 'Age', width: 50, editable: false, sortable: true, search: false }
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
    
    caption: 'Dashboard',
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
            url: "GetDependents", // "@Url.Action("GetDependents", "Superadmin")",
            datatype: "json",
            mtype: 'post',
            editurl: "Dummy", // "@Url.Action("Dummy", "Superadmin")",
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
                { name: 'PND', index: 'PND', align: 'center', width: 35, editable: true, edittype: "checkbox", editoptions: { value: "Y:''" }, },
                { name: 'XID', index: 'XID', align: 'center', width: 35, editable: true, edittype: "checkbox", editoptions: { value: "Y:''" } },
                { name: 'XBC', index: 'XBC', align: 'center', width: 35, editable: true, edittype: "checkbox", editoptions: { value: "Y:''" }, },
                { key: false, name: 'Notes', index: 'Notes', width: 150, sortable: false, editable: true, edittype: 'textarea', editoptions: { rows: '2', columns: '10' } }
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
                            url: "NowServing" // "@Url.Action("NowServing", "Superadmin")"
                             }).trigger('reloadGrid', { fromServer: true }).jqGrid('setSelection', nowServing, true);
                }
            },
            height: '100%',
            // https://stackoverflow.com/questions/3213984/jqgrid-giving-exception-when-json-is-attempted-to-be-loaded
            jsonReader: { repeatitems: false }
        });

        // Insert jquery("#" + subgrid_table_id).jqgrid('navGrid', ...  )  HERE
        jQuery("#" + subgrid_table_id).jqGrid('navGrid', "#" + pager_id, { edit: false, add: false, del: false, search: false, refresh: false })
            
    } // close subgridRowExpanded
})

jQuery("#clientsGrid").jqGrid('navGrid', '#clientsPager', { edit: true, add: false, del: false, search: false, refresh: false },
    {
        zIndex: 100,
        url: "EditClientServiceDate", // "@Url.Action("EditClientServiceDate", "Superadmin")",
        closeOnEscape: true,
        closeAfterEdit: true,
        recreateForm: true,

        afterComplete: function (response) {
            if (response.responseText) {
                //  alert("FrontDesk: " + response.responseText);
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