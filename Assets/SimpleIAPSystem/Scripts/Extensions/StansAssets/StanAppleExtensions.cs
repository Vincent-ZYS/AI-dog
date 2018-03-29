/*  This file is part of the "Simple IAP System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

#if UNITY_PURCHASING && STAN_APPLE_IAP
using System;
using UnityEngine.Purchasing;
using SA.Common.Models;

namespace SIS
{
    public class StanAppleExtensions : IAppleExtensions
    {
        private static Action<Action<bool>> restoreCallback;


        public StanAppleExtensions()
        {
            IOSInAppPurchaseManager.OnRestoreComplete += OnRestoreSucceeded;
            ISN_Security.OnReceiptLoaded += OnReceiptSucceeded;
            ISN_Security.OnReceiptRefreshComplete += OnReceiptRefreshSucceeded;
        }


        public void RefreshAppReceipt(Action<string> successCallback, Action errorCallback)
        {
            ISN_Security.Instance.RetrieveLocalReceipt();
        }


        private void OnReceiptSucceeded(ISN_LocalReceiptResult result)
        {
            if(result.Receipt == null)
            {
                ISN_Security.Instance.StartReceiptRefreshRequest();
                return;
            }
        }


        private void OnReceiptRefreshSucceeded(Result result)
        {
            if (result == null || !result.IsSucceeded)
            {
                return;
            }

            RefreshAppReceipt(null, null);
        }


        public void RegisterPurchaseDeferredListener(Action<Product> callback)
        {
            
        }

        
        public void RestoreTransactions(Action<bool> callback)
        {
            IOSInAppPurchaseManager.Instance.RestorePurchases();
        }


        private void OnRestoreSucceeded(Result result)
        {
            if (result == null || !result.IsSucceeded)
            {
                IAPManager.OnTransactionsRestored(false);
                return;
            }

            IAPManager.OnTransactionsRestored(true);
        }
    }
}
#endif

