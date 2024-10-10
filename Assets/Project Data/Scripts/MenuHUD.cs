using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Data.Scripts
{
    public class MenuHUD : MonoBehaviour {

        #region UNITY METHODS

        private void Start()
        {
            CheckRewardAvailabilityOnStart();
            CheckSoundSettingsOnStart();
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause)
                CheckRewardAvailabilityOnBack();
        }

        #endregion

        #region COMMUNITY

        [Header("Community")]

        public GameObject facebookRewardGO;
        public GameObject instagramRewardGO;
        public GameObject twitterRewardGO;
        public GameObject youTubeRewardGO;
        public GameObject backgroundGO;
        public GameObject rewardHUD;
    
        private string yourPublisherUrlGoogle = "set_your_publisher_page_url";
        private string yourPublisherUrlApple = "set_your_publisher_page_url";
        private string yourEmail = "set_your_email";

        private bool _facebookRewardAvailable, _instagramRewardAvailable, _twitterRewardAvailable, _youTubeRewardAvailable;
        private bool _facebookClicked, _instClicked, _twitterClicked, _youTubeClicked;
        private DateTime _clickTime;

        private void CheckRewardAvailabilityOnStart()
        {
            //_facebookRewardAvailable = PlayerPrefs.GetInt("fbAvailable") == 0 ? true : false;
            //_instagramRewardAvailable = PlayerPrefs.GetInt("instAvailable") == 0 ? true : false;
            //_twitterRewardAvailable = PlayerPrefs.GetInt("twitterAvailable") == 0 ? true : false;
            //_youTubeRewardAvailable = PlayerPrefs.GetInt("youTubeAvailable") == 0 ? true : false;

            //facebookRewardGO.SetActive(_facebookRewardAvailable);
            //instagramRewardGO.SetActive(_instagramRewardAvailable);
            //twitterRewardGO.SetActive(_twitterRewardAvailable);
            //youTubeRewardGO.SetActive(_youTubeRewardAvailable);
        }

        private void CheckRewardAvailabilityOnBack()
        {
            if ((_instagramRewardAvailable && _instClicked) ||
                (_youTubeRewardAvailable && _youTubeClicked) ||
                (_twitterRewardAvailable && _twitterClicked) ||
                (_facebookRewardAvailable && _facebookClicked))
            {
                TimeSpan diff = DateTime.Now - _clickTime;
                if (diff.TotalSeconds >= 7)
                { 
                    backgroundGO.SetActive(true);
                    rewardHUD.SetActive(true);
                    if (_instClicked) instagramRewardGO.SetActive(false);
                    if (_youTubeClicked) youTubeRewardGO.SetActive(false);
                    if (_twitterClicked) twitterRewardGO.SetActive(false);
                    if (_facebookClicked) facebookRewardGO.SetActive(false);
                }
            }
        }

        #region BUTTON HANDLERS

        public void OnClick_Rate()
        {
#if UNITY_ANDROID
            Application.OpenURL("market://details?id=" + Application.identifier);
#endif
#if UNITY_IPHONE
        string appStoreUrl = "https://apps.apple.com/app/" + GameManager.Instance.iosAppId;
        Application.OpenURL(appStoreUrl);
#endif

            PlayerPrefs.SetInt("RatedPressed", 1);
        }

        public void OnClick_Email()
        {
            SoundManager.Instance.PlayClickSound();
            string subject = MyEscapeURL("Idle Farm Manager Feedback/Bug");
            string body = MyEscapeURL("");
            Application.OpenURL("mailto:" + yourEmail + "?subject=" + subject + "&body=" + body);
        }
    
        string MyEscapeURL (string url)
        {
            return WWW.EscapeURL(url).Replace("+","%20");
        }

        public void OnClick_MoreGames()
        {
            AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.MoreGamesPressed);
            SoundManager.Instance.PlayClickSound();
#if UNITY_ANDROID
            Application.OpenURL(yourPublisherUrlGoogle);
#endif
#if UNITY_IPHONE
        Application.OpenURL(yourPublisherUrlApple);
#endif
        }

        public void OnClick_Facebook()
        {
            SoundManager.Instance.PlayClickSound();
            Application.OpenURL("");
            if (_facebookRewardAvailable)
            {
                _facebookClicked = true;
                _clickTime = DateTime.Now;
            }
        }

        public void OnClick_Inst()
        {
            SoundManager.Instance.PlayClickSound();
            Application.OpenURL("");
            if (_instagramRewardAvailable)
            {
                _instClicked = true;
                _clickTime = DateTime.Now;
            }
        }

        public void OnClick_Twitter()
        {
            SoundManager.Instance.PlayClickSound();
            Application.OpenURL("");
            if (_twitterRewardAvailable)
            {
                _twitterClicked = true;
                _clickTime = DateTime.Now;
            }
        }

        public void OnClick_YouTube()
        {
            SoundManager.Instance.PlayClickSound();
            Application.OpenURL("");
            if (_youTubeRewardAvailable)
            {
                _youTubeClicked = true;
                _clickTime = DateTime.Now;
            }
        }

        public void OnClick_Nice()
        {
            SoundManager.Instance.PlayClickSound();
            GameManager.Instance.addBags(20);

            backgroundGO.SetActive(false);
            rewardHUD.SetActive(false);
            if (_instagramRewardAvailable && _instClicked)
            {
                PlayerPrefs.SetInt("instAvailable", 1);
            }
            if (_youTubeRewardAvailable && _youTubeClicked)
            {
                PlayerPrefs.SetInt("youTubeAvailable", 1);
            }
            if (_twitterRewardAvailable && _twitterClicked)
            {
                PlayerPrefs.SetInt("twitterAvailable", 1);
            }
            if (_facebookRewardAvailable && _facebookClicked)
            {
                PlayerPrefs.SetInt("fbAvailable", 1);
            }
        }

        #endregion

        #endregion

        #region SETTINGS

        [Header("Settings")]

        public GameObject soundOnBtn, soundOffBtn;
        public GameObject musicOnBtn, musicOffBtn;
        public GameObject notificationsOnBtn, notificationsOffBtn;
        public GameObject particleOnBtn, particleOffBtn;

        private bool _soundOn, _musicOn, _notificationsOn, _particleOn;

        void CheckSoundSettingsOnStart()
        {
            _soundOn = PlayerPrefs.GetInt("SoundOn") == 0 ? true : false;
            _musicOn = PlayerPrefs.GetInt("MusicOn") == 0 ? true : false;
            _notificationsOn = PlayerPrefs.GetInt("NotificationsOn") == 0 ? true : false;
            _particleOn = PlayerPrefs.GetInt("ParticlesOn") == 0 ? true : false;
            SetSoundMusicNotificationsSettings();
        }

        void SetSoundMusicNotificationsSettings()
        {
            soundOnBtn.GetComponent<Image>().color  = _soundOn  ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
            soundOffBtn.GetComponent<Image>().color = !_soundOn ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
            musicOnBtn.GetComponent<Image>().color  = _musicOn  ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
            musicOffBtn.GetComponent<Image>().color = !_musicOn ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
            notificationsOnBtn.GetComponent<Image>().color  = _notificationsOn  ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
            notificationsOffBtn.GetComponent<Image>().color = !_notificationsOn ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);

            particleOnBtn.GetComponent<Image>().color  = _particleOn  ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
            particleOffBtn.GetComponent<Image>().color = !_particleOn ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);

            SoundManager.Instance.setSoundsOnOff(_soundOn);
            SoundManager.Instance.setMusicOnOff(_musicOn);
        }

        public void OnClick_Sound()
        {
            SoundManager.Instance.PlayClickSound();
            _soundOn = !_soundOn;
            if (_soundOn) PlayerPrefs.SetInt("SoundOn", 0);
            else PlayerPrefs.SetInt("SoundOn", 1);
            SetSoundMusicNotificationsSettings();
        }

        public void OnClick_Music()
        {
            SoundManager.Instance.PlayClickSound();
            _musicOn = !_musicOn;
            if (_musicOn) PlayerPrefs.SetInt("MusicOn", 0);
            else PlayerPrefs.SetInt("MusicOn", 1);
            SetSoundMusicNotificationsSettings();
        }

        public void OnClick_Notifications()
        {
            SoundManager.Instance.PlayClickSound();
            _notificationsOn = !_notificationsOn;
            if (_notificationsOn) PlayerPrefs.SetInt("NotificationsOn", 0);
            else PlayerPrefs.SetInt("NotificationsOn", 1);
            SetSoundMusicNotificationsSettings();
        }

        public void OnClick_Particle()
        {
            SoundManager.Instance.PlayClickSound();
            _particleOn = !_particleOn;
            if (_particleOn) PlayerPrefs.SetInt("ParticlesOn", 0);
            else PlayerPrefs.SetInt("ParticlesOn", 1);
            SetSoundMusicNotificationsSettings();
        }

        public void openPanel()
        {
            SoundManager.Instance.PlayOpenHudSound();
            gameObject.SetActive(true);

            GameObject popup = transform.Find("bg").gameObject;
            popup.transform.localScale = Vector3.zero;

            Sequence seq = DOTween.Sequence();
            seq.Append(popup.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f));
            seq.Append(popup.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));
        }

        public void closePanel()
        {
            SoundManager.Instance.PlayCloseHudSound();
            GameObject popup = transform.Find("bg").gameObject;
            popup.transform.localScale = Vector3.one;

            Sequence seq = DOTween.Sequence();
            seq.Append(popup.transform.DOScale(Vector3.zero, 0.2f));
            seq.AppendCallback(()=> gameObject.SetActive(false));
        }

        public void checkSoundOnOff()
        {
            _soundOn = PlayerPrefs.GetInt("SoundOn") == 0 ? true : false;
            _musicOn = PlayerPrefs.GetInt("MusicOn") == 0 ? true : false;
            _notificationsOn = PlayerPrefs.GetInt("NotificationsOn") == 0 ? true : false;
            _particleOn = PlayerPrefs.GetInt("ParticlesOn") == 0 ? true : false;
            SoundManager.Instance.setSoundsOnOff(_soundOn);
            SoundManager.Instance.setMusicOnOff(_musicOn);
        }

        #endregion
    }
}