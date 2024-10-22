using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Project_Data.Scripts;

public class SpecialPackages : MonoBehaviour
{
    // Tổng số giây ban đầu (3 ngày 22 giờ 0 phút 0 giây)
    private float totalSeconds = 0;
    private float remainingSeconds;

    // Bộ đếm thời gian
    private float countdownInterval = 1.0f;
    private float countdownTimer = 0.0f;
    // Đối tượng UI để hiển thị thời gian đếm ngược
    public TextMeshProUGUI countdownBntText;
    public TextMeshProUGUI countdownPackText;
    public GameObject SpecialPackagesPanel;



    void Start()
    {
        if (UserDataManager.Instance.UserData.timeToEndSpecial > 0)
        {
            totalSeconds = UserDataManager.Instance.UserData.timeToEndSpecial/1000;
        }

       
    

           // Đặt lại thời gian còn lại về giá trị ban đầu
        remainingSeconds = totalSeconds;
        UpdateCountdownText();

    }
    void Update()
    {
        // Giảm bộ đếm thời gian mỗi khung hình
        countdownTimer += Time.deltaTime;

        // Nếu bộ đếm thời gian đạt 1 giây, giảm số giây còn lại
        if (countdownTimer >= countdownInterval)
        {
            countdownTimer = 0.0f; // Đặt lại bộ đếm thời gian
            remainingSeconds--;    // Giảm 1 giây

            // Cập nhật văn bản UI
            UpdateCountdownText();

            // Nếu thời gian đếm ngược về 0, đặt lại thời gian ban đầu
            if (remainingSeconds <= 0)
            {
                remainingSeconds = 0;
            }
        }
    }

    // Hàm cập nhật văn bản đếm ngược
    void UpdateCountdownText()
    {
        if (remainingSeconds > 0)
        {
            TimeSpan time = TimeSpan.FromSeconds(remainingSeconds);
            countdownBntText.text = string.Format("{0:D2}d {1:D2}h {2:D2}m",
                                               time.Days, time.Hours, time.Minutes, time.Seconds);
            if (countdownPackText.gameObject.activeSelf)
            {
                countdownPackText.text = string.Format("{0:D2}d {1:D2}h {2:D2}m",
                                             time.Days, time.Hours, time.Minutes);
            }
        }
        else
        {
            countdownBntText.text = string.Format("{0:D2}d {1:D2}h {2:D2}m",
                                             0, 0, 0);
            countdownPackText.text = string.Format("{0:D2}d {1:D2}h {2:D2}m",
                                            0, 0, 0);
        }
       
    }
    public void BuyPackage(int index)
    {
         GameManager.Instance.tonConnectWallet.BuyPakage(index);

    }

    public void ClosePakage()
    {
        SpecialPackagesPanel.SetActive(false);

    }
}
