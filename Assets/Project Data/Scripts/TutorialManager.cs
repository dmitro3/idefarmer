using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Hướng dẫn
namespace Project_Data.Scripts
{
    public enum TutorialStep
    {
        UnlockFactory, //Mở khoá nhà máy
        FarmerWork, //Mở khoá cừu
        UnlockFarmHouse,
        StartTruck,
        UnlockWareHouse,
        HorseWork,
        OpenLevelUpPopup,
        BuyUpgrade,
        CloseUpgrade
    }

    public class TutorialManager : MonoBehaviour
    {
        public List<GameObject> steps = new List<GameObject>();
        public List<string> speechMessages = new List<string>();

       // public List<GameObject> managerSteps = new List<GameObject>();
        public GameObject farmer2TapObj;
        bool isCompleted;
        bool isTutorialWaiting;

        public List<GameObject> quickTravelSteps = new List<GameObject>();
        string postFix = "";

        void Start ()
        {
            speechMessages.Add("Click on the field to start growing your business");
            speechMessages.Add("Nice! But your worker is just standing still, tap him to make him work");
            speechMessages.Add("We need a place to storage all the wheat. Buy the storage");
            speechMessages.Add("Well done! Lets call the truck to collect your earnings");
            speechMessages.Add("Wohoo! Make your first money and unlock the shop");
            speechMessages.Add("Wow! There is a horse to help you");
            speechMessages.Add("Good job! Lets upgrade the level of our workers to get more goods");
            speechMessages.Add("");
            speechMessages.Add("");

            //Kiềm tra đã hướng dẫn chưa
            isCompleted = PlayerPrefs.GetInt("TutorialCompleted" + postFix, 0) == 0 ? false : true;

            Debug.Log("TutorialManager  isCompleted" + isCompleted);
            StartCoroutine("lateStart");
        }

        IEnumerator lateStart()
        {
            yield return new WaitForSeconds(0.2f);
            int step = PlayerPrefs.GetInt("TutorialStep" + postFix, 0);


            for (int i = 0; i <= step; i++)
            {
                TutorialStep eStep = (TutorialStep)i;
                sendCallbacks(eStep);
            }

            if (step == (int)TutorialStep.BuyUpgrade)
            {
                step -= 1;
                PlayerPrefs.SetInt("TutorialStep" + postFix, step);
            }

            if (step == (int)TutorialStep.CloseUpgrade)
            {
                tutorialCompleted();
                yield break;
            }
            showSpeechBubble(step);
        }
	
        void showStep()
        {
            if (isTutorialCompleted())
                return;

            //Hướng dẫn người chơi
            Debug.Log("Huongdan Step");
            if (isTutorialWaiting)
            {
                return;
            }

            int step = PlayerPrefs.GetInt("TutorialStep" + postFix, 0);
            Debug.Log("Huongdan nguoi choi step " + step);
            hideSteps();

            //Bước cuối cùng
            if (step + 1 == steps.Count)
            {
                PlayerPrefs.SetInt("TutorialStep" + postFix, step  + 1);
            }
           
            
            if (step < steps.Count && step != 0 && step!=5)
                steps[step].SetActive(true);

            if (step == 2)
            {
                GameManager.Instance.liftHandler.unlockTruck(true);
                GameManager.Instance.liftHandler.unlockTruck(true);
            }
            if (step == 3)
            {
                Debug.Log("NHan nut xe chay");
                GameManager.Instance.tutorialManager.updateStep(TutorialStep.UnlockWareHouse);
            }
            else if (step == 4)
            {
                GameManager.Instance.wareHouseHandler.unlockWarehouse(true);
            }
            else if (step == 6)
            {
                GameManager.Instance.tutorialManager.tutorialCompleted();
            }
            


        }

        public void updateStep(TutorialStep eStepNo)
        {
            //Cập nhật Hướng dẫn các bước
            if (isTutorialWaiting)
            {
                return;
            }

            int stepNo = (int)eStepNo;
            hideSteps();
            int step = PlayerPrefs.GetInt("TutorialStep" + postFix, 0);

            Debug.Log("Tutorail buoc so " + stepNo + " step: " + step);

            if (stepNo == step + 1)
            {
               
                sendCallbacks(eStepNo);
                PlayerPrefs.SetInt("TutorialStep" + postFix, stepNo);
                showSpeechBubble(stepNo);
            }
            else
            {
               
                showStep();
            }
        }

        public void hideSteps()
        {
            for (int i = 0; i < steps.Count; i++)
            {
                if (i != 5)
                {
                    steps[i].SetActive(false);
                }
               
            }
            farmer2TapObj.SetActive(false);
        }

        public void showSpeechBubble(int stepNo)
        {
            Debug.Log("showSpeechBubble " + stepNo);
            if (stepNo < steps.Count)
            {
                //AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.TutorialStep, stepNo);
                Debug.Log("ShowMessageWithIcon  " + stepNo);
                Speech.Instance.ShowMessageWithIcon("");
            }
        }

