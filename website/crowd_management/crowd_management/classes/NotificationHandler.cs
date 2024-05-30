using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace crowd_management.classes;

public class NotificationHandler
{
  public void AddNotification(string message, ENotificationCategories category, Page page)
  {
    HiddenField hiddenMessageField = page.FindControl("hiddenMessageField") as HiddenField;

    if (hiddenMessageField == null)
    {
      return;
    }

    try
    {
      List<Notification> notifications = new List<Notification>();

      if (!string.IsNullOrEmpty(hiddenMessageField.Value))
      {
        notifications = JsonConvert.DeserializeObject<List<Notification>>(hiddenMessageField.Value);
      }

      notifications.Add(new Notification
      {
        category = category.ToString(),
        message = message
      });

      string notificationString = JsonConvert.SerializeObject(notifications);
      hiddenMessageField.Value = notificationString;
    }
    catch (JsonException)
    {
      hiddenMessageField.Value = hiddenMessageField.Value;
    }
  }
}

public class Notification
{
  public string message { get; set; }
  public string category { get; set; }
}