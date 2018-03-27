/*  This file is part of the "Simple IAP System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SIS
{
    public class PluginSetupEditor : EditorWindow
    {
        private bool isChanged = false;

        private AndroidPlugin androidPlugin = AndroidPlugin.UnityIAP;
        private string[] androidPluginNames = new string[] { "", "OCULUS_GEAR_IAP", "VOXEL_GOOGLE_IAP", "STAN_GOOGLE_IAP" };
        private enum AndroidPlugin
        {
            UnityIAP = 0,
			GearVR = 1
            //VoxelBusters = 1,
            //StansAssets = 2,
        }

        /*
        private AppleAppPlugin appleAppPlugin = AppleAppPlugin.UnityIAP;
        private string[] appleAppPluginNames = new string[] { "", "VOXEL_APPLE_IAP", "STAN_APPLE_IAP" };
        private enum AppleAppPlugin
        {
            UnityIAP = 0,
            VoxelBusters = 1,
            StansAssets = 2
        }
        */

        private DesktopPlugin desktopPlugin = DesktopPlugin.UnityIAP;
        private string[] desktopPluginNames = new string[] { "", "PLAYFAB_PAYPAL", "PLAYFAB_STEAM", "OCULUS_RIFT_IAP" };
        private enum DesktopPlugin
        {
            UnityIAP = 0,
            PlayfabPaypal = 1,
            PlayfabSteam = 2,
            OculusRift = 3
        }

        private WebPlugin webPlugin = WebPlugin.UnityIAP;
        private string[] webPluginNames = new string[] { "", "PLAYFAB_PAYPAL" };
        private enum WebPlugin
        {
            UnityIAP = 0,
            PlayfabPaypal = 1
        }

        private PlayfabPlugin playfabPlugin = PlayfabPlugin.Disabled;
        private string[] playfabPluginNames = new string[] { "", "PLAYFAB_VALIDATION", "PLAYFAB" };
        private enum PlayfabPlugin
        {
            Disabled = 0,
            ValidationOnly = 1,
            FullSuite = 2
        }


        [MenuItem("Window/Simple IAP System/Plugin Setup", false, 0)]
        static void Init()
        {
            EditorWindow.GetWindowWithRect(typeof(PluginSetupEditor), new Rect(0, 0, 350, 200), false, "Plugin Setup");
        }


        void OnEnable()
        {
            androidPlugin = (AndroidPlugin)FindScriptingDefineIndex(BuildTargetGroup.Android, androidPluginNames);
            desktopPlugin = (DesktopPlugin)FindScriptingDefineIndex(BuildTargetGroup.Standalone, desktopPluginNames);
            webPlugin = (WebPlugin)FindScriptingDefineIndex(BuildTargetGroup.WebGL, webPluginNames);

            //check if cross-platform use exists
            playfabPlugin = (PlayfabPlugin)FindScriptingDefineIndex(BuildTargetGroup.Android, playfabPluginNames);
        }


        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Billing Plugin Setup", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Choose the store plugins you would like to use per platform:");
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            androidPlugin = (AndroidPlugin)EditorGUILayout.EnumPopup("Android", androidPlugin);
            desktopPlugin = (DesktopPlugin)EditorGUILayout.EnumPopup("Standalone", desktopPlugin);
            webPlugin = (WebPlugin)EditorGUILayout.EnumPopup("WebGL", webPlugin);

            GUILayout.Space(15);
            playfabPlugin = (PlayfabPlugin)EditorGUILayout.EnumPopup("PlayFab Service", playfabPlugin);

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Unity IAP will be used by default for all other platforms.");

            if (EditorGUI.EndChangeCheck())
            {
                isChanged = true;
            }

            if (isChanged)
                GUI.color = Color.yellow;

            if (GUILayout.Button("Apply"))
            {
                ApplyScriptingDefines();
                //SetPluginPlatformSettings();
                isChanged = false;
            }
        }


        void ApplyScriptingDefines()
        {
            SetScriptingDefine(BuildTargetGroup.Android, androidPluginNames, (int)androidPlugin);
            SetScriptingDefine(BuildTargetGroup.Standalone, desktopPluginNames, (int)desktopPlugin);
            SetScriptingDefine(BuildTargetGroup.WebGL, webPluginNames, (int)webPlugin);

            /*
            SetScriptingDefine(BuildTargetGroup.WP8, winRTPluginNames, (int)winRTPlugin);
            SetScriptingDefine(BuildTargetGroup.WSA, winRTPluginNames, (int)winRTPlugin);
            */

            BuildTargetGroup[] playfabTargets = new BuildTargetGroup[] { BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.tvOS,
                                                                         BuildTargetGroup.Standalone, BuildTargetGroup.WebGL, BuildTargetGroup.Facebook };

            for(int i = 0; i < playfabTargets.Length; i++)
                SetScriptingDefine(playfabTargets[i], playfabPluginNames, (int)playfabPlugin);
        }


        void SetScriptingDefine(BuildTargetGroup target, string[] oldDefines, int newDefine)
        {
            string str = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
            List<string> defs = new List<string>(str.Split(';'));
            if (defs.Count == 0 && !string.IsNullOrEmpty(str)) defs.Add(str);

            for (int i = 0; i < oldDefines.Length; i++)
            {
                if (string.IsNullOrEmpty(oldDefines[i])) continue;
                defs.Remove(oldDefines[i]);
            }

            if(newDefine > 0)
                defs.Add(oldDefines[newDefine]);

            str = "";
            for(int i = 0; i < defs.Count; i++)
                str = defs[i] + ";" + str;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, str);
        }


        int FindScriptingDefineIndex(BuildTargetGroup group, string[] names)
        {
            string str = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);

            for (int i = 1; i < names.Length; i++)
            {
                if (str.Contains(names[i]))
                {
                    return i;
                }
            }

            return 0;
        }


        /*
        void SetPluginPlatformSettings()
        {
            string[] dirs = Directory.GetDirectories(Application.dataPath, "UnityPurchasing", SearchOption.AllDirectories);
            string billingPath = string.Empty;
            if(dirs.Length > 0) billingPath = dirs[0].Replace(Application.dataPath, "Assets");
            if(string.IsNullOrEmpty(billingPath)) return;

            switch(googlePlayPlugin)
            {
                case GooglePlayPlugin.VoxelBusters:
                    (PluginImporter.GetAtPath(billingPath + "/Bin/Android/GoogleAIDL.aar") as PluginImporter).SetCompatibleWithPlatform(BuildTarget.Android, false);
                    (PluginImporter.GetAtPath(billingPath + "/Bin/Android/AmazonAppStore.aar") as PluginImporter).SetCompatibleWithPlatform(BuildTarget.Android, false);
                    break;
                case GooglePlayPlugin.StansAssets:
                    (PluginImporter.GetAtPath(billingPath + "/Bin/Android/GoogleAIDL.aar") as PluginImporter).SetCompatibleWithPlatform(BuildTarget.Android, false);
                    break;
                default:
                    (PluginImporter.GetAtPath(billingPath + "/Bin/Android/GoogleAIDL.aar") as PluginImporter).SetCompatibleWithPlatform(BuildTarget.Android, true);
                    (PluginImporter.GetAtPath(billingPath + "/Bin/Android/AmazonAppStore.aar") as PluginImporter).SetCompatibleWithPlatform(BuildTarget.Android, true);
                    break;
            }
        }
        */
    }
}
