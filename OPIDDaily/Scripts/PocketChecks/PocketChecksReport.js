 
 
$("#pocketChecksReportGrid").jqGrid({
    url: "GetPocketChecks",
    datatype: "json",
    pageable: true,
    mtype: "Get",
    colNames: ['Id', 'Agency', 'Name', 'HH', 'Date', 'Item', 'Check', 'Status', 'Notes'],
    colModel: [
        { key: true, hidden: true, name: 'Id', index: 'Id' },  // Id may not be unique!
        { key: false, name: 'AgencyName', index: 'AgencyName', width: 80, editable: false, sortable: false, search: false },
        { key: false, name: 'Name', index: 'Name', width: 80, editable: true, sortable: false, search: false },
        { key: false, name: 'HeadOfHousehold', index: 'HeadOfHousehold', width: 25, align: 'center', editable: false, sortable: false, search: false },
        { key: false, align: 'center', name: 'Date', index: 'Date', formatter: 'date', width: 40, editable: true, sortable: true, search: false },
        { key: false, align: 'center', name: 'Item', index: 'Item', width: 30, editable: true, sortable: false, search: false },
        { key: false, align: 'center', name: 'Check', index: 'Check', width: 30, editable: true, sortable: false, search: false },
        { key: false, name: 'Status', index: 'Status', width: 60, editable: true, edittype: 'select', editoptions: { value: { '': '', 'Cleared': 'Cleared', 'Voided': 'Voided', 'Voided/No Reissue': 'Voided/No Reissue', 'Voided/Resissued': 'Voided/Reissued', 'Voided/Replaced': 'Voided/Replaced', 'Scammed Check': 'Scammed Check', 'Used': 'Used', 'Not Used': 'Not Used' } }, sortable: false, search: false },
        { key: false, hidden: false, name: 'Notes', index: 'Notes', width: 150, editable: true, sortable: false, search: false, edittype: 'textarea', editoptions: { rows: '2', columns: '10' } }
    ],
    pager: '#pocketChecksReportPager',
    rowNum: 15,

    height: "100%",
    viewrecords: true,
    loadonce: false,
 
    caption: 'Pocket Checks Report',
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
            url: "GetDependentPocketChecks",
            datatype: "json",
            mtype: 'post',
            editurl: "Dummy",  // "@Url.Action("Dummy", "FrontDesk")", 
            cellsubmit: 'clientArray',
            colNames: ['Id', 'Name', 'Date', 'Item', 'Check', 'Status', 'Notes'],
            colModel: [
                { key: true, hidden: true, name: 'Id', index: 'Id', editable: true },
                { key: false, name: 'Name', index: 'Name', width: 120, editable: true },
                { key: false, align: 'center', name: 'Date', index: 'Date', formatter: 'date', width: 80, editable: true, sortable: true, search: false },
                { key: false, align: 'center', name: 'Item', index: 'Item', width: 50, editable: true, sortable: false, search: false },
                { key: false, align: 'center', name: 'Check', index: 'Check', width: 50, editable: true, sortable: false, search: false },
                { key: false, name: 'Status', index: 'Status', width: 120, editable: true, edittype: 'select', editoptions: { value: { '': '', 'Cleared': 'Cleared', 'Voided': 'Voided', 'Voided/No Reissue': 'Voided/No Reissue', 'Voided/Resissued': 'Voided/Reissued', 'Voided/Replaced': 'Voided/Replaced', 'Scammed Check': 'Scammed Check', 'Used': 'Used', 'Not Used': 'Not Used' } }, sortable: false, search: false },
                { key: false, name: 'Notes', index: 'Notes', width: 450, sortable: false, editable: true, edittype: 'textarea', editoptions: { rows: '2', cols: '300' } }
            ],
            rowNum: 10,
            pager: pager_id,

            height: '100%',
            // https://stackoverflow.com/questions/3213984/jqgrid-giving-exception-when-json-is-attempted-to-be-loaded
            jsonReader: { repeatitems: false }
        });

        jQuery("#" + subgrid_table_id).jqGrid('navGrid', "#" + pager_id, { edit: true, add: false, del: false, search: false, refresh: false },
            {
                zIndex: 100,
                url: "EditPCRPocketCheck", //"@Url.Action("EditDependentClient", "FrontDesk")",
                closeOnEscape: true,
                closeAfterEdit: true,
                recreateForm: true,
                afterComplete: function (response) {
                    if (response.responseText) {
                        //   alert("Edited dependent client: " + response.responseText);
                    }
                }
            })
    } // close subgridRowExpanded
})


jQuery("#pocketChecksReportGrid").jqGrid('navGrid', '#pocketChecksReportPager', { edit: true, add: false, del: false, search: false, refresh: false },
    {
        zIndex: 100,
        url: "EditPCRPocketCheck", 
        closeOnEscape: true,
        closeAfterEdit: true,
        recreateForm: true,
        afterComplete: function (response) {
            if (response.responseText) {
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
    addCaption: "",
    editCaption: "Edit NOTES",
    bSubmit: "Submit",
    bCancel: "Cancel",
    bClose: "Close",
    saveData: "Data has been changed! Save changes?",
    bYes: "Yes",
    bNo: "No",
    bExit: "Cancel"
});