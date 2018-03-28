/*  This file is part of the "Simple IAP System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

#if UNITY_PURCHASING && PLAYFAB_PAYPAL
using System;
using UnityEngine;

using UnityEngine.Purchasing.Extension;
using PlayFab;
using PlayFab.ClientModels;

namespace SIS
{
    /// <summary>
    /// Store implementation for PayPal, based on the PlayfabStore class.
    /// </summary>
    public class PlayfabPayPalStore : PlayfabStore
    {
        /// <summary>
        /// Reference to this store class, since the user needs to confirm the purchase
        /// transaction manually in-game, thus calling the confirm method of this script.
        /// </summary>
        public static PlayfabPayPalStore instance { get; private set; }

        
        /// <summary>
        /// Setting this store reference on initialization.
        /// </summary>
        public PlayfabPayPalStore()
        {
            instance = this;
        }


        /// <summary>
        /// Overriding the initialization with setting the correct store.
        /// </summary>
        public override void Initialize(IStoreCallback callback)
        {
            storeId = "PayPal";
            this.callback = callback;
        }


        /// <summary>
        /// Overriding the payment request for opening the PayPal website in the browser.
        /// </summary>
        public override void OnPurchaseResult(PayForPurchaseResult result)
        {
            ShopManager.ShowConfirmation();

            Application.OpenURL(result.PurchaseConfirmationPageURL);
        }


        /// <summary>
        /// Manually triggering purchase confirmation after a PayPal payment has been made.
        /// This is so that the transaction gets finished and PayPal actually substracts funds.
        /// </summary>
        public void ConfirmPurchase()
        {
            if (string.IsNullOrEmpty(orderId))
                return;

            ConfirmPurchaseRequest request = new ConfirmPurchaseRequest()
            {
                OrderId = orderId
            };

            PlayFabClientAPI.ConfirmPurchase(request, (result) => 
            {
                if (ShopManager.GetInstance() != null && ShopManager.GetInstance().confirmWindow != null)
                    ShopManager.GetInstance().confirmWindow.SetActive(false);

                OnPurchaseSucceeded(result);
            }, OnPurchaseFailed);
        }
    }
}
#endif