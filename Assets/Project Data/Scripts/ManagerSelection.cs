using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

//Nâng cấp quản lý
namespace Project_Data.Scripts
{
    [Serializable]
    public class ManagerInfo
    {
        public int index;
        public int iconIndex;
        public string name;
        public float  speed;
        public float  timer;
        public float  cooldownTimer;
        public ManagerType managerType;
        public EffectType  effectType;
        public BuldingType buildingType = BuldingType.Mine;

        public bool isPowerUp;
        public bool isPowerUpCooldown;
        public float timerCountdown;
        public float timerCooldown;
    }

    public enum EffectType
    {
        BootSpeed,
        FarmSpeed
    }

    public enum ManagerType
    {
        Junior,
        Senior,
        Executive
    }

    public class ManagerSelection : MonoBehaviour
    {
        public GameObject managerSelectionPopup;
        public GameObject selectedManagerArea;
        public ScrollRect scrollView;
        public GameObject cellPrefab;
        public Text costTxt;
        public GameObject hireButton;
        public GameObject tapObj;
        public List<ManagerInfo> managersList = new List<ManagerInfo>();
        private string[] names = new string[]{"Richie Piekarski",
            "Merrill Vannostrand",
            "Hugo Ponds",
            "Del Krishnan",
            "Osvaldo Devens",
            "Monte Davi",
            "Edison Lefkowitz",
            "Theo Stamm",
            "Fred Falk",
            "Bryan Hildreth",
            "Joan Rappold",
            "Randal Fulbright",
            "Frederic Glatt",
            "Gino Gouge",
            "Alden Bartolotta",
            "Brendan Noland",
            "Denis Ashcroft",
            "Rashad Compo",
            "Ismael Croce",
            "Kurt Yousef" };

        public Sprite[] iconList;
    
        int index = -1;
        BuldingType buildingType = BuldingType.Mine;

        void Update()
        {
            for (int i = 0; i < managersList.Count; i++)
            {
                ManagerInfo managerInfo = managersList[i];

                if (!managerInfo.isPowerUp && !managerInfo.isPowerUpCooldown)
                {
                    continue;
                }

                if (managerInfo.isPowerUp)
                {
                    managerInfo.timerCountdown -= Time.deltaTime;

                    if (managerInfo.timerCountdown <= 0)
                    {
                        managerInfo.isPowerUp = false;
                        managerInfo.isPowerUpCooldown = true;
                        managerInfo.timerCountdown = managerInfo.timer;
                        managerInfo.timerCooldown = managerInfo.cooldownTimer;
                    }
                }
                if (managerInfo.isPowerUpCooldown)
                {
                    managerInfo.timerCooldown -= Time.deltaTime;

                    if (managerInfo.timerCooldown <= 0)
                    {
                        managerInfo.isPowerUp = false;
                        managerInfo.isPowerUpCooldown = false;
                        managerInfo.timerCountdown = managerInfo.timer;
                        managerInfo.timerCooldown = managerInfo.cooldownTimer;

                        if (managerInfo.buildingType == BuldingType.Mine && managerInfo.index != -1)
                        {
                            GameManager.Instance.factoryList[managerInfo.index - 1].setManagerType();
                        }
                        else if (managerInfo.buildingType == BuldingType.Elevator && managerInfo.index != -1)
                        {
                            GameManager.Instance.liftHandler.setManagerType();
                        }
                        else if (managerInfo.buildingType == BuldingType.WareHouse && managerInfo.index != -1)
                        {
                            GameManager.Instance.wareHouseHandler.setManagerType();
                        }
                    }
                }
            }
        }

