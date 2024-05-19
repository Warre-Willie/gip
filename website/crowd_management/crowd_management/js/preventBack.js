(function () {
    function preventBack() {
        window.history.forward();
    }

    const url = new URL(window.location.href);
    const params = new URLSearchParams(url.search);

    if (params.has('pb')) {
        console.log("Back navigation prevention activated.");  // Log for debugging purposes
        setTimeout(preventBack, 0);
        window.onunload = function () { return null; };

        sessionStorage.setItem('preventBack', 'true');
    } else {
        sessionStorage.removeItem('preventBack');
    }

    if (sessionStorage.getItem('preventBack') === 'true') {
        setTimeout(preventBack, 0);
        window.onunload = function () { return null; };
    }
})();
