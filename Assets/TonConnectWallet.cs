using Project_Data.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using TonSdk.Connect;
using UnityEngine;

public class TonConnectWallet : MonoBehaviour
{


    [DllImport("__Internal")]
    private static extern void InitializeTonConnect();

    [DllImport("__Internal")]
    private static extern void ConnectTonWallet();


    [DllImport("__Internal")]
    private static extern void DisConnectTonWallet();

    [DllImport("__Internal")]
    private static extern void BuyFarmTool(string addressdeposit,string amout,string data);


    private double hirePrices = 0.01;
    private void Start()
        {
        //#if UNITY_WEBGL && !UNITY_EDITOR

        //#endif
        InitializeTonConnect();

    }

    #region Connect
    // Sau khi khời tạo xong
    public void OnTonConnectInitialized(string walletAddress)
    {
        Debug.Log("TonConnect Initialized: " + walletAddress);
        if (walletAddress.Trim().Length > 1)
        {
            UserDataManager.Instance.walletAddress = walletAddress;
            UserDataManager.Instance.isConnectWallet = true;
            //Cập Nhật UI
        }
        else
        {
            UserDataManager.Instance.walletAddress = "";
            UserDataManager.Instance.isConnectWallet = false;
        }
        if (GameManager.Instance.walletManager != null)
        {
            GameManager.Instance.walletManager.LoginWallet();
        }
    }

    //Mở kết nối

    public void OpenConnect()
    {
        Debug.Log("TonWallet OpenConnect");
        ConnectTonWallet();
    }
    //Sau khi kết nối xong
    public void OnTonConnected(string walletAddress)
    {
        Debug.Log("TonWallet OnTonConnected " + walletAddress);
        if (walletAddress.Trim().Length > 1)
        {
            UserDataManager.Instance.walletAddress = walletAddress;
            UserDataManager.Instance.isConnectWallet = true;
            //Cập Nhật UI
        }
        else
        {
            UserDataManager.Instance.walletAddress = "";
            UserDataManager.Instance.isConnectWallet = false;
        }

        if (GameManager.Instance.walletManager != null)
        {
            GameManager.Instance.walletManager.LoginWallet();
        }
    }

    //CLose connect
    public void CloseConnect()
    {
        Debug.Log("TonWallet CloseConnect");
        DisConnectTonWallet();
    }
    //Sau khi connect
    public void OnTonDisConnected(string walletAddress)
    {
        if (walletAddress.Trim().Equals("1"))
        {
            Debug.Log("TonWallet OnTonDisConnected " + walletAddress);
            UserDataManager.Instance.walletAddress = "";
            UserDataManager.Instance.isConnectWallet = false;
            if (GameManager.Instance.walletManager != null)
            {
                GameManager.Instance.walletManager.LoginWallet();
            }
        }

    }
    #endregion


    #region MUA
    //Thu chan cưu
    public void DepositHiredSheep(int index)
    {
     

        //Kiem tra ví
        if (UserDataManager.Instance.walletAddress.Trim().Length <= 0)
        {
            OpenConnect();
            return;
        }

        string type = "4"; //Thue chan cuu moi trai
        string data = UserDataManager.Instance.UserData.user.id + "," + type + "," + index+"";
        string addressReceiver = DataConfig.addressReceiver;
        string amount = (hirePrices * Math.Pow(10, 9)).ToString().Replace(",",".");
        BuyFarmTool(DataConfig.addressReceiver, amount, data);
        //Call js thực hien
    }

    //Mua Băng chuyền

    public void BuyLisft(double coins)
    {
        if (UserDataManager.Instance.walletAddress.Trim().Length <= 0)
        {
            OpenConnect();
            return;
        }

        //2 là băng chuyền
        string data = UserDataManager.Instance.UserData.user.id + "," + "2" + "," + "1";
        string addressReceiver = DataConfig.addressReceiver;
        string amount = (coins * Math.Pow(10, 9)).ToString().Replace(",", ".");
        BuyFarmTool(addressReceiver, amount, data);
    }
    //Mua kho
    public void BuyTruck(double coins)
    {
        if (UserDataManager.Instance.walletAddress.Trim().Length <= 0)
        {
            OpenConnect();
            return;
        }
        //3 là kho
        string data = UserDataManager.Instance.UserData.user.id + "," + "3" + "," + "1";
        string addressReceiver = DataConfig.addressReceiver;
        string amount = (coins * Math.Pow(10, 9)).ToString().Replace(",", ".");
        BuyFarmTool(addressReceiver, amount, data);
    }

    //Active trai cừu:
    public void ActiveFarm(double coins, int index)
    {

        if (UserDataManager.Instance.walletAddress.Trim().Length <= 0)
        {
            OpenConnect();
            return;
        }
        //3 là kho
        string data = UserDataManager.Instance.UserData.user.id + "," + "5" + "," + index+"";
        string addressReceiver = DataConfig.addressReceiver;
        string amount = (coins * Math.Pow(10, 9)).ToString().Replace(",", "."); ;
        BuyFarmTool(addressReceiver, amount, data);
    }

    //Mua trai cừu:
    public void UpgradeFarm(int index,double coins)
    {
        if (UserDataManager.Instance.walletAddress.Trim().Length <= 0)
        {
            OpenConnect();
            return;
        }
        //3 là kho
        string data = UserDataManager.Instance.UserData.user.id + "," + "1" + "," + index.ToString();
        string addressReceiver = DataConfig.addressReceiver;
        string amount = (coins * Math.Pow(10, 9)).ToString().Replace(",", ".");
        BuyFarmTool(addressReceiver, amount, data);
    }

    #endregion





}
