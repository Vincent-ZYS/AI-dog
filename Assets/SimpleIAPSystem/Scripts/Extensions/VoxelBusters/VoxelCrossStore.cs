/*  This file is part of the "Simple IAP System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

#if UNITY_PURCHASING && (VOXEL_GOOGLE_IAP || VOXEL_APPLE_IAP)
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using VoxelBusters.NativePlugins;

namespace SIS
{
    /// <summary>
    /// Represents the public interface of the underlying store system implemented via Voxel Busters (for Google Play or Apple App Store).
    /// </summary>
    public class VoxelCrossStore : IStore
    {      
        /// <summary>
        /// Callback for hooking into the native Unity IAP logic.
        /// </summary>
        public IStoreCallback callback;

        /// <summary>
        /// List of products which are declared and retrieved by the billing system.
        /// </summary>
        public Dictionary<string, ProductDescription> products;
        
        //keeping track of the product that is currently being processed
        private string currentProduct = "";


        /// <summary>
        /// Initialize the instance using the specified IStoreCallback.
        /// </summary>
        public virtual void Initialize(IStoreCallback callback)
        {
            this.callback = callback;

            Billing.DidFinishRequestForBillingProductsEvent += OnProductsRetrieved;
            Billing.DidFinishProductPurchaseEvent += OnPurchaseSucceeded;
            Billing.DidFinishRestoringPurchasesEvent += OnPurchaseSucceeded;

            #if UNITY_ANDROID
            if(string.IsNullOrEmpty(NPSettings.Billing.Android.PublicKey))
            {
                Debug.LogWarning("Google Store Public Key missing in VoxelBuster Native Settings. "
                               + "Purchases for real money won't be supported on the device.");
            }
            #endif
        }


        //converting Unity IAP product definitions into the VoxelBuster format
        private BillingProduct ConvertProduct(ProductDefinition product)
		{
            IAPObject obj = IAPManager.GetIAPObject(product.id);
            if (obj == null) return null;

            List<VoxelBusters.NativePlugins.PlatformID> platformIds = new List<VoxelBusters.NativePlugins.PlatformID>();
            platformIds.Add(VoxelBusters.NativePlugins.PlatformID.Editor(product.id));

			#if UNITY_ANDROID
				platformIds.Add(VoxelBusters.NativePlugins.PlatformID.Android(product.storeSpecificId));
			#elif UNITY_IOS || UNITY_TVOS
                platformIds.Add(VoxelBusters.NativePlugins.PlatformID.IOS(product.storeSpecificId));
			#endif

			return BillingProduct.Create(obj.title, product.type == ProductType.Consumable ? true : false, platformIds.ToArray());
		}


        /// <summary>
        /// Fetch the latest product metadata, including purchase receipts,
        /// asynchronously with results returned via IStoreCallback.
        /// </summary>
        public void RetrieveProducts(ReadOnlyCollection<ProductDefinition> products)
        {
            this.products = new Dictionary<string, ProductDescription>();
            List<BillingProduct> billingProducts = new List<BillingProduct>();
            for (int i = 0; i < products.Count; i++)
            {
                BillingProduct product = ConvertProduct(products[i]);
                if(product != null)
                    billingProducts.Add(product);
            }

			NPBinding.Billing.RequestForBillingProducts(billingProducts.ToArray());
        }


        //returning products retrieved by the billing system back to Unity IAP
        private void OnProductsRetrieved(BillingProduct[] list, string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                OnSetupFailed(error);
                return;
            }

            string itemId = null;
            for(int i = 0; i < list.Length; i++)
            {
                itemId = list[i].ProductIdentifier;

                if(!products.ContainsKey(itemId))
                {
					products.Add(itemId, new ProductDescription(itemId, new ProductMetadata(list[i].LocalizedPrice,
																list[i].Name, list[i].Description,
                                                                list[i].CurrencyCode, (decimal)list[i].Price)));
                }
                    
                if(PlayerPrefs.HasKey(itemId))
                    products[itemId] = new ProductDescription(itemId, products[itemId].metadata, DBManager.GetReceipt(itemId), "");
                
                //auto restore products in case database does not match
                #if UNITY_ANDROID
                string globalId = IAPManager.GetIAPIdentifier(itemId);
                if (NPBinding.Billing.IsProductPurchased(list[i]) && DBManager.GetPurchase(globalId) == 0)
                    DBManager.SetPurchase(globalId);
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

            if(!NPBinding.Billing.IsAvailable() || !NPBinding.Billing.CanMakePayments())
            {
                OnPurchaseFailed("Billing is not enabled on this device.", 2);
                return;
            }

            NPBinding.Billing.BuyProduct(NPBinding.Billing.GetStoreProduct(currentProduct));
        }


        /// <summary>
        /// 
        /// </summary>
        public void OnPurchaseSucceeded(BillingTransaction item)
        {
            if(item == null || !string.IsNullOrEmpty(item.Error))
            {
                OnPurchaseFailed(item == null ? "Unkown Billing Error." : item.Error, 1);
                return;
            }

            string transactionId = item.TransactionIdentifier;
            #if UNITY_EDITOR
                //allow for multiple test purchases with unique transactions
                transactionId = (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds.ToString();
            #endif

            callback.OnPurchaseSucceeded(item.ProductIdentifier, item.TransactionReceipt, transactionId);
        }


        // fired when a restore succeeds.
        // calls PurchaseSucceeded with each transaction seperately.
        private void OnPurchaseSucceeded(BillingTransaction[] transactions, string error)
        {
            if (transactions == null) return;
            else if(!string.IsNullOrEmpty(error))
            {
                OnPurchaseFailed(error, 0);
                return;
            }
        
            foreach (BillingTransaction t in transactions)
            {
                OnPurchaseSucceeded(t);
            }
        }


        /// <summary>
        /// Called by Unity Purchasing when a transaction has been recorded.
        /// Store systems should perform any housekeeping here, such as closing transactions or consuming consumables.
        /// </summary>
        public virtual void FinishTransaction(ProductDefinition product, string transactionId)
        {
            //nothing to do here
        }


        /// <summary>
        /// 
        /// </summary>
        public void OnSetupFailed(string error)
        {
            callback.OnSetupFailed(InitializationFailureReason.NoProductsAvailable);
        }


        /// <summary>
        /// 
        /// </summary>
        public void OnPurchaseFailed(string error, int code)
        {
            if(error.Contains("response:"))
            {
                int startAt = error.IndexOf("response:") + 10;
                int lastAt = error.IndexOf(":", startAt);
                int.TryParse(error.Substring(startAt, lastAt - startAt), out code);
            }

            PurchaseFailureReason reason = PurchaseFailureReason.Unknown;
            switch(code)
            {
                case 1:
                case -1005:
                    reason = PurchaseFailureReason.UserCancelled;
                    break;
                case 2:
                case 3:
                case -1001:
                case -1009:
                    reason = PurchaseFailureReason.PurchasingUnavailable;
                    break;
                case 4:
                    reason = PurchaseFailureReason.ProductUnavailable;
                    break;
                case 5:
                case -1003:
                case -1004:
                case -1007:
                    reason = PurchaseFailureReason.SignatureInvalid;
                    break;
                case 7:
                    reason = PurchaseFailureReason.ExistingPurchasePending;
                    break;
            }

            callback.OnPurchaseFailed(new PurchaseFailureDescription(currentProduct, reason, error));
        }
    }
}
#endif