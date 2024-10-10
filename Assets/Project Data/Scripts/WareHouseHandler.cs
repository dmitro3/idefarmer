using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static Project_Data.Scripts.LiftHandler;

namespace Project_Data.Scripts
{
    public class WareHouseWorkerData
    {
        public GameObject worker;
        public bool isWorking;
        public bool isWalkLeftAnim;
        public bool isWalkRightAnim;
        public double loadedAmount;
    }

    public class WareHouseHandler : MonoBehaviour
    {
        [System.Serializable]
        public class DataWareHouse
        {
            public int level;
            public double priceTon;
        }

        public GameObject workerObject;
        public GameObject workersParent;
        public GameObject destination;
       // public GameObject managerObject;
        public Text levelText;
        public Image bagImg;
        public ShineEffect shineEffect;
        public Button bootPowerUpBtn;
        public Button loadPowerUpBtn;
        public Text powerUpTimerText;
        public Image upgradeArrow;

        public Button levelupBtn;
        //public Button addManagerBtn;
        public Button unlockBtn;
        public Image fireSprite;
        public Image currencyParticle;

        public Sprite[] walkLeftSprites;
        public Sprite[] walkRightSprites;

        public Sprite[] managerSprites;
        public Sprite[] pipeAnimSprites;

        public Sprite[] fireAnimSprites;
        public Sprite[] fireAnimSteadySprites;
        public Sprite[] particles;

        public GameObject pipeObj;
        bool pipeAnim;
        bool shortPipeAnim;
        float animationTime;

        public List<DataWareHouse> dataWareHouses = new List<DataWareHouse>();
        List<WareHouseWorkerData> workersList = new List<WareHouseWorkerData>();

        double totalLoadCanBear = Double.MaxValue;
        double loadingSpeedPerSec = 1000;
        float  workerMoveSpeed = 3f;

        bool hasManager = true;

        int level1 = 0;
        double baseCost = 480;
        double baseLoad = 606;

        double cost = 480;
        double exp1 = 1.2;
        double exp2 = 1.13;
        double exp3 = 1.1;
        double earningExp = 1.229;

        float animationSpeed = 0.1f;
        bool isBoostEnable;
        ManagerInfo managerInfo;
        public int levelShop;
        private double baseHashrate = 0.1428571429;
        private double basePrice = 0.80;
        void Start ()
        {
            levelShop = 0;
            UserTruck userTruck = UserDataManager.Instance.GetOneUserTruck();
            if (userTruck != null)
            {
                levelShop = userTruck.Level;
            }

            int worldIndex = GameManager.Instance.worldIndex;
            calculateNewCostAndLoad();
            if (!hasManager)
                shineEffect.startShineEffect();

            GameObject worker = GameObject.Instantiate(workerObject, workersParent.transform);
            worker.SetActive(true);

            WareHouseWorkerData workerData = new WareHouseWorkerData();
            workerData.worker = worker;
            workersList.Add(workerData);
            StartCoroutine(PlayAnim(workerData));

            currencyParticle = worker.transform.Find("LoadText").Find("Image").GetComponent<Image>();
            currencyParticle.sprite = particles[worldIndex];
            managerPopupCloseCB();

            if (!GameManager.Instance.tutorialManager.isTutorialCompleted())
            {
                levelupBtn.gameObject.SetActive(true);
                //addManagerBtn.gameObject.SetActive(false);

                for (int i = 0; i < workersList.Count; i++)
                    workersList[i].worker.SetActive(false);
            }

            //if (GameManager.Instance.tutorialManager.isTutorialCompleted())
            //    unlockWarehouse(false);

            Sequence seq = DOTween.Sequence();
            seq.Append(upgradeArrow.transform.DOScale(0.8f, 0.75f)).SetEase(Ease.Linear);
            seq.Append(upgradeArrow.transform.DOScale(1.1f, 0.75f)).SetEase(Ease.Linear);
            seq.SetLoops(-1);

            StartCoroutine("managerAnimation");
            StartCoroutine("pipeAnimation");
        }
        public void LoadShop()
        {
            levelShop = 0;
            UserTruck userTruck = UserDataManager.Instance.GetOneUserTruck();
            if (userTruck != null)
            {
                levelShop = userTruck.Level;
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
                        fireSprite.gameObject.SetActive(true);
                        fireSprite.color = GameManager.Instance.managerColors[managerInfo.iconIndex];
                        fireSprite.GetComponent<Image>().overrideSprite = fireAnimSprites[j];
                    }
                    else
                        fireSprite.gameObject.SetActive(false);

                    //managerObject.GetComponent<Image>().overrideSprite = managerSprites[j];
                    yield return new WaitForSeconds(0.15f);
                }

