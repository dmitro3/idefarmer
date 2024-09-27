using System;
using System.Collections.Generic;
using DG.Tweening;
using Project_Data.Scripts.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project_Data.Scripts
{
    public enum BuldingType
    {
        Mine,
        Elevator,
        WareHouse
    }

    public class GameManager : MonoBehaviour
    {
        [System.Serializable]
        public class Data
        {
            public int index;
            public double cost;
            public double bagCost;
            public double earning;
            public float  speed;
        }

        [Header(" Total Gold Text")]
        public Text cashText; //Text hiển thị Total Gold
        [Header(" Coins/Sec Text")]
        public Text bagText;  //Text hiển thị Coins/Sec
        [Header(" Gem Text")]
        public Text earnPerSecText;//Text hiển thị Gem
        [Header("Scroll View")]
        public ScrollRect scrollView; // scrollView toàn bộ game

        public GameObject factoryPrefab;  //Prefab để tạo khu vuec chăn nuôi
        public GameObject addFactoryBtn; //Nút thêm khu vực chăn nuôi
       // public GameObject unlockWithBags; //Nút mở khoá các gói
        public GameObject toast; //Hộp thoại báo lỗi
        public GameObject ScrollUpBtn; //Nút kéo lên xuống
        public GameObject whatsNewObj;  //Bảng thông báo tin mới
    
        public GameObject x2Profits; //Tuỳ chọn tăng diem gấp đôi
        public Text x2ProfitsLabel;  // Text trong tuỳ chọn tăng diem gấp đôi

        public GameObject bottomBar; //Thanh Bottom bên dưới màn hình

        public List<FactoryHandler> factoryList = new List<FactoryHandler>(); //Danh sách nhà máy 
        public List<Data> dataList = new List<Data>(); //Danh sách data kiếm tiền --> Quan trọng
        public LiftHandler liftHandler;   //Quan lý người chở hàng
        public LevelPopup  levelPopup; // Lên cấp kho
        public LevelPopup  truckLevelPopup; // Lên cấp xe
        public LevelPopup  factoryLevelPopup; // Lên cấp farm
        public CheatPanel  cheatPanel; // Mẹo
        public ShopManager cashShopManager; //Shop mua hàng
        public WareHouseHandler wareHouseHandler; // Nhà kho
        public OfflineEarningPopup offlineEarningPopup; // giao dien kiem tien offline
        public TutorialManager tutorialManager;// hướng dẫn
        public IronSourceManager adsManager;// quảng cáo
        public BoostProfits boostProfitsPopup; // Xem quang cáo có thuong
        public MenuHUD menuHud;// Cài dat
        public RateMeHUD rateMeHud; // danh gia
        public ManagerSelection managerSelection; // Quảng lý nâng cấp
        public ParticleSystem explosionEffect; // hiệu ứng nổ
        public GameObject coinEffect; // Efrect coin
        public CongratsHud congratsHud; // Popup thuong
        public CongratsHud congratsHudAdBoost; // Popup thuong
        public ShopManager quickTimeTravelHud; // Mua gói
        public UnlockBridge bridgeManager; // quanlý mở khoá cầu

        double totalCash = 1e3;
        double totalBags;

        double maxCash = 1e300;

        public static GameManager Instance;


        public int factoryCount;
        public int totalFactoryCount = 4;
        public int multiplier = 1;

        public Color[] managerColors;
        public List<GameObject> bridges;
        public int worldIndex; //bản đồ vì game có cho mua bản đồ

        public string iosAppId;

        float boostDuration;
        bool isStartCalled;

        int prestigeLvl = 1;

        public static int MAX_LEVEL = 2000;

        void Awake()
        {
            Instance = this;
        }
    
        void Start ()
        {
            //Debug.Log("Gamemanager ");
            //if (PlayerPrefs.GetInt("WhatsNew", 0) == 1)
            //{
            //    whatsNewObj.SetActive(false);
            //}
            //else
            //{
            //    whatsNewObj.SetActive(true);
            //    PlayerPrefs.SetInt("WhatsNew", 1);
            //}
            //FixScreen();

            //Lấy giá trị maxlevel
            MAX_LEVEL = PlayerPrefs.GetInt("SET_MAX_LEVEL", GameManager.MAX_LEVEL);
            //Lấy giá trị wold index
            worldIndex = PlayerPrefs.GetInt("WORLD_INDEX", 0);
            //Lấy giá trị tổng tiền
            totalCash += totalCash * worldIndex * 0.5;


            //Lấy data da luu

            //Lấy danh sách manager serevr
            managerSelection.readFromFile();
            //Lấy danh sách cầu
            //bridgeManager.readFromFile();
            //lấy dữ liệu đã lưu server
            getDataAndLoad();
            //Hiện coin ăn duoc sau khi offline
            //StartCoroutine(offlineEarningPopup.calculateOfflineEarning());

           // unlockWithBags.SetActive(false);
            menuHud.checkSoundOnOff();

            isStartCalled = true;
            Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;

            //if (factoryCount == 3)
            //{
            //    tutorialManager.quickTravelTutorial();
            //}

            if (factoryCount >= totalFactoryCount)
            {
                addFactoryBtn.SetActive(false);
            }
        }
        //void FixScreen()
        //{
        //    Debug.Log("Fix man hình");
        //    // Lấy tỷ lệ khung hình của màn hình
        //    float targetAspect = 9.0f / 16.0f;
        //    float windowAspect = (float)Screen.width / (float)Screen.height;
        //    float scaleHeight = windowAspect / targetAspect;

        //    Camera camera = Camera.main;

        //    if (scaleHeight < 1.0f)
        //    {
        //        Rect rect = camera.rect;
//}
        //        rect.width = 1.0f;
        //        rect.height = scaleHeight;
        //        rect.x = 0;
        //        rect.y = (1.0f - scaleHeight) / 2.0f;

        //        camera.rect = rect;
        //    }
        //    else
        //    {
        //        float scaleWidth = 1.0f / scaleHeight;

        //        Rect rect = camera.rect;

        //        rect.width = scaleWidth;
        //        rect.height = 1.0f;
        //        rect.x = (1.0f - scaleWidth) / 2.0f;
        //        rect.y = 0;

        //        camera.rect = rect;
        //    }
        
        void Update()
        {
            //Kiem tra quan ly
            tutorialManager.showTapSignForWorkers();
            boostDuration -= Time.deltaTime;
            if (boostDuration <= 0)
            {
                boostDuration = 0;
                x2Profits.SetActive(false);
                wareHouseHandler.enableBoost(false);
            }
            else
            {
                x2Profits.SetActive(true);
                TimeSpan ts = TimeSpan.FromSeconds(boostDuration);
                x2ProfitsLabel.text = new DateTime(ts.Ticks).ToString("HH:mm:ss");
                wareHouseHandler.enableBoost(true);
            }

            if (factoryCount < totalFactoryCount)
            {
                Data data = dataList[factoryCount];
                Text costText = addFactoryBtn.transform.GetComponentInChildren<Text>();
                costText.text = GameUtils.currencyToString(data.cost);

                if (tutorialManager.isTutorialCompleted() && managerSelection.getManagersCount(BuldingType.Mine) > 0 &&
                    managerSelection.getManagersCount(BuldingType.Elevator) > 0 &&
                    managerSelection.getManagersCount(BuldingType.WareHouse) > 0)
                {
                    addFactoryBtn.SetActive(true);
                    //unlockWithBags.SetActive(true);
                    //Text bagText = unlockWithBags.transform.Find("Cost").GetComponent<Text>();
                    //bagText.text = data.bagCost + "";
                }
                //else
                //{
                //    unlockWithBags.SetActive(false);
                //}

                if (checkIfNextBridge() && !bridgeManager.isBridgeUnlocked(factoryCount))
                {
                    addFactoryBtn.SetActive(false);
                    //unlockWithBags.SetActive(false);
                    bridgeManager.showLockButton(factoryCount);
                }
            }

            if (scrollView.content.localPosition.y >= 500)
            {
                ScrollUpBtn.SetActive(true);
            }
            else
            {
                ScrollUpBtn.SetActive(false);
            }


            if (bottomBar.activeSelf == false && tutorialManager.isTutorialCompleted() &&
                PlayerPrefs.GetInt("IsFirstKReaches", 0) == 1)
            {
                bottomBar.SetActive(true);
            }


            double earning = getEarning();
            earnPerSecText.text = "" + GameUtils.currencyToString(earning);

            //Nêu có tiền
            if (totalCash >= 1000)
            {
                //Lần đầu thì hiển thị hướng dẫn
                if (!PlayerPrefs.HasKey("IsFirstKReaches"))
                {

                    Debug.Log("Huongdan bắt đầu 0");
                    Speech.Instance.ShowMessageWithIcon("Nice you finished the tutorial, here are x gems for free. You can invest them to time travel wisely");
                }

                PlayerPrefs.SetInt("IsFirstKReaches", 1);
            }

        }

        public void playCoinBounceEffect()
        {
            cashText.transform.parent.Find("coin").DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f);
        }
	
        public void addCash(double cash)
        {
            totalCash += cash;

            if (totalCash >= maxCash)
            {
                totalCash = maxCash;
            }

            cashText.text = GameUtils.currencyToString(totalCash);
        }

        public void addBags(double bags)
        {
            totalBags += bags;
            bagText.text = GameUtils.currencyToString(totalBags);
        }

        public double getBags()
        {
            return totalBags;
        }

        public double getCash()
        {
            return totalCash;
        }

        public double getCashByWorldIndex(int index)
        {
            double cash = 1000;
            cash += cash * index * 0.5;
            string postFix = index == 0 ? "" : index + "";
            string totalCashStr = PlayerPrefs.GetString("TotalCash" + postFix, cash.ToString());
            cash = double.Parse(totalCashStr);
            return cash;
        }

        public void addCashByWorldIndex(double cash1, int index)
        {
            double cash = 1000;
            cash += cash * index * 0.5;
            string postFix = index == 0 ? "" : index + "";
            string totalCashStr = PlayerPrefs.GetString("TotalCash" + postFix, cash.ToString());
            cash = double.Parse(totalCashStr);
            cash += cash1;
            PlayerPrefs.SetString("TotalCash" + postFix, cash.ToString());

            if (worldIndex == index)
            {
                totalCash = cash;
            }
        }
        /// <summary>
        /// Thêm 1 mảnh đất mới
        /// </summary>
        /// <param name="isNew"></param>
        public void addFactory(bool isNew)
        {
            //Debug.Log("Tham manh dat mơi");
            addNewFactory(isNew, false);
        }

        public void addFactoryWithBags()
        {
            addNewFactory(true, true);
        }

        private void addNewFactory(bool isNew, bool isBag = false)
        {
            //Nếu vuot qua so nha máy cho phép thì dừng
            if (factoryCount >= totalFactoryCount)
            {
                return;
            }

            //Lấy dữ liệu : lấy giá khu đất
            Data data = dataList[factoryCount];
            if (isNew)
            {
                //Bật âm thanh
                SoundManager.Instance.PlayClickSound();
                ////Kiểm tra tiền có đủ trả
                //if (!isBag && totalCash < data.cost)
                //{
                //    ShowToast("Not Enough Coins");
                //    return;
                //}
                ////Kiểm tra coin có đủ trả
                //if (isBag && totalBags < data.bagCost)
                //{
                //    ShowToast("Not Enough Gems");
                //    return;
                //}

                //nếu ko phải gói thì tạo hiệu ứng và trừ tiền hiện có
                if (!isBag)
                {
                    PlayExplosionEffect(addFactoryBtn.transform);
                    addCash(-data.cost);
                }
                else
                {
                    //Trừ giá trị gói
                    addBags(-data.bagCost);
                }

                Debug.Log("Gamemanager  tutorialManager");
                tutorialManager.updateStep(TutorialStep.FarmerWork);

                //Kiểm tra nếu chưa hoàn thành hướng dẫn thì ẩn

                if (!tutorialManager.isTutorialCompleted())
                {
                    addFactoryBtn.SetActive(false);
                    //unlockWithBags.SetActive(false);
                }
                //Hiện đánh giá
                //if ((factoryCount + 1) > 1 && (factoryCount + 1) % 2 == 1)
                //{
                //    rateMeHud.openPopup();
                //}
            }
           
            //Tạo 1 trang trại
            GameObject factory = GameObject.Instantiate(factoryPrefab, scrollView.content.Find("Factories"));
            factory.SetActive(true);
            //Lấy giá trị trang trại trước đó
            GameObject prevFactory = factoryCount == 0 ? factory : factoryList[factoryCount - 1].gameObject;
            //Lấy vi tri x,y trang trai truóc đó
            Vector3 pos  = factory.transform.localPosition;
            Vector3 pPos = prevFactory.transform.localPosition;
            float farmHeight = 0;
            //Tính lại vị trí mới
            if (factoryCount != 0 && factoryCount % 5 == 4)
            {
                farmHeight = 200f;
            }
            else if (factoryCount != 0 && factoryCount % 5 == 0)
            {
                farmHeight = 180f + 180f;
            }
            else
            {
                farmHeight = factoryCount == 0 ? 0f : 500f;
            }
            Debug.Log("farmHeight " + farmHeight);
            //Set lại vi trí cho farm mới
            pos = new Vector3(pos.x, pPos.y - farmHeight, pos.z);
            factory.transform.localPosition = pos;
            //Tăng số lượng farm 
            factoryCount++;

            //Nếu có 3 nhà máy trở lên thì hiện gợi ý mua hàng
            //if (factoryCount == 3)
            //{
            //    tutorialManager.quickTravelTutorial();
            //}

            //Tính lại vị trí farmHeight
            if (factoryCount != 0 && factoryCount % 5 == 0)
            {
                farmHeight = 150f + 180f;
            }
            else
            {
                farmHeight = factoryCount == 0 ? 0f : 300f;
            }

            //Tính lại vị trí cho 2 nút bâm mua hàng
            addFactoryBtn.transform.localPosition = new Vector3(addFactoryBtn.transform.localPosition.x, pos.y - farmHeight, addFactoryBtn.transform.localPosition.z);
            //unlockWithBags.transform.localPosition = new Vector3(unlockWithBags.transform.localPosition.x, pos.y - farmHeight + 26f, unlockWithBags.transform.localPosition.z);
            //Lấy thông tin profit
            factory.GetComponent<FactoryHandler>().getData(data.index);
            factory.GetComponent<FactoryHandler>().setWorkerWorkSpeed(data.speed);

            //lưu vô danh sách nhà máy xử lý
            factoryList.Add(factory.GetComponent<FactoryHandler>());

            //Nếu mà vượt quá số lương nông trại cho phép thì ẩn 2 nút mua  farm
            if (factoryCount >= totalFactoryCount)
            {
                addFactoryBtn.SetActive(false);
            }

            //Phân tích game

            //if(!isBag) AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.UnlockedPlot);
            //else AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.UnlockedPlotWithGems);
        }

        bool checkIfNextBridge()
        {
            if (factoryCount != 0 && factoryCount % 5 == 0)
            {
                return true;
            }

            return false;
        }

        public void bridgeProgressComplete(int index)
        {
            bridges[index].SetActive(true);
        }

        public void ShowToast(string message, float waitInterval = 1.5f)
        {
            Text toastText = toast.transform.Find("Text").GetComponent<Text>();
            toastText.text = message;
            toast.SetActive(true);

            Sequence seq = DOTween.Sequence();
            seq.Append(toast.transform.DOScale(1f, 0.25f));
            seq.AppendInterval(waitInterval);
            seq.Append(toast.transform.DOScale(0f, 0.25f));
        }

        public void showCheatPanel()
        {
            cheatPanel.openPanel();
        }

        public double getEarning()
        {
            double earningPerSec = 0;
            double totalCashInMines = 0;
            for (int i = 0; i < factoryList.Count; i++)
            {
                FactoryHandler factory = factoryList[i];
                double cashGenerationPerSec = factory.hasManagerEnabled() ? factory.getEarningPerSec() : 0;
                totalCashInMines += cashGenerationPerSec;
            }

            if (totalCashInMines >= liftHandler.getEarningPerSec())
            {
                earningPerSec = liftHandler.getEarningPerSec();
            }
            else
            {
                earningPerSec = totalCashInMines;
            }
            return earningPerSec;
        }

        public double getEarningForInapp()
        {
            double earningPerSec = 0;
            double totalCashInMines = 0;
            for (int i = 0; i < factoryList.Count; i++)
            {
                FactoryHandler factory = factoryList[i];
                double cashGenerationPerSec = factory.getEarningPerSec();
                totalCashInMines += cashGenerationPerSec;
            }
            earningPerSec = totalCashInMines;
            return earningPerSec;
        }

        void OnApplicationQuit()
        {
            saveGame();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus == true && isStartCalled)
            {
                saveGame();
            }
            else
            {
               // StartCoroutine(offlineEarningPopup.calculateOfflineEarning());
                recalculateBoostDuration();
                managerSelection.recalculateTimers();
                bridgeManager.recalculateTimers();
            }
        }
        //Lưu lại game
        public void saveGame()
        {
            string postFix = worldIndex == 0 ? "" : worldIndex + "";
            PlayerPrefs.SetString("closeTime", System.DateTime.Now.ToBinary().ToString());
            PlayerPrefs.SetInt("FactoryCount" + postFix, factoryCount);
            PlayerPrefs.SetString("TotalCash" + postFix, totalCash.ToString());
            PlayerPrefs.SetString("TotalBags", totalBags.ToString());
            PlayerPrefs.SetFloat("BoostTime", boostDuration);

            for (int i = 0; i < factoryList.Count; i++)
            {
                FactoryHandler factory = factoryList[i];
                factory.saveData();
            }

            liftHandler.saveData();
            wareHouseHandler.saveData();
            managerSelection.saveToFile();
            bridgeManager.saveToFile();
        }

        public void getDataAndLoad()
        {
            string postFix = worldIndex == 0 ? "" : worldIndex + "";
            //Khời tạo Farm , ko có thì mặc định bằng 1
            int count = PlayerPrefs.GetInt("FactoryCount" + postFix,0);

            if (count == 0){

                count = 1;
                // tutorialManager.updateStep(TutorialStep.FarmerWork);
                if (!tutorialManager.isTutorialCompleted())
                {
                    Debug.Log("Tutorail hướng dan nguoi choi lan dau");

                    Debug.Log("Tutorail 1. Tạo nhà");
                    // addFactoryBtn.SetActive(false);
                    //unlockWithBags.SetActive(false);
                    tutorialManager.updateStep(TutorialStep.FarmerWork);
                }
              
                
            }
            string totalCashStr = PlayerPrefs.GetString("TotalCash" + postFix, totalCash.ToString());
            totalCash = double.Parse(totalCashStr);
            if (totalCash >= maxCash)
            {
                totalCash = maxCash;
            }

            addCash(0);

            string totalBagStr = PlayerPrefs.GetString("TotalBags", totalBags.ToString());
            totalBags = double.Parse(totalBagStr);
            addBags(0);


            //Tao lại farm đã mua
            for (int i = 0; i < count; i++)
            {
                addFactory(false);
            }

            //Lây dữ liêu da lưu băng chuyền
            liftHandler.getData();
            //Lây dữ liêu da lưu băng chuyền
            wareHouseHandler.getData();
            recalculateBoostDuration();
            managerSelection.recalculateTimers();
            bridgeManager.recalculateTimers();
        }

        void recalculateBoostDuration()
        {
            boostDuration = PlayerPrefs.GetFloat("BoostTime", 0);

            string closeTime = PlayerPrefs.GetString("closeTime", "0");
            long closeTimeInSec = Convert.ToInt64(closeTime);
            DateTime oldDate = DateTime.FromBinary(closeTimeInSec);
            DateTime currentDate = System.DateTime.Now;

            TimeSpan difference = currentDate.Subtract(oldDate);

            if (boostDuration - (float)difference.TotalSeconds > 0)
            {
                boostDuration = boostDuration - (float)difference.TotalSeconds;
            }
            else
            {
                boostDuration = 0;
            }
        }

        public void openCoinShop()
        {
            quickTimeTravelHud.openPanel();
        }

        public void openCashShop()
        {
            cashShopManager.openPanel();
        }

        public void boostProfitsBtnPressed()
        {
            boostProfitsPopup.OpenPanel();
        }

        public void enableBoost()
        {
            int boostAmount = 2;
            if (boostDuration >= (24 - boostAmount) * 60 * 60)
            {
                boostDuration = 24 * 60 * 60;
            }
            else
            {
                boostDuration += boostAmount * 60 * 60;
            }

            List<CustomAnalyticsEvent> events = new List<CustomAnalyticsEvent>();
            events.Add(CustomAnalyticsEvent.WatchVideo_Hours2);
            events.Add(CustomAnalyticsEvent.WatchVideo_Hours4);
            events.Add(CustomAnalyticsEvent.WatchVideo_Hours6);
            events.Add(CustomAnalyticsEvent.WatchVideo_Hours8);
            events.Add(CustomAnalyticsEvent.WatchVideo_Hours10);
            events.Add(CustomAnalyticsEvent.WatchVideo_Hours12);
            events.Add(CustomAnalyticsEvent.WatchVideo_Hours14);
            events.Add(CustomAnalyticsEvent.WatchVideo_Hours16);
            events.Add(CustomAnalyticsEvent.WatchVideo_Hours18);
            events.Add(CustomAnalyticsEvent.WatchVideo_Hours20);
            events.Add(CustomAnalyticsEvent.WatchVideo_Hours22);
            events.Add(CustomAnalyticsEvent.WatchVideo_Hours24);

            for (int i = 0; i < events.Count; i++)
            {
                if (boostDuration <= boostAmount * (i + 1) * 60 * 60)
                {
                    //Debug.Log(events[i].ToString());
                    AnalyticsManager.Instance.SendEvent(events[i]);
                    break;
                }
            }
        
            congratsHudAdBoost.openPanelForAdBoost(boostDuration / 60f / 60f);
            PlayerPrefs.SetFloat("BoostTime", boostDuration);
        }

        public float getBoostDuration()
        {
            return boostDuration;
        }

        // public void openSettings()
        // {
        //     settingPopup.openPopup();
        // }
        //
        // public void moreGamesButton()
        // {
        //     SoundManager.Instance.PlayClickSound();
        //     #if UNITY_ANDROID
        //     Application.OpenURL("http://www.google.com");
        //     #endif
        //     #if UNITY_IPHONE
        //     Application.OpenURL("");
        //     #endif
        // }

        public void moveScrollToTop()
        {
            scrollView.content.DOLocalMoveY(0, 0.1f);
        }

        public void PlayExplosionEffect(Transform transform)
        {
            bool particleOn = PlayerPrefs.GetInt("ParticlesOn") == 0 ? true : false;
            if (!particleOn)
            {
                return;
            }

            GameObject particle = GameObject.Instantiate(explosionEffect.gameObject);
            particle.transform.position = transform.position;
            Destroy(particle, 2f);
        }

        public void PlayCoinsEffect(Transform transform)
        {
            GameObject particle = Instantiate(coinEffect, transform.position, Quaternion.identity);
            particle.GetComponent<CoinsEffect>().PlayEffect(cashText.transform);
        }

        public void setMaxLevel(int maxLevel)
        {
            MAX_LEVEL = maxLevel;
            PlayerPrefs.SetInt("SET_MAX_LEVEL", GameManager.MAX_LEVEL);
            SceneManager.LoadScene(0);
        }
    }
}