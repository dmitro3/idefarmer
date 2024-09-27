using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Project_Data.Scripts
{
    public class RateMeHUD : MonoBehaviour {

        public Animator[] starsAnimators;
        public Transform hudTr;

        private void OnEnable()
        {
            hudTr.DOScale(1.1f, 0.25f).OnComplete(() => hudTr.DOScale(1, 0.25f));
            StartCoroutine(PlayStarsBounceAnimation());
        }

        IEnumerator PlayStarsBounceAnimation()
        {
            for (int i = 0; i < starsAnimators.Length; i++)
            {
                yield return new WaitForSeconds(0.25f);
                starsAnimators[i].SetTrigger("Start");
            }
            yield return new WaitForSeconds(0.5f);
            if(gameObject.activeSelf)
                StartCoroutine(PlayStarsBounceAnimation());
        }

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

            SoundManager.Instance.PlayClickSound();
            AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.RateNowPressed);
            OnClick_CloseHUD();
        }

        public void OnClick_Later()
        {
            AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.RateLaterPressed);
            OnClick_CloseHUD();
            SoundManager.Instance.PlayClickSound();
        }

        public void OnClick_CloseHUD()
        {
            hudTr.DOScale(1.1f, 0.25f).OnComplete(() => hudTr.DOScale(0, 0.25f));
            Invoke("Deactivate", 0.5f);
            SoundManager.Instance.PlayClickSound();
        }

        void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void openPopup()
        {
            int isRatePressed = PlayerPrefs.GetInt("RatedPressed", 0);
            if (isRatePressed == 0)
            {
                gameObject.SetActive(true);
            }
        }
    }
}