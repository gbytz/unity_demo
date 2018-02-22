using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.iOS {
	public class UnityMapsyncLibNativeInterface {
		private bool isMappingMode = true;

		private static IntPtr s_UnityMapsyncLibNativeInterface = IntPtr.Zero;
		private static UnityMapsyncLibNativeInterface instance = null;

		[DllImport("__Internal")]
		private static extern IntPtr _CreateMapsyncSession(IntPtr arSession, string mapId, string appId, string userId, string developerKey, bool isMappingMode);

		[DllImport("__Internal")]
		private static extern void _SaveAsset(string assetJson);

		[DllImport("__Internal")]
		private static extern void _RegisterUnityCallbacks(string callbackGameObject, string assetReloadedCallback, string statusUpdatedCallback);

		private UnityMapsyncLibNativeInterface(IntPtr arSession) 
		{
			string unityCallbackGameObject = "Canvas";
			string unityAssetLoadedCallbackFunction = "AssetReloaded";
			string unityStatusUpdatedCallback = "StatusUpdated";
			_RegisterUnityCallbacks (unityCallbackGameObject, unityAssetLoadedCallbackFunction, unityStatusUpdatedCallback);

			Debug.Log ("UnityMapsyncLibNativeInterface()");
			this.isMappingMode = PlayerPrefs.GetInt ("IsMappingMode") == 1;
			string mapId = PlayerPrefs.GetString ("MapId");
			string userId = PlayerPrefs.GetString ("UserId");
			string appId = PlayerPrefs.GetString ("AppId");
			string developerKey = @"AKIAIQPSF4LP4V3IV55QxwU2T3GuaWFuneWqSqDIUuQe770dRqVAqUrV8/1u";

			Debug.Log(string.Format("UnityMapsyncLibNativeInterface: {0}, {1}, {2}, {3}", mapId, appId, userId, developerKey , isMappingMode));
			s_UnityMapsyncLibNativeInterface = _CreateMapsyncSession(arSession, mapId, appId, userId, developerKey, this.isMappingMode);
		}

		public static void Initialize(IntPtr arSession)
		{
			instance = new UnityMapsyncLibNativeInterface(arSession);
		}

		public static UnityMapsyncLibNativeInterface GetInstance() 
		{
			return instance;
		}

		public bool IsMappingMode()
		{
			return isMappingMode;
		}

		public void SaveAsset(Vector3 position, string assetId, float orientation)
		{
			AssetModel asset = new AssetModel (assetId, orientation, position.x, position.y, position.z);
			string assetJson = asset.ToJson ();

			Debug.Log ("Asset json: " + assetJson);
			_SaveAsset(assetJson);
		}
	}

}
