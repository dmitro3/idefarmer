using Project_Data.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using TonSdk.Connect;
using TonSdk.Core;
using UnityEngine.Networking;
using TonSdk.Core.Boc;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Linq;




public class WalletManager : MonoBehaviour
{
    public TextMeshProUGUI tonCoinText;
    public TextMeshProUGUI sheepCoinText;
    public TextMeshProUGUI accountNameTxt;
    public Button loginBtn;
    public Button logoutBtn;
    public Button depositBtn;
    public GameObject rowPrefab; // Prefab của RowTemplate
    public Transform content; // Vị trí của Content trong ScrollView
    public TextMeshProUGUI pageText; // Text hiển thị số trang
    public Button prevPageButton, nextPageButton; // Nút điều hướng
    private int currentPage = 0;
    private int rowsPerPage = 10; // Số hàng mỗi trang
    public TextMeshProUGUI wordText;
    List<DataItem> dataList = new List<DataItem>();
    [Header("References")]
    [SerializeField] private TonConnectHandler tonConnectHandler;
    public GameObject chooseWallets;
    public GameObject withDrawPanel;
    public GameObject withDrawSHeepPanel;
    public GameObject DepositPanel;
    private List<WalletConfig> walletConfigs;


    void Start()
    {
        tonCoinText.text = GameManager.Instance.GetBalanceTon();
        sheepCoinText.text = GameManager.Instance.GetBalanceSheepCoin();
        prevPageButton.onClick.AddListener(PreviousPage);
        nextPageButton.onClick.AddListener(NextPage);
        accountNameTxt.text = UserDataManager.Instance.UserData.user.userName;
        if (UserDataManager.Instance.isConnectWallet)
        {
           
            loginBtn.gameObject.SetActive(false);
            logoutBtn.gameObject.SetActive(true);

        }

        LoadHistory();
    }
    public void LoginWallet()
    {
        if (UserDataManager.Instance.isConnectWallet)
        {
         
            loginBtn.gameObject.SetActive(false);
            logoutBtn.gameObject.SetActive(true);

        }
        else
        {
            loginBtn.gameObject.SetActive(true);
            logoutBtn.gameObject.SetActive(false);
        }
    }
     #region Connect Wallet
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
          //  Debug.Log("API Response: " + jsonResponse);

