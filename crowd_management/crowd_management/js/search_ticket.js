//https://www.w3schools.com/howto/howto_js_filter_dropdown.asp

function searchTicket() {
    var input, filter, a, i;
    input = document.getElementById("tbTicketFilter");
    filter = input.value.toUpperCase();
    tickets = document.getElementById("divTicketList");
    a = tickets.getElementsByTagName("a");
    for (i = 0; i < a.length; i++) {
        txtValue = a[i].textContent || a[i].innerText;
        if (txtValue.toUpperCase().indexOf(filter) > -1) {
            a[i].style.display = "";
        } else {
            a[i].style.display = "none";
        }
    }
}

function clearInput() {
    document.getElementById("tbTicketFilter").value = "";
    document.getElementById("divTicketList").focus();
    searchTicket();
}
