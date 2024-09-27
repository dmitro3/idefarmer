using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Data.Scripts
{
    public class OfflineEarningPopup : MonoBehaviour
    {
        public Text text;
        public Text inactiveTimeText;
        public Button videoButton;
        public GameObject coinEffect;

        GameManager gameManager;
        double offlineEarning;

        public IEnumerator calculateOfflineEarning()
        {
            gameManager = GameManager.Instance;
            yield return new WaitForSeconds(2f);
            if (!PlayerPrefs.HasKey("closeTime"))
            {
                yield break;
            }

            string closeTime = PlayerPrefs.GetString("closeTime");
            long closeTimeLong = Convert.ToInt64(closeTime);
            DateTime oldDate = DateTime.FromBinary(closeTimeLong);
            DateTime currentDate = System.DateTime.Now;
            TimeSpan difference = currentDate.Subtract(oldDate);//TimeSpan.FromSeconds(26 * 60 * 60);//
            inactiveTimeText.text = GameUtils.TimeSpanToReadableString(difference);

            if (difference.TotalSeconds < 180)
            {
                yield break;
            }

            float maxOfflineEarnTime = 1 * 60 * 60;
            if (difference.TotalSeconds >= maxOfflineEarnTime)
            {
                difference = TimeSpan.FromSeconds(maxOfflineEarnTime);
            }
        
            offlineEarning = (gameManager.getEarning() / 7) * difference.TotalSeconds;
            text.text = GameUtils.currencyToString(offlineEarning);

            if (offlineEarning <= 0)
            {
                yield break;
            }

            OpenPanel();
        }

        public void OpenPanel()
        {
            SoundManager.Instance.PlayOpenHudSound();
            IronSourceManager adManager = gameManager.adsManager;

            if (adManager.IsVideoAvailable())
            {
                videoButton.interactable = true;
            }
            else
            {
                videoButton.interactable = false;
            }

            gameObject.SetActive(true);

            GameObject popup = transform.Find("bg").gameObject;
            popup.transform.localScale = Vector3.zero;

            Sequence seq = DOTween.Sequence();
            seq.Append(popup.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f));
            seq.Append(popup.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));
        }

        public void ClosePanel()
        {
            SoundManager.Instance.PlayGetCoinsSound();

            GameObject particle = Instantiate(coinEffect, transform.position, Quaternion.identity);
            particle.GetComponent<CoinsEffect>().PlayEffect(gameManager.cashText.transform);

            gameManager.addCash(offlineEarning);
            GameObject popup = transform.Find("bg").gameObject;
            popup.transform.localScale = Vector3.one;

            Sequence seq = DOTween.Sequence();
            seq.Append(popup.transform.DOScale(Vector3.zero, 0.2f));
            seq.AppendCallback(()=> gameObject.SetActive(false));
            AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.OfflineProduction_Collect);
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
            adManager.ShowRewardedVideoButtonClicked(IronSourceManager.VideoType.OfflineEarning);
            gameObject.SetActive(false);   
        }

        public void videoAdFinished()
        {
            gameManager.addCash(offlineEarning*2);
            SoundManager.Instance.PlayGetCoinsSound();

            GameObject particle = Instantiate(coinEffect, transform.position, Quaternion.identity);
            particle.GetComponent<CoinsEffect>().PlayEffect(gameManager.cashText.transform);
            AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.OfflineProduction_x2_VideoWatched);
        }
    }
}