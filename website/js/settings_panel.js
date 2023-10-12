function toggleSettings(show) {
  var infoPanel = document.getElementById("info-panel");
  var settingsPanel = document.getElementById("settings-panel");

  if (show == "settings") {
    infoPanel.classList.add("hide");
    settingsPanel.classList.remove("hide");
  } else if (show == "info") {
    infoPanel.classList.remove("hide");
    settingsPanel.classList.add("hide");
  };
};
