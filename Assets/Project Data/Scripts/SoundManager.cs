using UnityEngine;

namespace Project_Data.Scripts
{
    /// <summary>
    /// Quản lý âm thanh
    /// </summary>
    public class SoundManager : MonoBehaviour {

        public AudioClip levelUpSound;
        public AudioClip clickSound;
        public AudioClip getCoins;
        public AudioClip openHud;
        public AudioClip closeHud;
        public AudioClip upgradeLevelUp;
        public AudioClip farmerDropWheat;
        public AudioClip truckMoving;
        public AudioClip throwBags;
        public AudioClip collectBags;
        public AudioClip truckTap;
        public AudioClip tapWorker;
        public AudioClip powerUpClick;
        public AudioClip sheep1;
        public AudioClip sheep2;
        public AudioSource bgMusic;
        bool isSoundOn = true;
        bool isMusicOn = true;

        private static SoundManager _instance;

        public static SoundManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = FindObjectOfType<SoundManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        void Awake() 
        {
            if(_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                if(this != _instance)
                    Destroy(gameObject);
            }
        }

        public void PlayClickSound()
        {
            if (!isSoundOn)
                return;
            AudioSource.PlayClipAtPoint(clickSound ,transform.position);
        }

        public void PlayGetCoinsSound()
        {
            if (!isSoundOn)
                return;
            AudioSource.PlayClipAtPoint(getCoins ,transform.position);
        }

        public void PlayOpenHudSound()
        {
            if (!isSoundOn)
                return;
            AudioSource.PlayClipAtPoint(openHud ,transform.position);
        }

        public void PlayCloseHudSound()
        {
            if (!isSoundOn)
                return;
            AudioSource.PlayClipAtPoint(closeHud ,transform.position);
        }

        public void PlayLevelUpSound()
        {
            if (!isSoundOn)
                return;
            AudioSource.PlayClipAtPoint(upgradeLevelUp ,transform.position);
        }

        public void PlayFarmerDropWheatSound()
        {
            if (!isSoundOn)
                return;
            AudioSource.PlayClipAtPoint(farmerDropWheat ,transform.position);
        }

        public void PlayTruckMovingSound()
        {
            if (!isSoundOn)
                return;
            AudioSource.PlayClipAtPoint(truckMoving ,transform.position, 0.2f);
        }

        public void PlayThrowBagsSound()
        {
            if (!isSoundOn)
                return;
            AudioSource.PlayClipAtPoint(throwBags ,transform.position);
        }

        public void PlayCollectBagsSound()
        {
            if (!isSoundOn)
                return;
            AudioSource.PlayClipAtPoint(collectBags ,transform.position);
        }

        public void PlayTapTruckSound()
        {
            if (!isSoundOn)
                return;
            AudioSource.PlayClipAtPoint(truckTap ,transform.position);
        }

        public void PlayTapWorkerSound()
        {
            if (!isSoundOn)
                return;
            AudioSource.PlayClipAtPoint(tapWorker ,transform.position);
        }

        public void PlaySheep1Sound()
        {
            if (!isSoundOn)
                return;
            AudioSource.PlayClipAtPoint(sheep1, transform.position);
        }

        public void PlaySheep2Sound()
        {
            if (!isSoundOn)
                return;
            AudioSource.PlayClipAtPoint(sheep2, transform.position);
        }

        public void PlayUpgradeSound()
        {
            if (!isSoundOn)
                return;
            AudioSource.PlayClipAtPoint(levelUpSound, transform.position);
        }

        public void setSoundsOnOff(bool isOn)
        {
            isSoundOn = isOn;
        }

        public void setMusicOnOff(bool isOn)
        {
            isMusicOn = isOn;

            if (isMusicOn)
            {
                bgMusic.Play();
            }
            else
            {
                bgMusic.Stop();
            }
        }
    }
}