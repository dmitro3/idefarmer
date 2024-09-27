using UnityEngine;
using UnityEngine.UI;

namespace Project_Data.Scripts
{
    public class ManagerPopup : MonoBehaviour
    {
        public Text titleText;
        public Text costText;

        int index;
        BuldingType type;

        public void openPanel(int index, string title, string cost, BuldingType type)
        {
            SoundManager.Instance.PlayOpenHudSound();
            this.index = index;
            this.type = type;
            titleText.text = title;
            costText.text = cost;
            gameObject.SetActive(true);
        }

        public void closePanel()
        {
            SoundManager.Instance.PlayCloseHudSound();
            gameObject.SetActive(false);
        }
    }
}
