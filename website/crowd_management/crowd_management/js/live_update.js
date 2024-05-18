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

                // List of possible color classes
                var colorClasses = ["is-success", "is-warning","is-danger"]; // Add all possible color classes

                // Update Zone 1
                $("#tagZoneName1").text(data.Zone1.Name);
                $("#tagZonePercentage1").text(data.Zone1.Percentage + "%");
                $("#tagZoneColor1").removeClass(colorClasses.join(" ")).addClass('is-'+data.Zone1.Color);
                if (data.Zone1.Lockdown) {
                    $("#zoneLockdown1").removeClass('fa-lock-open').addClass('fa-lock').show();
                } else {
                    $("#zoneLockdown1").removeClass('fa-lock').addClass('fa-lock-open').show();
                }

                // Update Zone 2
                $("#tagZoneName2").text(data.Zone2.Name);
                $("#tagZonePercentage2").text(data.Zone2.Percentage + "%");
                $("#tagZoneColor2").removeClass(colorClasses.join(" ")).addClass('is-'+data.Zone2.Color);
                if (data.Zone2.Lockdown) {
                    $("#zoneLockdown2").removeClass('fa-lock-open').addClass('fa-lock').show();
                } else {
                    $("#zoneLockdown2").removeClass('fa-lock').addClass('fa-lock-open').show();
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
    setInterval(updateHeatMap, 3000); // Update every 3 seconds
});