        public void speechBubbleCloseCB()
        {
            showStep();
        }

        public void tutorialCompleted()
        {
            isCompleted = true;
            PlayerPrefs.SetInt("TutorialCompleted" + postFix, 1);
        }

        public bool isTutorialCompleted()
        {
            isCompleted = PlayerPrefs.GetInt("TutorialCompleted" + postFix, 0) == 0 ? false : true;
            return isCompleted;
        }

        void sendCallbacks(TutorialStep step)
        {
          
            int factoryCount = GameManager.Instance.factoryList.Count;
            if(step == TutorialStep.StartTruck)
            {
               
                 Debug.Log(" Tutorail 6.Sau khi mo khoá nhà kho băng chuyền");
                GameManager.Instance.liftHandler.tutorialCallback(step);
            }
            else if(step == TutorialStep.HorseWork)
            {
                GameManager.Instance.wareHouseHandler.tutorialCallback(step);
            }
            else if(step == TutorialStep.UnlockFarmHouse)
            {
                Debug.Log("Tutorail 3. Sau khi mo khoá Farm");
                GameManager.Instance.liftHandler.tutorialCallback(step);
            }
            else if(step == TutorialStep.UnlockWareHouse)
            {
                
                GameManager.Instance.wareHouseHandler.tutorialCallback(step);
            }
            else if (step == TutorialStep.CloseUpgrade)
            {
                if (factoryCount > 0)
                {
                    GameManager.Instance.factoryList[0].tutorialCallback(step);
                }
                GameManager.Instance.liftHandler.tutorialCallback(step);
                GameManager.Instance.wareHouseHandler.tutorialCallback(step);
            }
            else if (step == TutorialStep.OpenLevelUpPopup)
            {
                if (factoryCount > 0)
                {
                    GameManager.Instance.factoryList[0].tutorialCallback(step);
                }
            }
            else if (step == TutorialStep.BuyUpgrade)
            {
                GameManager.Instance.liftHandler.tutorialCallback(step);
                GameManager.Instance.wareHouseHandler.tutorialCallback(step);
            }
        }

        public bool checkCurrentStepDone(TutorialStep step)
        {
            int currentStep = PlayerPrefs.GetInt("TutorialStep" + postFix, 0);

            if ((int)step <= currentStep)
                return true;

            return false;
        }

        public bool checkCurrentStep(TutorialStep step)
        {
            int currentStep = PlayerPrefs.GetInt("TutorialStep" + postFix, 0);

            if ((int)step == currentStep)
                return true;

            return false;
        }

        public void waitForNextStep(bool isWaiting)
        {
            isTutorialWaiting = isWaiting;
        }

