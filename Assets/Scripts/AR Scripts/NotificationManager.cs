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
        if (focus == true) {
            AndroidNotificationCenter.CancelAllNotifications();
            androidNotification.SendNotification("Paws && Play","Hello! Come back", 0);

        }
    }

}
