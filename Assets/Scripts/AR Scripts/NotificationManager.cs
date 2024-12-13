using System.Collections;
using Unity.Notifications.Android;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] NotificationAndroid androidNotification;
    [SerializeField] CatStatus catStatus; // Reference to the CatStatus script
    

    // List of hunger notification messages
    private readonly string[] hungerMessages = {
        "Your cat is staring intently at the empty food bowl. They're clearly disappointed.",
        "Your cat is looking at you with the 'feed me now' eyes. You know what to do!",
        "Your cat is practicing their 'sad meow' symphony. They're definitely hungry.",
        "Your cat has officially entered 'starving artist' mode. Better grab the kibble!",
        "Your cat is giving you the side-eye. It's the 'feed me' stare. You've been warned."
    };


    private readonly string[] thirstMessages = {
    "Your cat is looking at their water bowl like it's a mirage in the desert. Time to refill!",
    "Your cat is practicing their 'I'm a parched desert cat' pose. They're hoping to inspire you to give them a drink.",
    "Your cat is meowing at the water bowl like it's the only thing they want in the world. They're thirsty!"
    };

    private readonly string[] affectionMessages = {
    "Your cat is rubbing against your legs like a furry, purring alarm clock. Time for some cuddles!",
    "Your cat gives you those adorable eyes. How about some playtime?",
    "Your cat is meowing softly. Looks like someone needs a belly rub!",
    "Your cat is pawing at you. It's cuddle o'clock!",
    "Your cat is purring loudly. Time for some love and attention!"
    };


    private readonly string[] sickMessages = {
    "Your cat isn't feeling well. Keep an eye on them and give them extra love.",
    "Your cat needs some rest and care. Let's make them comfortable.",
    "Your cat is under the weather. Time for some tender loving care."
    };

    private readonly string[] bathMessages = {
    "Your cat looks a bit scruffy. Time for a bath!",
    "Your cat is starting to smell like adventure. Let's get them cleaned up!",
    "Your cat could use a spa day. Bath time!"
    };

    private void Start() {
        // Request notification authorization and register the notification channel
        androidNotification.RequestAuthorization();
        androidNotification.RegisterNotificationChannel();
    }

    private void OnApplicationFocus(bool focus) {
        if (focus) {
            // If the app is focused, cancel all pending notifications
            AndroidNotificationCenter.CancelAllNotifications();
        } else {
            // If the app loses focus and the cat is hungry, send a hunger notification
            if (catStatus.hungerLevel <= 30f) { // You can adjust this threshold
                SendHungerNotification();
            } // If the app loses focus and the cat is hungry, send a hunger notification
            if (catStatus.thirstLevel <= 30f) { // You can adjust this threshold
                SendThirstNotification();
            } // If the app loses focus and the cat is hungry, send a hunger notification
            if (catStatus.affectionLevel <= 30f) { // You can adjust this threshold
                SendAffectionNotification();
            }
            if (catStatus.IsDirty) // Assuming `catStatus` has a bool `IsDirty` property
            {
                SendBathNotification();
            } if (catStatus.IsSick) // Assuming `catStatus` has a bool `IsDirty` property
            {
                SendSickNotification();
            }

        }
    }

    // Method to send a notification with a random message
    private void SendHungerNotification() {
        int randomIndex = Random.Range(0, hungerMessages.Length);
        string message = string.Format(hungerMessages[randomIndex]); 

        // Send the notification
        androidNotification.SendNotification("Time for Food!", message, 10);
    } 
    
    private void SendThirstNotification() {
        int randomIndex = Random.Range(0, thirstMessages.Length);
        string message = string.Format(thirstMessages[randomIndex]); 

        // Send the notification
        androidNotification.SendNotification("Bring water!", message, 10);
    } 
    
    private void SendAffectionNotification() {
        int randomIndex = Random.Range(0, affectionMessages.Length);
        string message = string.Format(affectionMessages[randomIndex]); 

        // Send the notification
        androidNotification.SendNotification("Show affection!", message, 10);
    }

    private void SendBathNotification() {
        int randomIndex = Random.Range(0, bathMessages.Length);
        string message = string.Format(bathMessages[randomIndex]);

        androidNotification.SendNotification("Time for a Bath!", message, 10);
    }   
    
    private void SendSickNotification() {
        int randomIndex = Random.Range(0, sickMessages.Length);
        string message = string.Format(sickMessages[randomIndex]);

        androidNotification.SendNotification("Not feeling well!", message, 10);
    }

}
