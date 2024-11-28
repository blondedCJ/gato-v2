using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using UnityEngine.Android;


public class NotificationAndroid : MonoBehaviour
{
    // Request authorisation to send notification
    public void RequestAuthorization() {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS")) {
            Permission.RequestUserPermission("android.POST_NOTIFICATIONS");
        }
    }

    // Register a notification channel
    public void RegisterNotificationChannel() 
    {
        var channel = new AndroidNotificationChannel {
            Id = "default_channel",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Paws && Play"
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    // Set up notification template
    public void SendNotification(string title, string text, int fireTimeinHours) {
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = text;
        notification.FireTime = System.DateTime.Now.AddHours(fireTimeinHours);
        notification.SmallIcon = "icon_0";
        notification.LargeIcon = "icon_1";

        AndroidNotificationCenter.SendNotification(notification, "default_channel");

    }


}

