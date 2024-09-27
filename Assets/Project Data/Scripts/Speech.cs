using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Data.Scripts
{
    //Quản lý giao tiếp
    public class Speech : SingletonBehaviour<Speech> {
    
        public Text messageText;
        public Image backgroundImg;
        public Image icon;
        public GameObject targetPos;
        public GameObject startPos;
        public GameObject speechObj;

        private bool _isOpened;
        private bool _isOpenedNoFrame;
        private bool isOneTimeBubble;

        public void ShowMessageWithIcon(string message, bool isOneTime = false)
        {

            if (message == "")
            {
                GameManager.Instance.tutorialManager.speechBubbleCloseCB();
                return;
            }

            speechObj.SetActive(false);
            messageText.text = message;

            icon.transform.localPosition = startPos.transform.localPosition;
            backgroundImg.transform.localScale = Vector3.zero;

            icon.transform.DOLocalMove(targetPos.transform.localPosition, 0.1f);
            backgroundImg.transform.DOScale(Vector3.one, 0.1f);
            isOneTimeBubble = isOneTime;
            Invoke("SetIsOpenedFlag", 0.2f);
        }

        void SetIsOpenedFlag()
        {
            _isOpened = true;
        }

        public void CloseMessageWithIcon()
        {
            _isOpened = false;
            icon.transform.DOLocalMove(startPos.transform.localPosition, 0.25f);
            backgroundImg.transform.DOScale(Vector3.zero, 0.25f);
            speechObj.SetActive(false);

            Debug.Log("Dong huong dan isOneTimeBubble "+ isOneTimeBubble);

            if(!isOneTimeBubble)
                GameManager.Instance.tutorialManager.speechBubbleCloseCB();
            else
                GameManager.Instance.tutorialManager.speechBubbleCloseCallback();
        }

        public void CloseMessageNoFrame()
        {
            _isOpenedNoFrame = false;
        }

        private void Update()
        {
            if (_isOpened)
            {
                Debug.Log("Huongdan dong CloseMessageWithIcon");
                CloseMessageWithIcon();
            }

            if (Input.anyKeyDown && _isOpenedNoFrame)
            {
                Debug.Log("Huongdan dong _isOpenedNoFrame");
                CloseMessageNoFrame();
            }
        }
    }
}