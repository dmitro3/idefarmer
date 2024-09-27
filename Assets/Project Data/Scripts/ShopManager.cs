using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Data.Scripts
{
    public class ShopManager : MonoBehaviour
    {
        public Text text1;
        public Text text2;
        public Text text3;
        public Text text4;
        public Text text5;
        public Text text6;
        public Text gemCount;

        public GameObject quickShopBtnFree;
        public GameObject quickShopBtnFree1;
        public GameObject quickShopBtn1;
        public GameObject quickShopBtn11;

        public GameObject congratsHud;
        public Text   rewardTxt;

        public void closeBtn()
        {
            SoundManager.Instance.PlayCloseHudSound();
            GameObject popup = transform.Find("bg").gameObject;
            popup.transform.localScale = Vector3.one;

            Sequence seq = DOTween.Sequence();
            seq.Append(popup.transform.DOScale(Vector3.zero, 0.2f));
            seq.AppendCallback(()=> gameObject.SetActive(false));
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

            if (gemCount != null)
            {
                gemCount.text = "Current Gem Count: " + GameManager.Instance.getBags();
            }

            if (quickShopBtn1 != null)
            {
                quickShopBtn1.SetActive(true);
                quickShopBtnFree.SetActive(false);
                quickShopBtn11.SetActive(true);
                quickShopBtnFree1.SetActive(false);
            }

            if (text1 == null)
                return;

            double coin1 = 200;
            double coin2 = 700;
            double coin3 = 1500;
            double coin4 = 3500;
            double coin5 = 10000;
            double coin6 = 100000;

            if (text5 == null)
            {
                coin1 = GameManager.Instance.getEarning() == 0 ? 100000 : 100000 + 100 * GameManager.Instance.getEarningForInapp();
                coin2 = GameManager.Instance.getEarning() == 0 ? 1000000 : 1000000 + 250 * GameManager.Instance.getEarningForInapp();
                coin3 = GameManager.Instance.getEarning() == 0 ? 10000000 : 10000000 + 500 * GameManager.Instance.getEarningForInapp();
                coin4 = GameManager.Instance.getEarning() == 0 ? 100000000 : 100000000 + 1000 * GameManager.Instance.getEarningForInapp();
            }

            text1.text = GameUtils.currencyToString(coin1, 0);
            text2.text = GameUtils.currencyToString(coin2, 0);
            text3.text = GameUtils.currencyToString(coin3, 0);
            text4.text = GameUtils.currencyToString(coin4, 0);

            if (text5 == null)
                return;

            text5.text = GameUtils.currencyToString(coin5, 0);
            text6.text = GameUtils.currencyToString(coin6, 0);
        }

        public void openQuickPanel(GameObject tap)
        {
            SoundManager.Instance.PlayOpenHudSound();
            gameObject.SetActive(true);
            GameObject popup = transform.Find("bg").gameObject;
            popup.transform.localScale = Vector3.zero;

            Sequence seq = DOTween.Sequence();
            seq.Append(popup.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f));
            seq.Append(popup.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));

            if (gemCount != null)
            {
                gemCount.text = "Current Gem Count: " + GameManager.Instance.getBags();
            }

            if (tap.activeSelf == true)
            {
                quickShopBtn1.SetActive(false);
                quickShopBtnFree.SetActive(true);
                quickShopBtn11.SetActive(false);
                quickShopBtnFree1.SetActive(true);
                GameManager.Instance.tutorialManager.quickTravelTutorialCB("open");
            }
            else
            {
                quickShopBtn1.SetActive(true);
                quickShopBtnFree.SetActive(false);
                quickShopBtn11.SetActive(true);
                quickShopBtnFree1.SetActive(false);
            }
        }

        public void timeTravelBuyButtonPressed(string button)
        {
            int cost = 50;
            int hours = 4;
            int index = 0;

            if (button == "CellFree")
            {
                cost = 0;
                hours = 1;
                index = 0;
                GameManager.Instance.tutorialManager.quickTravelTutorialCB("done");
            }
            else if (button == "Cell1")
            {
                cost = 50;
                hours = 4;
                index = 0;
                AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.QuickTravel_4);
            }
            else if (button == "Cell2")
            {
                cost = 150;
                hours = 24;
                index = 1;
                AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.QuickTravel_24);
            }
            else if (button == "Cell3")
            {
                cost = 350;
                hours = 72;
                index = 2;
                AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.QuickTravel_72);
            }

            if (GameManager.Instance.getBags() < cost)
            {
                GameManager.Instance.ShowToast("Not Enough Gems");
                closeBtn();
                return;
            }

            double timeTravelEarning = GameManager.Instance.getEarning() * hours * 3600 / 7;
            GameManager.Instance.congratsHud.openPanel("YOU GOT " + GameUtils.currencyToString(timeTravelEarning)
                                                                  + " CASH FROM TIME TRAVEL.", timeTravelEarning, index);

            GameManager.Instance.addBags(-cost);
            closeBtn();
        }
    }
}