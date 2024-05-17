$(document).ready(function () {
    function updateHeatMap() {
        $.ajax({
            type: "POST",
            url: "index.aspx/GetHeatMapData",
            data: '{}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var data = JSON.parse(response.d);

                // Update Zone 1
                $("#tagZoneName1").text(data.Zone1.Name);
                $("#tagZonePercentage1").text(data.Zone1.Percentage + "%");
                if (data.Zone1.Lockdown) {
                    $("#zoneLockdown1").show();
                } else {
                    $("#zoneLockdown1").hide();
                }

                // Update Zone 2
                $("#tagZoneName2").text(data.Zone2.Name);
                $("#tagZonePercentage2").text(data.Zone2.Percentage + "%");
                if (data.Zone2.Lockdown) {
                    $("#zoneLockdown2").show();
                } else {
                    $("#zoneLockdown2").hide();
                }
            },
            error: function (error) {
                console.log("Error: ", error);
            }
        });
    }

    // Initial call to update the heatmap
    updateHeatMap();

    // Set an interval to update the heatmap periodically
    setInterval(updateHeatMap, 5000); // Update every 5 seconds
});
