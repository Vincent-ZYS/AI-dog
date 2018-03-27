/*  This file is part of the "Simple IAP System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace SIS
{
    /// <summary>
    /// Custom Unity IAP purchasing module for overwriting default store subsystems.
    /// </summary>
    public class SISPurchasingModule : AbstractPurchasingModule, IStoreConfiguration
    {
        public override void Configure()
		{
            /*
			//Voxel Busters
            #if VOXEL_GOOGLE_IAP
                RegisterStore(GooglePlay.Name, new VoxelCrossStore());
            #endif
                        
            #if VOXEL_APPLE_IAP
                base.BindExtension<IAppleExtensions>(new VoxelAppleExtensions());   
                RegisterStore(AppleAppStore.Name, new VoxelCrossStore());
            #endif
			
			//Stans Assets
			#if STAN_GOOGLE_IAP
                RegisterStore(GooglePlay.Name, new StanGooglePlayStore());
            #endif
                        
            #if STAN_APPLE_IAP
                base.BindExtension<IAppleExtensions>(new StanAppleExtensions());   
                RegisterStore(AppleAppStore.Name, new StanAppleAppStore());
            #endif
            */

            //PlayFab
            #if PLAYFAB_PAYPAL
                RegisterStore("PayPal", new PlayfabPayPalStore());
            #endif

            #if PLAYFAB_STEAM
                RegisterStore("SteamStore", new PlayfabSteamStore());
            #endif

            #if UNITY_FACEBOOK && PLAYFAB
                //RegisterStore(FacebookStore.Name, new PlayfabFacebookStore());
            #endif

            //VR
            #if OCULUS_RIFT_IAP || OCULUS_GEAR_IAP
                RegisterStore("OculusStore", new OculusStore());
            #endif
        }
    }
}
#endif