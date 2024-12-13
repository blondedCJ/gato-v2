using System.Collections;
using Unity.Notifications.Android;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] NotificationAndroid androidNotification;
    [SerializeField] CatStatus catStatus; // Reference to the CatStatus script

    // List of messages for various conditions
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

    private string catID; // To store the cat's ID (name)

    private void Start() {
        // Request notification authorization and register the notification channel
        androidNotification.RequestAuthorization();
        androidNotification.RegisterNotificationChannel();

        // Get the catID from CatStatus (You can change how you retrieve this ID)
        catID = catStatus.GetCatID(); // Assuming GetCatID() method exists in CatStatus script



        if (PlayerPrefs.HasKey(catID + "_Hunger")) {
            float hungerLevel = PlayerPrefs.GetFloat(catID + "_Hunger", 0);
            Debug.Log("Hunger Level: " + hungerLevel);
        } else {
            Debug.LogError("No data found for hunger key: " + catID + "_Hunger");
        }




        // Ensure catStatus is assigned
        if (catStatus == null) {
            Debug.LogError("catStatus is not assigned!");
        } else {
            catID = catStatus.GetCatID();
            Debug.Log("Cat ID: " + catID);  // Debugging the catID
        }
        // Check and notify hunger and thirst
        //sendNotification();
    }

    private void OnApplicationFocus(bool focus) {
        if (focus) {
            // If the app is focused, cancel all pending notifications
            AndroidNotificationCenter.CancelAllNotifications();
            sendNotification();


            
        } else {
            // If the app loses focus, send a random notification based on the cat's status
            androidNotification.SendNotification("Come back", "This is 60 seconds", PlayerPrefs.GetInt("TreatsGoalAchieved", 0)); 
            // sa 60 pede value true or false and calculate kung ilang value pa bago mag <= 30 thru player prefs   
            // halimbawa 
            //pano calculations hm 
            //kung ang natitira sa hunger level is 50f how long does it take to reach 30f yung sagot yung sasave sa player prefs
            //tapos ayun lalagay sa schedule 


            sendNotification();
        }
    }

 

    // Method to send a notification with a random message
    private void SendHungerNotification() {
        Debug.Log("Cat is hungry");
        int randomIndex = Random.Range(0, hungerMessages.Length);
        string message = string.Format(hungerMessages[randomIndex], catID);
        androidNotification.SendNotification("Time for Food!", message, 0);
    }

    private void SendThirstNotification() {
        Debug.Log("Cat is Thirsdty");
        int randomIndex = Random.Range(0, thirstMessages.Length);
        string message = string.Format(thirstMessages[randomIndex], catID);
        androidNotification.SendNotification("Bring water!", message, 0);
    }

    private void SendAffectionNotification() {
        int randomIndex = Random.Range(0, affectionMessages.Length);
        string message = string.Format(affectionMessages[randomIndex], catID);
        androidNotification.SendNotification("Show affection!", message, 0);
    }

    private void SendBathNotification() {
        Debug.Log("Cat is dirty");
        int randomIndex = Random.Range(0, bathMessages.Length);
        string message = string.Format(bathMessages[randomIndex], catID);
        androidNotification.SendNotification("Time for a Bath!", message, 0);
    }

    private void SendSickNotification() {
        Debug.Log("Cat is sick");
        int randomIndex = Random.Range(0, sickMessages.Length);
        string message = string.Format(sickMessages[randomIndex], catID);
        androidNotification.SendNotification("Not feeling well!", message, 0);
    }

    private void sendNotification() {
        Debug.Log("sendnotification method");
        

        // Get the hunger, thirst, affection levels, and other conditions from PlayerPrefs
        float hungerLevel = PlayerPrefs.GetFloat(catID + "_Hunger", 0);
        float thirstLevel = PlayerPrefs.GetFloat(catID + "_Thirst", 0);
        float affectionLevel = PlayerPrefs.GetFloat(catID + "_Affection", 0);

        bool isSick = PlayerPrefs.GetInt(catID + "_IsSick", 0) == 1;
        bool isDirty = PlayerPrefs.GetInt(catID + "_IsDirty", 0) == 1;

        if (hungerLevel <= 30f) {
            SendHungerNotification();
        }


        if (thirstLevel <= 30f) {
            SendThirstNotification();
        }

        if (affectionLevel <= 30f) {
            SendAffectionNotification();
        }

        if (isSick) {
            Debug.Log("is sick from sendnotification method");
            SendSickNotification();
        }

        if (isDirty) {
            SendBathNotification();
        }


        int msg = PlayerPrefs.GetInt("GoalsCounter", 0);

        PlayerPrefs.GetFloat(catID + "_Hunger", 100f);
        Debug.Log("Went through if statements??? display cat id hunger >>>>>>>>>>>" + PlayerPrefs.GetFloat(catID + "_Hunger", 100f)); 
        Debug.Log("Went through if statements??? display cat id thirsy>>>>>>>>>>>" + thirstLevel);
        Debug.Log("Went through if statements??? display cat id >>>>>>>>>>>" + catID);  
        Debug.Log("Wvalue ng msg  >>>>>>>>>>>" + msg);

   


    }

}
