using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] NotificationAndroid androidNotification;
    [SerializeField] PetStatus petStatus; // Reference to the PetStatus script

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
            // Notify the user when the app loses focus and hunger is low
            //CheckHungerAndNotify();
        }
    }

    //private void CheckHungerAndNotify() {
    //    if (petStatus.GetHungerLevel() <= 30f) // Assuming `GetHungerLevel` exists in PetStatus
    //    {
    //        //string petName = petStatus.GetPetName(); // Assuming `GetPetName` exists in PetStatus
    //        //string message = $"{petName} is hungry! Come feed me!";
    //        androidNotification.SendNotification("Pet Needs Attention!", "Hungry! Come feed me!", 0); // Notify
    //    }
    //}



}
