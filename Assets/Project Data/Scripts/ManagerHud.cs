using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Data.Scripts
{
    public class ManagerHud : MonoBehaviour
    {
        public GameObject bootManager;
        public GameObject farmManager;
        public Button assignBootBtn;
        public Button assignFarmBtn;
        public Button dismissBootBtn;
        public Button dismissFarmBtn;

        public Button hireManagerBtn;
        public GameObject costPanel;

        public Text titleText;
        public Text costText;

        public Vector3 topPos;
        public Vector3 botPos;

        int index;
        BuldingType type;

        public void openPanel(int index, string title, string cost, BuldingType type, bool hasManager)
        {
            SoundManager.Instance.PlayOpenHudSound();
            this.index = index;
            this.type = type;
            titleText.text = title;
            costText.text = cost;
            gameObject.SetActive(true);

            string managerType = "";
            if (type == BuldingType.Mine)
            {
                managerType = PlayerPrefs.GetString("ManagerType" + index, index % 2 == 0 ? "boot" : "farm");
            }
            else if (type == BuldingType.Elevator)
            {
                managerType = PlayerPrefs.GetString("LiftManagerType", "boot");
            }
            else if (type == BuldingType.WareHouse)
            {
                managerType = PlayerPrefs.GetString("WarehouseManagerType", "boot");
            }

            if (managerType == "boot")
            {
                assignBootBtn.gameObject.SetActive(false);
                assignFarmBtn.gameObject.SetActive(true);
                dismissBootBtn.gameObject.SetActive(false);
                dismissFarmBtn.gameObject.SetActive(false);

                bootManager.transform.localPosition = topPos;
                farmManager.transform.localPosition = botPos;
            }
            else
            {
                assignBootBtn.gameObject.SetActive(true);
                assignFarmBtn.gameObject.SetActive(false);
                dismissBootBtn.gameObject.SetActive(false);
                dismissFarmBtn.gameObject.SetActive(false);

                bootManager.transform.localPosition = botPos;
                farmManager.transform.localPosition = topPos;
            }

            if (hasManager)
            {
                hireManagerBtn.interactable = false;
                hireManagerBtn.transform.GetComponentInChildren<Text>().text = "HIRED";
                costPanel.SetActive(false);
            }
            else
            {
                hireManagerBtn.interactable = true;
                hireManagerBtn.transform.GetComponentInChildren<Text>().text = "Hire Manager";
                costPanel.SetActive(true);
            }
        }

        public void closePanel()
        {
            SoundManager.Instance.PlayCloseHudSound();
            gameObject.SetActive(false);
            Debug.Log("ManageHub  tutorialManager");
            Debug.Log("TutorialManager bat dau");
        }

        public void bootManagerBtnPressed()
        {
            SoundManager.Instance.PlayClickSound();
            assignBootBtn.gameObject.SetActive(false);
            assignFarmBtn.gameObject.SetActive(true);
            dismissBootBtn.gameObject.SetActive(false);
            dismissFarmBtn.gameObject.SetActive(false);

            bootManager.transform.DOLocalMove(topPos, 0.2f);
            farmManager.transform.DOLocalMove(botPos, 0.2f);
        }

        public void farmManagerBtnPressed()
        {
            SoundManager.Instance.PlayClickSound();
            assignBootBtn.gameObject.SetActive(true);
            assignFarmBtn.gameObject.SetActive(false);
            dismissBootBtn.gameObject.SetActive(false);
            dismissFarmBtn.gameObject.SetActive(false);

            bootManager.transform.DOLocalMove(botPos, 0.2f);
            farmManager.transform.DOLocalMove(topPos, 0.2f);
        }
    }
}