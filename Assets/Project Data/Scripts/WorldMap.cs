using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project_Data.Scripts
{
    public class WorldMap : MonoBehaviour {

        public GameObject [] plots;
        public GameObject [] locked;
        public GameObject [] unlocked;

        public Text [] currentValue;
        public Text [] cost;
        public Sprite[] plotImage;

        public double[] buyCost;

        public void Start()
        {
            worldSettings();
        }

        void worldSettings()
        {
            int worldIndex = PlayerPrefs.GetInt("WORLD_INDEX", 0);
            string postFix = worldIndex == 0 ? "" : worldIndex + "";

            for (int i = 1; i < plots.Length; i++)
            {
                int isBought = PlayerPrefs.GetInt("IWB" + i, 0);
                if (isBought == 1)
                {
                    plots[i].GetComponent<Image>().sprite = plotImage[1];
                    unlocked[i].SetActive(true);
                    locked[i].SetActive(false);
                    currentValue[i].text = GameUtils.currencyToString(GameManager.Instance.getCashByWorldIndex(i));
                }
                else
                {
                    plots[i].GetComponent<Image>().sprite = plotImage[0];
                    unlocked[i].SetActive(false);
                    locked[i].SetActive(true);
                    cost[i].text = GameUtils.currencyToString(buyCost[i]);
                }
            }

            currentValue[0].text = GameUtils.currencyToString(GameManager.Instance.getCashByWorldIndex(0));
        }

        public void openWorld(int index)
        {
            GameManager.Instance.saveGame();
            int isBought = PlayerPrefs.GetInt("IWB" + index, 0);

            if (index != 0 && isBought == 0)
            {
                if (GameManager.Instance.getCashByWorldIndex(index - 1) >= buyCost[index])
                {
                    PlayerPrefs.SetInt("IWB" + index, 1);
                    GameManager.Instance.addCashByWorldIndex(-buyCost[index], index - 1);
                    worldSettings();
                }
                else
                {
                    GameManager.Instance.ShowToast("Not Enough Coins");
                }
            }
            else
            {
                PlayerPrefs.SetInt("WORLD_INDEX", index);
                GameManager.Instance.saveGame();
                SceneManager.LoadScene(0);
            }
        }

        public void closeBtn()
        {
            SoundManager.Instance.PlayClickSound();
            gameObject.SetActive(false);
        }

        public void openBtn()
        {
            SoundManager.Instance.PlayClickSound();
            gameObject.SetActive(true);
            currentValue[0].text = GameUtils.currencyToString(GameManager.Instance.getCash());
        }

        public void openMap(int index)
        {
            PlayerPrefs.SetInt("WORLD_INDEX", index);
            GameManager.Instance.saveGame();
            SceneManager.LoadScene(0);
        }
    }
}