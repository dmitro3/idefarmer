using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Data.Scripts
{
    [Serializable]
    public class BridgeData
    {
        public double cost;
        public double timeInHours;
        public bool isBought;
        public bool isProgressComplete;
        public double remainingTime;
        public string timeString;
        public double gemCost;
    }

    public class UnlockBridge : MonoBehaviour {

        public GameObject unlockButton;
        public List<Vector2> positions;
        public List<BridgeData> bridgeInfo;

        Transform lockedArea;
        Transform unlockedArea;
        Slider slider;
        Text timer;
        Text gemCostTxt;
        Button watchVideoBtn;
        Button buyWithGemBtn;
    
        void Start ()
        {
            lockedArea = unlockButton.transform.Find("locked");
            unlockedArea = unlockButton.transform.Find("unlocked");
            slider = unlockedArea.Find("Slider").GetComponent<Slider>();
            timer = slider.transform.Find("time").GetComponent<Text>();
            watchVideoBtn = unlockedArea.Find("watchVideoBtn").GetComponent<Button>();
            buyWithGemBtn = unlockedArea.Find("BuyWithGem").GetComponent<Button>();
            gemCostTxt = buyWithGemBtn.transform.Find("Text").GetComponent<Text>();

            for (int i = 0; i < bridgeInfo.Count; i++)
            {
                BridgeData bridgeData = bridgeInfo[i];
                if (bridgeData.isBought && bridgeData.isProgressComplete)
                {
                    GameManager.Instance.bridgeProgressComplete(i);
                }
            }
        }
    
        void Update () {
		
            for (int i = 0; i < bridgeInfo.Count; i++)
            {
                BridgeData bridgeData = bridgeInfo[i];
                if (bridgeData.isBought && !bridgeData.isProgressComplete)
                {
                    bridgeData.remainingTime -= Time.deltaTime;
                    if (bridgeData.remainingTime <= 0)
                    {
                        bridgeData.remainingTime = 0;
                        bridgeData.isProgressComplete = true;
                        unlockButton.gameObject.SetActive(false);
                        GameManager.Instance.bridgeProgressComplete(i);
                    }

                    TimeSpan ts = TimeSpan.FromSeconds(bridgeData.remainingTime);
                    timer.text = GameUtils.TimeSpanToReadableString(ts);
                    unlockedArea.gameObject.SetActive(true);
                    lockedArea.gameObject.SetActive(false);


                    float sliderValue = (float)(1 - (bridgeData.remainingTime / (bridgeData.timeInHours * 3600)));
                    slider.value = sliderValue;

                    gemCostTxt.text = bridgeData.gemCost + "";
                }
            }

            if (GameManager.Instance.adsManager.IsVideoAvailable())
            {
                watchVideoBtn.interactable = true;
            }
            else
            {
                watchVideoBtn.interactable = false;
            }
        }

        public bool isBridgeUnlocked(int plotNo)
        {
            int index = (plotNo / 5) - 1;
            return bridgeInfo[index].isProgressComplete;
        }

        public void showLockButton(int plotNo)
        {
            int index = (plotNo / 5) - 1;
            BridgeData bridgeData = bridgeInfo[index];

            if (!bridgeData.isBought)
            {
                unlockButton.SetActive(true);
                Vector3 pos = unlockButton.transform.localPosition;
                unlockButton.transform.localPosition = new Vector3(pos.x, positions[index].y, pos.z);

                Text costTxt = lockedArea.Find("costTxt").GetComponent<Text>();
                Text timeTxt = lockedArea.Find("timeTxt").GetComponent<Text>();

                unlockedArea.gameObject.SetActive(false);
                lockedArea.gameObject.SetActive(true);
                costTxt.text = GameUtils.currencyToString(bridgeData.cost);
                timeTxt.text = "Time: " + bridgeData.timeString;
            }
            else if (bridgeData.isBought && !bridgeData.isProgressComplete)
            {
                unlockButton.SetActive(true);
                Vector3 pos = unlockButton.transform.localPosition;
                unlockButton.transform.localPosition = new Vector3(pos.x, positions[index].y, pos.z);
            }
        }

        public void onBuyClick()
        {
            GameManager gm = GameManager.Instance;
            int index = (gm.factoryCount / 5) - 1;
            BridgeData bridgeData = bridgeInfo[index];

            if (bridgeData.isBought)
            {
                return;
            }

            if (gm.getCash() >= bridgeData.cost)
            {
                bridgeData.isBought = true;
                bridgeData.remainingTime = bridgeData.timeInHours * 3600;

                TimeSpan ts = TimeSpan.FromSeconds(bridgeData.remainingTime);
                timer.text = GameUtils.TimeSpanToReadableString(ts);
                unlockedArea.gameObject.SetActive(true);
                lockedArea.gameObject.SetActive(false);
            }
            else
            {
                gm.ShowToast("Not enough cash");
            }
        }

        public void saveToFile()
        {
            string postFix = GameManager.Instance.worldIndex == 0 ? "" : GameManager.Instance.worldIndex + "";
            FileStream file = File.Create(Application.persistentDataPath + "/Bridges" + postFix + ".dat");
            BinaryFormatter writer = new BinaryFormatter();
            writer.Serialize(file, bridgeInfo);
            file.Close();
        }

        public void readFromFile()
        {
            string postFix = GameManager.Instance.worldIndex == 0 ? "" : GameManager.Instance.worldIndex + "";
            if (File.Exists(Application.persistentDataPath + "/Bridges" + postFix + ".dat"))
            {
                FileStream file = File.OpenRead(Application.persistentDataPath + "/Bridges" + postFix + ".dat");
                Debug.Log(Application.persistentDataPath);
                var reader = new BinaryFormatter();
                bridgeInfo = (List<BridgeData>)reader.Deserialize(file);
                file.Close();
            }
        }

        public void recalculateTimers()
        {
            string closeTime = PlayerPrefs.GetString("closeTime", "0");
            long closeTimeInSec = Convert.ToInt64(closeTime);
            DateTime oldDate = DateTime.FromBinary(closeTimeInSec);
            DateTime currentDate = System.DateTime.Now;
            TimeSpan difference = currentDate.Subtract(oldDate);

            for (int i = 0; i < bridgeInfo.Count; i++)
            {
                BridgeData bridgeData = bridgeInfo[i];

                if (bridgeData.isBought && !bridgeData.isProgressComplete)
                {
                    float totalDiff = (float)difference.TotalSeconds;
                    bridgeData.remainingTime -= totalDiff;
                }
            }
        }

        public void showVideoAd()
        {
            SoundManager.Instance.PlayClickSound();

#if UNITY_EDITOR
            videoAdFinished();
            return;
#endif

            IronSourceManager adManager = GameManager.Instance.adsManager;
            adManager.ShowRewardedVideoButtonClicked(IronSourceManager.VideoType.UnlockBridge);
        }

        public void videoAdFinished()
        {
            for (int i = 0; i < bridgeInfo.Count; i++)
            {
                BridgeData bridgeData = bridgeInfo[i];
                if (bridgeData.isBought && !bridgeData.isProgressComplete)
                {
                    bridgeData.remainingTime -= 30 * 60;
                }
            }
        }

        public void buyWithGemClicked()
        {
            for (int i = 0; i < bridgeInfo.Count; i++)
            {
                BridgeData bridgeData = bridgeInfo[i];
                if (bridgeData.isBought && !bridgeData.isProgressComplete)
                {
                    if (GameManager.Instance.getBags() >= bridgeData.gemCost)
                    {
                        bridgeData.remainingTime = 0;
                    }
                    else
                    {
                        GameManager.Instance.ShowToast("Not Enough Gems");
                    }
                }
            }
        }
    }
}