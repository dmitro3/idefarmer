using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Data.Scripts
{
    public class BoostProfits : MonoBehaviour
    {
        public GameManager gameManager;
        public Slider slider;
        public Text   sliderText;
    
        void Update ()
        {
            float boostDuration = gameManager.getBoostDuration();
            float totalDuration = 24 * 60 * 60;

            slider.value = boostDuration / totalDuration;

            TimeSpan ts = TimeSpan.FromSeconds(boostDuration);
            string timeStr = GameUtils.TimeSpanToReadableString(ts);

            sliderText.text = timeStr + " / 24h";
        }

        public void OpenPanel()
        {
            SoundManager.Instance.PlayOpenHudSound();
            gameObject.SetActive(true);

            GameObject popup = transform.Find("bg").gameObject;
            popup.transform.localScale = Vector3.zero;

            Sequence seq = DOTween.Sequence();
            seq.Append(popup.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f));
            seq.Append(popup.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));
        }

        public void ClosePanel()
        {
            SoundManager.Instance.PlayClickSound();
            GameObject popup = transform.Find("bg").gameObject;
            popup.transform.localScale = Vector3.one;

            Sequence seq = DOTween.Sequence();
            seq.Append(popup.transform.DOScale(Vector3.zero, 0.2f));
            seq.AppendCallback(()=> gameObject.SetActive(false));
        }

        public void showVideoAd()
        {
            SoundManager.Instance.PlayClickSound();
#if UNITY_EDITOR
            videoAdFinished();
            gameObject.SetActive(false);
            return;
#endif
            IronSourceManager adManager = gameManager.adsManager;
            if (adManager.IsVideoAvailable())
            {
                adManager.ShowRewardedVideoButtonClicked(IronSourceManager.VideoType.BoostProfits);
            }
            else
            {
                gameManager.ShowToast("Video not available");
            }


            gameObject.SetActive(false);
        }

        public void videoAdFinished()
        {
            gameManager.enableBoost();
        }
    }
}