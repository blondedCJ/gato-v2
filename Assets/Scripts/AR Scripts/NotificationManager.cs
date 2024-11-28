using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] NotificationAndroid androidNotification;

    private void Start() {

        androidNotification.RequestAuthorization();
        androidNotification.RegisterNotificationChannel();

    }

    private void OnApplicationFocus(bool focus) {
        // on the app
        if (focus == true) {
            AndroidNotificationCenter.CancelAllNotifications();
            androidNotification.SendNotification("Play the game", "Hello! Come back.", 0);
        }
        // off the app
        if (focus == false) {
            androidNotification.SendNotification("15 Seconds", "This is fifteen seconds", 15);
        }
    }

}
