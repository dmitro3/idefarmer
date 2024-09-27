using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Data.Scripts
{
    public class PowerUpManager : MonoBehaviour
    {
        public Button bootPowerUpBtn;
        public Button farmingPowerUpBtn;
        public Text bootPowerUpTxt;
        public Text farmingPowerUpTxt;

        float timer = 60f;
        float cooldownTimer = 240f;
        bool isPowerUp  = false;
        bool isPowerUpCooldown  = false;

        void Start()
        {
            bootPowerUpTxt.gameObject.SetActive(false);
            farmingPowerUpTxt.gameObject.SetActive(false);

            bootPowerUpBtn.gameObject.SetActive(false);
            farmingPowerUpBtn.gameObject.SetActive(false);
        }

        void Update()
        {
            if (isPowerUp)
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    isPowerUp = false;
                    isPowerUpCooldown = true;
                    timer = 60f;
                    cooldownTimer = 240f;

                    for (int i = 0; i < GameManager.Instance.factoryList.Count; i++)
                    {
                        FactoryHandler factory = GameManager.Instance.factoryList[i];
                        factory.enablePowerUp();
                    }
                }

                TimeSpan ts = TimeSpan.FromSeconds(timer);
                bootPowerUpTxt.text = new DateTime(ts.Ticks).ToString("HH:mm:ss");
                farmingPowerUpTxt.text = new DateTime(ts.Ticks).ToString("HH:mm:ss");
            }
            if (isPowerUpCooldown)
            {
                cooldownTimer -= Time.deltaTime;

                if (cooldownTimer <= 0)
                {
                    isPowerUp = false;
                    isPowerUpCooldown = false;
                    timer = 60f;
                    cooldownTimer = 240f;

                    bootPowerUpBtn.interactable = true;
                    farmingPowerUpBtn.interactable = true;
                    bootPowerUpTxt.gameObject.SetActive(false);
                    farmingPowerUpTxt.gameObject.SetActive(false);
                }

                TimeSpan ts = TimeSpan.FromSeconds(cooldownTimer);
                bootPowerUpTxt.text = new DateTime(ts.Ticks).ToString("HH:mm:ss");
                farmingPowerUpTxt.text = new DateTime(ts.Ticks).ToString("HH:mm:ss");
            }
        }
    }
}