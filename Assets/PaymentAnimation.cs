using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System;
public class PaymentAnimation : MonoBehaviour
{
    public TextPool textPool; // Tham chiếu đến TextPool
    private float moveDuration = 10; // Thời gian di chuyển từ trái sang phải
    private float fadeDuration = 1f; // Thời gian làm mờ
    private float delayBetweenAccounts = 2.6f; // Thời gian chờ giữa các tài khoản

    private Queue<string> accountQueue = new Queue<string>(); // Hàng đợi tài khoản
    private bool isAnimating = false; // Trạng thái đang chạy animation
    int maxQueueSize = 50; // Giới hạn kích thước hàng đợi
    void OnReceiveAccountName(string res)
    {
        try
        {
            EnqueuePaymentAccount(res);
        }
        catch (Exception ex)
        {
            Debug.Log("Error Add "+ex.Message);
        }

    }


    public void EnqueuePaymentAccount(string accountName)
    {

        // Kiểm tra nếu hàng đợi đã đầy, thoát hàm
        if (accountQueue.Count >= maxQueueSize) return;

        // Thêm phần tử vào hàng đợi
        accountQueue.Enqueue(accountName);

        // Bắt đầu Coroutine nếu không có animation đang chạy
        if (!isAnimating)
        {
            StartCoroutine(DisplayNextAccount());
        }
    }

    private IEnumerator DisplayNextAccount()
    {
        while (accountQueue.Count > 0)
        {
            isAnimating = true;
            string accountName = accountQueue.Dequeue();

            // Lấy đối tượng Text từ pool
            TextMeshProUGUI pooledText = textPool.GetPooledText();
            if (pooledText != null)
            {
                StartCoroutine(DisplayPaymentAccount(pooledText, accountName));
            }
            else
            {
                Debug.LogWarning("No available text objects in pool.");
            }

            // Chờ 5 giây trước khi xử lý tài khoản tiếp theo
            yield return new WaitForSeconds(delayBetweenAccounts);
        }

        isAnimating = false;
    }

    private IEnumerator DisplayPaymentAccount(TextMeshProUGUI textObject, string accountName)
    {
        // Cài đặt text và hiển thị đối tượng
       
        textObject.text = accountName+ "<sprite index=1>";
        textObject.color = new Color(0f, 0f, 0f, 0.6f); // Màu đen với alpha
        textObject.fontStyle = FontStyles.Normal;
        textObject.transform.localPosition = new Vector3(-Screen.width, 15, 0);
        textObject.gameObject.SetActive(true);

        // Di chuyển từ trái sang phải
        Tween moveTween = textObject.transform.DOLocalMoveX(Screen.width, moveDuration);

        // Đợi đến khi di chuyển hoàn tất
        yield return moveTween.WaitForCompletion();

        // Làm mờ dần đối tượng
        Tween fadeTween = textObject.DOFade(0, fadeDuration);

        // Đợi đến khi hoàn tất làm mờ
        yield return fadeTween.WaitForCompletion();

        // Ẩn đối tượng và trả về pool
        textObject.gameObject.SetActive(false);
        textObject.text = "";
    }
}
