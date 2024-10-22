using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
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
            public float speed;
        }

        [Header("balanceTon Text")]
        public Text balanceTonText; //Text hiển thị Total Gold

        [Header("balanceSheepCoin Text")]
        public Text balanceSheepCoinText;  //Text hiển thị Coins/Sec

        [Header(" userHashRate Text")]
        public Text userHashRateText;//Text hiển thị Gem

        [Header("Scroll View")]
        public ScrollRect scrollView; // scrollView toàn bộ game

        public GameObject factoryPrefab;  //Prefab để tạo khu vuec chăn nuôi
        public GameObject addFactoryBtn; //Nút thêm khu vực chăn nuôi

        // public GameObject unlockWithBags; //Nút mở khoá các gói
        public GameObject toast; //Hộp thoại báo lỗi

        public GameObject faileWithdrawToast; //Hộp thoại báo lỗi
        public TextMeshProUGUI txtToast; //Hộp thoại báo lỗi
        public GameObject ScrollUpBtn; //Nút kéo lên xuống
        public GameObject whatsNewObj;  //Bảng thông báo tin mới

        public GameObject x2Profits; //Tuỳ chọn tăng diem gấp đôi
        public Text x2ProfitsLabel;  // Text trong tuỳ chọn tăng diem gấp đôi

        public GameObject bottomBar; //Thanh Bottom bên dưới màn hình

        public List<FactoryHandler> factoryList = new List<FactoryHandler>(); //Danh sách nhà máy
        public List<Data> dataList = new List<Data>(); //Danh sách data kiếm tiền --> Quan trọng
        public LiftHandler liftHandler;   //Quan lý người chở hàng
        public LevelPopup levelPopup; // Lên cấp kho
        public LevelPopup truckLevelPopup; // Lên cấp xe
        public LevelPopup factoryLevelPopup; // Lên cấp farm
        public CheatPanel cheatPanel; // Mẹo
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

        //  public GameObject coinEffect; // Efrect coin
        public CongratsHud congratsHud; // Popup thuong

        public CongratsHud congratsHudAdBoost; // Popup thuong
        public ShopManager quickTimeTravelHud; // Mua gói
        public UnlockBridge bridgeManager; // quanlý mở khoá cầu
        public BuyManager buyManager;

        private double totalCash = 1e3;
        private double totalBags;

        private double maxCash = 1e300;

        public static GameManager Instance;

        public double coinsWallet;  //tien tu vi

        public int factoryCount;
        public int totalFactoryCount = 5;
        public int multiplier = 1;

        public Color[] managerColors;
        public List<GameObject> bridges;
        public int worldIndex; //bản đồ vì game có cho mua bản đồ

        public string iosAppId;

        private float boostDuration;
        private bool isStartCalled;

        private int prestigeLvl = 1;

        //public static int MAX_LEVEL = 25;

        public GameObject taskPanel;
        public GameObject invitePanel;
        public GameObject wallettaskPanel;
        public HireManager hireManager;
        public TonConnectWallet tonConnectWallet;
        public GameObject parrfentLift;
        public GameObject destination;
        public WalletManager walletManager;
        public GameObject ManagerPopup;
        public GameObject LiftPopup;
        public GameObject ShopPopup;
        public GameObject FarmPopup;
        public Text PriceUpdareText;
        private List<GameObject> instances = new List<GameObject>();
        public GameObject SpecialPackagesPanel;
        public GameObject SpecialPackagesBtn;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // Debug.Log("factoryCount " + factoryCount);
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
            instances = new List<GameObject>();
            //Lấy giá trị maxlevel
            // MAX_LEVEL = PlayerPrefs.GetInt("SET_MAX_LEVEL", GameManager.MAX_LEVEL);
            //Lấy giá trị wold index
            worldIndex = PlayerPrefs.GetInt("WORLD_INDEX", 0);
            //Lấy giá trị tổng tiền
            totalCash += totalCash * worldIndex * 0.5;

            int isFisrt = PlayerPrefs.GetInt("isFisrt", 0);
            if (isFisrt > 0)
            {
                tutorialManager.tutorialCompleted();
            }

            LoadDataServer();

            //Lấy danh sách manager serevr
            // managerSelection.readFromFile();
            //Lấy danh sách cầu
            //bridgeManager.readFromFile();
            //lấy dữ liệu đã lưu server
            getDataAndLoad();
            menuHud.checkSoundOnOff();

            //isStartCalled = true;
            //Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;

            ////if (factoryCount == 3)
            ////{
            ////    tutorialManager.quickTravelTutorial();
            ////}

            //Nha hien có >=5 thì ẩn
            if (factoryCount >= totalFactoryCount)
            {
                addFactoryBtn.SetActive(false);
            }
        }

        private void Update()
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
                //Data data = dataList[factoryCount];
                UserSheepFarm userSheepFarm = UserDataManager.Instance.GetOneInAllSheepFarm(factoryCount);
                //Text costText = addFactoryBtn.transform.GetComponentInChildren<Text>();
                //costText.text = GameUtils.currencyToString(userSheepFarm.ActivePrice);

                addFactoryBtn.SetActive(true);
                //else
                //{
                //    unlockWithBags.SetActive(false);
                //}

                //if (checkIfNextBridge() && !bridgeManager.isBridgeUnlocked(factoryCount))
                //{
                //    addFactoryBtn.SetActive(false);
                //    //unlockWithBags.SetActive(false);
                //    bridgeManager.showLockButton(factoryCount);
                //}
            }

            if (scrollView.content.localPosition.y >= 500)
            {
                ScrollUpBtn.SetActive(true);
            }
            else
            {
                ScrollUpBtn.SetActive(false);
            }

            bottomBar.SetActive(true);

            //Lần đầu thì hiển thị hướng dẫn
            if (!PlayerPrefs.HasKey("IsFirstKReaches"))
            {
                Speech.Instance.ShowMessageWithIcon("Nice you finished the tutorial, here are x gems for free. You can invest them to time travel wisely");
            }

            PlayerPrefs.SetInt("IsFirstKReaches", 1);
        }

        public void ShowBalance()
        {
            double earning = UserDataManager.Instance.GetuserHashRate();
            userHashRateText.text = "" + earning.ToString("F8");
            ///Lấy  balanceTon
            double balanceTon = UserDataManager.Instance.GetBalanceTon();
            balanceTonText.text = balanceTon.ToString("F8");

            ///Lấy  balanceSheepTon
            double balanceSheepTon = UserDataManager.Instance.GetBalanceSheepCoin();
            balanceSheepCoinText.text = balanceSheepTon.ToString("F8");

            long timeToEndSpecial = UserDataManager.Instance.UserData.timeToEndSpecial;
            if (timeToEndSpecial > 0)
            {
                SpecialPackagesBtn.SetActive(true);
            }
            else
            {
                SpecialPackagesBtn.SetActive(false);
            }
        }

        public void ShowPakages()
        {
            SpecialPackagesPanel.SetActive(true);
        }

        public string GetBalanceTon()
        {
            return balanceTonText.text;
        }

        public string GetBalanceSheepCoin()
        {
            return balanceSheepCoinText.text;
        }

        /// <summary>
        /// Load data từ serever
        /// </summary>
        public void LoadDataServer()
        {
            ShowBalance();
            CreateFarm();

            PriceUpdareText.text = UserDataManager.Instance.UpgradefarmPrice().ToString();
            liftHandler.LoadData();
            wareHouseHandler.LoadShop();
        }

        public void playCoinBounceEffect()
        {
            balanceTonText.transform.parent.Find("coin").DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f);
        }

        public void addBallanceTon()
        {
            double balanceTon = UserDataManager.Instance.GetBalanceTon();
            balanceTonText.text = balanceTon.ToString("F8");
        }

        public void addBallanceSheepTon()
        {
            double balanceTon = UserDataManager.Instance.GetBalanceSheepCoin();
            balanceSheepCoinText.text = balanceTon.ToString("F8");
        }

        public void addCash(double cash)
        {
            totalCash += cash;

            if (totalCash >= maxCash)
            {
                totalCash = maxCash;
            }

            balanceTonText.text = GameUtils.currencyToString(totalCash);
        }

        public void addBags(double bags)
        {
            totalBags += bags;
            balanceSheepCoinText.text = GameUtils.currencyToString(totalBags);
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
        public void addOldFactory(bool isNew, int sheepFarmId = -1)
        {
            addNewFactory(isNew, false, sheepFarmId);
        }

        public void addFactory(bool isNew)
        {
            addNewFarm(isNew, false, -1);
        }

        public void addServerFactory()
        {
            int index = UserDataManager.Instance.GetindexFarm();
            double price = UserDataManager.Instance.UpgradefarmPrice();
            tonConnectWallet.ActiveFarm(price, index);
        }

        //public void addFactoryWithBags()
        //{
        //    addNewFactory(true, true);
        //}

        private void addNewFarm(bool isNew, bool isBag = false, int sheepFarmId = -1)
        {
            //  Debug.Log("mua dat ");
            SoundManager.Instance.PlayClickSound();
            int indexSheep = UserDataManager.Instance.GetSheepFarms().Count + 1;
            //Debug.Log("indexSheep  "+ indexSheep);
            buyManager.BuyFarm(indexSheep, () =>
            {
                //  Debug.Log("mua OK ");
                //Tạo 1 trang trại
                GameObject factory = GameObject.Instantiate(factoryPrefab, scrollView.content.Find("Factories"));
                factory.SetActive(true);
                //Lấy giá trị trang trại trước đó
                GameObject prevFactory = factoryCount == 0 ? factory : factoryList[factoryCount - 1].gameObject;
                //Lấy vi tri x,y trang trai truóc đó
                Vector3 pos = factory.transform.localPosition;
                Vector3 pPos = prevFactory.transform.localPosition;
                float farmHeight = 0;
                farmHeight = factoryCount == 0 ? 0f : 500f;

                //Debug.Log("farmHeight " + farmHeight);
                //Set lại vi trí cho farm mới
                pos = new Vector3(pos.x, pPos.y - farmHeight, pos.z);
                factory.transform.localPosition = pos;
                //Tăng số lượng farm
                factoryCount++;

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

                factory.GetComponent<FactoryHandler>().getDataFarmTonServer(indexSheep);
                factory.GetComponent<FactoryHandler>().setWorkerWorkSpeed(1.0f);

                //lưu vô danh sách nhà máy xử lý
                factoryList.Add(factory.GetComponent<FactoryHandler>());

                //Nếu mà vượt quá số lương nông trại cho phép thì ẩn 2 nút mua  farm
                if (factoryCount >= totalFactoryCount)
                {
                    addFactoryBtn.SetActive(false);
                }
            });
        }

        private void addNewFactory(bool isNew, bool isBag = false, int sheepFarmId = -1)
        {
            if (isNew)
            {
                //Bật âm thanh
                SoundManager.Instance.PlayClickSound();

                //Debug.Log("Gamemanager  tutorialManager");
                if (!tutorialManager.isTutorialCompleted())
                {
                    //  Debug.Log("TutorialManager bat dau");
                    tutorialManager.updateStep(TutorialStep.FarmerWork);
                }
            }

            //Tạo 1 trang trại
            GameObject factory = GameObject.Instantiate(factoryPrefab, scrollView.content.Find("Factories"));
            factory.SetActive(true);
            instances.Add(factory);
            //Lấy giá trị trang trại trước đó
            GameObject prevFactory = factoryCount == 0 ? factory : factoryList[factoryCount - 1].gameObject;
            //Lấy vi tri x,y trang trai truóc đó
            Vector3 pos = factory.transform.localPosition;
            Vector3 pPos = prevFactory.transform.localPosition;
            float farmHeight = 0;

            farmHeight = factoryCount == 0 ? 0f : 500f;

            //Debug.Log("farmHeight " + farmHeight);
            //Set lại vi trí cho farm mới
            pos = new Vector3(pos.x, pPos.y - farmHeight, pos.z);
            factory.transform.localPosition = pos;
            //Tăng số lượng farm
            factoryCount++;

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

            //Xac dinh index
            int indexSheep = UserDataManager.Instance.GetSheepFarms().Count + 1;
            if (isNew) //Tao moi lay tu thong tin chung
            {
                //Lấy thông tin profit level tu
                factory.GetComponent<FactoryHandler>().getDataFarmTonServer(indexSheep);
                factory.GetComponent<FactoryHandler>().setWorkerWorkSpeed(1.0f);
            }
            else //Da lu7u lay tu server
            {
                //Lấy thông tin profit level tu
                factory.GetComponent<FactoryHandler>().getDataFarmTonServer(sheepFarmId);
                factory.GetComponent<FactoryHandler>().setWorkerWorkSpeed(1.0f);
            }

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
            //Lưu
            //if (isNew)
            //{
            //    buyManager.BuyFarm(indexSheep);
            //}
        }

        private bool checkIfNextBridge()
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

        public void ShowFailToast(string message)
        {
            txtToast.text = message;
            faileWithdrawToast.SetActive(true);
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

        private void OnApplicationQuit()
        {
            // saveGame();
        }

        //void OnApplicationPause(bool pauseStatus)
        //{
        //    if (pauseStatus == true && isStartCalled)
        //    {
        //        saveGame();
        //    }
        //    else
        //    {
        //       // StartCoroutine(offlineEarningPopup.calculateOfflineEarning());
        //        recalculateBoostDuration();
        //        managerSelection.recalculateTimers();
        //        bridgeManager.recalculateTimers();
        //    }
        //}
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
            int count = UserDataManager.Instance.GetNumberSheepFarms();

            if (count == 0)
            {
                count = 1;
            }

            //Nếu chưa hoàn tất hướng dẫn thì hiện
            if (!tutorialManager.isTutorialCompleted())
            {
                // Debug.Log("1. TutorialManager bat dau tư gamemanager");
                PlayerPrefs.SetInt("isFisrt", 1);
                tutorialManager.updateStep(TutorialStep.FarmerWork);
            }

            string totalCashStr = PlayerPrefs.GetString("TotalCash" + postFix, totalCash.ToString());
            totalCash = double.Parse(totalCashStr);
            if (totalCash >= maxCash)
            {
                totalCash = maxCash;
            }

            // addCash(0);

            string totalBagStr = PlayerPrefs.GetString("TotalBags", totalBags.ToString());
            totalBags = double.Parse(totalBagStr);
            // addBags(0);

            //Tao lại farm đã mua
            //for (int i = 0; i < count; i++)
            //{
            //    addFactory(false);
            //}

            //Lây dữ liêu da lưu băng chuyền
            liftHandler.getData();
            //Lây dữ liêu da lưu băng chuyền
            wareHouseHandler.getData();
            recalculateBoostDuration();
            managerSelection.recalculateTimers();
            //bridgeManager.recalculateTimers();
        }

        private void CreateFarm()
        {
            factoryList = new List<FactoryHandler>();
            factoryCount = 0;
            try
            {
                if (instances != null && instances.Count > 0)
                {
                    // Xóa tất cả các đối tượng hiện tại
                    foreach (GameObject instance in instances)
                    {
                        if (instance != null)
                        {
                            // Hủy đối tượng nông trại
                            Destroy(instance);
                        }
                    }

                    // Xóa danh sách cũ để tạo lại danh sách mới
                    instances.Clear();
                }
                //Lây danh sach farm đã mua
                List<UserSheepFarm> userSheepFarms = UserDataManager.Instance.GetSheepFarms();
                if (userSheepFarms.Count > 0)
                {
                    foreach (UserSheepFarm item in userSheepFarms)
                    {
                        addOldFactory(false, item.SheepFarmId);
                    }
                }
            }
            catch { }
        }

        private void recalculateBoostDuration()
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

        //public void PlayExplosionEffect(Transform transform)
        //{
        //    bool particleOn = PlayerPrefs.GetInt("ParticlesOn") == 0 ? true : false;
        //    if (!particleOn)
        //    {
        //        return;
        //    }

        //    GameObject particle = GameObject.Instantiate(explosionEffect.gameObject);
        //    particle.transform.position = transform.position;
        //    Destroy(particle, 2f);
        //}

        public void PlayCoinsEffect(Transform transform)
        {
            //GameObject particle = Instantiate(coinEffect, transform.position, Quaternion.identity);
            //particle.GetComponent<CoinsEffect>().PlayEffect(balanceTonText.transform);
        }

        public void setMaxLevel(int maxLevel)
        {
            //MAX_LEVEL = maxLevel;
            PlayerPrefs.SetInt("SET_MAX_LEVEL", 1000);
            SceneManager.LoadScene(0);
        }

        //popup
        public void OpenPopUp(int type)
        {
            if (type == 0)
            {
                taskPanel.SetActive(true);
                invitePanel.SetActive(false);
                wallettaskPanel.SetActive(false);

                //btnBootom[type].GetComponent<Image>().sprite = Imagebtn[0];
                //btnBootom[type+1].GetComponent<Image>().sprite = Imagebtn[3];
                //btnBootom[type+2].GetComponent<Image>().sprite = Imagebtn[5];
                //btnBootom[type+3].GetComponent<Image>().sprite = Imagebtn[7];
            }
            else if (type == 1)
            {
                taskPanel.SetActive(false);
                invitePanel.SetActive(true);
                wallettaskPanel.SetActive(false);
                //btnBootom[type].GetComponent<Image>().sprite = Imagebtn[2];
                //btnBootom[type - 1].GetComponent<Image>().sprite = Imagebtn[1];
                //btnBootom[type + 1].GetComponent<Image>().sprite = Imagebtn[5];
                //btnBootom[type + 2].GetComponent<Image>().sprite = Imagebtn[7];
            }
            else if (type == 2)
            {
                taskPanel.SetActive(false);
                invitePanel.SetActive(false);
                wallettaskPanel.SetActive(true);
                //btnBootom[type].GetComponent<Image>().sprite = Imagebtn[4];
                //btnBootom[type - 2].GetComponent<Image>().sprite = Imagebtn[1];
                //btnBootom[type - 1].GetComponent<Image>().sprite = Imagebtn[5];
                //btnBootom[type + 1].GetComponent<Image>().sprite = Imagebtn[7];
            }
            else
            {
                taskPanel.SetActive(false);
                invitePanel.SetActive(false);
                wallettaskPanel.SetActive(false);
                //btnBootom[type].GetComponent<Image>().sprite = Imagebtn[6];
                //btnBootom[type - 3].GetComponent<Image>().sprite = Imagebtn[1];
                //btnBootom[type - 2].GetComponent<Image>().sprite = Imagebtn[5];
                //btnBootom[type -1].GetComponent<Image>().sprite = Imagebtn[7];
            }
        }
    }
}