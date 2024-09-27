using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace Project_Data.Scripts
{
    public enum CustomAnalyticsEvent
    {
        TutorialStep,
        UnlockedPlot,
        UnlockedPlotWithGems,
        ManagerHiredInPlot,
        ElevatorUpgraded,
        WirehouseUpgraded,
        RateNowPressed,
        RateLaterPressed,
        QuickTravel_4,
        QuickTravel_24,
        QuickTravel_72,
        RateInMenuPressed, // send from button in scene
        MoreGamesPressed,
        WatchVideo_Hours2,
        WatchVideo_Hours4,
        WatchVideo_Hours6,
        WatchVideo_Hours8,
        WatchVideo_Hours10,
        WatchVideo_Hours12,
        WatchVideo_Hours14,
        WatchVideo_Hours16,
        WatchVideo_Hours18,
        WatchVideo_Hours20,
        WatchVideo_Hours22,
        WatchVideo_Hours24,
        QuickTravelHud_Open, // send from button in scene
        x2Hud_Open, // send from button in scene
        InAppShop_Open, // send from button in scene
        InAppShopIClickedItem, 
        Continent_Open, // send from button in scene
        SetttingsHud_Open, // send from button in scene
        OfflineProduction_Collect,
        OfflineProduction_x2_VideoWatched
    }

    public class AnalyticsManager : MonoBehaviour
    {
        public static AnalyticsManager Instance;

        private void Awake()
        {
            Instance = this;
        }
    
        public void SendEvent(CustomAnalyticsEvent e)
        {
            Analytics.CustomEvent (e.ToString ());
        }
    
        /// <summary>
        /// Can be called from button events on scene
        /// </summary>
        /// <param name="analyticsName"></param>
        public void SendEvent(string analyticsName)
        {
            Analytics.CustomEvent (analyticsName);
        }

        public void SendEvent(CustomAnalyticsEvent e, int number)
        {
            Analytics.CustomEvent(e.ToString() + number.ToString());
        }
    
        public void SendPlotUpgradedEvent(int plotIndex, int level)
        {
            Analytics.CustomEvent("PlotUpgraded", new Dictionary<string, object>
            {
                { "plotIndex", plotIndex },
                { "level", level }
            });    
        }
    }
}