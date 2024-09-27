using System;
using UnityEngine;

using UnityEngine.UI;

namespace Project_Data.Scripts
{
    public class Purchaser : MonoBehaviour
    {
      //  private static IStoreController m_StoreController;          // The Unity Purchasing system.
        //private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    
        public string kProductIDPack1 = "com.company.gem1"; // 1k gems $0.99
        public string kProductIDPack2 = "com.company.gem2"; // 15k gems $2.99
        public string kProductIDPack3 = "com.company.gem3"; // 50k gems $4.99
        public string kProductIDPack4 = "com.company.gem4"; // 100k gems $9.99

        public ShopManager storeHUD, successHUD;
        public Text successAmountOfGemsText;

        private bool _isPricesLocalized;

        void Start()
        {
            // If we haven't set up the Unity Purchasing reference
            //if (m_StoreController == null)
            //{
            //    // Begin to configure our connection to Purchasing
            //    InitializePurchasing();
            //}
        }

        private void Update()
        {
            //if(IsInitialized() && !_isPricesLocalized)
            //{
            //    InitLocalizePrices();
            //}

            if(!_isPricesLocalized)
            {
                UpdateLocalizedPrices();
            }
        }

        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            //if (IsInitialized())
            //{
            //    // ... we are done here.
            //    return;
            //}

            //var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            //builder.AddProduct(kProductIDPack1, ProductType.NonConsumable);
            //builder.AddProduct(kProductIDPack2, ProductType.Consumable);
            //builder.AddProduct(kProductIDPack3, ProductType.Consumable);
            //builder.AddProduct(kProductIDPack4, ProductType.Consumable);

            //UnityPurchasing.Initialize(this, builder);
        }

        //private bool IsInitialized()
        //{
        //    // Only say we are initialized if both the Purchasing references are set.
        //    return m_StoreController != null && m_StoreExtensionProvider != null;
        //}