        public ManagerInfo createManager()
        {
            ManagerInfo manager = new ManagerInfo();
            ManagerType managerType;
            if (managersList.Count <= 5)
            {
                managerType = ManagerType.Junior;
            }
            else
            {
                if(buildingType == BuldingType.Mine)
                    managerType = (ManagerType)UnityEngine.Random.Range(0, 3);
                else
                    managerType = (ManagerType)UnityEngine.Random.Range(0, 2);
            }

            manager.name = names[UnityEngine.Random.Range(0, names.Length)];

            manager.index = index;
            manager.iconIndex = (int)getManagersCount(BuldingType.Mine) % 4;
            manager.buildingType = buildingType;
            foreach (var m in managersList)
            {
                if (m.index == index && m.buildingType == buildingType)
                {
                    manager.index = -1;
                    break;
                }
            }

            manager.effectType = (EffectType)UnityEngine.Random.Range(0, 2);

            if (managerType == ManagerType.Junior)
            {
                manager.speed = 2;
                manager.timer = 60;
                manager.cooldownTimer = 4 * 60;
                manager.timerCountdown = manager.timer;
                manager.timerCooldown  = manager.cooldownTimer;
                manager.managerType = managerType;
            }
            else if (managerType == ManagerType.Senior)
            {
                manager.speed = 3;
                manager.timer = 3 * 60;
                manager.cooldownTimer = 7 * 60;
                manager.timerCountdown = manager.timer;
                manager.timerCooldown  = manager.cooldownTimer;
                manager.managerType = managerType;
            }
            else if (managerType == ManagerType.Executive)
            {
                manager.speed = 5;
                manager.timer = 5 * 60;
                manager.cooldownTimer = 10 * 60;
                manager.timerCountdown = manager.timer;
                manager.timerCooldown  = manager.cooldownTimer;
                manager.managerType = managerType;
            }

            return manager;
        }

        public void openPopup(int index, BuldingType type)
        {
            managerSelectionPopup.SetActive(true);
            this.index = index;
            buildingType = type;

            int managerCount = 0;
            for (int i = 0; i < managersList.Count; i++)
            {
                ManagerInfo manager = managersList[i];
                if (buildingType == manager.buildingType)
                {
                    managerCount++;
                }
            }

            double managerCost = 100 * Math.Pow(2, managerCount);
            costTxt.text = GameUtils.currencyToString(managerCost);

            if (managerCount == 0)
            {
                tapObj.SetActive(true);
            }
            else
            {
                tapObj.SetActive(false);
            }
            
            for (int j = 0; j < selectedManagerArea.transform.childCount; j++)
            {
                GameObject child = selectedManagerArea.transform.GetChild(j).gameObject;
                Destroy(child);
            }

            for (int j = 0; j < scrollView.content.childCount; j++)
            {
                GameObject child = scrollView.content.GetChild(j).gameObject;
                Destroy(child);
            }
        
            int unAssignedCount = 0;
            for (int i = 0; i < managersList.Count; i++)
            {
                ManagerInfo manager = managersList[i];

                if (manager.buildingType != buildingType)
                {
                    continue;
                }

                if (manager.index == index)
                {
                    GameObject cell = GameObject.Instantiate(cellPrefab, Vector3.zero, Quaternion.identity, selectedManagerArea.transform) as GameObject;
                    RectTransform rectT = cell.transform.GetComponent<RectTransform>();
                    rectT.anchorMin = new Vector2(0.5f, 0.5f);
                    rectT.anchorMax = new Vector2(0.5f, 0.5f);
                    cell.transform.localPosition = new Vector3(0f, 0f, 0f);
                    setCellInfo(cell, manager);
                }
                else if(manager.index == -1)
                {
                    GameObject cell = GameObject.Instantiate(cellPrefab, cellPrefab.transform.position, Quaternion.identity, scrollView.content) as GameObject;
                    cell.transform.localPosition = new Vector3(248f, -90 - (160 * unAssignedCount), 0f);

                    RectTransform rectT = scrollView.content.GetComponent<RectTransform>();
                    rectT.sizeDelta = new Vector2(rectT.sizeDelta.x, 160 * (unAssignedCount + 1));
                    setCellInfo(cell, manager);
                    unAssignedCount++;
                }
            }
        }

