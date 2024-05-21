/*
 * File: search_ticket.js
 * Author: Warre Willeme & Jesse UijtdeHaag
 * Date: May 12, 2024
 * Description: This file contains the JavaScript code for the search ticket functionality on the ticket beheer page.
 */

//https://www.w3schools.com/howto/howto_js_filter_dropdown.asp

function searchTicket() {
    var input, filter, a, i;
    input = document.getElementById("tbTicketFilter");
    filter = input.value.toUpperCase();
    var tickets = document.getElementById("divTicketList");
    a = tickets.getElementsByTagName("a");

    var filterAttributes = {
        "barcode": "data-barcode",
        "badgerights": "data-badgerights",
        "rfid": "data-RFID"
    };

    for (i = 0; i < a.length; i++) {

	    var ddTicketSearch = document.getElementById("ddTicketSearch");
        var selectedFilter = ddTicketSearch.value;

        var attribute = a[i].getAttribute(filterAttributes[selectedFilter]);
        if (attribute && attribute.toUpperCase().indexOf(filter) > -1) {
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

function changeSearchType() {
    clearInput();
}