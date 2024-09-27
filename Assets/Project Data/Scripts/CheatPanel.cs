using UnityEngine;

namespace Project_Data.Scripts
{
    public class CheatPanel : MonoBehaviour
    {
        public void cheatBtnPressed(int cash)
        {
            if (cash < 0)
            {
                GameManager.Instance.addCash(5000000000000000000);
            }
            else
            {
                GameManager.Instance.addCash(cash);
            }
        }

        public void addCheatBags(int bags)
        {
            GameManager.Instance.addBags(bags);
        }

        public void openPanel()
        {
            SoundManager.Instance.PlayCloseHudSound();
            gameObject.SetActive(true);
        }

        public void closeBtn()
        {
            SoundManager.Instance.PlayCloseHudSound();
            gameObject.SetActive(false);
        }
    }
}