        void setCellInfo(GameObject cell, ManagerInfo info)
        {
            Text nameTxt = cell.transform.Find("Name").GetComponent<Text>();
            Text effectTxt = cell.transform.Find("effectTxt").GetComponent<Text>();
            Button assignBtn = cell.transform.Find("assignBtn").GetComponent<Button>();
            Button unassignBtn = cell.transform.Find("unassignBtn").GetComponent<Button>();
            Image icon = cell.transform.Find("Image").Find("Image").GetComponent<Image>();
            Text jobPositionTxt = cell.transform.Find("JobPosition").GetComponent<Text>();
            icon.overrideSprite = iconList[info.iconIndex];

            assignBtn.onClick.AddListener( () => { assignBtnClicked(cell, info); });
            unassignBtn.onClick.AddListener( () => { unassignBtnClicked(cell, info); });

            nameTxt.text = info.name;

            string effectName = info.effectType == EffectType.BootSpeed ? "movement speed" : "farming speed";
            effectTxt.text = "Effect: " + info.speed + "x " + effectName;

            if (info.index == -1)
            {
                assignBtn.gameObject.SetActive(true);
                unassignBtn.gameObject.SetActive(false);
            }
            else
            {
                assignBtn.gameObject.SetActive(false);
                unassignBtn.gameObject.SetActive(true);
            }

            if (info.managerType == ManagerType.Junior)
            {
                jobPositionTxt.text = "Junior";
            }
            else if (info.managerType == ManagerType.Senior)
            {
                jobPositionTxt.text = "Senior";
            }
            if (info.managerType == ManagerType.Executive)
            {
                jobPositionTxt.text = "Executive";
            }
        }

        void assignBtnClicked(GameObject cell, ManagerInfo info)
        {
            ManagerInfo mInfo = getManagerInfo(index, buildingType);
            if (mInfo != null)
            {
                mInfo.index = -1;
            }
            info.index = index;

            openPopup(index, buildingType);
        }

        void unassignBtnClicked(GameObject cell, ManagerInfo info)
        {
            ManagerInfo mInfo = getManagerInfo(index, buildingType);
            mInfo.index = -1;
            openPopup(index, buildingType);
        }

        public void closePopup()
        {
            managerSelectionPopup.SetActive(false);
            if (buildingType == BuldingType.Mine)
            {
                GameManager.Instance.factoryList[index - 1].managerPopupCloseCB();
            }
            else if (buildingType == BuldingType.Elevator)
            {
                GameManager.Instance.liftHandler.managerPopupCloseCB();
            }
            else if (buildingType == BuldingType.WareHouse)
            {
                GameManager.Instance.wareHouseHandler.managerPopupCloseCB();
            }
        }

        public void hireManager()
        {
            SoundManager.Instance.PlayClickSound();
            int managerCount = 0;
            for (int i = 0; i < managersList.Count; i++)
            {
                ManagerInfo manager1 = managersList[i];
                if (buildingType == manager1.buildingType)
                {
                    managerCount++;
                }
            }

            double managerCost = 100 * Math.Pow(2, managerCount);
            if (GameManager.Instance.getCash() < managerCost)
            {
                GameManager.Instance.ShowToast("Not Enough Coins");
                return;
            }

            GameManager.Instance.PlayExplosionEffect(hireButton.transform);
            GameManager.Instance.addCash(-managerCost);

            ManagerInfo manager = createManager();
            managersList.Add(manager);
            openPopup(index, buildingType);
            saveToFile();

            if (!GameManager.Instance.tutorialManager.isTutorialCompleted())
            {
                closePopup();
            }
        }

        public void saveToFile()
        {
            string postFix = GameManager.Instance.worldIndex == 0 ? "" : GameManager.Instance.worldIndex + "";
            FileStream file = File.Create(Application.persistentDataPath + "/Managers" + postFix + ".dat");
            BinaryFormatter writer = new BinaryFormatter();
            writer.Serialize(file, managersList);
            file.Close();
        }

