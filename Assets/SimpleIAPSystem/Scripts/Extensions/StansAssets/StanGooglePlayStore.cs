/*  This file is part of the "Simple IAP System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

#if UNITY_PURCHASING && STAN_GOOGLE_IAP
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace SIS
{
    /// <summary>
    /// Represents the public interface of the underlying store system via Stans Assets (for Google Play).
    /// </summary>
    public class StanGooglePlayStore : IStore
    {      
        /// <summary>
        /// Callback for hooking into the native Unity IAP logic.
        /// </summary>
        public IStoreCallback callback;

        /// <summary>
        /// List of products which are declared and retrieved by the billing system.
        /// </summary>
        public Dictionary<string, ProductDescription> products;

        //caching product definitions that should be populated into Stans Assets
        private ProductDefinition[] definitions;
        //keeping track of the product that is currently being processed
        private string currentProduct = "";


        /// <summary>
        /// Initialize the instance using the specified IStoreCallback.
        /// </summary>
        public virtual void Initialize(IStoreCallback callback)
        {
            this.callback = callback;

            AndroidInAppPurchaseManager.ActionBillingSetupFinished += OnInitialized;
            AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += OnProductsRetrieved;
            AndroidInAppPurchaseManager.ActionProductPurchased += OnPurchaseSucceeded;
            AndroidInAppPurchaseManager.ActionProductConsumed += OnTransactionFinished;

            if(!AndroidNativeSettings.Instance.IsBase64KeyWasReplaced)
            {
                Debug.LogWarning("Google Store Public Key missing in Android Native Settings. "
                               + "Purchases for real money won't be supported on the device.");
            }

            definitions = IAPManager.GetProductDefinitions();
            AndroidNativeSettings.Instance.InAppProducts.Clear();
            for (int i = 0; i < definitions.Length; i++)
            {
                AndroidInAppPurchaseManager.Client.AddProduct(definitions[i].storeSpecificId);

                #if UNITY_EDITOR
                    IAPObject obj = IAPManager.GetIAPObject(definitions[i].id);
                    AndroidNativeSettings.Instance.InAppProducts[i].Title = obj.title;
                    AndroidNativeSettings.Instance.InAppProducts[i].Description = obj.description;
                    AndroidNativeSettings.Instance.InAppProducts[i].LocalizedPrice = obj.realPrice;
                #endif
            }

            AndroidInAppPurchaseManager.Client.Connect();
        }


		private void OnInitialized(BillingResult result)
		{
            this.products = new Dictionary<string, ProductDescription>();

            if (!result.IsSuccess)
            {
                OnSetupFailed(result.Response + ", " + result.Message);
                return;
            }

    		AndroidInAppPurchaseManager.Client.RetrieveProducDetails();
		}


        /// <summary>
        /// Fetch the latest product metadata, including purchase receipts,
        /// asynchronously with results returned via IStoreCallback.
        /// </summary>
        public void RetrieveProducts(ReadOnlyCollection<ProductDefinition> products)
        {
        }


        private void OnProductsRetrieved(BillingResult result)
        {
            if (!result.IsSuccess)
            {
                OnSetupFailed(result.Response + ", " + result.Message);
                return;
            }

            AndroidInventory inventory = AndroidInAppPurchaseManager.Client.Inventory;
            List<GoogleProductTemplate> list = inventory.Products;
            List<GooglePurchaseTemplate> purchases =  inventory.Purchases;

            string globalId = null;
            string storeId = null;
            for(int i = 0; i < list.Count; i++)
            {
                storeId = list[i].SKU;
                globalId = IAPManager.GetIAPIdentifier(storeId);

                if(!products.ContainsKey(globalId))
                {
					products.Add(globalId, new ProductDescription(storeId, new ProductMetadata(list[i].LocalizedPrice,
																list[i].Title, list[i].Description,
                                                                list[i].PriceCurrencyCode, (decimal)list[i].Price)));
                }

                //check for non-consumed consumables
                if(storeId in purchases)
                {
                    IAPObject obj = IAPManager.GetIAPObject(IAPManager.GetIAPIdentifier(purchases[i].SKU));
                    if (obj != null && obj.type == ProductType.Consumable)
                    {
                        AndroidInAppPurchaseManager.Client.Consume(purchases[i].SKU);
                        continue;
                    }
                }

                bool storeResult = inventory.IsProductPurchased(storeId);
                if(storeResult == true)
                    products[globalId] = new ProductDescription(storeId, products[globalId].metadata, inventory.GetPurchaseDetails(storeId).Token, inventory.GetPurchaseDetails(storeId).OrderId);

                #if !UNITY_EDITOR
                //auto restore products in case database does not match
                if (storeResult != DBManager.isPurchased(globalId))
                {
                    if (storeResult) DBManager.SetToPurchased(globalId);
                    else DBManager.RemovePurchased(globalId);
                }
                #endif
            }

            callback.OnProductsRetrieved(products.Values.ToList());
        }


        /// <summary>
        /// Handle a purchase request from a user.
        /// Developer payload is provided for stores that define such a concept (Google Play).
        /// </summary>
        public virtual void Purchase(ProductDefinition product, string developerPayload)
        {
            currentProduct = product.storeSpecificId;

            if(!AndroidInAppPurchaseManager.Client.IsConnected)
            {
                OnPurchaseFailed("Billing is not loaded on this device.", 3);
                return;
            }

            AndroidInAppPurchaseManager.Client.Purchase(currentProduct);
        }


        public void OnPurchaseSucceeded(BillingResult item)
        {
            if(!item.IsSuccess)
            {
                OnPurchaseFailed(item.Message, item.Response);
                return;
            }

            string transactionId = item.Purchase.OrderId;
            #if UNITY_EDITOR
                //allow for multiple test purchases with unique transactions
                transactionId = (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds.ToString();
            #endif

            callback.OnPurchaseSucceeded(item.Purchase.SKU, item.Purchase.OriginalJson, transactionId);
        }


        /// <summary>
        /// Called by Unity Purchasing when a transaction has been recorded.
        /// Store systems should perform any housekeeping here, such as closing transactions or consuming consumables.
        /// </summary>
        public virtual void FinishTransaction(ProductDefinition product, string transactionId)
        {
            if (product.type != ProductType.Consumable)
                return;

            AndroidInAppPurchaseManager.Client.Consume(product.storeSpecificId);
        }


        private void OnTransactionFinished(BillingResult item)
        {
        }


        public void OnSetupFailed(string error)
        {
            callback.OnSetupFailed(InitializationFailureReason.NoProductsAvailable);
        }


        public void OnPurchaseFailed(string error, int code)
        {
            PurchaseFailureReason reason = PurchaseFailureReason.Unknown;
            switch(code)
            {
                case 1:
                case -1005:
                    reason = PurchaseFailureReason.UserCancelled;
                    break;
                case 3:
                case -1009:
                    reason = PurchaseFailureReason.PurchasingUnavailable;
                    break;
                case 4:
                case -1006:
                    reason = PurchaseFailureReason.ProductUnavailable;
                    break;
                case 5:
                case -1002:
                case -1003:
                case -1004:
                    reason = PurchaseFailureReason.SignatureInvalid;
                    break;
                case 7:
                case -1010:
                    reason = PurchaseFailureReason.ExistingPurchasePending;
                    break;
            }

            callback.OnPurchaseFailed(new PurchaseFailureDescription(currentProduct, reason, error));
        }
    }
}
#endif