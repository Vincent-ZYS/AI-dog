/*  This file is part of the "Simple IAP System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

#if UNITY_PURCHASING && STAN_APPLE_IAP
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using SA.Common.Models;

namespace SIS
{
    /// <summary>
    /// Represents the public interface of the underlying store system via Stans Assets (for Apple App store).
    /// </summary>
    public class StanAppleAppStore : IStore
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
        public void Initialize(IStoreCallback callback)
        {
            this.callback = callback;

            IOSInAppPurchaseManager.OnStoreKitInitComplete += OnProductsRetrieved;
            IOSInAppPurchaseManager.OnTransactionComplete += OnPurchaseSucceeded;

            #if !UNITY_EDITOR
            if (!IOSInAppPurchaseManager.Instance.IsInAppPurchasesEnabled)
            {
                OnSetupFailed("Apple App Store billing is disabled.");
                return;
            }
            #endif

            definitions = IAPManager.GetProductDefinitions();
            IOSNativeSettings.Instance.InAppProducts.Clear();
            for (int i = 0; i < definitions.Length; i++)
            {
                IOSInAppPurchaseManager.Instance.AddProductId(definitions[i].storeSpecificId);

                #if UNITY_EDITOR
                    IAPObject obj = IAPManager.GetIAPObject(definitions[i].id);
                    IOSNativeSettings.Instance.InAppProducts[i].DisplayName = obj.title;
                    IOSNativeSettings.Instance.InAppProducts[i].Description = obj.description;
                    IOSNativeSettings.Instance.InAppProducts[i].LocalizedPrice = obj.realPrice;
                #endif
            }        

            IOSInAppPurchaseManager.Instance.LoadStore();              
        }


        /// <summary>
        /// Fetch the latest product metadata, including purchase receipts,
        /// asynchronously with results returned via IStoreCallback.
        /// </summary>
        public void RetrieveProducts(ReadOnlyCollection<ProductDefinition> products)
        {
            //we need to implement this because of the store interface, but we do not
            //make use of it since Stans Assets has its own callback with a different parameter
        }


        private void OnProductsRetrieved(Result result)
        {
            this.products = new Dictionary<string, ProductDescription>();

            if (!result.IsSucceeded)
            {
                OnSetupFailed(result.Error.Code + ", " + result.Error.Message);
                return;
            }

            List<IOSProductTemplate> list = IOSInAppPurchaseManager.Instance.Products;

            string globalId = null;
            string storeId = null;
            for(int i = 0; i < list.Count; i++)
            {
                storeId = list[i].Id;
                globalId = IAPManager.GetIAPIdentifier(storeId);
                if(!products.ContainsKey(globalId))
                {
					products.Add(globalId, new ProductDescription(storeId, new ProductMetadata(list[i].LocalizedPrice,
																list[i].DisplayName, list[i].Description,
                                                                list[i].CurrencyCode, (decimal)list[i].Price)));
                }
                    
                if(PlayerPrefs.HasKey(globalId))
                    products[globalId] = new ProductDescription(storeId, products[globalId].metadata, DBManager.GetReceipt(globalId), "");
            }

            callback.OnProductsRetrieved(products.Values.ToList());
        }


        /// <summary>
        /// Handle a purchase request from a user.
        /// Developer payload is provided for stores that define such a concept (Google Play).
        /// </summary>
        public void Purchase(ProductDefinition product, string developerPayload)
        {
            currentProduct = product.storeSpecificId;

            if(!IOSInAppPurchaseManager.Instance.IsStoreLoaded)
            {
                OnPurchaseFailed("Billing is not loaded on this device.", 4);
                return;
            }

            IOSInAppPurchaseManager.Instance.BuyProduct(currentProduct);
        }


        public void OnPurchaseSucceeded(IOSStoreKitResult item)
        {
            switch(item.State)
            {
                case InAppPurchaseState.Deferred:
                    return;
                case InAppPurchaseState.Restored:
                    ProductDefinition definition = definitions.FirstOrDefault(x => x.storeSpecificId == item.ProductIdentifier);
                    if (definition == null || definition.type == ProductType.Subscription || DBManager.isPurchased(definition.id))
                        return;
                    break;
                case InAppPurchaseState.Failed:
                    OnPurchaseFailed(item.Error.Message, item.Error.Code);
                    return;
            }

            string transactionId = item.TransactionIdentifier;
            #if UNITY_EDITOR
                //allow for multiple test purchases with unique transactions
                transactionId = (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds.ToString();
            #endif

            callback.OnPurchaseSucceeded(item.ProductIdentifier, item.Receipt, transactionId);
        }


        /// <summary>
        /// Called by Unity Purchasing when a transaction has been recorded.
        /// Store systems should perform any housekeeping here, such as closing transactions or consuming consumables.
        /// </summary>
        public void FinishTransaction(ProductDefinition product, string transactionId)
        {
            //nothing to do here
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
                case 2:
                    reason = PurchaseFailureReason.UserCancelled;
                    break;
                case 4:
                case 7:
                    reason = PurchaseFailureReason.PurchasingUnavailable;
                    break;
                case 5:
                case 6:
                    reason = PurchaseFailureReason.ProductUnavailable;
                    break;
                case 1:
                    reason = PurchaseFailureReason.SignatureInvalid;
                    break;
                case 3:
                    reason = PurchaseFailureReason.ExistingPurchasePending;
                    break;
            }

            callback.OnPurchaseFailed(new PurchaseFailureDescription(currentProduct, reason, error));
        }
    }
}
#endif