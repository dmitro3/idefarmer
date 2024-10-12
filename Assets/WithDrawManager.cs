using Project_Data.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static WalletManager;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

public class WithDrawManager : MonoBehaviour
{
    public int type;
    public TextMeshProUGUI tonCoinText;
    public TextMeshProUGUI sheepCoinText;
    public TextMeshProUGUI walletbanlaceText;
    
    public GameObject closeBtn;
    public GameObject successPanel;
    public GameObject failedPanel;
    public TMP_InputField inputCoin;
    public Button maxBn,fifthyBtn;
    public TextMeshProUGUI bumberCoin;
    private double maxCoinInPut = 0.8;
    private string nameTitle = "";
    private bool isdrawcall;

    //[DllImport("__Internal")]
    //private static extern void ShowNumberInput(string inputFieldName);

    //[DllImport("__Internal")]
    //private static extern void CreateInputFieldElement(string inputFieldName);
    private void Start()
    {
        isdrawcall = false;
        nameTitle = GetName(type);
        // Tạo một input element cho WebGL
        //#if UNITY_WEBGL && !UNITY_EDITOR
        //        CreateInputFieldElement(inputCoin.name);
        //#endif

        // Đăng ký sự kiện khi người dùng click vào InputField
        //inputCoin.onEndEdit.AddListener(OnInputFieldClicked);
        //Debug.Log("Wallet Balance WithDrawManager "+ UserDataManager.Instance.walletAddress);
        if (UserDataManager.Instance.walletAddress.Trim().Length > 0)
        {
            StartCoroutine(GetWalletBalance(UserDataManager.Instance.walletAddress.Trim()));
        }

    }
    // Khi InputField được click, gọi hàm JavaScript để hiển thị hộp thoại nhập số
    //void OnInputFieldClicked(string value)
    //{
    //    #if UNITY_WEBGL && !UNITY_EDITOR
    //            ShowNumberInput(inputCoin.name);
    //    #endif
    //}
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
        double tonCoinx = double.Parse(tonCoinText.text)/2;
        double tonSheepx = double.Parse(sheepCoinText.text) / 2;
        if (type == 1)
        {
            inputCoin.text = tonCoinx.ToString("F8");
        }
        else
        {
            inputCoin.text = tonSheepx.ToString("F8");
        }
       
    }

    public void GetMaxTon()
    {
        double tonCoinx = double.Parse(tonCoinText.text);
        double tonSheepx = double.Parse(sheepCoinText.text);
        if (type == 1)
        {
            inputCoin.text = tonCoinx.ToString("F8");
        }
        else
        {
            inputCoin.text = tonSheepx.ToString("F8");
        }
    }
    private string GetName(int type)
    {
        if (type == 1)
            return " TON Number";
        else
            return " Sheep Number";
    }
    public void WIthDraw()
    {
        if (isdrawcall)
            return;
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

        if (type == 1)
        {
            double tonCoin = double.Parse(inputCoin.text.Trim().ToString());
            double toncurrent = double.Parse(tonCoinText.text);

            if (tonCoin < maxCoinInPut)
            {
                GameManager.Instance.ShowToast("Please Input " + nameTitle + " Minum 0.8 TON");
                return;
            }

            if (tonCoin > toncurrent)
            {
                GameManager.Instance.ShowToast(nameTitle + " exceeds the allowed number");
                return;
            }
            isdrawcall = true;
            GameManager.Instance.buyManager.WithDrawTonCoin(UserDataManager.Instance.walletAddress.Trim(), type + "", tonCoin.ToString(), () =>
            {

                OpenSucessPannel();

            });
        }
        else
        {
            double sheepCoin = double.Parse(inputCoin.text.Trim().ToString());
            double sheepCoinCurrent = double.Parse(sheepCoinText.text);

            if (sheepCoin < maxCoinInPut)
            {
                GameManager.Instance.ShowToast("Please Input " + nameTitle + " Minum 0.8 TON");
                return;
            }

            if (sheepCoin > sheepCoinCurrent)
            {
                GameManager.Instance.ShowToast(nameTitle + " exceeds the allowed number");
                return;
            }
            //GameManager.Instance.buyManager.WithDrawTonCoin(DataConfig.walletAddress, type+"", sheepCoin.ToString(), () =>
            //{

            //    OpenSucessPannel();

            //});
        }

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
        GameManager.Instance.buyManager.LoadData(() => {
            successPanel.SetActive(true);
            GameManager.Instance.walletManager.LoadHistory();
            isdrawcall = false;
        });
      
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

}
