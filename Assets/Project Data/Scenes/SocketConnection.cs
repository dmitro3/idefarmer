using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using System.Text;
using System;
using System.Runtime.InteropServices;
using Project_Data.Scripts;
using System.Reflection;
public class SocketConnection : MonoBehaviour
{
    // URL của WebSocket server THAT
     private string serverUrl = "https://api.tonthesheep.com";  // Thay bằng URL server của bạn game.tonthesheep.com
    //<script src="https://cdn.socket.io/4.0.1/socket.io.js"></script>
    [DllImport("__Internal")]
    private static extern void socketConnect(string url, string token);

    [DllImport("__Internal")]
    private static extern void SocketDisconnect();

    private SocketIOUnity socket;
    void Start()
    {

        // Ví dụ token được nhận từ đâu đó, bạn có thể thay thế bằng token thực tế
        string token = DataConfig.token;// Thay thế bằng token thực tế

        // Khởi tạo kết nối tới Socket.IO server với token xác thực
        socketConnect(serverUrl, token);
    }

    //Thue xong
    public void HireOnMessageReceived(string res)
    {
        try
        {

            if (GameManager.Instance.ManagerPopup.activeSelf)
            {
                GameManager.Instance.ManagerPopup.SetActive(false);
            }
          

          
            GameManager.Instance.buyManager.LoadData(() => {

                GameManager.Instance.LoadDataServer();
            });

            //Tắt popup
            //GameManager.Instance.CloseLoading();
            GameManager.Instance.ShowToast("Hire Successful");
        }
        catch
        {
           
        }

    }

    //Tang bang chuyen xong
    public void LiftOnMessageReceived(string res)
    {
        try
        {

            if (GameManager.Instance.LiftPopup.activeSelf)
            {
                GameManager.Instance.LiftPopup.SetActive(false);
            }

            
            GameManager.Instance.buyManager.LoadData(() => {

                GameManager.Instance.LoadDataServer();
            });
           // GameManager.Instance.CloseLoading();
            GameManager.Instance.ShowToast("Upgrade Successful");
        }
        catch
        {
        }

    }



    //Shop
    public void ShopOnMessageReceived(string res)
    {
        try
        {

            if (GameManager.Instance.ShopPopup.activeSelf)
            {
                GameManager.Instance.ShopPopup.SetActive(false);
            }

           
            GameManager.Instance.buyManager.LoadData(() => {

                GameManager.Instance.LoadDataServer();
            });
            GameManager.Instance.ShowToast("Upgrade Successful");
        }
        catch
        {
        }

    }

    //upgradeFarmSheep   activeSheepFarm activeSheepFarm  
    public void UpgradeFarmOnMessageReceived(string res)
    {
        try
        {

            if (GameManager.Instance.FarmPopup.activeSelf)
            {
                GameManager.Instance.FarmPopup.SetActive(false);
            }
           
            GameManager.Instance.buyManager.LoadData(() => {

                GameManager.Instance.LoadDataServer();
            });

            GameManager.Instance.ShowToast("Upgrade Successful");;
        }
        catch
        {
        }

    }

    public void ActiveFarmOnMessageReceived(string res)
    {
        try
        {
          
            GameManager.Instance.buyManager.LoadData(() => {

                GameManager.Instance.LoadDataServer();
            });
           
            GameManager.Instance.ShowToast("Active Successful");
        }
        catch
        {

        }

    }

    public void BuyPackageOnMessageReceived(string res)
    {
        try
        {

            if (GameManager.Instance.SpecialPackagesPanel.activeSelf)
            {
                GameManager.Instance.SpecialPackagesPanel.SetActive(false);
            }

            GameManager.Instance.buyManager.LoadData(() => {

                GameManager.Instance.LoadDataServer();
            });

            GameManager.Instance.ShowToast("Payment Successful"); ;
        }
        catch
        {
        }

    }


    // Hàm này sẽ được gọi khi đối tượng bị hủy
    void OnDestroy()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log("Destroying SocketIOClient...");
            SocketDisconnect(); // Ngắt kết nối khi đối tượng bị hủy
       #endif

    }

}
