$(document).ready(function () {
    $('img[usemap]').rwdImageMaps();
});

window.onload = function () {
    checkCoordsChange();
};

function checkCoordsChange() {
    var heatMap = document.getElementById("heatMap");
    var areas = heatMap.getElementsByTagName('area');
    for (var i = 0; i < areas.length; i++) {
        var area = areas[i];
        handleCoordsChange(area.id, area.coords);
    }
}

function handleCoordsChange(id, coords) {
    // Parse the coordinates string into an array
    var coordArray = coords.split(',');

    // Get the highlight div
    var heatMapZone = document.getElementById("div" + id);

    // Set the size and position of the highlight div based on the coordinates
    heatMapZone.style.width = coordArray[2] - coordArray[0] + 'px';
    heatMapZone.style.height = coordArray[3] - coordArray[1] + 'px';
    heatMapZone.style.left = coordArray[0] + 'px';
    heatMapZone.style.top = coordArray[1] + 'px';

    if (heatMapZone.hasAttribute("data-first_load")) {
        heatMapZone.removeAttribute("data-first_load")
    } else {
        heatMapZone.classList.remove("hide")
    }
}