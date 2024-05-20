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

				var colorClasses = ["is-success", "is-warning", "is-danger"]; // Add all possible color classes

				for (var id in data) {
					$("#tagZoneName" + id).text(data[id].Name);
					if (data[id].Percentage === -1) {
						$("#tagZoneColor" + id).removeClass(colorClasses.join(" ")).addClass('is-' + data[id].Color);
					} else {
						$("#tagZonePercentage" + id).text(data[id].Percentage + "%");
					}

					if (data[id].Lockdown) {
						$("#zoneLockdown" + id).removeClass('fa-lock-open').addClass('fa-lock').show();
					} else {
						$("#zoneLockdown" + id).removeClass('fa-lock').addClass('fa-lock-open').show();
					}
				};
			},
			error: function (error) {
				console.log("Error: ", error);
			}
		});
	}

	// Initial call to update the heatmap
	updateHeatMap();

	// Set an interval to update the heatmap periodically
	setInterval(updateHeatMap, 3000); // Update every 5 seconds
});
