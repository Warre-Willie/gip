(function () {
    const url = new URL(window.location.href);
    const params = new URLSearchParams(url.search);

    function activateBackPrevention() {
        console.log("Back navigation prevention activated.");
        window.history.forward();
    }

    if (url.pathname == '/pages/login.aspx' && !params.has["pb"] || url.pathname == '/pages/index.aspx') {
        console.log("login page");
        activateBackPrevention()
    }

    //// Add event listener for the logout button
    //const logoutButton = document.getElementById('btnLogout');  // Replace with the actual ID of the logout button
    //if (logoutButton) {
    //    logoutButton.addEventListener('click', function () {
    //        sessionStorage.setItem('preventBack', 'true');
    //        console.log("Back navigation prevention activated on logout.");  // Log for debugging purposes
    //        // Optionally redirect to a logout page or perform other logout operations
    //    });
    //}
})();