        public void readFromFile()
        {
            string postFix = GameManager.Instance.worldIndex == 0 ? "" : GameManager.Instance.worldIndex + "";
            if (File.Exists(Application.persistentDataPath + "/Managers" + postFix + ".dat"))
            {
                FileStream file = File.OpenRead(Application.persistentDataPath + "/Managers" + postFix + ".dat");
                //Debug.Log(Application.persistentDataPath);
                var reader = new BinaryFormatter();
                managersList = (List<ManagerInfo>)reader.Deserialize(file);
                file.Close();
            }
        }

        public ManagerInfo getManagerInfo(int index, BuldingType type)
        {
            foreach (var m in managersList)
            {
                if (m.index == index && m.buildingType == type)
                {
                    return m;
                }
            }
            return null;
        }

        public double getManagerCost(BuldingType type)
        {
            int managerCount = 0;
            for (int i = 0; i < managersList.Count; i++)
            {
                ManagerInfo manager = managersList[i];
                if (type == manager.buildingType)
                {
                    managerCount++;
                }
            }

            double managerCost = 100 * Math.Pow(2, managerCount);
            return managerCost;
        }

        public double getManagersCount(BuldingType type)
        {
            int managerCount = 0;
            for (int i = 0; i < managersList.Count; i++)
            {
                ManagerInfo manager = managersList[i];
                if (type == manager.buildingType)
                {
                    managerCount++;
                }
            }

            return managerCount;
        }

        public void recalculateTimers()
        {
            string closeTime = PlayerPrefs.GetString("closeTime", "0");
            long closeTimeInSec = Convert.ToInt64(closeTime);
            DateTime oldDate = DateTime.FromBinary(closeTimeInSec);
            DateTime currentDate = DateTime.Now;
            TimeSpan difference = currentDate.Subtract(oldDate);

            for (int i = 0; i < managersList.Count; i++)
            {
                ManagerInfo managerInfo = managersList[i];

                if (!managerInfo.isPowerUp && !managerInfo.isPowerUpCooldown)
                {
                    continue;
                }

                float totalDiff = (float)difference.TotalSeconds;

                if (managerInfo.isPowerUp)
                {
                    if (managerInfo.timerCountdown - totalDiff > 0)
                    {
                        managerInfo.timerCountdown -= totalDiff;
                        totalDiff = 0;
                    }
                    else if (managerInfo.timerCountdown - totalDiff <= 0)
                    {
                        totalDiff -= managerInfo.timerCountdown;
                        managerInfo.isPowerUp = false;
                        managerInfo.isPowerUpCooldown = true;
                        managerInfo.timerCountdown = managerInfo.timer;
                        managerInfo.timerCooldown = managerInfo.cooldownTimer;
                    }
                }
                if (managerInfo.isPowerUpCooldown)
                {
                    managerInfo.timerCooldown -= Time.deltaTime;

                    if (managerInfo.timerCooldown - totalDiff > 0)
                    {
                        managerInfo.timerCooldown -= totalDiff;
                    }
                    else if (managerInfo.timerCooldown - totalDiff <= 0)
                    {
                        managerInfo.isPowerUp = false;
                        managerInfo.isPowerUpCooldown = false;
                        managerInfo.timerCountdown = managerInfo.timer;
                        managerInfo.timerCooldown = managerInfo.cooldownTimer;

                        if (managerInfo.buildingType == BuldingType.Mine && managerInfo.index != -1)
                        {
                            GameManager.Instance.factoryList[managerInfo.index - 1].setManagerType();
                        }
                        else if (managerInfo.buildingType == BuldingType.Elevator && managerInfo.index != -1)
                        {
                            GameManager.Instance.liftHandler.setManagerType();
                        }
                        else if (managerInfo.buildingType == BuldingType.WareHouse && managerInfo.index != -1)
                        {
                            GameManager.Instance.wareHouseHandler.setManagerType();
                        }
                    }
                }
            }
        }
    }
}