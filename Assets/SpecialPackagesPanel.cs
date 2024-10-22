using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPackagesPanel : MonoBehaviour
{
    public GameObject pack1Btn;
    public GameObject pack2Btn;
    public GameObject pack3Btn;

    private void OnEnable()
    {
        try
        {
            bool isBoughtCombo = UserDataManager.Instance.UserData.user.isBoughtCombo;
            if (!isBoughtCombo)
            {
                pack1Btn.SetActive(true);
                pack2Btn.SetActive(true);
                pack3Btn.SetActive(true);
            }
            else
            {
                pack1Btn.SetActive(false);
                pack2Btn.SetActive(false);
                pack3Btn.SetActive(false);

            }
        }
        catch
        {
            pack1Btn.SetActive(true);
            pack2Btn.SetActive(true);
            pack3Btn.SetActive(true);
        }
    }

 
}
