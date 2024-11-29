using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using UnityEngine.Android;


public class NotificationAndroid : MonoBehaviour
{
    // Request authorisation to send notification
    // Request authorization to send notifications
    public void RequestAuthorization() {
        if (Application.platform == RuntimePlatform.Android && GetAndroidAPILevel() >= 33) // Android 13+
        {
            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS")) {
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }
        }
    }

    // Register a notification channel
    public void RegisterNotificationChannel() {
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
        notification.FireTime = System.DateTime.Now.AddSeconds(fireTimeinHours); // seconds for awhile
        notification.SmallIcon = "icon_0";
        notification.LargeIcon = "icon_1";

        AndroidNotificationCenter.SendNotification(notification, "default_channel");

    }

    // Get Android API level
    private int GetAndroidAPILevel() {
        using (var versionClass = new AndroidJavaClass("android.os.Build$VERSION")) {
            return versionClass.GetStatic<int>("SDK_INT");
        }
    }


}

