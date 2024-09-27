using UnityEngine;

namespace Project_Data.Scripts
{
    public class IronSourceManager : MonoBehaviour {
    
        public enum VideoType
        {
            Default,
            OfflineEarning,
            BoostProfits,
            UnlockBridge
        }
    
        public static string appKey = "YOUR_LEVEL_PLAY_KEY";
        private VideoType _videoType;
    
        /*

    void Start()
    {
        IronSourceConfig.Instance.setClientSideCallbacks(true);
        string id = IronSource.Agent.getAdvertiserId();
        IronSource.Agent.validateIntegration();
        IronSource.Agent.setAdaptersDebug(true);
        IronSource.Agent.init(appKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.OFFERWALL, IronSourceAdUnits.BANNER);
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
    }

    private void OnDisable()
    {
        IronSourceRewardedVideoEvents.onAdRewardedEvent -= RewardedVideoOnAdRewardedEvent;
    }

    private void RewardedVideoOnAdRewardedEvent(IronSourcePlacement arg1, IronSourceAdInfo arg2)
    {
        SoundManager.Instance.bgMusic.mute = false;
        switch (_videoType)
        {
            case VideoType.Default:
                break;

            case VideoType.OfflineEarning:
                GameManager.Instance.offlineEarningPopup.videoAdFinished();
                break;

            case VideoType.BoostProfits:
                GameManager.Instance.boostProfitsPopup.videoAdFinished();
                break;
            case VideoType.UnlockBridge:
                GameManager.Instance.bridgeManager.videoAdFinished();
                break;
        }
    }

    public void LoadBanner()
    {
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }

    public void DestroyBanner()
    {
        IronSource.Agent.destroyBanner();
    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    public void LoadInterstitialButtonClicked()
    {
        IronSource.Agent.loadInterstitial();
    }

    public void ShowInterstitialButtonClicked()
    {
        if (IronSource.Agent.isInterstitialReady())
        {
            IronSource.Agent.showInterstitial();
        }
        else
        {
            Debug.Log("IronSource.Agent.isInterstitialReady - False");
        }
    }

    public void Test_OnClickShowRewardedVideo() // only for testing
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
    }

    int testCallbacksCounter;
    void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
    {
        SoundManager.Instance.bgMusic.mute = false;
        switch (_videoType)
        {
            case VideoType.Default:
                break;

            case VideoType.OfflineEarning:
                GameManager.Instance.offlineEarningPopup.videoAdFinished();
                break;

            case VideoType.BoostProfits:
                GameManager.Instance.boostProfitsPopup.videoAdFinished();
                break;
            case VideoType.UnlockBridge:
                GameManager.Instance.bridgeManager.videoAdFinished();
                break;
        }
    }

    public bool isInterstatialAvailable()
    {
        return IronSource.Agent.isInterstitialReady();
    }

    public string GetAppKey()
    {
        return appKey;
    }
    */
    
        public bool IsVideoAvailable()
        {
            //#if UNITY_EDITOR
            return true;
            //#endif
            //return IronSource.Agent.isRewardedVideoAvailable();
        }
    
        public void ShowRewardedVideoButtonClicked(VideoType type)
        {
            _videoType = type;
            SoundManager.Instance.bgMusic.mute = true;
            //IronSource.Agent.showRewardedVideo();
        }
    }
}