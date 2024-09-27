using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.Notifications.Android;
using UnityEngine;

//Báo tín hieu
namespace Project_Data.Scripts
{
    public class LocalNotification : SingletonBehaviour<LocalNotification> {

        public List<int> hoursLst;
    
        private void Awake()
        {
#if UNITY_ANDROID
            ClearAllNotifications();

            var c = new AndroidNotificationChannel();
            c.Id = "default_test_channel_5";
            c.Name = "Default Channel 5";
            c.Description = "test_channel 5";
            c.Importance = Importance.High;
            AndroidNotificationCenter.RegisterNotificationChannel(c);
#endif
        }
    
        private void Start()
        {
            Invoke("Init", 0.1f);
        }

        private void Init()
        {
            ClearAllNotifications();
            StartCoroutine(RequestAuthorization());
        }
    
        IEnumerator RequestAuthorization()
        {
#if UNITY_IOS
        var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
        using (var req = new AuthorizationRequest(authorizationOption, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            };

            string res = "\n RequestAuthorization:";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted;
            res += "\n error:  " + req.Error;
            res += "\n deviceToken:  " + req.DeviceToken;
            Debug.Log(res);
        }
#endif
            yield return new WaitForSeconds(1);
        }
    
        public void ClearAllNotifications()
        {
#if UNITY_ANDROID
            AndroidNotificationCenter.CancelAllNotifications();
#elif UNITY_IPHONE
        // Cancel all scheduled notifications
        iOSNotificationCenter.RemoveAllScheduledNotifications();

        // Cancel all delivered notifications, i.e., remove them from the Notification Center
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
#endif
        }
    
        private void SendAllNotifications()
        {
#if UNITY_ANDROID
            ClearAllNotifications();
#elif UNITY_IPHONE
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
#endif
            SendNotif(60 * 15); // send first notification in 15 min
            for (int i = 0; i < hoursLst.Count - 1; i++)
                SendNotif(hoursLst[i] * 3600);
        }
    
        void SendNotif(int seconds)
        {
            string message1 = "Idle Farm Tycoon 😀"; // 
            string message2 = "🎁🤗Come back your workers are waiting for you. 🤗🎁"; 

#if UNITY_ANDROID
            var n = new AndroidNotification();
            if (seconds != 0)
                n.FireTime = System.DateTime.Now.AddSeconds(seconds);

            n.Title = message1;
            n.Text = message2;
            n.LargeIcon = "notify_icon_big";
            n.SmallIcon = "notify_icon_small";

            AndroidNotificationCenter.SendNotification(n, "default_test_channel_5");
#elif UNITY_IPHONE
        for (int i = 0; i < hoursLst.Count; i++)
        {
            ScheduleNotification(message2, new TimeSpan(hoursLst[i], 0, 0));
        }
#endif
        }
    
        void ScheduleNotification(string message, TimeSpan delay)
        {
#if UNITY_IOS
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = delay,
            Repeats = false
        };

        var notification = new iOSNotification()
        {
            Identifier = $"notification_{message.GetHashCode()}",
            Title = "Idle Farm Tycoon 😀",
            Body = message,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };

        iOSNotificationCenter.ScheduleNotification(notification);
#endif
        }
    
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                SendAllNotifications();
        }
    
        void OnApplicationExit()
        {
            SendAllNotifications();
        }

        void OnApplicationQuit()
        {
            SendAllNotifications();
        }
    }
}