            // Xử lý kết quả JSON
            WalletResponse response = JsonUtility.FromJson<WalletResponse>(jsonResponse);
            if (response.result != null)
            {
                UserDataManager.Instance.balanceWalletAddress = double.Parse(response.result) / 1000000000;
            }

        }
        request.Dispose();
    }
    void EnableConnectWalletButton()
    {
        loginBtn.gameObject.SetActive(true);
        logoutBtn.gameObject.SetActive(false);
        depositBtn.gameObject.SetActive(false);
    }
    void DisableWalletInfoButton()
    {
        loginBtn.gameObject.SetActive(false);
        logoutBtn.gameObject.SetActive(true);
        depositBtn.gameObject.SetActive(false);
    }
    private string ProcessWalletAddress(string address)
    {
        if (address.Length < 8) return address;

        string firstFourChars = address[..4];
        string lastFourChars = address[^4..];

        return firstFourChars + "..." + lastFourChars;
    }

    private void EnableWalletInfoButton(string wallet)
    {
        //Debug.Log("UserDataManager.Instance.nameAddress " + UserDataManager.Instance.walletAddress);
        accountNameTxt.text = wallet;

    }
    private void CloseConnectModal()
    {
        if (!tonConnectHandler.tonConnect.IsConnected) tonConnectHandler.tonConnect.PauseConnection();

    }
    private void OnProviderStatusChangeError(string message)
    {
        UserDataManager.Instance.walletAddress = "";
        UserDataManager.Instance.balanceWalletAddress = 0;
        GameManager.Instance.ShowToast("Not Connect Wallet. Try again");
    }
    //Ket noi vi
    public void ConnectWallet()
    {

        chooseWallets.SetActive(true);


    }


    public void LoadUrlWallet(string name)
    {

     

        if (tonConnectHandler.tonConnect.IsConnected)
        {
           // Debug.LogWarning("Wallet already connected. The connection window has not been opened. Before proceeding, please disconnect from your wallet.");
            chooseWallets.SetActive(false);
            return;
        }

        //HIện popup
        WalletConfig config = walletConfigs.Where(n => n.AppName.Equals(name)).FirstOrDefault();
        tonConnectHandler.CreateTonConnectInstance();
        chooseWallets.SetActive(false);
        OpenWalletQRContent(config);

    }

    private async void OpenWalletQRContent(WalletConfig config)
    {
        string connectUrl = await tonConnectHandler.tonConnect.Connect(config);
        OpenWalletUrl(connectUrl);
    }
    private void OpenWalletUrl(string url)
    {
        string escapedUrl = Uri.EscapeUriString(url);
        Application.OpenURL(escapedUrl);
    }
    /// <summary>
    /// Logout wallet
    /// </summary>
    public void LogOut()
    {
        DisconnectWalletButtonClick();
    }
    private async void DisconnectWalletButtonClick()
    {

        accountNameTxt.text = "Account Name";
        UserDataManager.Instance.walletAddress = "";
        //Debug.Log("UserDataManager.Instance.nameAddress " + UserDataManager.Instance.walletAddress);
        tonConnectHandler.RestoreConnectionOnAwake = false;
        await tonConnectHandler.tonConnect.Disconnect();
    }

    #endregion

    #region Loaddata
    private void LoadHistory()
    {
        GameManager.Instance.buyManager.LoadHistory(1, 100, () =>
        {

            WithDrawHistoryData withDrawHistoryData = UserDataManager.Instance.withDrawHistoryData;
            dataList = withDrawHistoryData.Data;
            if (dataList != null)
            {
                wordText.text = "(" + dataList.Count + ")";
            }
            DisplayPage(currentPage);


        });
    }
    void DisplayPage(int pageIndex)
    {
        // Xóa các hàng hiện tại
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Tính toán số hàng hiển thị trên trang hiện tại
        int start = pageIndex * rowsPerPage;
        int end = Mathf.Min(start + rowsPerPage, dataList.Count);

        // Tạo các hàng cho dữ liệu hiện tại
        for (int i = start; i < end; i++)
        {
            GameObject row = Instantiate(rowPrefab, content);
            TextMeshProUGUI[] columns = row.GetComponentsInChildren<TextMeshProUGUI>();

            columns[0].text = dataList[i].CreatedAt.ToString("dd/MM/yyyy HH:mm");
            columns[1].text = dataList[i].Amount.ToString("F4") + " TON";
            if (!string.IsNullOrEmpty(dataList[i].Status))
            {
                if (dataList[i].Status.Contains("PENDING"))
                {
                    columns[2].text = "Pending...";
                }
                else if (dataList[i].Status.Contains("DONE"))
                {
                    columns[2].text = "Done";
                    columns[2].color = Color.green;
                }
                else
                {
                    columns[2].text = "Failed";
                    columns[2].color = Color.green;

                }
            }

        }

        // Cập nhật Text số trang
        pageText.text = $"{pageIndex + 1}/{Mathf.CeilToInt((float)dataList.Count / rowsPerPage)}";

        // Vô hiệu hóa nút nếu không có trang tiếp theo hoặc trước đó
        prevPageButton.interactable = pageIndex > 0;
        nextPageButton.interactable = end < dataList.Count;
    }
    void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            DisplayPage(currentPage);
        }
    }
    void NextPage()
    {
        if ((currentPage + 1) * rowsPerPage < dataList.Count)
        {
            currentPage++;
            DisplayPage(currentPage);
        }
    }

    #endregion

    #region withDraw

    public void DrawTon()
    {

        if (UserDataManager.Instance.walletAddress.Trim().Length <= 0)
        {
            GameManager.Instance.ShowToast("Please Connect Wallet ");
            return;
        }
           

        withDrawPanel.SetActive(true);

    }
    public void DrawSheep()
    {
        GameManager.Instance.ShowToast("Coming Soon");
        //if (walletAddress.Trim().Length <= 0)
        //{
        //    GameManager.Instance.ShowToast("Please Connect Wallet ");
        //    return;
        //}


      //  withDrawSHeepPanel.SetActive(true);
    }
    public void  BuyCoinAsync()
    {
        //Debug.Log("NAP COIN");
        DepositPanel.SetActive(true);


    }
    //private async void SendCOinAsync()
    //{
    //    string receiverAddress = "UQBYLNmEIjlB3ESpbUPP8t5mg0jv7oHeSXBb0Ldn29ptUQ0T";
    //    double sendValue = 0.0001;
    //    if (string.IsNullOrEmpty(receiverAddress) || sendValue <= 0) return;

    //    Address receiver = new(receiverAddress);
    //    Coins amount = new(sendValue);

    //    string memo = UserDataManager.Instance.UserData.user.id;

    //    Debug.Log("memo " + memo);
    //    /// Thay userId từ user data

    //    // create transaction body query + memo
    //    Cell? payload = new CellBuilder().StoreUInt(0, 32).StoreString(memo).Build();

    //    Message[] sendTons =
    //    {
    //        new Message(receiver, amount,null,payload),
    //    };

    //    long validUntil = DateTimeOffset.Now.ToUnixTimeSeconds() + 3000;

    //    SendTransactionRequest transactionRequest = new SendTransactionRequest(sendTons, validUntil);
    //    SendTransactionResult resu= (SendTransactionResult)await tonConnectHandler.tonConnect.SendTransaction(transactionRequest);
    //}
    //public void CloseChooseWalalet()
    //{
    //    chooseWallets.SetActive(false);
    //}

    #endregion

    [System.Serializable]
    public class WalletResponse
    {
        public string ok;
        public string result; // Số dư ví trả về
    }
}
