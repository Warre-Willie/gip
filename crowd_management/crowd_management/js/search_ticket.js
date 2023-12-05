//https://www.w3schools.com/howto/howto_js_filter_dropdown.asp

function searchTicket() {
  var input, filter, a, i;
  input = document.getElementById("search-input");
  filter = input.value.toUpperCase();
  tickets = document.getElementById("ticket-list");
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
  document.getElementById("search-input").value = "";
  document.getElementById("ticket-list").focus();
  searchTicket();
}
