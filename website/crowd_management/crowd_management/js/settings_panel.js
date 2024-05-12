/*
 * File: settings_panel.js
 * Author: Warre Willeme & Jesse UijtdeHaag
 * Date: May 12, 2024
 * Description: This file contains the JavaScript code for the settings panel on the crowd management page.
 */

function toggleSettings() {
    var divInfoPanel = document.getElementById("divInfoPanel");
    var divSettingsPanel = document.getElementById("divSettingsPanel");

    divInfoPanel.classList.toggle("hide");
    divSettingsPanel.classList.toggle("hide");
}