        public void showTapSignForWorkers()
        {
           
            if (GameManager.Instance.worldIndex != 0)
            {
                return;
            }

            if (!isTutorialCompleted() && !isTutorialWaiting)
            {
                return;
            }
          
            ManagerSelection managerHandler = GameManager.Instance.managerSelection;
            GameObject farmerTapObj = steps[(int)TutorialStep.FarmerWork];
            GameObject truckTapObj  = steps[(int)TutorialStep.StartTruck];
           // GameObject horseTapObj  = steps[(int)TutorialStep.HorseWork];

            if (farmerTapObj.activeSelf || truckTapObj.activeSelf  || farmer2TapObj.activeSelf)
            {
                return;
            }

            ManagerInfo warehouseManager = managerHandler.getManagerInfo(0, BuldingType.WareHouse);
            ManagerInfo elevatorManager  = managerHandler.getManagerInfo(0, BuldingType.Elevator);
            ManagerInfo farmManager = managerHandler.getManagerInfo(1, BuldingType.Mine);
            ManagerInfo farmManager2 = managerHandler.getManagerInfo(2, BuldingType.Mine);

            int factoryCount = GameManager.Instance.factoryList.Count;

            if (farmManager != null && elevatorManager != null && warehouseManager != null && farmManager2 != null)
            {
                farmerTapObj.SetActive(false);
                truckTapObj.SetActive(false);
                //horseTapObj.SetActive(false);
                farmer2TapObj.SetActive(false);

                //managerSteps[0].SetActive(false);
                //managerSteps[1].SetActive(false);
                //managerSteps[2].SetActive(false);
                //managerSteps[3].SetActive(false);
                return;
            }

            if (farmManager == null && GameManager.Instance.getCash() >= managerHandler.getManagerCost(BuldingType.Mine))
            {
                if (PlayerPrefs.GetInt("OneTimeSpeechBubble" + postFix, 0) == 0)
                {
                    PlayerPrefs.SetInt("OneTimeSpeechBubble" + postFix, 1);
                    showOneTimeSpeechBubble("Lets automate the process and hire your first Supervisor.");
                }
                else
                {
                    //managerSteps[0].SetActive(true);
                    //managerSteps[1].SetActive(false);
                    //managerSteps[2].SetActive(false);
                    //managerSteps[3].SetActive(false);
                }
            }
            else if (farmManager2 == null && factoryCount > 1 && GameManager.Instance.getCash() >= managerHandler.getManagerCost(BuldingType.Mine))
            {
                //managerSteps[0].SetActive(false);
                //managerSteps[1].SetActive(false);
                //managerSteps[2].SetActive(false);
                //Debug.Log("aniamtion");
                //managerSteps[3].SetActive(true);
            }
            else if (elevatorManager == null && GameManager.Instance.getCash() >= managerHandler.getManagerCost(BuldingType.Elevator))
            {
                //managerSteps[0].SetActive(false);
                //managerSteps[1].SetActive(true);
                //managerSteps[2].SetActive(false);
                //managerSteps[3].SetActive(false);
            }
            else if (warehouseManager == null && GameManager.Instance.getCash() >= managerHandler.getManagerCost(BuldingType.WareHouse))
            {
                //managerSteps[0].SetActive(false);
                //managerSteps[1].SetActive(false);
                //managerSteps[2].SetActive(true);
                //managerSteps[3].SetActive(false);
            }
            else if (warehouseManager == null && !GameManager.Instance.wareHouseHandler.isOnWork()
                                              && GameManager.Instance.liftHandler.getProfits() >= 1)
            {
                farmerTapObj.SetActive(false);
                truckTapObj.SetActive(false);
                //horseTapObj.SetActive(true);
                farmer2TapObj.SetActive(false);

                //managerSteps[0].SetActive(false);
                //managerSteps[1].SetActive(false);
                //managerSteps[2].SetActive(false);
                //managerSteps[3].SetActive(false);
            }
            else if (elevatorManager == null && !GameManager.Instance.liftHandler.isOnWork()
                                             && GameManager.Instance.factoryList[0].getProfitsAndClear() >= 1)
            {
                farmerTapObj.SetActive(false);
                truckTapObj.SetActive(true);
                //horseTapObj.SetActive(false);
                farmer2TapObj.SetActive(false);

                //managerSteps[0].SetActive(false);
                //managerSteps[1].SetActive(false);
                //managerSteps[2].SetActive(false);
                //managerSteps[3].SetActive(false);
            }
            else if (farmManager == null && factoryCount > 0 && !GameManager.Instance.factoryList[0].isOnWork())
            {
                farmerTapObj.SetActive(true);
                truckTapObj.SetActive(false);
                //horseTapObj.SetActive(false);
                farmer2TapObj.SetActive(false);

                //managerSteps[0].SetActive(false);
                //managerSteps[1].SetActive(false);
                //managerSteps[2].SetActive(false);
                //managerSteps[3].SetActive(false);
            }
            else if (farmManager2 == null && factoryCount > 1 && !GameManager.Instance.factoryList[1].isOnWork())
            {
                farmerTapObj.SetActive(false);
                truckTapObj.SetActive(false);
               // horseTapObj.SetActive(false);
                //farmer2TapObj.SetActive(true);

                //managerSteps[0].SetActive(false);
                //managerSteps[1].SetActive(false);
                //managerSteps[2].SetActive(false);
                //managerSteps[3].SetActive(false);
            }
            else
            {
                farmerTapObj.SetActive(false);
                truckTapObj.SetActive(false);
                //horseTapObj.SetActive(false);
                farmer2TapObj.SetActive(false);

                //managerSteps[0].SetActive(false);
                //managerSteps[1].SetActive(false);
                //managerSteps[2].SetActive(false);
                //managerSteps[3].SetActive(false);
            }
        }

        public void showOneTimeSpeechBubble(string message)
        {
            Debug.Log("ShowMessageWithIcon  showOneTimeSpeechBubble");
            Speech.Instance.ShowMessageWithIcon(message, true);
        }

        public void speechBubbleCloseCallback()
        {
            //managerSteps[0].SetActive(false);
            //managerSteps[1].SetActive(false);
            //managerSteps[2].SetActive(false);
            //managerSteps[3].SetActive(false);
        }

        public void quickTravelTutorial()
        {
            if (GameManager.Instance.factoryCount == 3 && PlayerPrefs.GetInt("QUICK_TRAVEL", 0) == 0)
            {
                quickTravelSteps[0].SetActive(true);
            }
        }

        public void quickTravelTutorialCB(string name)
        {
            if (name == "open")
            {
                quickTravelSteps[0].SetActive(false);
                quickTravelSteps[1].SetActive(true);
            }
            else if (name == "done")
            {
                quickTravelSteps[0].SetActive(false);
                quickTravelSteps[1].SetActive(false);
                PlayerPrefs.SetInt("QUICK_TRAVEL", 1);
            }
        }
    }
}