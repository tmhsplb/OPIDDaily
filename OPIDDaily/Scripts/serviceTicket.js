
    // https://stackoverflow.com/questions/19028419/how-to-adjust-print-options-through-code
    // See also:
    // https://stackoverflow.com/questions/21379605/printing-div-content-with-css-applied
    function printServiceTicket() {
        var win = window.open('', '', 'left=0,top=0,width=552,height=477,toolbar=0,scrollbars=0,status=0');

        var comments = document.getElementById("comments");
        if (comments != null && comments.value.trim() != '') {
            $('#comments').replaceWith(comments.value);
        }

        var content = "<html>";
        content += "<head><title></title></head>";
        content += "<body onload=\"window.print(); window.close();\">";
        content += document.getElementById("printerDiv").innerHTML;
        content += "</body>";
        content += "</html>";
        win.document.write(content);
        win.document.close();
    }