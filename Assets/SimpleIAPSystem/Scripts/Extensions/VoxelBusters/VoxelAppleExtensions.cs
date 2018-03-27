/*  This file is part of the "Simple IAP System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

#if UNITY_PURCHASING && VOXEL_APPLE_IAP
using System;
using UnityEngine.Purchasing;

namespace SIS
{
    public class VoxelAppleExtensions : IAppleExtensions
    {
        public void RefreshAppReceipt(Action<string> successCallback, Action errorCallback)
        {
            
        }

        
        public void RegisterPurchaseDeferredListener(Action<Product> callback)
        {
            
        }

        
        public void RestoreTransactions(Action<bool> callback)
        {
            NPBinding.Billing.RestorePurchases();
            
            callback(true);
        }
    }
}
#endif

