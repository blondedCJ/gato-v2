using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] NotificationAndroid androidNotification;
    [SerializeField] CatStatus catStatus; // Reference to the CatStatus script

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
            
            CheckHungerAndNotify();
            CheckAffectionAndNotify();
            CheckThirstAndNotify();

            androidNotification.SendNotification("15 Seconds", "This is fifteen seconds", 15);
            
        }
    }

    private void CheckHungerAndNotify() {
        if (catStatus != null && catStatus.hungerLevel <= 30f) {
            string catID = catStatus.gameObject.name;
            string title = "Your Cat is Hungry!";
            string message = $"{catID} is hungry, please feed me!";

            // Schedule a notification immediately
            androidNotification.SendNotification(title, message, 0);
        }
    }     private void CheckAffectionAndNotify() {
        if (catStatus != null && catStatus.affectionLevel <= 30f) {
            string catID = catStatus.gameObject.name;
            string title = "Your Cat needs attention!";
            string message = $"{catID} is needs affection, please touch me!";

            // Schedule a notification immediately
            androidNotification.SendNotification(title, message, 0);
        }
    }     private void CheckThirstAndNotify() {
        if (catStatus != null && catStatus.thirstLevel <= 30f) {
            string catID = catStatus.gameObject.name;
            string title = "Your Cat is thirsty!";
            string message = $"{catID} is thirsty, please bring me water!";

            // Schedule a notification immediately
            androidNotification.SendNotification(title, message, 0);
        }
    }
}