        private bool _canClickInAppBtn = true;
        public void BuyProductID(string productId)
        {
            if (_canClickInAppBtn)
            {
                if (productId.Contains("gem1"))
                    productId = kProductIDPack1;
                else if (productId.Contains("gem2"))
                    productId = kProductIDPack2;
                else if (productId.Contains("gem3"))
                    productId = kProductIDPack3;
                else if (productId.Contains("gem4"))
                    productId = kProductIDPack4;
            
                _canClickInAppBtn = false;
                Invoke("ResetCanClickInAppBtnFlag", 1);
            
                if(productId == kProductIDPack1)
                    AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.InAppShopIClickedItem, 1);
                else if(productId == kProductIDPack2)
                    AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.InAppShopIClickedItem, 2);
                else if(productId == kProductIDPack3)
                    AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.InAppShopIClickedItem, 3);
                else if(productId == kProductIDPack4)
                    AnalyticsManager.Instance.SendEvent(CustomAnalyticsEvent.InAppShopIClickedItem, 4);

                //// If Purchasing has been initialized ...
                //if (IsInitialized())
                //{
                //    // ... look up the Product reference with the general product identifier and the Purchasing 
                //    // system's products collection.
                //    Product product = m_StoreController.products.WithID(productId);

                //    // If the look up found a product for this device's store and that product is ready to be sold ... 
                //    if (product != null && product.availableToPurchase)
                //    {
                //        Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                //        // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                //        // asynchronously.
                //        m_StoreController.InitiatePurchase(product);
                //    }
                //    // Otherwise ...
                //    else if (product == null)
                //    {
                //        Debug.Log("product is null!!!");
                //    }
                //    else if (!product.availableToPurchase)
                //    {
                //        // ... report the product look-up failure situation  
                //        Debug.Log(
                //            "BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                //    }
                //}
                //// Otherwise ...
                //else
                //{
                //    // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                //    // retrying initiailization.
                //    Debug.Log("BuyProductID FAIL. Not initialized.");
                //}
            }
        }

        void ResetCanClickInAppBtnFlag()
        {
            _canClickInAppBtn = true;
        }

        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            //if (!IsInitialized())
            //{
            //    // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            //    Debug.Log("RestorePurchases FAIL. Not initialized.");
            //    return;
            //}

            //// If we are running on an Apple device ... 
            //if (Application.platform == RuntimePlatform.IPhonePlayer ||
            //    Application.platform == RuntimePlatform.OSXPlayer)
            //{
            //    // ... begin restoring purchases
            //    Debug.Log("RestorePurchases started ...");

            //    // Fetch the Apple store-specific subsystem.
            //    var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            //    // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            //    // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            //    apple.RestoreTransactions((result) => {
            //        // The first phase of restoration. If no more responses are received on ProcessPurchase then 
            //        // no purchases are available to be restored.
            //        Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            //    });
            //}
            //// Otherwise ...
            //else
            //{
            //    // We are not running on an Apple device. No work is necessary to restore purchases.
            //    Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            //}
        }

        //  
        // --- IStoreListener
        //

        //public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        //{
        //    // Purchasing has succeeded initializing. Collect our Purchasing references.
        //    Debug.Log("OnInitialized: PASS");

        //    // Overall Purchasing system, configured with products for this application.
        //    m_StoreController = controller;
        //    // Store specific subsystem, for accessing device-specific store features.
        //    m_StoreExtensionProvider = extensions;
        //}

        //public void OnInitializeFailed(InitializationFailureReason error)
        //{
        //    // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        //    Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        //    //purchaseLoadingIndicatorGO.SetActive(false);
        //}

        //public void OnInitializeFailed(InitializationFailureReason error, string message)
        //{
        //    throw new NotImplementedException();
        //}

        //public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        //{
        //    storeHUD.closeBtn();
        //    successHUD.openPanel();

        //    if (String.Equals(args.purchasedProduct.definition.id, kProductIDPack1, StringComparison.Ordinal))
        //    {
        //        GameManager.Instance.addBags(1000);
        //        successAmountOfGemsText.text = "+1000";
        //    }
        //    else if (String.Equals(args.purchasedProduct.definition.id, kProductIDPack2, StringComparison.Ordinal))
        //    {
        //        GameManager.Instance.addBags(15000);
        //        successAmountOfGemsText.text = "+15000";
        //    }
        //    else if (String.Equals(args.purchasedProduct.definition.id, kProductIDPack3, StringComparison.Ordinal))
        //    {
        //        GameManager.Instance.addBags(50000);
        //        successAmountOfGemsText.text = "+50000";
        //    }
        //    else if (String.Equals(args.purchasedProduct.definition.id, kProductIDPack4, StringComparison.Ordinal))
        //    {
        //        GameManager.Instance.addBags(100000);
        //        successAmountOfGemsText.text = "+100000";
        //    }

        //    return PurchaseProcessingResult.Complete;
        //}

        //public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        //{
        //    // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        //    // this reason with the user to guide their troubleshooting actions.
        //    Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        //}

        public Text pack1Text, pack2Text, pack3Text, pack4Text;
        private string pack1Price, pack2Price, pack3Price, pack4Price;

        //void InitLocalizePrices()
        //{
        //    _isPricesLocalized = true;

        //    for (int i = 0; i < m_StoreController.products.all.Length; i++)
        //    {
        //        if (m_StoreController.products.all[i].definition.id.Contains(kProductIDPack1))
        //        {
        //            pack1Price = m_StoreController.products.all[i].metadata.localizedPriceString;
        //            pack1Price.Replace(",00", "");
        //        }
        //        else if (m_StoreController.products.all[i].definition.id.Contains(kProductIDPack2))
        //        {
        //            pack2Price = m_StoreController.products.all[i].metadata.localizedPriceString;
        //            pack2Price.Replace(",00", "");
        //        }
        //        else if (m_StoreController.products.all[i].definition.id.Contains(kProductIDPack3))
        //        {
        //            pack3Price = m_StoreController.products.all[i].metadata.localizedPriceString;
        //            pack3Price.Replace(",00", "");
        //        }
        //        else if (m_StoreController.products.all[i].definition.id.Contains(kProductIDPack4))
        //        {
        //            pack4Price = m_StoreController.products.all[i].metadata.localizedPriceString;
        //            pack4Price.Replace(",00", "");
        //        }
        //    }
        //}

        public void UpdateLocalizedPrices()
        {
            if (pack1Price != "") pack1Text.text = pack1Price;
            else
            {
               // InitLocalizePrices();
                pack1Text.text = "$0.99";
            }

            if (pack2Price != "") pack2Text.text = pack2Price;
            else pack2Text.text = "$2.99";

            if (pack3Price != "") pack3Text.text = pack3Price;
            else pack3Text.text = "$4.99";

            if (pack4Price != "") pack4Text.text = pack4Price;
            else pack4Text.text = "$9.99";
        }
    }
}
