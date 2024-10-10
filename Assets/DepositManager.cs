using Project_Data.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TonSdk.Connect;
using TonSdk.Core.Boc;
using TonSdk.Core;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static WalletManager;

public class DepositManager : MonoBehaviour
{
    public TextMeshProUGUI tonCoinText;
    public TextMeshProUGUI sheepCoinText;
    public TextMeshProUGUI walletbanlaceText;

    public GameObject closeBtn;
    public GameObject successPanel;
    public GameObject failedPanel;
    public TMP_InputField inputCoin;
    public Button maxBn, fifthyBtn;
    public TextMeshProUGUI bumberCoin;
    private double maxCoinInPut = 0.8;
    private string nameTitle = "";
    [SerializeField] private TonConnectHandler tonConnectHandler;
    private void Start()
    {
        if (UserDataManager.Instance.walletAddress.Trim().Length > 0)
        {
            StartCoroutine(GetWalletBalance(UserDataManager.Instance.walletAddress.Trim()));
        }

    }
    IEnumerator GetWalletBalance(string address)
    {

        string apiUrl = "https://toncenter.com/api/v2/getAddressBalance?address=" + address;
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            // Lấy kết quả JSON từ API
            string jsonResponse = request.downloadHandler.text;
            if (!string.IsNullOrEmpty(jsonResponse))
            {
                // Xử lý kết quả JSON
                WalletResponse response = JsonUtility.FromJson<WalletResponse>(jsonResponse);
                double tonbalance = double.Parse(response.result) / 1000000000;
                UserDataManager.Instance.balanceWalletAddress = tonbalance;
                walletbanlaceText.text = "Wallet Balance: " + tonbalance.ToString("F8") + " TON";
            }

        }
        request.Dispose();
    }

    public void Gete50Ton()
    {
        double tonCoinx = UserDataManager.Instance.balanceWalletAddress / 2;
      
        inputCoin.text = tonCoinx.ToString("F8");

    }

    public void GetMaxTon()
    {
        double tonCoinx = UserDataManager.Instance.balanceWalletAddress;
        inputCoin.text = tonCoinx.ToString("F8");
    }

    
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void SucessPanleClose()
    {

        successPanel.SetActive(false);
    }
    public void OpenSucessPannel()
    {

        successPanel.SetActive(true);
    }

    public void ClodeAllPanel()
    {

        failedPanel.SetActive(false);
        gameObject.SetActive(false);
    }
    public void ClodeFailedPanel()
    {
        failedPanel.SetActive(false);
    }
    public void Deposit()
    {
        //Kiểm tra qua ví
        if (UserDataManager.Instance.walletAddress.Trim().Length <= 0)
        {
            GameManager.Instance.ShowToast("Please Connect Wallet");
            return;
        }

        //Kiểm tra qua số luong TON
        if (inputCoin.text.Trim().Length <= 0)
        {

            GameManager.Instance.ShowToast("Please Input " + nameTitle);
            return;
        }

        double tonCoin = double.Parse(inputCoin.text.Trim().ToString());
        double tonBalance = UserDataManager.Instance.balanceWalletAddress;

        //if (tonCoin < maxCoinInPut)
        //{
        //    GameManager.Instance.ShowToast("Please Input " + nameTitle + " Minum 0.8 TON");
        //    return;
        //}

        if (tonCoin > tonBalance)
        {
            GameManager.Instance.ShowToast("TON Number  exceeds the allowed number");
            return;
        }
        
        SendCOinAsync(tonCoin);
    }


    // Hàm hiển thị/tắt popup loading


    private async void SendCOinAsync(double tonCoin)
    {
     
        string receiverAddress = "UQBYLNmEIjlB3ESpbUPP8t5mg0jv7oHeSXBb0Ldn29ptUQ0T";
        double sendValue = tonCoin;
        if (string.IsNullOrEmpty(receiverAddress) || sendValue <= 0) return;

        Address receiver = new(receiverAddress);
        Coins amount = new(sendValue);

        string memo = UserDataManager.Instance.UserData.user.id;

        Debug.Log("memo " + memo);
        /// Thay userId từ user data

        // create transaction body query + memo
        Cell? payload = new CellBuilder().StoreUInt(0, 32).StoreString(memo).Build();

        Message[] sendTons =
        {
            new Message(receiver, amount,null,payload),
        };
       
        long validUntil = DateTimeOffset.Now.ToUnixTimeSeconds() + 3000;

        SendTransactionRequest transactionRequest = new SendTransactionRequest(sendTons, validUntil);
        await tonConnectHandler.tonConnect.SendTransaction(transactionRequest)
            .ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    // Xử lý callback khi giao dịch thành công
                     Debug.Log("Transaction completed successfully!");
                    
                   // OpenSucessPannel();
                    // Thực hiện các hành động khác ở đây
                }
                else if (task.IsFaulted)
                {
                    // Xử lý callback khi có lỗi xảy ra
                    Debug.Log($"Transaction failed: {task.Exception?.Message}");
                }
            });


    }

}
