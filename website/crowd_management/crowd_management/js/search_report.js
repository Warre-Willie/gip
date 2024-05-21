/*
 * File: search_report.js
 * Author: Warre Willeme & Jesse UijtdeHaag
 * Date: May 12, 2024
 * Description: This file contains the JavaScript code for the search report functionality on the crowd management page.
 */

//https://www.w3schools.com/howto/howto_js_filter_dropdown.asp

function searchReport() {
	var input, filter, a, i;
	input = document.getElementById("tbReportFilter");
	filter = input.value.toUpperCase();
	var tickets = document.getElementById("divPdfList");
	a = tickets.getElementsByTagName("div");

	for (i = 0; i < a.length; i++) {
		txtValue = a[i].textContent || a[i].innerText;
		if (txtValue.toUpperCase().indexOf(filter) > -1) {
			a[i].style.display = "";
		} else {
			a[i].style.display = "none";
		}
	}
}

function clearInputReport() {
	document.getElementById("tbReportFilter").value = "";
	document.getElementById("divPdfList").focus();
	searchTicket();
}