using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonBehavior : MonoBehaviour {

	public List<GameObject> prefabs;
	public List<GameObject> assetToSave;

	//UI
	public GameObject saveAssetButton;
	public Text notification;
	public GameObject placeAssetButtons;
	private bool initialized = false;

	//References
	private GameObject mapSessionGO;
	private MapSession mapSession;
	private GameObject focusSquareGO;
	private FocusSquare focusSquare;

	void Start(){

		//Set up references
		mapSessionGO = GameObject.Find("MapSession");
		mapSession = mapSessionGO.GetComponent<MapSession> ();
		focusSquareGO = GameObject.Find("FocusSquare");
		focusSquare = focusSquareGO.GetComponent<FocusSquare> ();

		//Mapsession initialization
		bool isMappingMode = PlayerPrefs.GetInt ("IsMappingMode") == 1;
		string mapID = PlayerPrefs.GetString ("MapID");
		string userID = PlayerPrefs.GetString ("UserID");
		string developerKey = @"AKIAIQPSF4LP4V3IV55QxwU2T3GuaWFuneWqSqDIUuQe770dRqVAqUrV8/1u";

		mapSession.Init (isMappingMode ? MapMode.MapModeMapping : MapMode.MapModeLocalization, userID, mapID, developerKey);

		//Set callback to handly MapStatus updates
		mapSession.StatusChangedEvent += mapStatus => {
			Debug.Log ("status updated: " + mapStatus);
		};
			
		//Set callback that confirms when assets are stored
		mapSession.AssetStoredEvent += stored => {
			Debug.Log ("Assets stored: " + stored);
			Toast("Your bear's garden has been planted!", 1.0f);
		};

		//Set Callback for when assets are reloaded
		mapSession.AssetLoadedEvent += mapAsset => {
			Vector3 position = new Vector3 (mapAsset.X, mapAsset.Y, mapAsset.Z);
			Quaternion orientation = Quaternion.Euler(0, mapAsset.Orientation, 0);
			Debug.Log(mapAsset.AssetId + " found at: " + position.ToString());
			Instantiate(GetPrefab(mapAsset.AssetId), position, orientation);

			Toast("Your bear's garden has been found!", 1.0f);

		};

		//Set up the UI of the scene
		saveAssetButton.SetActive (false);

		if (mapSession.Mode == MapMode.MapModeLocalization) {
			placeAssetButtons.SetActive (false);
		} 

		Toast ("First scan around your area to start!", 10.0f);

	}

	void Update(){
		if (!initialized && focusSquare.SquareState == FocusSquare.FocusState.Found) {
			if (mapSession.Mode == MapMode.MapModeMapping) {
				Toast ("Great job! Now you can plant a nice garden for your bear!", 2.0f);
			} else {
				Toast ("Now keep scanning the area until your bear's garden reloads", 20.0f);
			}

			initialized = true;
		}
	}

	// Placing new assets in the scene
	public void PlaceAsset(String assetName) { 

		//Only place asset if focused on a plane
		if (focusSquare.SquareState != FocusSquare.FocusState.Found) {
			Debug.Log ("Focus square hasn't been found yet");
			Toast ("Point to a surface to place plants.", 1.0f);
			return;
		}

		//Instantiate prefab on focus square
		Vector3 position = focusSquare.foundSquare.transform.position;
		Quaternion orientation = Quaternion.identity;
		orientation.y =  -focusSquare.foundSquare.transform.rotation.y;
		GameObject asset = Instantiate(GetPrefab(assetName), position, orientation);
		assetToSave.Add (asset); //Store for saving

		saveAssetButton.SetActive (true);
	}

	//Saving assets using the MapSession
	public void SaveAsset() {
		saveAssetButton.SetActive (false);

		List<MapAsset> mapAssetsToSave = new List<MapAsset> ();

		foreach (GameObject asset in assetToSave) {
			MapAsset mapAsset = new MapAsset (asset.name, asset.transform.rotation.y, asset.transform.position);
			mapAssetsToSave.Add (mapAsset);
			Debug.Log ("Asset stored at: " + asset.transform.position.ToString ());
		}

		mapSession.StorePlacements (mapAssetsToSave);

		Toast ("Please wait while your bear's garden is planted.", 10.0f);



	}

	private GameObject GetPrefab(string name) {
		Debug.Log ("Looking for a " + name);
		foreach(GameObject prefab in prefabs ){
			if(prefab.name == name){
				return prefab;
			} else if (name.Length > 7 && prefab.name == name.Substring(0, name.Length - 7)) {
				return prefab;
			}
		}
		Debug.Log ("Could not find correct prefab");
		return prefabs [0];
			
	}


	private void Toast(String message, float time) {
		notification.text = message;
		notification.gameObject.SetActive (true);
		CancelInvoke ();
		Invoke ("ToastOff", time);
	}

	private void ToastOff(){
		notification.gameObject.SetActive (false);
	}

}
