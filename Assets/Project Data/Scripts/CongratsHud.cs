using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Data.Scripts
{
    public class CongratsHud : MonoBehaviour
    {
        public Text congratsTxt;
        public GameObject coinEffect;
        public Image bg;
        public Sprite [] bgs;

        double rewardAmount;

        public void closeBtn()
        {
            SoundManager.Instance.PlayCloseHudSound();
            GameObject popup = transform.Find("bg").gameObject;
            popup.transform.localScale = Vector3.one;

            Sequence seq = DOTween.Sequence();
            seq.Append(popup.transform.DOScale(Vector3.zero, 0.2f));
            seq.AppendCallback(()=> gameObject.SetActive(false));
        }

        public void closeBtnForAdBoost()
        {
            SoundManager.Instance.PlayCloseHudSound();
            GameObject popup = transform.Find("bg").gameObject;
            popup.transform.localScale = Vector3.one;

            Sequence seq = DOTween.Sequence();
            seq.Append(popup.transform.DOScale(Vector3.zero, 0.2f));
            seq.AppendCallback(()=> gameObject.SetActive(false));

            GameManager.Instance.boostProfitsPopup.OpenPanel();
        }

        public void openPanel(string congratsMsg, double reward, int index)
        {
            SoundManager.Instance.PlayOpenHudSound();
            gameObject.SetActive(true);

            bg.overrideSprite = bgs[index];

            GameObject popup = transform.Find("bg").gameObject;
            popup.transform.localScale = Vector3.zero;

            Sequence seq = DOTween.Sequence();
            seq.Append(popup.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f));
            seq.Append(popup.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));

            congratsTxt.text = congratsMsg;
            rewardAmount = reward;
        }

        public void collectButtonPressed()
        {
            GameManager.Instance.addCash(rewardAmount);
            SoundManager.Instance.PlayGetCoinsSound();
            GameObject particle = Instantiate(coinEffect, transform.position, Quaternion.identity);
            particle.GetComponent<CoinsEffect>().PlayEffect(GameManager.Instance.balanceTonText.transform);
            closeBtn();
        }


        public void openPanelForAdBoost(float hours)
        {
            SoundManager.Instance.PlayOpenHudSound();
            gameObject.SetActive(true);

            GameObject popup = transform.Find("bg").gameObject;
            popup.transform.localScale = Vector3.zero;

            Sequence seq = DOTween.Sequence();
            seq.Append(popup.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f));
            seq.Append(popup.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));

            congratsTxt.text = "Nice! We doubled our income for " + Mathf.Round(hours) + " hours";
        }
    }
}