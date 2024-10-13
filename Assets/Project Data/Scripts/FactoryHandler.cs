using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DG.Tweening;
using Org.BouncyCastle.Math.EC.Multiplier;
using TonSdk.Core.Boc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Project_Data.Scripts
{
    public class WorkerData
    {
        public GameObject worker;
        public bool isWorking;
        public bool isIdleAnim;
        public bool isWalkAnim;
        public bool isLoadBagAnim;
        public bool isLoadAnim;
        public bool isWorkAnim;
        public bool isWalkLeftAnim;
    }

    //Cách farm hoạt động
    public class FactoryHandler : MonoBehaviour
    {
        public int index; //Chỉ số
        public GameObject workerObject; //Người làm việc
        public GameObject managerObject;
        public GameObject collecterObject;//Đối tượng thu hoạch
        public GameObject destination;
        public Text levelText; //Hiển thị level
        public Image upgradeArrow; //Nút mũi tên lên cấp 
        public GameObject workerParent;

        public Text profitsText; //Hiện profit khi cừu cho lông

        //public Button bootPowerUpBtn;
        //public Button miningPowerUpBtn;
        //public Text   powerUpTimerText;
        public ParticleSystem particleSystem; //Hiệu ứng farm

        public Button levelUpBtn; //Nút lên cấp farm
        public Button addManagerBtn; //Nút thêm người quản lý
        public Image fireSprite;
        public Image currencyImage;  //Hình ảnh hiện tại 

        public Text plotIndexTxt; //Danh dấu chỉ số farm ở đương đi
    
        public Sprite[] walkSprites;
        public Sprite[] workSprites;

        public Sprite[] boyBagSprites;
        public Sprite[] boyBagWalkSprites;

        //public Sprite[] girlBagSprites;
        //public Sprite[] girlBagWalkSprites;

        //public Sprite[] girlWalkSprites;
        //public Sprite[] girlWorkSprites;
        [Header("Hình ảnh manager")]
        public Sprite[] managerSprites;
        [Header("Hình ảnh bóng của manager")]
        public Sprite[] addManagerSprites;

        public Sprite[] wheelBarrowSprites;
        public Sprite[] fireAnim1Sprites;
        public Sprite[] fireAnim1SteadySprites;
        public Sprite[] fireAnim2Sprites;
        public Sprite[] fireAnim2SteadySprites;

        [Header("Hình ảnh farm")]
        public Sprite[] plotBg; 
        [Header("Hình ảnh nơi thu hoạch sản phẩm")]
        public Sprite[] wheelBarrow; //Danh sách hình ảnh xe thu hoach hoac chỗ thu hoạch
        public Texture2D[] particleTexture; //Textture cho hiệu ứng
        [Header("Danh sách con cừu")]
        List<WorkerData> workersList = new List<WorkerData>();

        [Header("Vị trí tạo lông cừu")]
        public GameObject productParents;

        [Header("Prefabs san pham")]
        public GameObject prefabProduct;
        [Header("hình anh cừu đang ăn")]
        public Image sheepCurrent;
        public Sprite[] sheepAnimSprites;
        public Text framBalanceText;
        float walkAnimationSpeed = 0.1f;
        float workAnimationSpeed = 0.1f;
        float loadAnimationSpeed = 0.1f;

        float workerMoveSpeed = 8f;
        float workerWorkTime  = 2f;
        float workerDefaultWorkTime  = 2f;
        bool  wheelBarrowAnim;

        bool hasManager;

        double profits;

        int level = 1;
        double baseCost = 10;
        double baseEarning = 20.2;
        double cost = 10;
        double earning = 20.2;

        double exp1 = 1.16;
        double exp2 = 1.148;
        double exp3 = 1.1;
        double earningExp = 1.1;

        double baseBonusMultiplier = 1.5;

        ManagerInfo managerInfo;

        bool startDone;
        Vector2 size = Vector2.zero;
        public ObjectPooler objectPooler;

        public bool Shepherd;
        public double UpgradePrice;
        private double baseHashrate = 0.2142857143;
        private double basePrice = 1.20;
        private double currentBalance = 0.0;
        private List<GameObject> instances = new List<GameObject>(); // Danh sách lưu trữ các đối tượng đã được tạo

        void Start ()
        {
            baseHashrate = 0.2142857143;
            basePrice = 1.20;
            workersList = new List<WorkerData>();
            int worldIndex = GameManager.Instance.worldIndex; //Lấy bản đồ hiện tại
            //Debug.Log("Map hien tai " + worldIndex);

            //Set hình ảnh  giao diện
            GetComponent<Image>().sprite = plotBg[worldIndex];
            //Set hình ảnh nơi thu hoạch sản phẩm
            //collecterObject.GetComponent<Image>().sprite = wheelBarrow[1];

            //Set textture cho mình anh3
            particleSystem.GetComponent<Renderer>().material.mainTexture = particleTexture[worldIndex];

            Texture2D tex = particleTexture[worldIndex];
            //Set hình ảnh hiện tại farm
            currencyImage.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            //Kiếm kinh nghiệm
            earningExp += worldIndex * .05;



            workerParent = gameObject;
            //Lấy dữ liệu từ data
            //GameManager.Data data = GameManager.Instance.dataList[index - 1];
            //baseCost = data.cost;
            //baseEarning = data.earning * earningExp;
            //exp2 = exp2 - (0.002 * (index - 1));
            //exp3 = exp3 - (0.002 * (index - 1));



            //Tính giá trị kiếm tiền

            calculateNewCostAndEarning();

            levelText.text = "Level\n" + (level);
            profitsText.text = "" + GameUtils.currencyToString(profits);

            //Khởi tạo người làm việc
            size = workerObject.GetComponent<RectTransform>().sizeDelta;
            if (index % 2 == 0)
            {
                size.x = size.x - size.x * 0.1f;
                size.y = size.y - size.y * 0.1f;
            }
            
            for (int i = 0; i < 3; i++)
            {
                GameObject worker = Instantiate(workerObject, workerParent.transform);
                worker.name = "W" + i.ToString();
                Vector2 position = worker.transform.localPosition;

                if (i != 0)
                {
                    position = new Vector2(position.x, position.y + 60 * i);
                    worker.transform.localPosition = position;
                }

                worker.SetActive(true);

                WorkerData workerData = new WorkerData();
                workerData.worker = worker;
                workerData.isIdleAnim = true;
                ////Thêm 1 con cừu
                workersList.Add(workerData);
                StartCoroutine(PlayAnim(workerData));
            }
          
            

            for (int i = 0; i < workersList.Count; i++)
            {
                GameObject worker1 = workersList[i].worker;
                worker1.GetComponent<Image>().sprite =  walkSprites[0] ;
                worker1.GetComponent<Image>().SetNativeSize();
            }


            if (managerInfo == null)
            {
                //Debug.Log("managerInfo is null");
                //Set hình ảnh manager khi mua
                managerObject.GetComponent<Image>().overrideSprite = managerSprites[(index % 4) * 8];
                //Set bóng của manager lúc chưa mua
                addManagerBtn.transform.Find("Image").GetComponent<Image>().overrideSprite = addManagerSprites[index % 4];
            }
            else
            {
                //Set hình ảnh manager khi mua
                managerObject.GetComponent<Image>().overrideSprite = managerSprites[managerInfo.iconIndex * 8];
                //Set bóng của manager lúc chưa mua
                addManagerBtn.transform.Find("Image").GetComponent<Image>().overrideSprite = addManagerSprites[managerInfo.iconIndex];
            }
            //Người quan lý chuyển động
            StartCoroutine("managerAnimation");
            //Tác vụ người quản lý
            managerPopupCloseCB();

            startDone = true;
            //Nếu chưa haon2 thanh hướng dẫn thì ẩn đi nút thêm manager và lên cấp
            //if (!GameManager.Instance.tutorialManager.isTutorialCompleted())
            //{
            //    levelUpBtn.gameObject.SetActive(false);
            //    addManagerBtn.gameObject.SetActive(false);
            //}
            //ẩn nút thêm mananger
            //addManagerBtn.gameObject.SetActive(false);

            //Nếu hoàn tất và có tiền thì nâng cấp
            //if (GameManager.Instance.tutorialManager.isTutorialCompleted() &&
            //    GameManager.Instance.getCash() >= GameManager.Instance.managerSelection.getManagerCost(BuldingType.Mine))
            //{
            //    addManagerBtn.gameObject.SetActive(true);
            //}
            ////Bật nút tăng cấp farm
            //if (GameManager.Instance.tutorialManager.checkCurrentStepDone(TutorialStep.OpenLevelUpPopup))
            //{
            //    levelUpBtn.gameObject.SetActive(true);
            //}
            //Debug.Log("level level " + level);
            //if(level >= GameManager.MAX_LEVEL)
            //{
                
            //    upgradeArrow.gameObject.SetActive(false);
            //}

            //Chỉ số từng farm để đánh dấu thứ tự
            plotIndexTxt.text = index + "";
            //Load so du balcae
            LoadBanlanceOneFarm();

            //Nút mũi tên lên cấp 
            Sequence seq = DOTween.Sequence();
            seq.Append(upgradeArrow.transform.DOScale(0.8f, 0.75f)).SetEase(Ease.Linear);
            seq.Append(upgradeArrow.transform.DOScale(1.1f, 0.75f)).SetEase(Ease.Linear);
            seq.SetLoops(-1);
        }

        void LoadBanlanceOneFarm()
        {
            currentBalance = 0.0;
          
            UserSheepFarm userSheepFarm= UserDataManager.Instance.GetOneSheepFarm(index);
          //  UserConveyor userConveyor = UserDataManager.Instance.UserData.userConveyor[0];
            if (userSheepFarm != null){

                currentBalance = userSheepFarm.Balance;
            }
            //if (userConveyor != null)
            //{
            //    hashRateConveyor = userConveyor.HashRate;
            //}

            ////hashrate trại cừu - hashrate băng chuyền<0 băng chuyền đang tải > trại cừu, tồn kho = 0
            ////hashrate trại cừu - hashrate băng chuyền > 0 băng chuyền đang tải<trại cừu, tồn kho = hashRate chênh lệch *thời gian(note lại)
            //if (hashRateFarm - hashRateConveyor> 0){

            //    UserBalance userBalance = UserDataManager.Instance.UserData.userBalances[0];
            //    long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            //    currentBalance = (hashRateFarm - hashRateConveyor) * ((currentTime - userBalance.lastUpdate) / 1000 / 86400);
            //}
            framBalanceText.text=currentBalance.ToString("F8");
        }
        void IncreaBalance()
        {
          //  Debug.Log("coib Tang ba");
            UserSheepFarm userSheepFarm = UserDataManager.Instance.GetOneSheepFarm(index);
            UserBalance userBalance = UserDataManager.Instance.UserData.userBalances[0];
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            double addBalance = userSheepFarm.HashRate / 86400 * 2;
            currentBalance += addBalance;
            framBalanceText.text = currentBalance.ToString("F8");
        }

        void DeIncreaBalance()
        {

            UserConveyor userConveyor = UserDataManager.Instance.UserData.userConveyor[0];
            UserBalance userBalance = UserDataManager.Instance.UserData.userBalances[0];
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            double addBalance = userConveyor.HashRate  / 86400 * 2;

            currentBalance -= addBalance;
            if (currentBalance < 0)
                currentBalance = 0;

            framBalanceText.text = currentBalance.ToString("F8");
        }

        public void setWorkerWorkSpeed(float speed)
        {
            workerWorkTime = speed;
            workerDefaultWorkTime = speed;
        }

        void Update()
        {
            if (managerInfo != null && managerInfo.isPowerUp)
            {
                if (managerInfo.timerCountdown <= 0)
                {
                    workerMoveSpeed = 2f;
                    workerWorkTime  = workerDefaultWorkTime;
                    walkAnimationSpeed  = 0.1f;
                    workAnimationSpeed  = 0.1f;
                }

                TimeSpan ts = TimeSpan.FromSeconds(managerInfo.timerCountdown);
                //powerUpTimerText.text = GameUtils.TimeSpanToReadableString1(ts);
            }
            if (managerInfo != null &&  managerInfo.isPowerUpCooldown)
            {
                if (managerInfo.timerCooldown <= 0)
                {
                    workerMoveSpeed = 2f;
                    workerWorkTime  = workerDefaultWorkTime;
                    walkAnimationSpeed  = 0.1f;
                    workAnimationSpeed  = 0.1f;

                    setManagerType();
                    //powerUpTimerText.gameObject.SetActive(false);
                }

                TimeSpan ts = TimeSpan.FromSeconds(managerInfo.timerCooldown);
                //powerUpTimerText.text = GameUtils.TimeSpanToReadableString1(ts);
            
                managerObject.GetComponent<Image>().color = new Color(0.39f, 0.39f, 0.39f, 0.82f);
            }

            if (managerInfo == null || (managerInfo != null && !managerInfo.isPowerUp))
            {
                workerMoveSpeed = 2f;
                workerWorkTime  = workerDefaultWorkTime;
                walkAnimationSpeed  = 0.1f;
                workAnimationSpeed  = 0.1f;

                if (managerInfo != null && !managerInfo.isPowerUpCooldown)
                {
                    managerObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                }
            }

            //if (GameManager.Instance.getCash() >= cost && level < GameManager.MAX_LEVEL)
            //{
            //    upgradeArrow.gameObject.SetActive(true);

            //}
            //else
            //{
            //    upgradeArrow.gameObject.SetActive(false);
            //}

            //if (!hasManager && ((GameManager.Instance.tutorialManager.isTutorialCompleted() &&
            //                     GameManager.Instance.getCash() >= GameManager.Instance.managerSelection.getManagerCost(BuldingType.Mine)) ||
            //                    GameManager.Instance.managerSelection.getManagersCount(BuldingType.Mine) > 0))
            //{
            //    addManagerBtn.gameObject.SetActive(true);
            //}
            //else
            //{
            //    addManagerBtn.gameObject.SetActive(false);
            //}
        }

        public void moveWorkerToWorkUnit()
        {
            //Debug.Log("Con cừu di chuyển");
            
            if (!hasManager)
            {
                SoundManager.Instance.PlayTapWorkerSound();
            }
            GameManager.Instance.tutorialManager.hideSteps();

            for (int j = 0; j < workersList.Count; j++)
            {
                WorkerData workerData = workersList[j];
                //Hàm di chuyển
                StartCoroutine(moveWorkerToWorkUnit1(workerData, j * 1.5f));
            }
        }
        //Hàm di chuyển con cừu
        public IEnumerator moveWorkerToWorkUnit1(WorkerData workerData, float yieldTime)
        {

            {
                GameObject worker1 = workerData.worker;
                if (workerData.isWorking)
                    yield break;

                if (!startDone)
                {
                    yield return 0;
                }

                workerData.isWorking = true;
                yield return new WaitForSeconds(yieldTime);

                workerData.isIdleAnim = false;
                workerData.isWalkAnim = true;
                workerData.isWorkAnim = false;
                workerData.isLoadBagAnim = false;
                workerData.isLoadAnim = false;


                float startPosX = workerObject.transform.localPosition.x;
                float destPoint = destination.transform.localPosition.x;
                float workerMoveSpeed1 = workerMoveSpeed;
                //Debug.Log("Đến nơi uống nước");
                Tween destTween;
                {
                    destTween = worker1.transform.DOLocalMoveX(destPoint, workerMoveSpeed1).SetEase(Ease.Linear);
                }
                yield return destTween.WaitForCompletion();

                //Debug.Log("uống nước uông nước");
                workerData.isIdleAnim = false;
                workerData.isWalkAnim = false;
                workerData.isWorkAnim = true;
                workerData.isLoadBagAnim = false;
                workerData.isLoadAnim = false;

                {
                    Slider slider = worker1.transform.Find("Slider").GetComponent<Slider>();
                    slider.DOValue(1, workerWorkTime);
                }
                particleSystem.Play();
                yield return new WaitForSeconds(workerWorkTime);

                //Debug.Log("Quay lai den noi nguoi quan lý");

                workerData.isIdleAnim = false;
                workerData.isWalkAnim = false;
                workerData.isWorkAnim = false;
                workerData.isLoadBagAnim = false;
                workerData.isLoadAnim = true;

                Tween startTween;
                {
                    worker1.transform.rotation = Quaternion.Euler(0, 0, 0);
                    startTween = worker1.transform.DOLocalMoveX(startPosX, workerMoveSpeed1).SetEase(Ease.Linear);
                }
                yield return startTween.WaitForCompletion();

                //Debug.Log("Tăng tiền");

                IncreaBalance();

                {
                    Slider slider = worker1.transform.Find("Slider").GetComponent<Slider>();
                    slider.DOValue(0, 0.01f);
                    profits += earning;
                }
                SoundManager.Instance.PlayFarmerDropWheatSound();

                workerData.isWorking = false;
                workerData.isIdleAnim = !hasManager;
                workerData.isWalkAnim = false;
                workerData.isWorkAnim = false;
                workerData.isLoadBagAnim = false;
                workerData.isLoadAnim = false;

            
                GetCoin(worker1, workerData);

               
            } 
        }
        //Hàm xử lý
        public void GetCoin(GameObject worker, WorkerData workerData)
        {
            worker.SetActive(false);
            sheepCurrent.gameObject.SetActive(true);
            StartCoroutine(AnimationSheep());
            StartCoroutine(GetCoinsAnimation(worker, workerData, index));

        }

        IEnumerator  AnimationSheep()
        {
            while (true)
            {
                for (int j = 0; j < 10; j++)
                {
                    sheepCurrent.overrideSprite = sheepAnimSprites[j];
                    yield return new WaitForSeconds(.15f);
                }
            }
        }

        public IEnumerator GetCoinsAnimation(GameObject worker, WorkerData workerData,int index)
        {
            yield return new WaitForSeconds(0.5f);
            //Giam tiền bên farm
            subtractProfitsCollected(20);
            //Giam balances
            DeIncreaBalance();
            //Tạo lông cừu và di chuyển xuống băng chuyền
            //GameObject pro = Instantiate(prefabProduct, productParents.transform);


            GameObject pro = objectPooler.GetPooledObject();
            pro.transform.localPosition = Vector3.zero;

            // Thiết lập điều khiển cho quả bóng (chỉ định Object Pool và hướng di chuyển)
            BallController ballController = pro.GetComponent<BallController>();
            ballController.Initialize(objectPooler);

            Sequence seq = DOTween.Sequence();
            seq.Append(pro.transform.DOLocalMoveY(-90f, 0.5f)).OnComplete(() => {

                //objectPooler.ReturnObjectToPool(pro);
                // GameManager.Instance.liftHandler.collectProduct();
                //Xuống băng chuyền rùi di chuyển đến kho
                collectProduct(pro, index);
            }); ;


          
           // GameManager.Instance.liftHandler.collectProduct(pro, index);
            //Debug.Log("LiftHandler hiện cuộn len");
            sheepCurrent.gameObject.SetActive(false);
            worker.SetActive(true);

            //Nếu có nguoi quản lý thì tự quay về
            if (hasManager)
            {
                //Hàm di chuyển
                StartCoroutine(moveWorkerToWorkUnit1(workerData, 0));
            }

            profitsText.text = "" + GameUtils.currencyToString(profits);
            //Debug.Log("TutorialManager cap nhat");
            //Khi con cu7u di chuyen xong thi mo khoa nha cuu
            GameManager.Instance.tutorialManager.updateStep(TutorialStep.UnlockFarmHouse);
        }

        //Hàm các quả tơ về kho
        void collectProduct(GameObject pro, int index)
        {

            //int positionDestination = 200 * index;
            //float time = 5 * index;
            //if (index == 1)
            //{
            //    positionDestination = 200 * index;
            //    time = 5;

            //}
            //else if (index == 2)
            //{
            //    //positionDestination = 700;
            //    //time = 4 * index;
            //    positionDestination = 700;
            //    time = 17.5f;
            //}
            //else if(index == 3)
            //{
            //    //positionDestination = 700;
            //    //time = 4 * index;
            //    positionDestination = 1200;
            //    time = 30;
            //}
            //else if(index == 4)
            //{
            //    //positionDestination = 700;
            //    //time = 4 * index;
            //    positionDestination =1700;
            //    time = 42.5f;
            //}
            //else  if (index == 5)
            //{
            //    //positionDestination = 700;
            //    //time = 4 * index;
            //    positionDestination = 2200;
            //    time = 55.0f;
            //}
           

            //else
            //{
            //    positionDestination = 700 + 500 * (index-2);
            //}


            Sequence seq = DOTween.Sequence();
            seq.Append(pro.transform.DOLocalMoveX(-90, 0.5f)).OnComplete(() => {

                pro.gameObject.transform.SetParent(GameManager.Instance.parrfentLift.transform);
                float distance = Vector3.Distance(GameManager.Instance.destination.transform.localPosition, pro.transform.localPosition);


                float moveDuration = distance /50f;

                pro.GetComponent<BallController>().isActive = true;
                //Debug.Log("distance " + distance);
                 gotoTarget(pro, GameManager.Instance.destination.transform.localPosition.y, moveDuration);
            });

         


        }
        void gotoTarget(GameObject pro, float positionDestination, float time)
        {
            // Tính toán hướng di chuyển từ vị trí hiện tại đến điểm đích
            Vector2 direction = Vector2.up;

            pro.GetComponent<Rigidbody2D>().velocity = direction * 0.5f;
            // Áp dụng vận tốc theo hướng đã tính toán
          

            //float delay = index* 0.5f;
            //Sequence seq = DOTween.Sequence();
            //seq.Append(pro.transform.DOLocalMoveY(positionDestination, time))
            //      .SetEase(Ease.Linear)   

            //      // Tốc độ đều đặn (không thay đổi gia tốc)
            //    // Áp dụng độ trễ để tạo hiệu ứng "đồng bộ hóa"
            //.SetLoops(-1, LoopType.Restart) // Lặp lại mãi mãi (restart từ đầu)
            //    .OnComplete(() =>
            //{

              //  objectPooler.ReturnObjectToPool(pro);

            //    GameManager.Instance.liftHandler.collectProduct();
            //});
        }
        public double getProfitsAndClear()
        {
            return profits;
        }

        public void subtractProfitsCollected(double collected)
        {
            profits -= collected;
            if (profits < 0)
            {
                profitsText.text = "0";
            }
            else if (profits < 1)
            {
                profitsText.text = "0";
                profitsText.text = "" + GameUtils.currencyToString(profits);
            }
            else
            {
                profitsText.text = "" + GameUtils.currencyToString(profits);
            }
        }
        //Mua
        public void addManager()
        {
            SoundManager.Instance.PlayClickSound();
            GameManager.Instance.hireManager .openPopup(index, BuldingType.Mine);

           
        }
        public void BuyManager()
        {

            GameManager.Instance.tonConnectWallet.DepositHiredSheep(index);
            GameManager.Instance.tutorialManager.updateStep(TutorialStep.OpenLevelUpPopup);
            //GameManager.Instance.buyManager.BuyManagerSheep(index, () =>
            //{
            //    GameManager.Instance.hireManager.ClosePopup();
            //    hasManager = true;
            //    addManagerBtn.gameObject.SetActive(false);
            //    managerObject.SetActive(true);
            //});
        }


        public void levelUpBtnPressed()
        {


            //if (level >= GameManager.MAX_LEVEL)
            //{
            //    GameManager.Instance.ShowToast("You have reached max level");
            //}


            //double cc = 0;


            //double newEarning = 0;
            //double newHarvestSpeed = 0;
            //int newWorkerCount = 0;
            //double harvestSpeed = earning / workerWorkTime;

            //double bonusMul = Math.Pow(baseBonusMultiplier, GameUtils.getUpgradeCount(level) + 1);

            //Tinh theo cong thuc o day
            // double hashCurrentRate = 0, hashNextRate = 0;

            //double currenthashRate = 0, newthashRate = 0, priceTon = 0;
            //Debug.Log("index hien tai " + index);
            //Debug.Log("level hien tai " + level);
            //double basePricetmp = basePrice;
            //double baseHashratetmp = baseHashrate;
            //if (index > 1)
            //{
            //    basePricetmp = basePricetmp * Math.Pow(1.25, index - 1);
            //    baseHashratetmp = baseHashrate * Math.Pow(1.25, index - 1);
            //}

            //if (level == 0)
            //{
            //    currenthashRate = 0.018;
            //    priceTon = basePricetmp;
            //    newthashRate = baseHashratetmp;
            //}
            //else if (level == 1)
            //{
            //    currenthashRate = baseHashratetmp;
            //    newthashRate = baseHashratetmp * Math.Pow(1.5, ((level + 1) - 1));
            //    priceTon = basePricetmp * Math.Pow(1.5, ((level + 1) - 1));
            //}
            //else
            //{
            //    currenthashRate = baseHashratetmp * Math.Pow(1.5, (level - 1)); ;
            //    newthashRate = baseHashratetmp * Math.Pow(1.5, ((level + 1) - 1));
            //    priceTon = basePricetmp * Math.Pow(1.5, ((level + 1) - 1));
            //}
            int multiplier = getMaxMultiplier();
            //Debug.Log("index farm " + index);
            //Debug.Log("index farm getMaxMultiplier " + multiplier); 
            double roiBase = 142.8, rateRoi = 2.5;
            double currenthashRate = 0, newthashRate = 0, upgradePrice = 0, roi = 0.0, newroi = 0.0, currenthashRateSheep = 0.0, newhashRateSheep = 0.0;
            if (UserDataManager.Instance.UserData.userSheepFarms != null)
            {
                if (UserDataManager.Instance.UserData.userSheepFarms.Count > 0)
                {
                    UserSheepFarm userConveyor = UserDataManager.Instance.GetOneSheepFarm(index);
                    if (index == 1)
                    {
                        roi = roiBase + level * rateRoi;
                        if (multiplier == 1)
                        {
                            newroi = roiBase + (level+1) * rateRoi;

                        }
                        else if (multiplier == 3)
                        {
                            newroi = roiBase + (level + 3) * rateRoi;

                        }
                        else if (multiplier == 5)
                        {
                            newroi = roiBase + (level + 5) * rateRoi;
                        }

                    }
                    else
                    {
                        roi = roiBase + (level-1) * rateRoi;
                        if (multiplier == 1)
                        {
                            newroi = roiBase + (level + 1-1) * rateRoi;

                        }
                        else if (multiplier == 3)
                        {
                            newroi = roiBase + (level + 3-1) * rateRoi;

                        }
                        else if (multiplier == 5)
                        {
                            newroi = roiBase + (level + 5-1) * rateRoi;
                        }

                    }

                    if (multiplier == 1)
                    {

                       
                        currenthashRate = userConveyor.HashRate;
                        upgradePrice = userConveyor.UpgradePrice;
                        newthashRate = userConveyor.HashRate1Lv;
                        currenthashRateSheep = userConveyor.SheepHashRate;
                        newhashRateSheep = userConveyor.SheepHashRate1Lv;
                    }
                    else if (multiplier == 3)
                    {
                        currenthashRate = userConveyor.HashRate;
                        upgradePrice = userConveyor.Upgrade3LvPrice;
                        newthashRate = userConveyor.HashRate3Lv;
                        currenthashRateSheep = userConveyor.SheepHashRate;
                        newhashRateSheep = userConveyor.SheepHashRate3Lv;

                    }
                    else if (multiplier == 5)
                    {
                        currenthashRate = userConveyor.HashRate;
                        upgradePrice = userConveyor.Upgrade5LvPrice;
                        newthashRate = userConveyor.HashRate5Lv;
                        currenthashRateSheep = userConveyor.SheepHashRate;
                        newhashRateSheep = userConveyor.SheepHashRate5Lv;
                    }

                }
            }
            double bonusMul = 0;
            double newEarning = 0;
            double newHarvestSpeed = 0;
            int newWorkerCount = 0;
            double harvestSpeed = earning / workerWorkTime;

            GameManager.Instance.factoryLevelPopup.showMinePopup(index, upgradePrice, currenthashRate, newthashRate,
                getEarningPerSec(), workersList.Count, workerMoveSpeed, harvestSpeed, earning,
                newEarning, newWorkerCount, 0, newHarvestSpeed - harvestSpeed, getIncrementedEarning() - earning,
                0, level, multiplier, bonusMul, "Mine", roi, newroi, currenthashRateSheep, newhashRateSheep);

            GameManager.Instance.tutorialManager.updateStep(TutorialStep.BuyUpgrade);
        }
        public IEnumerator UpgrapeFarm(int index, Action onComplete = null)
        {
            // Thực hiện công việc nâng cấp và chờ trong một khoảng thời gian giả định (thay thế bằng quá trình thực tế)
            yield return new WaitForSeconds(2.0f); // Ví dụ thời gian nâng cấp

            // Khi hoàn thành, gọi callback
            onComplete?.Invoke();
        }
        public void levelUp(double cc)
        {

            SoundManager.Instance.PlayLevelUpSound();
            GameManager.Instance.tonConnectWallet.UpgradeFarm(index, cc);

            levelText.text = "Level\n" + (level);

            //if (level >= 25 && workersList.Count < 2)
            //{
            //    GameObject worker1 = GameObject.Instantiate(workerObject, workerParent.transform);
            //    worker1.SetActive(true);

            //    WorkerData workerData1 = new WorkerData();
            //    workerData1.worker = worker1;
            //    workerData1.isIdleAnim = true;
            //    workersList.Add(workerData1);

            //    worker1.GetComponent<Image>().sprite = index % 2 == 1 ? walkSprites[0] : walkSprites[0];
            //    worker1.GetComponent<Image>().SetNativeSize();
            //    StartCoroutine(PlayAnim(workerData1));
            //    if (hasManager)
            //    {
            //        //Hàm di chuyển
            //        StartCoroutine(moveWorkerToWorkUnit1(workerData1, 1f));
            //    }
            //}
            //if (level >= 50 && workersList.Count < 3)
            //{
            //    GameObject worker1 = GameObject.Instantiate(workerObject, workerParent.transform);
            //    worker1.SetActive(true);

            //    WorkerData workerData1 = new WorkerData();
            //    workerData1.worker = worker1;
            //    workerData1.isIdleAnim = true;
            //    workersList.Add(workerData1);

            //    worker1.GetComponent<Image>().sprite = walkSprites[0];
            //    worker1.GetComponent<Image>().SetNativeSize();
            //    StartCoroutine(PlayAnim(workerData1));
            //    if (hasManager)
            //    {
            //        //Hàm di chuyển
            //        StartCoroutine(moveWorkerToWorkUnit1(workerData1, 1.5f));
            //    }
            //}

            //levelUpBtnPressed();
            GameManager.Instance.tutorialManager.tutorialCompleted();
        }

        public void calculateNewCostAndEarning()
        {
            if (level > 99)
            {
                cost = baseCost * Math.Pow(exp1, 19) * Math.Pow(exp2, 80) * Math.Pow(exp3, level - 99);
            }
            else if (level > 19)
            {
                cost = baseCost * Math.Pow(exp1, 19) * Math.Pow(exp2, level - 19);
            }
            else
            {
                cost = baseCost * Math.Pow(exp1, level);
            }

            double bonusMul = Math.Pow(baseBonusMultiplier, GameUtils.getUpgradeCount(level));
            earning = baseEarning * Math.Pow(earningExp, level - 1) * bonusMul;
        }

        public double getEarningPerSec()
        {
            double earnPerSec = earning / (workerWorkTime + (workerMoveSpeed * 2));
            return earnPerSec;
        }

        public double getIncrementedEarningPerSec()
        {
            int maxMul = getMaxMultiplier();
            if (maxMul == 0)
            {
                return getEarningPerSec();
            }

            double bonusMul = Math.Pow(baseBonusMultiplier, GameUtils.getUpgradeCount(level + maxMul - 1));
            double earningNew = baseEarning * Math.Pow(earningExp, level + maxMul - 1) * bonusMul;
            double earnPerSec = earningNew / (workerWorkTime + (workerMoveSpeed * 2));
            return earnPerSec;
        }

        public double getIncrementedEarning()
        {
            int maxMul = getMaxMultiplier();
            if (maxMul == 0)
            {
                return earning;
            }

            double bonusMul = Math.Pow(baseBonusMultiplier, GameUtils.getUpgradeCount(level + maxMul - 1));
            double earningNew = baseEarning * Math.Pow(earningExp, level + maxMul - 1) * bonusMul;
            return earningNew;
        }

        public void saveData()
        {
            string postFix = GameManager.Instance.worldIndex == 0 ? "" : GameManager.Instance.worldIndex + "";
            PlayerPrefs.SetString("Factory" + index + "profits" + postFix, profits.ToString());
            PlayerPrefs.SetString("Factory" + index + "level" + postFix, level.ToString());
            PlayerPrefs.SetInt("Factory" + index + "hasManager" + postFix, hasManager ? 1 : 0);
        }

        public void getData(int index)
        {
            this.index = index;
            string postFix = GameManager.Instance.worldIndex == 0 ? "" : GameManager.Instance.worldIndex + "";
            string profitStr = PlayerPrefs.GetString("Factory" + index + "profits" + postFix, "0");
            string levelStr  = PlayerPrefs.GetString("Factory" + index + "level" + postFix, "1");

            profits = double.Parse(profitStr);
            level = int.Parse(levelStr);
            //if (level > GameManager.MAX_LEVEL)
            //{
            //    level = GameManager.MAX_LEVEL;
            //}
        }

        public void getDataFarmTonData(int sheepFarmId)
        {

            this.index = sheepFarmId;

            UserSheepFarm userSheepFarm = UserDataManager.Instance.UserData.userSheepFarms.Where(n => n.SheepFarmId == sheepFarmId).FirstOrDefault();

            if (userSheepFarm != null)
            {
                level = userSheepFarm.Level;
            }

            // string profitStr = PlayerPrefs.GetString("Factory" + index + "profits" + postFix, "0");
            //string levelStr = PlayerPrefs.GetString("Factory" + index + "level" + postFix, "1");

            profits = 0;
            ////level = int.Parse(levelStr);
            //if (level > GameManager.MAX_LEVEL)
            //{
            //    level = GameManager.MAX_LEVEL;
            //}
        }
        public void getDataFarmTonServer(int sheepFarmId)
        {

            this.index = sheepFarmId;

            UserSheepFarm userSheepFarm = UserDataManager.Instance.UserData.userSheepFarms.Where(n => n.SheepFarmId == sheepFarmId).FirstOrDefault();

            if (userSheepFarm != null)
            {
                level = userSheepFarm.Level;
                Shepherd = userSheepFarm.Shepherd;
                UpgradePrice = userSheepFarm.UpgradePrice;
            }


            // string profitStr = PlayerPrefs.GetString("Factory" + index + "profits" + postFix, "0");
            //string levelStr = PlayerPrefs.GetString("Factory" + index + "level" + postFix, "1");

            profits = 0;
            //if (level > GameManager.MAX_LEVEL)
            //{
            //    level = GameManager.MAX_LEVEL;
            //}
        }

        public IEnumerator managerAnimation()
        {
            while (true)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (managerInfo != null && managerInfo.isPowerUp)
                    {
                        fireSprite.color = GameManager.Instance.managerColors[managerInfo.iconIndex];
                        fireSprite.gameObject.SetActive(true);
                        {
                            if (managerInfo.iconIndex == 3)
                            {
                                fireSprite.GetComponent<Image>().overrideSprite = fireAnim2Sprites[j];
                            }
                            else
                            {
                                fireSprite.GetComponent<Image>().overrideSprite = fireAnim1Sprites[j];
                            }
                        }
                    }
                    else
                    {
                        fireSprite.gameObject.SetActive(false);
                    }

                    if (managerInfo != null)
                    {
                        managerObject.GetComponent<Image>().overrideSprite = managerSprites[(managerInfo.iconIndex * 8) + j];
                    }
                    yield return new WaitForSeconds(.15f);
                }

                if (managerInfo != null && managerInfo.isPowerUp)
                {
                    {
                        for (int i = 0; i < 6 && managerInfo != null && managerInfo.isPowerUp; i++)
                        {
                            if (managerInfo.iconIndex == 3)
                            {
                                fireSprite.GetComponent<Image>().overrideSprite = fireAnim2SteadySprites[i];
                            }
                            else
                            {
                                fireSprite.GetComponent<Image>().overrideSprite = fireAnim1SteadySprites[i];
                            }
                            yield return new WaitForSeconds(.15f);
                        }
                    }
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                }
            }
        }
    
        public IEnumerator PlayAnim(WorkerData workerData)
        {
            while (true)
            {
                GameObject worker1 = workerData.worker;
                worker1.GetComponent<Image>().transform.rotation = Quaternion.Euler(0, 0, 0);
                if (workerData.isIdleAnim)
                {
                    GameObject slider = worker1.transform.Find("Slider").gameObject;
                    slider.SetActive(false);
                   
                    worker1.GetComponent<Image>().sprite = walkSprites[0];
                    worker1.GetComponent<Image>().SetNativeSize();
                    yield return new WaitForSeconds(walkAnimationSpeed);
                }
                else if (workerData.isWalkAnim)
                {
                    GameObject slider = worker1.transform.Find("Slider").gameObject;
                    slider.SetActive(false);

                    for (int j = 0; j < walkSprites.Length && workerData.isWalkAnim; j++)
                    {
                        worker1.GetComponent<Image>().sprite =  walkSprites[j] ;
                        worker1.GetComponent<Image>().SetNativeSize();
                        yield return new WaitForSeconds(walkAnimationSpeed);
                    }
                }
                else if (workerData.isLoadBagAnim)
                {
                    GameObject slider = worker1.transform.Find("Slider").gameObject;
                    slider.SetActive(false);

                    for (int j = 0; j < boyBagWalkSprites.Length && workerData.isLoadBagAnim; j++)
                    {
                        worker1.GetComponent<Image>().sprite =  boyBagSprites[j] ;
                        worker1.GetComponent<Image>().SetNativeSize();
                        yield return new WaitForSeconds(loadAnimationSpeed);
                    }
                    workerData.isLoadBagAnim = false;
                }
                else if (workerData.isLoadAnim)
                {
                    GameObject slider = worker1.transform.Find("Slider").gameObject;
                    slider.SetActive(false);
                    worker1.GetComponent<Image>().transform.rotation = Quaternion.Euler(0, 180, 0);
                    for (int j = 0; j < boyBagWalkSprites.Length && workerData.isLoadAnim; j++)
                    {
                        worker1.GetComponent<Image>().sprite =  boyBagWalkSprites[j] ;
                        worker1.GetComponent<Image>().SetNativeSize();
                        yield return new WaitForSeconds(loadAnimationSpeed);
                    }
                }
                else if (workerData.isWorkAnim)
                {
                    GameObject slider = worker1.transform.Find("Slider").gameObject;
                    slider.SetActive(true);

                    for (int j = 0; j < workSprites.Length && workerData.isWorkAnim; j++)
                    {
                        worker1.GetComponent<Image>().sprite =  workSprites[j] ;
                        worker1.GetComponent<Image>().SetNativeSize();
                        yield return new WaitForSeconds(workAnimationSpeed);
                    }
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        public void enablePowerUp()
        {
            if (managerInfo == null || managerInfo.isPowerUpCooldown)
            {
                return;
            }

            SoundManager.Instance.PlayClickSound();

            managerInfo.isPowerUp = true;

            if (managerInfo.effectType == EffectType.BootSpeed)
            {
                workerMoveSpeed = workerMoveSpeed / managerInfo.speed;
                walkAnimationSpeed = walkAnimationSpeed / managerInfo.speed;
            }
            else if (managerInfo.effectType == EffectType.FarmSpeed)
            {
                workerWorkTime = workerWorkTime / managerInfo.speed;
                workAnimationSpeed = workAnimationSpeed / managerInfo.speed;
            }

            //bootPowerUpBtn.gameObject.SetActive(false);
            //miningPowerUpBtn.gameObject.SetActive(false);
            //powerUpTimerText.gameObject.SetActive(true);
            TimeSpan ts = TimeSpan.FromSeconds(managerInfo.timerCountdown);
            //powerUpTimerText.text = GameUtils.TimeSpanToReadableString1(ts);
        }

        public void managerPopupCloseCB()
        {
            managerInfo = GameManager.Instance.managerSelection.getManagerInfo(index, BuldingType.Mine);

            if ( Shepherd || managerInfo != null)
            {
                hasManager = true;
                addManagerBtn.gameObject.SetActive(false);
                managerObject.SetActive(true);
                moveWorkerToWorkUnit();
                setManagerType();
                managerObject.GetComponent<Image>().overrideSprite = managerSprites[0];
                addManagerBtn.transform.Find("Image").GetComponent<Image>().overrideSprite = addManagerSprites[0];
            }
            else
            {
                hasManager = false;
                addManagerBtn.gameObject.SetActive(true);
                managerObject.SetActive(false);
            }
        }

        public void setManagerType()
        {
            if (managerInfo == null)
            {
                return;
            }

            workerMoveSpeed = 2f;
            workerWorkTime  = workerDefaultWorkTime;
            walkAnimationSpeed  = 0.1f;
            workAnimationSpeed  = 0.1f;

            if (managerInfo.isPowerUp)
            {
                if (managerInfo.effectType == EffectType.BootSpeed)
                {
                    workerMoveSpeed = workerMoveSpeed / managerInfo.speed;
                    walkAnimationSpeed = walkAnimationSpeed / managerInfo.speed;
                }
                else if (managerInfo.effectType == EffectType.FarmSpeed)
                {
                    workerWorkTime = workerWorkTime / managerInfo.speed;
                    workAnimationSpeed = workAnimationSpeed / managerInfo.speed;
                }
            }

            if (managerInfo.isPowerUp || managerInfo.isPowerUpCooldown)
            {
                //bootPowerUpBtn.gameObject.SetActive(false);
                //miningPowerUpBtn.gameObject.SetActive(false);
                //powerUpTimerText.gameObject.SetActive(true);
                TimeSpan ts = TimeSpan.FromSeconds(managerInfo.isPowerUp ? managerInfo.timerCountdown : managerInfo.timerCooldown);
               // powerUpTimerText.text = GameUtils.TimeSpanToReadableString1(ts);
                return;
            }

            //if (managerInfo.effectType == EffectType.BootSpeed)
            //{
            //    bootPowerUpBtn.gameObject.SetActive(true);
            //    miningPowerUpBtn.gameObject.SetActive(false);
            //    powerUpTimerText.gameObject.SetActive(false);
            //}
            //else
            //{
            //    bootPowerUpBtn.gameObject.SetActive(false);
            //    miningPowerUpBtn.gameObject.SetActive(true);
            //    powerUpTimerText.gameObject.SetActive(false);
            //}
        }

        public void openChooseManagerPanel()
        {
            SoundManager.Instance.PlayClickSound();
            GameManager.Instance.managerSelection.openPopup(index, BuldingType.Mine);
        }

        public bool hasManagerEnabled()
        {
            return hasManager;
        }

        public void tutorialCallback(TutorialStep step)
        {
            if (step == TutorialStep.CloseUpgrade)
            {
                if (!hasManager &&
                    GameManager.Instance.getCash() >= GameManager.Instance.managerSelection.getManagerCost(BuldingType.Mine))
                {
                    addManagerBtn.gameObject.SetActive(true);
                }
                else
                {
                    addManagerBtn.gameObject.SetActive(false);
                }
            }
            else if (step == TutorialStep.HireManager)
            {
                if (!hasManager)
                {
                    addManagerBtn.gameObject.SetActive(true);
                }
                else
                {
                    addManagerBtn.gameObject.SetActive(false);
                }
               
            }
            
            else if (step == TutorialStep.OpenLevelUpPopup)
            {
                levelUpBtn.gameObject.SetActive(true);
            }
        }

        public bool isOnWork()
        {
            for (int i = 0; i < workersList.Count; i++)
            {
                WorkerData workerData = workersList[i];
                if(workerData.isWorking)
                    return true;
            }

            return false;
        }

        int getMaxMultiplier()
        {
            //if (level >= GameManager.MAX_LEVEL)
            //{
            //    return 0;
            //}

            if (GameManager.Instance.multiplier != -1)
            {
                //if (GameManager.Instance.multiplier + level > GameManager.MAX_LEVEL)
                //{
                //    return GameManager.MAX_LEVEL - level;
                //}

                return GameManager.Instance.multiplier;
            }

            int multiplier = 1;

            int ll = level;
            double cc = 0;

            //for (int i = 0; ll <= GameManager.MAX_LEVEL ; i++)
            //{
            //    double cp = 0;
            //    if (ll > 99)
            //    {
            //        cp = baseCost * Math.Pow(exp1, 19) * Math.Pow(exp2, 80) * Math.Pow(exp3, ll - 99);
            //    }
            //    else if (ll > 19)
            //    {
            //        cp = baseCost * Math.Pow(exp1, 19) * Math.Pow(exp2, ll - 19);
            //    }
            //    else
            //    {
            //        cp = baseCost * Math.Pow(exp1, ll);
            //    }
            //    ll++;
            //    cc += cp;
            //    multiplier = i;
            //    if (cc > GameManager.Instance.getCash())
            //    {
            //        multiplier = i;
            //        break;
            //    } 
            
            //    if (cc == GameManager.Instance.getCash())
            //    {
            //        multiplier = i + 1;
            //        break;
            //    }
            //}
            multiplier = multiplier == 0 ? 1 : multiplier;
            return multiplier;
        }

        public void playWheelBarrowAnim(bool isWheelBarrowAnim)
        {
            wheelBarrowAnim = isWheelBarrowAnim;
        }
    }
}