using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using Org.BouncyCastle.Math.EC.Multiplier;
using TonSdk.Core.Boc;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Data.Scripts
{
    public class LiftHandler : MonoBehaviour
    {
        [System.Serializable]
        public class DataLift
        {
            public int level;
            public double priceTon;
            public double hashRate;
        }


        public Text loadText;
        public Text profitsText;
        public GameObject managerObject;
        //public GameObject AddManagerBtn;
        public Slider fillSlider;
        public Text levelText;
        public Image bagImg;
        public Image truckBackBagsImg;
        [Header("shineEffect")]
        public ShineEffect shineEffect;
        public Button bootPowerUpBtn;
        public Button loadPowerUpBtn;
        public Text powerUpTimerText;
        [Header("upgradeArrow")]
        public Image upgradeArrow;
        [Header("sideBag")]
        public GameObject sideBag;
        public List<DataLift> dataLifts = new List<DataLift>();

        public Button unlockBtn;
        public GameObject liftHouseObj;
        [Header("levelUpBtn")]
        public GameObject levelUpBtn;

        public Image currencyParticle1; //Hình ảnh cuộn len ở ngoài kho
        public Image currencyParticle2; //Hình ảnh cuộn len  ở trong kho

        public Image fireSprite;

        public Sprite[] managerSprites;

        public Sprite[] fireAnimSprites;
        public Sprite[] fireAnimSteadySprites;
        [Header("Danh sách hiệu ứng cuộn len")]
        public Sprite[] particles;
        public Transform destinationPath;

        bool isWorking;
        bool hasManager;

        double totalLoadCanBear = 60;
        double loadingSpeedPerSec = 20;
        float  defaultMoveSpeed = 1.5f;
        float  liftMoveSpeed = 1.5f;
        double loadedAmount;
         double totalProfits { get; set; }

        public int level = 0;
        double baseCost = 480;
        double baseLoad = 606;

        double cost = 480;
        double exp1 = 1.2;
        double exp2 = 1.13;
        double exp3 = 1.1;
        double earningExp = 1.229;

        ManagerInfo managerInfo;
        public int levelLisft;
        private double baseHashrate= 0.015; //level 1
        private double basePrice = 0.01; //level 1
        private double baseHashrateLevel0 = 0.0075; //level 0

        private double baseHashratelv2 = 0.1785714286; //level 1
        private double basePricelv2 = 1.00; //level 1
        private double currentBalance = 0.0;
        public Text liftBalanceText;

        void Start ()
        {
           
            UserConveyor userConveyor = UserDataManager.Instance.GetOneUserConveyor();
            if (userConveyor != null)
            {
                levelLisft = userConveyor.Level;
            }
            
            //lấy bản đồ hiện tại
            int worldIndex = GameManager.Instance.worldIndex;
            currencyParticle1.sprite = particles[worldIndex];
            currencyParticle2.sprite = particles[worldIndex];
            //set vận tốc
            liftMoveSpeed = defaultMoveSpeed;
            //Tính toán giá trị mới
            calculateNewCostAndLoad();

            loadingSpeedPerSec = totalLoadCanBear / 3;

            //Ko có quản lý thì hieu ứng
            if (!hasManager)
            {
                shineEffect.startShineEffect();
            }

            //Thông tin quản lý --> có quản lý thì sẽ tự thu sản phẩm
            //
            managerPopupCloseCB();

            if (!GameManager.Instance.tutorialManager.isTutorialCompleted())
            {
                //levelUpBtn.gameObject.SetActive(false);
                //AddManagerBtn.gameObject.SetActive(false);
            }


            ////Nếu hoàn thành thì mở khoá xe
            if (GameManager.Instance.tutorialManager.isTutorialCompleted())
            {
                
                unlockTruck(false);
            }
            //Hàm chuyển động button
            Sequence seq = DOTween.Sequence();
            seq.Append(upgradeArrow.transform.DOScale(0.8f, 0.75f)).SetEase(Ease.Linear);
            seq.Append(upgradeArrow.transform.DOScale(1.1f, 0.75f)).SetEase(Ease.Linear);
            seq.SetLoops(-1);

            StartCoroutine("managerAnimation");
           // gameObject.SetActive(false);
        }

       public void LoadData()
        {
            UserConveyor userConveyor = UserDataManager.Instance.GetOneUserConveyor();
            if (userConveyor != null)
            {
                levelLisft = userConveyor.Level;
            }
            LoadBanlance();

        }
        void LoadBanlance()
        {
            currentBalance = 0.0;

            UserConveyor userConveyor = UserDataManager.Instance.GetOneUserConveyor();
            currentBalance = userConveyor.Balance;
            

            liftBalanceText.text = currentBalance.ToString("F8");
        }
        //Hàm chuyển động quản lý
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
                    {
                        fireSprite.gameObject.SetActive(false);
                    }

                    managerObject.GetComponent<Image>().overrideSprite = managerSprites[j];
                    yield return new WaitForSeconds(0.15f);
                }

                if (managerInfo != null && managerInfo.isPowerUp)
                {
                    {
                        for (int i = 0; i < 6 && managerInfo != null && managerInfo.isPowerUp; i++)
                        {
                            fireSprite.GetComponent<Image>().overrideSprite = fireAnimSteadySprites[i];
                            yield return new WaitForSeconds(0.15f);
                        }
                    }
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                }
            }
        }
    
        void Update ()
        {
            levelText.text = "Level\n" + ( levelLisft);

            if (managerInfo != null && managerInfo.isPowerUp)
            {
                if (managerInfo.timerCountdown <= 0)
                {
                    liftMoveSpeed = defaultMoveSpeed;
                    loadingSpeedPerSec = totalLoadCanBear / 3;
                }

                TimeSpan ts = TimeSpan.FromSeconds(managerInfo.timerCountdown);
                powerUpTimerText.text = GameUtils.TimeSpanToReadableString1(ts);
            }
            if (managerInfo != null &&  managerInfo.isPowerUpCooldown)
            {
                if (managerInfo.timerCooldown <= 0)
                {
                    liftMoveSpeed = defaultMoveSpeed;
                    loadingSpeedPerSec = totalLoadCanBear / 3;
                    setManagerType();
                    powerUpTimerText.transform.parent.gameObject.SetActive(false);
                }

                TimeSpan ts = TimeSpan.FromSeconds(managerInfo.timerCooldown);
                powerUpTimerText.text = GameUtils.TimeSpanToReadableString1(ts);

                Color c = managerObject.GetComponent<Image>().color;
                managerObject.GetComponent<Image>().color = new Color(0.39f, 0.39f, 0.39f, 0.82f);
            }

            if (managerInfo == null || (managerInfo != null && !managerInfo.isPowerUp))
            {
                liftMoveSpeed = defaultMoveSpeed;
                loadingSpeedPerSec = totalLoadCanBear / 3;

                if (managerInfo != null && !managerInfo.isPowerUpCooldown)
                {
                    Color c = managerObject.GetComponent<Image>().color;
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
            //                     GameManager.Instance.getCash() >= GameManager.Instance.managerSelection.getManagerCost(BuldingType.Elevator) &&
            //                     GameManager.Instance.managerSelection.getManagersCount(BuldingType.Mine) > 0) || 
            //                    GameManager.Instance.managerSelection.getManagersCount(BuldingType.Elevator) > 0))
            //{
            //    //AddManagerBtn.gameObject.SetActive(true);
            //}
            //else
            //{
            //    AddManagerBtn.gameObject.SetActive(false);
            //}
        }


        //Ham xử lý coin từ nhà máy
        public void collectProduct(float time)
        {


            if (GameManager.Instance.factoryList.Count == 0)
            {
                return;
            }

            if (!hasManager)
                SoundManager.Instance.PlayTapTruckSound();
            GameManager.Instance.tutorialManager.hideSteps();

            totalProfits += 20;

            if (totalProfits <= 0)
            {
                totalProfits = 0;
            }

            //Tinh luon tang tăng = hashRate băng chuyền/86400* số giây tơ mới vào kho băng chuyền

            AddBalance(time);
        }
        //Tăng bang chuyen
        void AddBalance( float time)
        {

          //  Debug.Log("thoi gian ve ");
            UserConveyor userConveyor = UserDataManager.Instance.GetOneUserConveyor();
           
            double addBalance = userConveyor.HashRate * time / 86400 ;
            currentBalance += addBalance;
            liftBalanceText.text = currentBalance.ToString("F8");
        }

        public void SubBalance()
        {

            UserTruck userTruck = UserDataManager.Instance.GetOneUserTruck();

            double addBalance = userTruck.HashRate * 3 / 86400;
            currentBalance -= addBalance;

            if (currentBalance < 0)
                currentBalance = 0;
            liftBalanceText.text = currentBalance.ToString("F8");
        }


        //Hàm thu coin
        public void collectAllProfitsBtn()
        {
            //Debug.Log("Thu coin");
            if (GameManager.Instance.factoryList.Count == 0)
            {
                return;
            }

            if(!hasManager)
                SoundManager.Instance.PlayTapTruckSound();
            GameManager.Instance.tutorialManager.hideSteps();

            StartCoroutine("collectAllProfits");
        }

        public IEnumerator collectAllProfits()
        {

           // Debug.Log("LiftHandler Xe di chuyen thu sản phẩm 1");

            if (isWorking)
                yield break;

            isWorking = true;
            //Vị trí bắt đầu
            float startPosY = transform.localPosition.y;
            //Load hình ảnh xe
            GetComponent<Image>().overrideSprite = Resources.Load("tractor-front", typeof(Sprite)) as Sprite;
            //Hiệu ứng
            shineEffect.stopShineEffect();



            double loadingSpeed = loadingSpeedPerSec;
            double totalLoad = totalLoadCanBear;

            float startPos = startPosY;

            //Debug.Log("LiftHandler factoryCount "+ GameManager.Instance.factoryCount);
            //Xe sẽ đi đến từng farm các nhà nông trại
            for (int i = 0; i < GameManager.Instance.factoryCount; i++)
            {
                float farmHeight = 200f;
                if (i != 0 && i % 5 == 4)
                {
                    farmHeight = 200f;
                }
                else if (i != 0 && i % 5 == 0)
                {
                    farmHeight = 180f + 180f;
                }
                else
                {
                    farmHeight = i == 0 ? 180f : 160f;
                }

                startPos = startPos - (farmHeight);
                //Lấy 1  farm
                FactoryHandler factory = GameManager.Instance.factoryList[i];
                SoundManager.Instance.PlayTruckMovingSound();
                //Xe Di chuyển đến vị trí
                Tween moveDownTween = transform.DOLocalMoveY(startPos, liftMoveSpeed).SetEase(Ease.Linear);
                yield return moveDownTween.WaitForCompletion();
                //Di chuyển xong mới làm hành động

                //Debug.Log("LiftHandler Xe di chuyen đến vi tri thu san phẩm");

                //Tính loading
                float time = 0f;
                double profits = GameManager.Instance.factoryList[i].getProfitsAndClear();
                double remainingAmount = totalLoad - loadedAmount;

                if (profits < remainingAmount)
                {
                    time = (float)(profits / loadingSpeed);
                }
                else
                {
                    time = (float)(remainingAmount / loadingSpeed);
                }

                while (time > 0)
                {
                    factory.playWheelBarrowAnim(true);
                    float t = 0;
                    double colledted = 0;
                    if (time >= 1f)
                    {
                        t = 1f;
                        colledted = loadingSpeed;
                    }
                    else
                    {
                        t = time;
                        colledted = loadingSpeed * t;
                        t = 0.75f;
                        time = 0.75f;
                    }

                    time -= t;
                    yield return new WaitForSeconds(t);
                    SoundManager.Instance.PlayCollectBagsSound();
                    //farm cập nhật lại
                    factory.subtractProfitsCollected(colledted);
                    loadedAmount += colledted;
                    loadText.text = "" + GameUtils.currencyToString(loadedAmount);
                    fillSlider.value = (float)(loadedAmount / totalLoad);


                    //Hiệu ứng chuyển động cuộn len
                    //Debug.Log("LiftHandler hiện cuộn len");
                    Sequence seq = DOTween.Sequence();
                    seq.Append(sideBag.transform.GetComponent<Image>().DOFade(1, 0.0001f));
                    seq.Append(sideBag.transform.DOLocalMoveX(140f, 0.0001f));
                    seq.Append(sideBag.transform.DOLocalMoveX(0, 0.2f));
                    seq.Append(sideBag.transform.GetComponent<Image>().DOFade(0, 0.1f));
                }

                double profits1 = GameManager.Instance.factoryList[i].getProfitsAndClear();
                double remainingAmount1 = totalLoad - loadedAmount;


                if (profits1 > 0 && remainingAmount1 >= profits1)
                {
                    factory.subtractProfitsCollected(profits1);
                    loadedAmount += profits1;
                    loadText.text = "" + GameUtils.currencyToString(loadedAmount);
                    fillSlider.value = (float)(loadedAmount / totalLoad);
                }

                factory.playWheelBarrowAnim(false);
                if (loadedAmount >= totalLoad || fillSlider.value >= 0.999f)
                    break;
            }

            //Xe di chuyển về kho
            GetComponent<Image>().overrideSprite = Resources.Load("tractor-back", typeof(Sprite)) as Sprite;
            truckBackBagsImg.gameObject.SetActive(true);

            SoundManager.Instance.PlayTruckMovingSound();
            Tween moveUpTween = transform.DOLocalMoveY(startPosY, liftMoveSpeed).SetEase(Ease.Linear);
            yield return moveUpTween.WaitForCompletion();

            float unloadTime = (float)(loadedAmount / loadingSpeed);

            if (loadedAmount > 0)
            {
                Sequence seq = DOTween.Sequence();
                seq.Append(bagImg.transform.GetComponent<Image>().DOFade(1, 0.0001f));
                seq.AppendCallback(() => { SoundManager.Instance.PlayThrowBagsSound(); });
                seq.Append(bagImg.transform.DOLocalMoveY(bagImg.transform.localPosition.y + 100f, 0.4f));
                seq.Append(bagImg.transform.GetComponent<Image>().DOFade(0, 0.1f));
                seq.Append(bagImg.transform.DOLocalMoveY(0, 0.0001f));
                seq.SetLoops((int)(unloadTime / 0.5f));
            }

            fillSlider.DOValue(0, unloadTime);
            yield return new WaitForSeconds(unloadTime);

            truckBackBagsImg.gameObject.SetActive(false);

            totalProfits += loadedAmount;

            if (totalProfits <= 0)
            {
                totalProfits = 0;
            }

            profitsText.text = "" + GameUtils.currencyToString(totalProfits);
            loadText.text = "0";

            loadedAmount = 0;
            isWorking = false;
            shineEffect.startShineEffect();
            if (hasManager)
            {
                shineEffect.stopShineEffect();
                collectAllProfitsBtn();
            }
           // Debug.Log("Lift Handler  tutorialManager");
            //GameManager.Instance.tutorialManager.updateStep(TutorialStep.UnlockWareHouse);
        }

        public double getProfits()
        {
            return totalProfits;
        }

        public void subtractProfitsCollected(double collected)
        {
            totalProfits -= collected;
            if (totalProfits < 0)
            {
                profitsText.text = "0";
            }
            else
            {
                profitsText.text = "" + GameUtils.currencyToString(totalProfits);
            }

        }

        public void addManager()
        {
            GameManager.Instance.managerSelection.openPopup(0, BuldingType.Elevator);
        }

        public void levelUpBtnPressed()
        {

            //double cc = 0;
            //int ll = level;

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
            //calculateNewCostAndLoad();
            //Cong thuc
            //DataLift dataLift = dataLifts[levelLisft];




            //levelLisft = 3;
            //if (levelLisft == 0) //cap độ hien tai duong ray
            //{
            //    currenthashRate = baseHashrateLevel0;
            //    priceNextTon = basePrice; //level 1
            //    newthashRate = baseHashrate; //level 1


            //}
            //else if (levelLisft == 1)
            //{
            //    currenthashRate = baseHashrate;
            //    newthashRate = baseHashratelv2;
            //    priceNextTon = basePricelv2;
            //}
            //else if (levelLisft == 2)
            //{
            //    currenthashRate = baseHashratelv2;

            //    newthashRate = baseHashratelv2 * Math.Pow(1.5, ((levelLisft) - 1));
            //    priceNextTon = basePricelv2 * Math.Pow(1.5, ((levelLisft ) - 1));
            //}
            //else
            //{
            //    currenthashRate = baseHashratelv2 * Math.Pow(1.5, (levelLisft - 1)); ;
            //    newthashRate = baseHashratelv2 * Math.Pow(1.5, ((levelLisft + 1) - 1));
            //    priceNextTon = basePricelv2 * Math.Pow(1.5, ((levelLisft + 1) - 1));
            //}

            //Lay tu backend
            double roiBase = 142.8, rateRoi = 2.5;
            double currenthashRate = 0, newthashRate = 0, upgradePrice = 0,roi=0.0,newroi=0.0, currenthashRateSheep=0.0, newhashRateSheep = 0.0;
            if (UserDataManager.Instance.UserData.userConveyor != null)
            {
                if (UserDataManager.Instance.UserData.userConveyor.Count > 0)
                {
                    UserConveyor userConveyor = UserDataManager.Instance.UserData.userConveyor[0];
                    roi = roiBase + levelLisft * rateRoi;
                    if (multiplier ==1)
                    {

                        upgradePrice = userConveyor.UpgradePrice;
                        currenthashRate = userConveyor.HashRate;
                        newthashRate = userConveyor.HashRate1Lv;
                       
                        newroi = roiBase + (levelLisft+1) * rateRoi;
                        currenthashRateSheep = userConveyor.SheepHashRate;
                        newhashRateSheep = userConveyor.SheepCal1LvHashRate;
                    }
                    if (multiplier == 3)
                    {
                        currenthashRate = userConveyor.HashRate;
                        upgradePrice = userConveyor.Upgrade3LvPrice;               
                        newthashRate = userConveyor.HashRate3Lv;
                        newroi = roiBase + (levelLisft + 3) * rateRoi;
                        currenthashRateSheep = userConveyor.SheepHashRate;
                        newhashRateSheep = userConveyor.SheepHashRate3Lv;
                    }
                    else if (multiplier == 5)
                    {
                        currenthashRate = userConveyor.HashRate;
                        upgradePrice = userConveyor.Upgrade5LvPrice;
                        newthashRate = userConveyor.HashRate5Lv;
                        newroi = roiBase + (levelLisft + 5) * rateRoi;
                        currenthashRateSheep = userConveyor.SheepHashRate;
                        newhashRateSheep = userConveyor.SheepHashRate5Lv;
                    }
                   
                }
            }
            //Debug.Log("nhanangcap");


            GameManager.Instance.truckLevelPopup.showPopup(1, upgradePrice, currenthashRate, newthashRate,
                getEarningPerSec(), 1, liftMoveSpeed, loadingSpeedPerSec, totalLoadCanBear,
                0.0,0, 0.0f, 0.0, getIncrementedEarning() - totalLoadCanBear,
                0.0f, levelLisft, multiplier, "Lift", roi, newroi, currenthashRateSheep, newhashRateSheep);

           
        }

        public void levelUp(double cc)
        {


            //if (GameManager.Instance.coinsWallet < cc)
            //{
            //    GameManager.Instance.ShowToast("Not Enough Coins");
            //    return;
            //}

            SoundManager.Instance.PlayLevelUpSound();

            GameManager.Instance.tonConnectWallet.BuyLisft(cc);

            //GameManager.Instance.buyManager.UpgrapeuConveyor(1, () =>
            //{
            //    UserConveyor userConveyor =UserDataManager.Instance.GetOneUserConveyor();
            //    levelLisft = userConveyor.Level;
            //});

           // GameManager.Instance.addCash(-cc);
            //calculateNewCostAndLoad();
            levelText.text = "Level\n" + (level);
           // levelUpBtnPressed();

            //
            //int multiplier = getMaxMultiplier();
            //for (int i = 0; i < multiplier; i++)
            //{
            //    int nextUpgradeLevel = GameUtils.getNextUpgradeLevel(level);

            //    if (level + 1 == nextUpgradeLevel)
            //    {
            //        SoundManager.Instance.PlayUpgradeSound();
            //        GameManager.Instance.addBags(10);
            //    }
            //    level++;

            //    if(level < 10) AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.ElevatorUpgraded, 10);
            //    else if(level < 25) AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.ElevatorUpgraded, 25);
            //    else if(level < 50) AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.ElevatorUpgraded, 50);
            //    else if(level < 100) AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.ElevatorUpgraded, 100);
            //    else if(level < 150) AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.ElevatorUpgraded, 150);
            //    else if(level < 200) AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.ElevatorUpgraded, 200);
            //}

            //GameManager.Instance.addCash(-cc);
            //calculateNewCostAndLoad();
            //levelText.text = "Level\n" + ((level == GameManager.MAX_LEVEL) ? "MAX" : level + "");
            //levelUpBtnPressed();
        }

        public void calculateNewCostAndLoad()
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

            totalLoadCanBear = baseLoad * Math.Pow(earningExp, level - 1);
            loadingSpeedPerSec = totalLoadCanBear / 3;
        }

        public double getEarningPerSec()
        {
            double earningPerSec = totalLoadCanBear / (3 * liftMoveSpeed);
            return earningPerSec;
        }

        public double getIncrementedEarningPerSec()
        {
            int maxMul = getMaxMultiplier();
            if (maxMul == 0)
            {
                return getEarningPerSec();
            }

            double earningNew = baseLoad * Math.Pow(earningExp, level + getMaxMultiplier() - 1);
            double earnPerSec = earningNew / (3 * liftMoveSpeed);
            return earnPerSec;
        }

        public double getIncrementedEarning()
        {
            int maxMul = getMaxMultiplier();
            if (maxMul == 0)
            {
                return totalLoadCanBear;
            }

            double earningNew = baseLoad * Math.Pow(earningExp, level + getMaxMultiplier() - 1);
            return earningNew;
        }

        public void saveData()
        {
            string postFix = GameManager.Instance.worldIndex == 0 ? "" : GameManager.Instance.worldIndex + "";
            PlayerPrefs.SetInt("ElevatorHasManager" + postFix, hasManager ? 1 : 0);
            PlayerPrefs.SetInt("ElevatorLevel" + postFix, level);
            PlayerPrefs.SetString("ElevatorProfits" + postFix, totalProfits.ToString());
        }

        public void getData()
        {
            string postFix = GameManager.Instance.worldIndex == 0 ? "" : GameManager.Instance.worldIndex + "";
            level = PlayerPrefs.GetInt("ElevatorLevel" + postFix, 1);

            //if (level > GameManager.MAX_LEVEL)
            //{
            //    level = GameManager.MAX_LEVEL;
            //}

            totalProfits = double.Parse(PlayerPrefs.GetString("ElevatorProfits" + postFix, "0"));
            profitsText.text = GameUtils.currencyToString(totalProfits);
            calculateNewCostAndLoad();
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
                liftMoveSpeed = liftMoveSpeed / managerInfo.speed;
            }
            else if (managerInfo.effectType == EffectType.FarmSpeed)
            {
                loadingSpeedPerSec = loadingSpeedPerSec * managerInfo.speed;
            }

            bootPowerUpBtn.gameObject.SetActive(false);
            loadPowerUpBtn.gameObject.SetActive(false);
            powerUpTimerText.transform.parent.gameObject.SetActive(true);
            TimeSpan ts = TimeSpan.FromSeconds(managerInfo.timerCountdown);
            powerUpTimerText.text = GameUtils.TimeSpanToReadableString1(ts);//new DateTime(ts.Ticks).ToString("HH:mm:ss");
        }

        public void setManagerType()
        {
            if (managerInfo == null)
            {
                return;
            }

            liftMoveSpeed = defaultMoveSpeed;
            loadingSpeedPerSec = totalLoadCanBear / 3;

            if (managerInfo.isPowerUp)
            {
                if (managerInfo.effectType == EffectType.BootSpeed)
                {
                    liftMoveSpeed = liftMoveSpeed / managerInfo.speed;
                }
                else if (managerInfo.effectType == EffectType.FarmSpeed)
                {
                    loadingSpeedPerSec = loadingSpeedPerSec * managerInfo.speed;
                }
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
        //Load quản lý
        public void managerPopupCloseCB()
        {
            managerInfo = GameManager.Instance.managerSelection.getManagerInfo(0, BuldingType.Elevator);

            if (managerInfo != null)
            {
                hasManager = true;
                //AddManagerBtn.gameObject.SetActive(false);
                managerObject.SetActive(true);
                //collectAllProfitsBtn();
                setManagerType();
            }
            else
            {
                hasManager = false;
                //AddManagerBtn.gameObject.SetActive(true);
                managerObject.SetActive(false);
            }
        }

        public void unlockTruck(bool isButtonPressed)
        {
            if (!GameManager.Instance.tutorialManager.checkCurrentStepDone(TutorialStep.UnlockFarmHouse))
            {
                GameManager.Instance.ShowToast("Unlock farm first.");
                return;
            }

            if (isButtonPressed)
            {
                //GameManager.Instance.PlayExplosionEffect(unlockBtn.transform);
                //GameManager.Instance.addCash(-100);
                SoundManager.Instance.PlayGetCoinsSound();
            }
            unlockTruckSetting();
            //Debug.Log("Tutorail 5. Mo khoá nhà kho băng chuyền");
            //Debug.Log("TutorialManager bat dau");
            GameManager.Instance.tutorialManager.updateStep(TutorialStep.StartTruck);
        }

        void unlockTruckSetting()
        {
           
            unlockBtn.gameObject.SetActive(false);
            liftHouseObj.GetComponent<Button>().interactable = true;
            liftHouseObj.GetComponent<Button>().enabled = false;
            levelUpBtn.SetActive(true);
            //AddManagerBtn.SetActive(false);
            profitsText.gameObject.SetActive(false);

            //gameObject.GetComponent<Image>().enabled = true;
            //gameObject.GetComponent<Button>().enabled = true;
            fillSlider.gameObject.SetActive(false);
            loadText.gameObject.SetActive(false);
            //shineEffect.gameObject.SetActive(true);
        }

        public void tutorialCallback(TutorialStep step)
        {
            //HƯớng dan nâng cap kho
            if (step == TutorialStep.CloseUpgrade)
            {
                //if(!hasManager)
                //    AddManagerBtn.SetActive(true);
            }
            else if (step == TutorialStep.BuyUpgrade)
            {
                levelUpBtn.gameObject.SetActive(true);
            }
            else if (step == TutorialStep.StartTruck)
            {
               // Debug.Log("Tutorail 7. Mở khoá xe");
                unlockTruckSetting();
               
            }
            else if (step == TutorialStep.UnlockFarmHouse)
            {
                //Debug.Log("Tutorail 4. Hiện nút mo khoa băng chuyền kho");
                //Hien nut nag cap bang chuyen
                levelUpBtn.gameObject.SetActive(true);
                //unlockBtn.gameObject.SetActive(true);
            }
        }

        public bool isOnWork()
        {
            return isWorking;
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

            return 1;
            //int multiplier = 1;

            //int ll = level;
            //double cc = 0;

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
            // multiplier = multiplier == 0 ? 1 : multiplier;
           
        }
    }
}
