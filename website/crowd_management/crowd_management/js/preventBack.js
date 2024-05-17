(function () {
    function preventBack() {
        window.history.forward();
    }

    const url = new URL(window.location.href);
    const params = new URLSearchParams(url.search);

    if (params.has('pb')) {
        console.log("Back navigation prevention activated.");  // Log for debugging purposes
        // Prevent back navigation
        setTimeout(preventBack, 0);
        window.onunload = function () { return null; };

        // Use sessionStorage to remember that back navigation should be prevented
        sessionStorage.setItem('preventBack', 'true');
    } else {
        // Remove the flag if navigating to a different page
        sessionStorage.removeItem('preventBack');
    }

    // If the flag is set in sessionStorage, prevent back navigation
    if (sessionStorage.getItem('preventBack') === 'true') {
        setTimeout(preventBack, 0);
        window.onunload = function () { return null; };
    }
})();
