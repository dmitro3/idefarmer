using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
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

        public Button bootPowerUpBtn;
        public Button miningPowerUpBtn;
        public Text   powerUpTimerText;
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

        void Start ()
        {

            int worldIndex = GameManager.Instance.worldIndex; //Lấy bản đồ hiện tại
            Debug.Log("Map hien tai " + worldIndex);

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
            GameManager.Data data = GameManager.Instance.dataList[index - 1];
            baseCost = data.cost;
            baseEarning = data.earning * earningExp;
            exp2 = exp2 - (0.002 * (index - 1));
            exp3 = exp3 - (0.002 * (index - 1));



            //Tính giá trị kiếm tiền

            calculateNewCostAndEarning();

            levelText.text = "Level\n" + ((level == GameManager.MAX_LEVEL) ? "MAX" : level + "");
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
          
            //Thêm con cưu
            if (level >= 25)
            {
                GameObject worker1 = Instantiate(workerObject, workerParent.transform);
                worker1.SetActive(true);

                WorkerData workerData1 = new WorkerData();
                workerData1.worker = worker1;
                workerData1.isIdleAnim = true;
                workersList.Add(workerData1);
                StartCoroutine(PlayAnim(workerData1));
            }
            if (level >= 50)
            {
                GameObject worker1 = Instantiate(workerObject, workerParent.transform);
                worker1.SetActive(true);

                WorkerData workerData1 = new WorkerData();
                workerData1.worker = worker1;
                workerData1.isIdleAnim = true;
                workersList.Add(workerData1);
                StartCoroutine(PlayAnim(workerData1));
            }

            for (int i = 0; i < workersList.Count; i++)
            {
                GameObject worker1 = workersList[i].worker;
                worker1.GetComponent<Image>().sprite =  walkSprites[0] ;
                worker1.GetComponent<Image>().SetNativeSize();
            }


            if (managerInfo == null)
            {
                Debug.Log("managerInfo is null");
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
            if (!GameManager.Instance.tutorialManager.isTutorialCompleted())
            {
                levelUpBtn.gameObject.SetActive(false);
                addManagerBtn.gameObject.SetActive(false);
            }
            //ẩn nút thêm mananger
            addManagerBtn.gameObject.SetActive(false);

            //Nếu hoàn tất và có tiền thì nâng cấp
            if (GameManager.Instance.tutorialManager.isTutorialCompleted() &&
                GameManager.Instance.getCash() >= GameManager.Instance.managerSelection.getManagerCost(BuldingType.Mine))
            {
                addManagerBtn.gameObject.SetActive(true);
            }
            //Bật nút tăng cấp farm
            if (GameManager.Instance.tutorialManager.checkCurrentStepDone(TutorialStep.OpenLevelUpPopup))
            {
                levelUpBtn.gameObject.SetActive(true);
            }

            //Chỉ số từng farm để đánh dấu thứ tự
            plotIndexTxt.text = index + "";

            //Nút mũi tên lên cấp 
            Sequence seq = DOTween.Sequence();
            seq.Append(upgradeArrow.transform.DOScale(0.8f, 0.75f)).SetEase(Ease.Linear);
            seq.Append(upgradeArrow.transform.DOScale(1.1f, 0.75f)).SetEase(Ease.Linear);
            seq.SetLoops(-1);
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
                powerUpTimerText.text = GameUtils.TimeSpanToReadableString1(ts);
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
                    powerUpTimerText.gameObject.SetActive(false);
                }

                TimeSpan ts = TimeSpan.FromSeconds(managerInfo.timerCooldown);
                powerUpTimerText.text = GameUtils.TimeSpanToReadableString1(ts);
            
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

            if (GameManager.Instance.getCash() >= cost && level < GameManager.MAX_LEVEL)
            {
                upgradeArrow.gameObject.SetActive(true);
            }
            else
            {
                upgradeArrow.gameObject.SetActive(false);
            }

            if (!hasManager && ((GameManager.Instance.tutorialManager.isTutorialCompleted() &&
                                 GameManager.Instance.getCash() >= GameManager.Instance.managerSelection.getManagerCost(BuldingType.Mine)) ||
                                GameManager.Instance.managerSelection.getManagersCount(BuldingType.Mine) > 0))
            {
                addManagerBtn.gameObject.SetActive(true);
            }
            else
            {
                addManagerBtn.gameObject.SetActive(false);
            }
        }

        public void moveWorkerToWorkUnit()
        {
            Debug.Log("Con cừu di chuyển");
            
            if (!hasManager)
            {
                SoundManager.Instance.PlayTapWorkerSound();
            }
            GameManager.Instance.tutorialManager.hideSteps();

            for (int j = 0; j < workersList.Count; j++)
            {
                WorkerData workerData = workersList[j];
                //Hàm di chuyển
                StartCoroutine(moveWorkerToWorkUnit1(workerData, j * 3.5f));
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

                Debug.Log("Quay lai den noi nguoi quan lý");

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

                Debug.Log("Tăng tiền");

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
            StartCoroutine(GetCoinsAnimation(worker, workerData,index));

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
            //Tạo lông cừu và di chuyển xuống băng chuyền
            GameObject pro = Instantiate(prefabProduct, productParents.transform);
            Sequence seq = DOTween.Sequence();
            seq.Append(pro.transform.DOLocalMoveY(-250f, 0.2f));
            //Xuống băng chuyền rùi di chuyển đến kho
            GameManager.Instance.liftHandler.collectProduct(pro, index);
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
            //Debug.Log("Factory  tutorialManager");
            //Hàm cap nhat cac buoc
            Debug.Log("Tutorail 2. Đã mở khoá farm");
            GameManager.Instance.tutorialManager.updateStep(TutorialStep.UnlockFarmHouse);
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

        public void addManager()
        {
            SoundManager.Instance.PlayClickSound();
            GameManager.Instance.managerSelection.openPopup(index, BuldingType.Mine);
            AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.ManagerHiredInPlot, index);
        }

        public void levelUpBtnPressed()
        {
            double cc = 0;
            int ll = level;
            int multiplier = getMaxMultiplier();
            for (int i = 0; i < multiplier; i++)
            {
                double cp = 0;
                if (ll > 99)
                {
                    cp = baseCost * Math.Pow(exp1, 19) * Math.Pow(exp2, 80) * Math.Pow(exp3, ll - 99);
                }
                else if (ll > 19)
                {
                    cp = baseCost * Math.Pow(exp1, 19) * Math.Pow(exp2, ll - 19);
                }
                else
                {
                    cp = baseCost * Math.Pow(exp1, ll);
                }
                ll++;
                cc += cp;
            }

            double newEarning = getIncrementedEarningPerSec() - getEarningPerSec();
            double newHarvestSpeed = getIncrementedEarning() / workerWorkTime;
            int newWorkerCount = level < 25 && ll >= 25 ? 1 : 0;
            double harvestSpeed = earning / workerWorkTime;
        
            double bonusMul = Math.Pow(baseBonusMultiplier, GameUtils.getUpgradeCount(level) + 1);

            GameManager.Instance.factoryLevelPopup.showMinePopup(index, cc, 
                getEarningPerSec(), workersList.Count, workerMoveSpeed, harvestSpeed, earning,
                newEarning, newWorkerCount, 0, newHarvestSpeed - harvestSpeed, getIncrementedEarning() - earning,
                0, level, multiplier, bonusMul, "Mine");
            Debug.Log("FactoryHanler  tutorialManager");
            GameManager.Instance.tutorialManager.updateStep(TutorialStep.BuyUpgrade);
        }

        public void levelUp(double cc)
        {
            if (GameManager.Instance.getCash() < cc)
            {
                GameManager.Instance.ShowToast("Not Enough Coins");
                return;
            }

            SoundManager.Instance.PlayLevelUpSound();
            int multiplier = getMaxMultiplier();
            for (int i = 0; i < multiplier; i++)
            {
                int nextUpgradeLevel = GameUtils.getNextUpgradeLevel(level);

                if (level + 1 == nextUpgradeLevel)
                {
                    SoundManager.Instance.PlayUpgradeSound();
                    GameManager.Instance.addBags(10);
                }
                level++;
            
                AnalyticsManager.Instance.SendPlotUpgradedEvent(index, level);
            }

            GameManager.Instance.addCash(-cc);
            calculateNewCostAndEarning();
            levelText.text = "Level\n" + ((level == GameManager.MAX_LEVEL) ? "MAX" : level+"");

            if (level >= 25 && workersList.Count < 2)
            {
                GameObject worker1 = GameObject.Instantiate(workerObject, workerParent.transform);
                worker1.SetActive(true);

                WorkerData workerData1 = new WorkerData();
                workerData1.worker = worker1;
                workerData1.isIdleAnim = true;
                workersList.Add(workerData1);

                worker1.GetComponent<Image>().sprite = index % 2 == 1 ? walkSprites[0] : walkSprites[0];
                worker1.GetComponent<Image>().SetNativeSize();
                StartCoroutine(PlayAnim(workerData1));
                if (hasManager)
                {
                    //Hàm di chuyển
                    StartCoroutine(moveWorkerToWorkUnit1(workerData1, 1f));
                }
            }
            if (level >= 50 && workersList.Count < 3)
            {
                GameObject worker1 = GameObject.Instantiate(workerObject, workerParent.transform);
                worker1.SetActive(true);

                WorkerData workerData1 = new WorkerData();
                workerData1.worker = worker1;
                workerData1.isIdleAnim = true;
                workersList.Add(workerData1);

                worker1.GetComponent<Image>().sprite = walkSprites[0];
                worker1.GetComponent<Image>().SetNativeSize();
                StartCoroutine(PlayAnim(workerData1));
                if (hasManager)
                {
                    //Hàm di chuyển
                    StartCoroutine(moveWorkerToWorkUnit1(workerData1, 1.5f));
                }
            }

            levelUpBtnPressed();
            Debug.Log("Factory Handler  tutorialManager");
            GameManager.Instance.tutorialManager.updateStep(TutorialStep.CloseUpgrade);
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
            if (level > GameManager.MAX_LEVEL)
            {
                level = GameManager.MAX_LEVEL;
            }
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

            bootPowerUpBtn.gameObject.SetActive(false);
            miningPowerUpBtn.gameObject.SetActive(false);
            powerUpTimerText.gameObject.SetActive(true);
            TimeSpan ts = TimeSpan.FromSeconds(managerInfo.timerCountdown);
            powerUpTimerText.text = GameUtils.TimeSpanToReadableString1(ts);
        }

        public void managerPopupCloseCB()
        {
            managerInfo = GameManager.Instance.managerSelection.getManagerInfo(index, BuldingType.Mine);

            if (managerInfo != null)
            {
                hasManager = true;
                addManagerBtn.gameObject.SetActive(false);
                managerObject.SetActive(true);
                moveWorkerToWorkUnit();
                setManagerType();
                managerObject.GetComponent<Image>().overrideSprite = managerSprites[managerInfo.iconIndex * 8];
                addManagerBtn.transform.Find("Image").GetComponent<Image>().overrideSprite = addManagerSprites[managerInfo.iconIndex];
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
                bootPowerUpBtn.gameObject.SetActive(false);
                miningPowerUpBtn.gameObject.SetActive(false);
                powerUpTimerText.gameObject.SetActive(true);
                TimeSpan ts = TimeSpan.FromSeconds(managerInfo.isPowerUp ? managerInfo.timerCountdown : managerInfo.timerCooldown);
                powerUpTimerText.text = GameUtils.TimeSpanToReadableString1(ts);
                return;
            }

            if (managerInfo.effectType == EffectType.BootSpeed)
            {
                bootPowerUpBtn.gameObject.SetActive(true);
                miningPowerUpBtn.gameObject.SetActive(false);
                powerUpTimerText.gameObject.SetActive(false);
            }
            else
            {
                bootPowerUpBtn.gameObject.SetActive(false);
                miningPowerUpBtn.gameObject.SetActive(true);
                powerUpTimerText.gameObject.SetActive(false);
            }
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
            if (level >= GameManager.MAX_LEVEL)
            {
                return 0;
            }

            if (GameManager.Instance.multiplier != -1)
            {
                if (GameManager.Instance.multiplier + level > GameManager.MAX_LEVEL)
                {
                    return GameManager.MAX_LEVEL - level;
                }

                return GameManager.Instance.multiplier;
            }

            int multiplier = 1;

            int ll = level;
            double cc = 0;

            for (int i = 0; ll <= GameManager.MAX_LEVEL ; i++)
            {
                double cp = 0;
                if (ll > 99)
                {
                    cp = baseCost * Math.Pow(exp1, 19) * Math.Pow(exp2, 80) * Math.Pow(exp3, ll - 99);
                }
                else if (ll > 19)
                {
                    cp = baseCost * Math.Pow(exp1, 19) * Math.Pow(exp2, ll - 19);
                }
                else
                {
                    cp = baseCost * Math.Pow(exp1, ll);
                }
                ll++;
                cc += cp;
                multiplier = i;
                if (cc > GameManager.Instance.getCash())
                {
                    multiplier = i;
                    break;
                } 
            
                if (cc == GameManager.Instance.getCash())
                {
                    multiplier = i + 1;
                    break;
                }
            }
            multiplier = multiplier == 0 ? 1 : multiplier;
            return multiplier;
        }

        public void playWheelBarrowAnim(bool isWheelBarrowAnim)
        {
            wheelBarrowAnim = isWheelBarrowAnim;
        }
    }
}