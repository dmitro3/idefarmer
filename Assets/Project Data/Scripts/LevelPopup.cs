using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Data.Scripts
{
    public class LevelPopup : MonoBehaviour
    {
        public Text   mineIndexText;
        public Text   titleText;
        public Text   costText;

        public Text totalExtrectionTxt;
        public Text incrementExtrectionTxt;
        public Text workersTxt;
        public Text incrementWorkerTxt;
        public Text walkSpeedTxt;
        public Text incrementWalkSpeedTxt;
        public Text harvestSpeedTxt;
        public Text incrementHarvestSpeedTxt;
        public Text workerCapTxt;
        public Text incrementWorkerCapTxt;
    
        public Slider slider;
        public Text   sliderBottomText;
        public Text   sliderText;
        public Text   levelBtnText;

        public Text   sliderDescText;
        public Image  sliderIcon;

        public Button x1Button;
        public Button x10Button;
        public Button x50Button;
        public Button xMaxButton;

        public GameObject levelUpBtn;

        int index;
        string popupType = "Mine";
        double cost;

        Color cNormal = new Color(255/255f, 255/255f, 255/255f, 0f);
        Color cSelected = new Color(255/255f, 255/255f, 255/255f, 1f);

        public void showPopup(int index1, double cost, 
            double totalExtrection, int workers, float walkingSpeed, double harvestSpeed, double workerCapacity,
            double newExtrection, int newWorkers, float newWalkingSpeed, double newHarvestSpeed, double newCapacity,
            float sliderValue, int level, int multiplier, string popupType1)
        {
            SoundManager.Instance.PlayOpenHudSound();
            index = index1;
            popupType = popupType1;
            this.cost = cost;

            if (popupType == "Lift")
            {
                titleText.text = "LEVEL " + level;
            }
            else if (popupType == "WareHouse")
            {
                titleText.text = "LEVEL " + level;
            }

            costText.text = "" + GameUtils.currencyToString(cost);

            totalExtrectionTxt.text = GameUtils.currencyToString(totalExtrection) + "/sec";
            workersTxt.text = "" + workers;
            walkSpeedTxt.text = walkingSpeed + "/sec";
            harvestSpeedTxt.text = GameUtils.currencyToString(harvestSpeed);
            workerCapTxt.text = "" + GameUtils.currencyToString(workerCapacity);

            incrementExtrectionTxt.text = "+" +GameUtils.currencyToString(newExtrection);
            incrementWorkerTxt.text = "+" + newWorkers;
            incrementWalkSpeedTxt.text = "+" + newWalkingSpeed;
            incrementHarvestSpeedTxt.text = "+" + GameUtils.currencyToString(newHarvestSpeed);
            incrementWorkerCapTxt.text = "+" + GameUtils.currencyToString(newCapacity);

            slider.value = sliderValue;
            gameObject.SetActive(true);

            if (cost > GameManager.Instance.getCash())
            {
                levelUpBtn.GetComponent<Button>().interactable = false;
            }
            else
            {
                levelUpBtn.GetComponent<Button>().interactable = true;
            }

            levelBtnText.text = "Level up x" + multiplier + "";

            int nextUpgradeLevel = GameUtils.getNextUpgradeLevel(level);
            int previousUpgradeLevel = GameUtils.getPreviousUpgradeLevel(level);

            int currentLevel = level - previousUpgradeLevel;
            int totalLevel   = nextUpgradeLevel - previousUpgradeLevel;

            sliderBottomText.text = "Next upgrade at level: " + nextUpgradeLevel;
            slider.value = currentLevel / (float)totalLevel;//level / (float)nextUpgradeLevel;
            setMultiplierButtonSetting();
            sliderText.text = "+10";
            sliderDescText.gameObject.SetActive(false);
            sliderIcon.gameObject.SetActive(true);
        }

        public void showMinePopup(int index1, double cost, 
            double totalExtrection, int workers, float walkingSpeed, double harvestSpeed, double workerCapacity,
            double newExtrection, int newWorkers, float newWalkingSpeed, double newHarvestSpeed, double newCapacity,
            float sliderValue, int level, int multiplier, double bonusMul, string popupType1)
        {
            SoundManager.Instance.PlayOpenHudSound();
            index = index1;
            popupType = popupType1;
            this.cost = cost;
            if (popupType == "Mine")
            {
                mineIndexText.text = "Farm " + index;
                titleText.text = "LEVEL " + level;
                costText.text = "" + GameUtils.currencyToString(cost);

                totalExtrectionTxt.text = GameUtils.currencyToString(totalExtrection) + "/sec";
                workersTxt.text = "" + workers;
                walkSpeedTxt.text = walkingSpeed + "/sec";
                harvestSpeedTxt.text = GameUtils.currencyToString(harvestSpeed);
                workerCapTxt.text = "" + GameUtils.currencyToString(workerCapacity);

                incrementExtrectionTxt.text = "+" +GameUtils.currencyToString(newExtrection);
                incrementWorkerTxt.text = "+" + newWorkers;
                incrementWalkSpeedTxt.text = "+" + newWalkingSpeed;
                incrementHarvestSpeedTxt.text = "+" + GameUtils.currencyToString(newHarvestSpeed);
                incrementWorkerCapTxt.text = "+" + GameUtils.currencyToString(newCapacity);

                slider.value = sliderValue;
                gameObject.SetActive(true);
            }

            if (cost > GameManager.Instance.getCash())
            {
                levelUpBtn.GetComponent<Button>().interactable = false;
            }
            else
            {
                levelUpBtn.GetComponent<Button>().interactable = true;
            }

            levelBtnText.text = "Level up x" + multiplier + "";
            int nextUpgradeLevel = GameUtils.getNextUpgradeLevel(level);
            int previousUpgradeLevel = GameUtils.getPreviousUpgradeLevel(level);

            int currentLevel = level - previousUpgradeLevel;
            int totalLevel   = nextUpgradeLevel - previousUpgradeLevel;

            sliderBottomText.text = "Next upgrade at level: " + nextUpgradeLevel;
            slider.value = currentLevel / (float)totalLevel;
            setMultiplierButtonSetting();
            sliderText.text = GameUtils.currencyToString(bonusMul);
            sliderDescText.gameObject.SetActive(true);
            sliderIcon.gameObject.SetActive(false);
        }

        void setMultiplierButtonSetting()
        {
            if (GameManager.Instance.multiplier == 1)
            {
                x1Button.GetComponent<Image>().color  = cSelected;
                x10Button.GetComponent<Image>().color = cNormal;
                x50Button.GetComponent<Image>().color = cNormal;
                xMaxButton.GetComponent<Image>().color = cNormal;
            }
            else if (GameManager.Instance.multiplier == 10)
            {
                x1Button.GetComponent<Image>().color  = cNormal;
                x10Button.GetComponent<Image>().color = cSelected;
                x50Button.GetComponent<Image>().color = cNormal;
                xMaxButton.GetComponent<Image>().color = cNormal;
            }
            else if (GameManager.Instance.multiplier == 50)
            {
                x1Button.GetComponent<Image>().color  = cNormal;
                x10Button.GetComponent<Image>().color = cNormal;
                x50Button.GetComponent<Image>().color = cSelected;
                xMaxButton.GetComponent<Image>().color = cNormal;
            }
            else if(GameManager.Instance.multiplier == -1)
            {
                x1Button.GetComponent<Image>().color  = cNormal;
                x10Button.GetComponent<Image>().color = cNormal;
                x50Button.GetComponent<Image>().color = cNormal;
                xMaxButton.GetComponent<Image>().color = cSelected;
            }
        }

        public void levelUpBtnPressed()
        {
            SoundManager.Instance.PlayClickSound();
            GameManager.Instance.PlayExplosionEffect(levelUpBtn.transform);

            Sequence seq = DOTween.Sequence();
            seq.Append(levelUpBtn.transform.DOScale(0.85f, 0.1f));
            seq.Append(levelUpBtn.transform.DOScale(1.0f, 0.1f));

            if (popupType == "Mine")
            {
                FactoryHandler factory = GameManager.Instance.factoryList[index - 1];
                factory.levelUp(cost);
            }
            else if (popupType == "Lift")
            {
                GameManager.Instance.liftHandler.levelUp(cost);
            }
            else if (popupType == "WareHouse")
            {
                GameManager.Instance.wareHouseHandler.levelUp(cost);
            }
        }

        public void closePopup()
        {
            SoundManager.Instance.PlayCloseHudSound();
            gameObject.SetActive(false);
            Debug.Log("HOANTAT");
            GameManager.Instance.tutorialManager.tutorialCompleted();
        }

        public void setMultiplier(int multiplier)
        {
            GameManager.Instance.multiplier = multiplier;
            setMultiplierButtonSetting();
            if (popupType == "Mine")
            {
                FactoryHandler factory = GameManager.Instance.factoryList[index - 1];
                factory.levelUpBtnPressed();
            }
            else if (popupType == "Lift")
            {
                GameManager.Instance.liftHandler.levelUpBtnPressed();
            }
            else if (popupType == "WareHouse")
            {
                GameManager.Instance.wareHouseHandler.levelUpBtnPressed();
            }
        }
    }
}