                if (managerInfo != null && managerInfo.isPowerUp)
                {
                    for (int i = 0; i < 6 && managerInfo != null && managerInfo.isPowerUp; i++)
                    {
                        fireSprite.GetComponent<Image>().overrideSprite = fireAnimSteadySprites[i];
                        yield return new WaitForSeconds(0.15f);
                    }
                }
                else
                    yield return new WaitForSeconds(1f);
            }
        }
	
        void Update ()
        {
            levelText.text = "Level\n" + (levelShop + "");

            if (managerInfo != null && managerInfo.isPowerUp)
            {
                if (managerInfo.timerCountdown <= 0)
                {
                    workerMoveSpeed = 2f;
                    loadingSpeedPerSec = totalLoadCanBear / 3;
                }

                TimeSpan ts = TimeSpan.FromSeconds(managerInfo.timerCountdown);
                powerUpTimerText.text = GameUtils.TimeSpanToReadableString1(ts);
            }
            if (managerInfo != null &&  managerInfo.isPowerUpCooldown)
            {
                if (managerInfo.timerCooldown <= 0)
                {
                    workerMoveSpeed = 2f;
                    loadingSpeedPerSec = totalLoadCanBear / 3;
                    setManagerType();
                    powerUpTimerText.transform.parent.gameObject.SetActive(false);
                }

                TimeSpan ts = TimeSpan.FromSeconds(managerInfo.timerCooldown);
                powerUpTimerText.text = GameUtils.TimeSpanToReadableString1(ts);

                //managerObject.GetComponent<Image>().color = new Color(0.39f, 0.39f, 0.39f, 0.82f);
            }

            if (managerInfo == null || (managerInfo != null && !managerInfo.isPowerUp))
            {
                workerMoveSpeed = 2f;
                loadingSpeedPerSec = totalLoadCanBear / 3;

                //if (managerInfo != null && !managerInfo.isPowerUpCooldown)
                //    managerObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }

            //if (level1 < GameManager.MAX_LEVEL)
            //    upgradeArrow.gameObject.SetActive(true);
            //else
            //    upgradeArrow.gameObject.SetActive(false);

            //if (!hasManager && ((GameManager.Instance.tutorialManager.isTutorialCompleted() &&
            //                     GameManager.Instance.getCash() >= GameManager.Instance.managerSelection.getManagerCost(BuldingType.WareHouse) &&
            //                     GameManager.Instance.managerSelection.getManagersCount(BuldingType.Elevator)  > 0) || 
            //                    GameManager.Instance.managerSelection.getManagersCount(BuldingType.WareHouse) > 0))
            //{
            //    addManagerBtn.gameObject.SetActive(true);
            //}
            //else
            //{
            //    addManagerBtn.gameObject.SetActive(false);
            //}
        }

        public void collectAllProfitsBtn()
        {
            if(!hasManager)
                SoundManager.Instance.PlayTapWorkerSound();
            GameManager.Instance.tutorialManager.hideSteps();

            for (int j = 0; j < workersList.Count; j++)
            {
                WareHouseWorkerData workerData = workersList[j];
                StartCoroutine(collectAllProfits(workerData, j * 1f));
            }
        }

        public IEnumerator collectAllProfits(WareHouseWorkerData workerData, float yieldTime)
        {
           // Debug.Log("xe lay tien ");
            GameObject worker1 = workerData.worker;

            if (workerData.isWorking)
                yield break;

            workerData.isWorking = true;
            yield return new WaitForSeconds(yieldTime);

            while (GameManager.Instance.liftHandler.getProfits() <= 1)
                yield return new WaitForSeconds(0.1f);

            shineEffect.stopShineEffect();
            float startPoint = workerObject.transform.localPosition.x;
            float destPoint  = destination.transform.localPosition.x;
        
            workerData.isWalkLeftAnim  = true;
            workerData.isWalkRightAnim = false;
            float workerMoveSpeed1 = workerMoveSpeed;

            Tween destTween;
            destTween = worker1.transform.DOLocalMoveX(destPoint, workerMoveSpeed1).SetEase(Ease.Linear);

            yield return destTween.WaitForCompletion();
            workerData.isWalkLeftAnim  = false;
            workerData.isWalkRightAnim = false;

            float time;
            double profits = GameManager.Instance.liftHandler.getProfits();
            //Giam khi băng chuyền khi xe 
            GameManager.Instance.liftHandler.SubBalance();

            double remainingAmount = totalLoadCanBear - workerData.loadedAmount;
            double loadingSpeed = loadingSpeedPerSec;

            if (profits < remainingAmount)
                time = (float)(profits / loadingSpeed);
            else
                time = (float)(remainingAmount / loadingSpeed);
        
            animationTime = time;

            if (time == 0 && profits > 0)
            {
                LiftHandler liftHandler = GameManager.Instance.liftHandler;
                liftHandler.subtractProfitsCollected(profits);
                workerData.loadedAmount = profits;
                Text loadText = worker1.transform.Find("LoadText").GetComponent<Text>();
                loadText.text = "" + GameUtils.currencyToString(workerData.loadedAmount);
                Slider slider = worker1.transform.Find("Slider").GetComponent<Slider>();
                slider.value = (float)(workerData.loadedAmount / totalLoadCanBear);

                if (workerData.loadedAmount <= 0)
                    loadText.gameObject.SetActive(false);
                else
                    loadText.gameObject.SetActive(true);
            }

            while(time > 0)
            {
                float t;
                double colledted;
                if (time >= 1f)
                {
                    pipeAnim = true;
                    shortPipeAnim = false;
                    t = 1f;
                    colledted = loadingSpeed;
                }
                else
                {
                    pipeAnim = false;
                    shortPipeAnim = true;
                    t = time;
                    colledted = loadingSpeed * t;
                }

                time -= t;
                yield return new WaitForSeconds(t);
                SoundManager.Instance.PlayCollectBagsSound();
                LiftHandler liftHandler = GameManager.Instance.liftHandler;
                liftHandler.subtractProfitsCollected(colledted);
                workerData.loadedAmount += colledted;
                {
                    Text loadText = worker1.transform.Find("LoadText").GetComponent<Text>();
                    loadText.text = "" + GameUtils.currencyToString(workerData.loadedAmount);
                    Slider slider = worker1.transform.Find("Slider").GetComponent<Slider>();
                    slider.value = (float)(workerData.loadedAmount / totalLoadCanBear);

                    if (workerData.loadedAmount <= 0)
                        loadText.gameObject.SetActive(false);
                    else
                        loadText.gameObject.SetActive(true);
                }
            }
            pipeAnim = false;
            shortPipeAnim = false;
            workerData.isWalkLeftAnim  = false;
            workerData.isWalkRightAnim = true;
            Tween startTween;
            startTween = worker1.transform.DOLocalMoveX(startPoint, workerMoveSpeed1).SetEase(Ease.Linear);
        
            yield return startTween.WaitForCompletion();
            workerData.isWalkLeftAnim  = false;
            workerData.isWalkRightAnim = false;

            float unloadTime = (float)(workerData.loadedAmount / loadingSpeed);
            if (workerData.loadedAmount > 0)
            {
                Sequence seq = DOTween.Sequence();
                seq.Append(bagImg.transform.GetComponent<Image>().DOFade(1, 0.0001f));
                seq.AppendCallback(() => {SoundManager.Instance.PlayThrowBagsSound();});
                seq.Append(bagImg.transform.DOLocalMoveY(bagImg.transform.localPosition.y + 100f, 0.4f));
                seq.Append(bagImg.transform.GetComponent<Image>().DOFade(0, 0.1f));
                seq.Append(bagImg.transform.DOLocalMoveY(0, 0.0001f));
                seq.SetLoops((int)(unloadTime / 0.5f));
            }
            {
                Slider slider = worker1.transform.Find("Slider").GetComponent<Slider>();
                slider.DOValue(0, unloadTime);
            }
            yield return new WaitForSeconds(unloadTime);


            //Bat dau
           // Debug.Log("1.TutorialManager bat dau tu day");
            if (PlayerPrefs.GetInt("IsFirstDelivery", 0) == 0)
            {
                PlayerPrefs.SetInt("IsFirstDelivery", 1);
                GameManager.Instance.PlayCoinsEffect(workerObject.transform);
            }


            //Cap nhat TON
            GameManager.Instance.addBallanceTon();
            GameManager.Instance.addBallanceSheepTon();
            //GameManager.Instance.addCash(isBoostEnable ? workerData.loadedAmount * 2 : workerData.loadedAmount);

            workerData.loadedAmount = 0;
            {
                Text loadText = worker1.transform.Find("LoadText").GetComponent<Text>();
                loadText.text = "" + GameUtils.currencyToString(workerData.loadedAmount);

                if (workerData.loadedAmount <= 0)
                    loadText.gameObject.SetActive(false);
                else
                    loadText.gameObject.SetActive(true);
            }

            workerData.isWorking = false;
            shineEffect.startShineEffect();
            if (hasManager)
            {
                shineEffect.stopShineEffect();
                StartCoroutine(collectAllProfits(workerData, 0));
            }

           // GameManager.Instance.tutorialManager.updateStep(TutorialStep.OpenLevelUpPopup);

            ////bo sau
            //if (GameManager.Instance.getCash() >= 100)
            //{
            //    Debug.Log("TutorialManager bat dau");
            //    GameManager.Instance.tutorialManager.waitForNextStep(false);
            //    GameManager.Instance.tutorialManager.updateStep(TutorialStep.OpenLevelUpPopup);
            //}
            //else
            //    GameManager.Instance.tutorialManager.waitForNextStep(true);
        }

        public void addManager()
        {
            GameManager.Instance.managerSelection.openPopup(0, BuldingType.WareHouse);
        }

        public void levelUpBtnPressed()
        {
            //double cc = 0;
            //int ll = level1;

            int multiplier = getMaxMultiplier();
            //for (int i = 0; i < multiplier; i++)
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
            //}
            //Cong thuc

            double roiBase = 142.8, rateRoi = 2.5;
            double currenthashRate = 0, newthashRate = 0, upgradePrice = 0, roi = 0.0, newroi = 0.0, currenthashRateSheep = 0.0, newhashRateSheep = 0.0;
            if (UserDataManager.Instance.UserData.userTruck != null)
            {
                if (UserDataManager.Instance.UserData.userTruck.Count > 0)
                {
                    UserTruck userConveyor = UserDataManager.Instance.UserData.userTruck[0];
                    roi = roiBase + levelShop * rateRoi;

                    if (multiplier == 1)
                    {

                        upgradePrice = userConveyor.UpgradePrice;
                        currenthashRate = userConveyor.HashRate;
                        newthashRate = userConveyor.HashRate1Lv;
                       
                        newroi  = roiBase + (levelShop+1) * rateRoi;
                        currenthashRateSheep = userConveyor.SheepHashRate;
                        newhashRateSheep = userConveyor.SheepHashRate1Lv;
                    }
                    if (multiplier == 3)
                    {
                        currenthashRate = userConveyor.HashRate;
                        upgradePrice = userConveyor.Upgrade3LvPrice;
                        newthashRate = userConveyor.HashRate3Lv;
                        newroi = roiBase + (levelShop + 3) * rateRoi;
                        currenthashRateSheep = userConveyor.SheepHashRate;
                        newhashRateSheep = userConveyor.SheepHashRate3Lv;

                    }
                    else if (multiplier == 5)
                    {
                        currenthashRate = userConveyor.HashRate;
                        upgradePrice = userConveyor.Upgrade5LvPrice;
                        newthashRate = userConveyor.HashRate5Lv;
                        newroi = roiBase + (levelShop + 5) * rateRoi;
                        currenthashRateSheep = userConveyor.SheepHashRate;
                        newhashRateSheep = userConveyor.SheepHashRate5Lv;
                    }

                }
            }


            GameManager.Instance.levelPopup.showPopup(1, upgradePrice, currenthashRate, newthashRate,
                getEarningPerSec(), 1, workerMoveSpeed, loadingSpeedPerSec, totalLoadCanBear,
                0.0, 0, 0.0f, 0.0, getIncrementedEarning() - totalLoadCanBear,
                0.0f, levelShop, multiplier, "WareHouse", roi, newroi, currenthashRateSheep, newhashRateSheep);

        

            //GameManager.Instance.levelPopup.showPopup(1, priceTon, currenthashRate, newthashRate,
            //    getEarningPerSec(), /*workersList.Count*/ 1, workerMoveSpeed, loadingSpeedPerSec, totalLoadCanBear,
            //    newEarning, 0, 0, newHarvestSpeed - loadingSpeedPerSec, getIncrementedEarning() - totalLoadCanBear,
            //    0, levelShop, multiplier, "WareHouse");
        }

        public void levelUp(double cc)
        {
            

            //if (GameManager.Instance.coinsWallet < cc)
            //{
            //    GameManager.Instance.ShowToast("Not Enough Coins");
            //    return;
            //}

            SoundManager.Instance.PlayLevelUpSound();
            GameManager.Instance.tonConnectWallet.BuyTruck(cc);
            //GameManager.Instance.buyManager.UpgrapeTruck(1, () =>
            //{
            //    UserTruck userTruck = UserDataManager.Instance.GetOneUserTruck();
            //    levelShop = userTruck.Level;

            //});


            //int multiplier = getMaxMultiplier();
            //for (int i = 0; i < multiplier; i++)
            //{
            //    int nextUpgradeLevel = GameUtils.getNextUpgradeLevel(level);

            //    if (level + 1 == nextUpgradeLevel)
            //    {
            //        SoundManager.Instance.PlayUpgradeSound();
            //    }
            //    level++;

            //    if(level < 10) AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.WirehouseUpgraded, 10);
            //    else if(level < 25) AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.WirehouseUpgraded, 25);
            //    else if(level < 50) AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.WirehouseUpgraded, 50);
            //    else if(level < 100) AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.WirehouseUpgraded, 100);
            //    else if(level < 150) AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.WirehouseUpgraded, 150);
            //    else if(level < 200) AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.WirehouseUpgraded, 200);
            //}

            //GameManager.Instance.addCash(-cc);
            //calculateNewCostAndLoad();


            levelText.text = "Level\n" + ( levelShop );
            //levelUpBtnPressed();
        }

        public void calculateNewCostAndLoad()
        {
            if (level1 > 99)
            {
                cost = baseCost * Math.Pow(exp1, 19) * Math.Pow(exp2, 80) * Math.Pow(exp3, level1 - 99);
            }
            else if (level1 > 19)
            {
                cost = baseCost * Math.Pow(exp1, 19) * Math.Pow(exp2, level1 - 19);
            }
            else
            {
                cost = baseCost * Math.Pow(exp1, level1);
            }

            totalLoadCanBear = baseLoad * Math.Pow(earningExp, level1 - 1);
            loadingSpeedPerSec = totalLoadCanBear / 3;
        }

        public void saveData()
        {
            string postFix = GameManager.Instance.worldIndex == 0 ? "" : GameManager.Instance.worldIndex + "";
            PlayerPrefs.SetInt("WarehouseHasManager" + postFix, hasManager ? 1 : 0);
            PlayerPrefs.SetInt("WarehouseLevel" + postFix, level1);
        }

        public void getData()
        {
            string postFix = GameManager.Instance.worldIndex == 0 ? "" : GameManager.Instance.worldIndex + "";
            level1 = PlayerPrefs.GetInt("WarehouseLevel" + postFix, 1);

            //if (level1 > GameManager.MAX_LEVEL)
            //    level1 = GameManager.MAX_LEVEL;

            calculateNewCostAndLoad();
        }

        public IEnumerator PlayAnim(WareHouseWorkerData workerData)
        {
            GameObject worker1 = workerData.worker;
            while (true)
            {
                if (workerData.isWalkLeftAnim)
                {
                    worker1.transform.Find("Image").rotation = Quaternion.Euler(0, 180, 0);
                    for (int i = 0; i < walkLeftSprites.Length && workerData.isWalkLeftAnim; i++)
                    {
                        worker1.transform.Find("Image").GetComponent<Image>().sprite = walkLeftSprites[i];
                        yield return new WaitForSeconds(animationSpeed);
                    }
                }
                else if (workerData.isWalkRightAnim)
                {
                    worker1.transform.Find("Image").rotation = Quaternion.Euler(0, 0, 0);
                    for (int i = 0; i < walkRightSprites.Length && workerData.isWalkRightAnim; i++)
                    {
                        worker1.transform.Find("Image").GetComponent<Image>().sprite = walkRightSprites[i];
                        yield return new WaitForSeconds(animationSpeed);
                    }
                }
                else
                {
                    worker1.transform.Find("Image").rotation = Quaternion.Euler(0, 0, 0);
                    worker1.transform.Find("Image").GetComponent<Image>().sprite = walkLeftSprites[0];
                    yield return new WaitForSeconds(animationSpeed);
                }
            }
        }

        public void enablePowerUp()
        {
            if (managerInfo == null || managerInfo.isPowerUpCooldown)
                return;

            SoundManager.Instance.PlayClickSound();
            managerInfo.isPowerUp = true;

            if (managerInfo.effectType == EffectType.BootSpeed)
                workerMoveSpeed = workerMoveSpeed / managerInfo.speed;
            else if (managerInfo.effectType == EffectType.FarmSpeed)
                loadingSpeedPerSec = loadingSpeedPerSec * managerInfo.speed;

            bootPowerUpBtn.gameObject.SetActive(false);
            loadPowerUpBtn.gameObject.SetActive(false);
            powerUpTimerText.transform.parent.gameObject.SetActive(true);
            TimeSpan ts = TimeSpan.FromSeconds(managerInfo.timerCountdown);
            powerUpTimerText.text = GameUtils.TimeSpanToReadableString1(ts);
        }

        public double getEarningPerSec()
        {
            double earningPerSec = totalLoadCanBear / (3 * workerMoveSpeed);
            return earningPerSec;
        }

        public double getIncrementedEarningPerSec()
        {
            int maxMul = getMaxMultiplier();
            if (maxMul == 0)
                return getEarningPerSec();

            double earningNew = baseLoad * Math.Pow(earningExp, level1 + getMaxMultiplier() - 1);
            double earnPerSec = earningNew / (3 * workerMoveSpeed);
            return earnPerSec;
        }

        public double getIncrementedEarning()
        {
            int maxMul = getMaxMultiplier();
            if (maxMul == 0)
                return totalLoadCanBear;

            double earningNew = baseLoad * Math.Pow(earningExp, level1 + getMaxMultiplier() - 1);
            return earningNew;
        }

        public void enableBoost(bool isEnable)
        {
            isBoostEnable = isEnable;
        }

        public void setManagerType()
        {
            if (managerInfo == null)
                return;

            workerMoveSpeed = 2f;
            loadingSpeedPerSec = totalLoadCanBear / 3;

            if (managerInfo.isPowerUp)
            {
                if (managerInfo.effectType == EffectType.BootSpeed)
                    workerMoveSpeed = workerMoveSpeed / managerInfo.speed;
                else if (managerInfo.effectType == EffectType.FarmSpeed)
                    loadingSpeedPerSec = loadingSpeedPerSec * managerInfo.speed;
            }

            if (managerInfo.isPowerUp || managerInfo.isPowerUpCooldown)
            {
                bootPowerUpBtn.gameObject.SetActive(false);
                loadPowerUpBtn.gameObject.SetActive(false);
                powerUpTimerText.transform.parent.gameObject.SetActive(true);
                TimeSpan ts = TimeSpan.FromSeconds(managerInfo.isPowerUp ? managerInfo.timerCountdown : managerInfo.timerCooldown);
                powerUpTimerText.text = GameUtils.TimeSpanToReadableString1(ts);
                return;
            }

            if (managerInfo.effectType == EffectType.BootSpeed)
            {
                bootPowerUpBtn.gameObject.SetActive(true);
                loadPowerUpBtn.gameObject.SetActive(false);
                powerUpTimerText.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                bootPowerUpBtn.gameObject.SetActive(false);
                loadPowerUpBtn.gameObject.SetActive(true);
                powerUpTimerText.transform.parent.gameObject.SetActive(false);
            }
        }

        public void managerPopupCloseCB()
        {
            hasManager = true;
           // addManagerBtn.gameObject.SetActive(false);
            //managerObject.SetActive(true);
            collectAllProfitsBtn();
            setManagerType();

            //managerInfo = GameManager.Instance.managerSelection.getManagerInfo(0, BuldingType.WareHouse);
            //if (managerInfo != null)
            //{
            //    hasManager = true;
            //    addManagerBtn.gameObject.SetActive(false);
            //    managerObject.SetActive(true);
            //    collectAllProfitsBtn();
            //    setManagerType();
            //}
            //else
            //{
            //    hasManager = false;
            //    addManagerBtn.gameObject.SetActive(true);
            //    managerObject.SetActive(false);
            //}
        }

        public void unlockWarehouse(bool isButtonPressed)
        {
            //if (!GameManager.Instance.tutorialManager.checkCurrentStepDone(TutorialStep.UnlockWareHouse))
            //{
            //    GameManager.Instance.ShowToast("Unlock farm house first.");
            //    return;
            //}

            if (isButtonPressed)
            {
               // GameManager.Instance.PlayExplosionEffect(unlockBtn.transform);
                //GameManager.Instance.addCash(-100);
                SoundManager.Instance.PlayGetCoinsSound();
            }
            unlockWareHouseSettings();
            //Debug.Log("TutorialManager bat dau");
            GameManager.Instance.tutorialManager.updateStep(TutorialStep.HorseWork);
        }

        void unlockWareHouseSettings()
        {
            unlockBtn.gameObject.SetActive(false);
            gameObject.GetComponent<Button>().interactable = true;
            gameObject.GetComponent<Button>().enabled = false;
            levelupBtn.gameObject.SetActive(true);
           // addManagerBtn.gameObject.SetActive(false);
            for (int j = 0; j < workersList.Count; j++)
            {
                GameObject worker1 = workersList[j].worker;
                worker1.SetActive(true);
            }

           
        }

        public void tutorialCallback(TutorialStep step)
        {
            if (step == TutorialStep.CloseUpgrade)
            {
                //if(!hasManager)
                //    addManagerBtn.gameObject.SetActive(true);

                for (int i = 0; i < workersList.Count; i++)
                    workersList[i].worker.SetActive(true);
            }
            else if (step == TutorialStep.BuyUpgrade)
            {
                levelupBtn.gameObject.SetActive(true);
            }
            else if (step == TutorialStep.HorseWork)
            {
                unlockWareHouseSettings();
            }
            else if (step == TutorialStep.UnlockWareHouse)
            {
               // Debug.Log("Tutorail 9. Hiện nút  nâng nhà kho");
                levelupBtn.gameObject.SetActive(true);
            }
        }

        public bool isOnWork()
        {
            for (int i = 0; i < workersList.Count; i++)
            {
                WareHouseWorkerData workerData = workersList[i];
                if(workerData.isWorking)
                    return true;
            }

            return false;
        }

        int getMaxMultiplier()
        {
            //if (level1 >= GameManager.MAX_LEVEL)
            //    return 0;

            if (GameManager.Instance.multiplier != -1)
            {
                //if (GameManager.Instance.multiplier + level1 > GameManager.MAX_LEVEL)
                //    return GameManager.MAX_LEVEL - level1;
            
                return GameManager.Instance.multiplier;
            }

            return 1;

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
            //multiplier = multiplier == 0 ? 1 : multiplier;
            //return multiplier;
        }

        public IEnumerator pipeAnimation()
        {
            while (true)
            {
                if (pipeAnim || shortPipeAnim)
                {
                    if (animationTime <= 0.5f)
                    {
                        for (int j = 8; j < pipeAnimSprites.Length; j++)
                        {
                            pipeObj.GetComponent<Image>().overrideSprite = pipeAnimSprites[j];
                            yield return new WaitForSeconds(0.05f);
                        }
                    }
                    else if (animationTime <= 1.0f)
                    {
                        for (int j = 8; j < pipeAnimSprites.Length; j++)
                        {
                            pipeObj.GetComponent<Image>().overrideSprite = pipeAnimSprites[j];
                            yield return new WaitForSeconds(0.05f);
                        }
                        yield return new WaitForSeconds(0.25f);

                        for (int j = 8; j < pipeAnimSprites.Length; j++)
                        {
                            pipeObj.GetComponent<Image>().overrideSprite = pipeAnimSprites[j];
                            yield return new WaitForSeconds(0.05f);
                        }
                    }
                    else if (animationTime <= 1.5f)
                    {
                        for (int j = 0; j < pipeAnimSprites.Length; j++)
                        {
                            pipeObj.GetComponent<Image>().overrideSprite = pipeAnimSprites[j];
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                    else if (animationTime <= 2.5f)
                    {
                        for (int j = 0; j < pipeAnimSprites.Length; j++)
                        {
                            pipeObj.GetComponent<Image>().overrideSprite = pipeAnimSprites[j];
                            yield return new WaitForSeconds(0.15f);
                        }
                    }
                    else if (animationTime <= 3f)
                    {
                        for (int j = 0; j < pipeAnimSprites.Length; j++)
                        {
                            pipeObj.GetComponent<Image>().overrideSprite = pipeAnimSprites[j];
                            yield return new WaitForSeconds(0.15f);
                        }

                        for (int j = 0; j < pipeAnimSprites.Length; j++)
                        {
                            pipeObj.GetComponent<Image>().overrideSprite = pipeAnimSprites[j];
                            yield return new WaitForSeconds(0.15f);
                        }
                    }

                    pipeAnim = false;
                    shortPipeAnim = false;
                }
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}