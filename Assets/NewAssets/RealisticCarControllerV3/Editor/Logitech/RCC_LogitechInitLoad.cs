//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2019 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEngine;
using UnityEditor;

public class RCC_LogitechInitLoad : MonoBehaviour {

	[InitializeOnLoad]
	public class InitOnLoad {

		static InitOnLoad(){
			
			RCC_SetScriptingSymbol.SetEnabled("BCG_LOGITECH", true);

			if(!EditorPrefs.HasKey("BCG_Logitech" + RCC_Settings.RCCVersion.ToString())){
				
				EditorPrefs.SetInt("BCG_Logitech" + RCC_Settings.RCCVersion.ToString(), 1);

				if(EditorUtility.DisplayDialog("Logitech Gaming SDK For Realistic Car Controller", "Be sure you have imported latest Logitech Gaming SDK to your project.", "Download", "Close"))
					Application.OpenURL (RCC_AssetPaths.logitech);

			}

		}

	}

}
