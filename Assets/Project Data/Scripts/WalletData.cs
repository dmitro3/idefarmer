using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
//using TonSdk.Connect;
using Unity.VisualScripting;
using UnityEngine;

public class WalletApp
{
    public int index { get; set; }
    public string Name { get; set; }

    public string Image { get; set; }

    public string AboutUrl { get; set; }

    public string BridgeUrl { get; set; }

    public string JsBridgeKey { get; set; }

    public string UniversalUrl { get; set; }

    public string AppName { get; set; }
    public WalletApp()
    {


    }
   
}

public class WalletData : MonoBehaviour
{
   
    public List<WalletApp> Wallets=new List<WalletApp>();
    


    // Start is called before the first frame update
    void Start()
    {
       


    }
    public void ConnectWallet(int index)
    {


    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
