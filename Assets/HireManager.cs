using Project_Data.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HireManager : MonoBehaviour
{
    public GameObject managerSelectionPopup;
    int index;
    public void openPopup(int index, BuldingType type)
    {
        managerSelectionPopup.SetActive(true);
        this.index = index;


    }
    public void ClosePopup()
    {
        managerSelectionPopup.SetActive(false);
        GameManager.Instance.tutorialManager.updateStep(TutorialStep.OpenLevelUpPopup);

    }
    public void BuyManager()
    {
        FactoryHandler factory = GameManager.Instance.factoryList[index - 1];
        factory.BuyManager();
        GameManager.Instance.tutorialManager.updateStep(TutorialStep.OpenLevelUpPopup);
    }

}
