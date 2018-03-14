using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// The ARScene UI script with button callbacks. Also, initializes MapSession when the scene loads.
/// </summary>
public class SceneControl : MonoBehaviour {
	public List<GameObject> prefabs;
	public List<GameObject> assetsToSave;

	public GameObject saveAssetButton;
	public Text notification;
	public GameObject placeAssetButtons;
	private bool initialized = false;

	private MapSession mapSession;
	private FocusSquare focusSquare;
	private List<string> loadedAssets = new List<string>();

	void Start(){
		//Set up references
		mapSession = GameObject.Find("MapSession").GetComponent<MapSession> ();
		focusSquare = GameObject.Find("FocusSquare").GetComponent<FocusSquare> ();

		//Mapsession initialization
		bool isMappingMode = PlayerPrefs.GetInt ("IsMappingMode") == 1;
		string mapID = PlayerPrefs.GetString ("MapID");
		string userID = PlayerPrefs.GetString ("UserID");
		string developerKey = @"PUT DEV KEY HERE";

		mapSession.Init (isMappingMode ? MapMode.MapModeMapping : MapMode.MapModeLocalization, userID, mapID, developerKey);

		//Set callback to handly MapStatus updates
		mapSession.StatusChangedEvent += mapStatus => {
			Debug.Log ("status updated: " + mapStatus);
		};
			
		//Set callback that confirms when assets are stored
		mapSession.AssetStoredEvent += stored => {
			Debug.Log ("Assets stored: " + stored);
			Toast("Your bear's garden has been planted!", 2.0f);
		};

		//Set Callback for when assets are reloaded
		mapSession.AssetLoadedEvent += mapAsset => {
			if(loadedAssets.Contains(mapAsset.AssetId)) {
				Debug.Log(mapAsset.AssetId + " already loaded");
				return;
			}

			Vector3 position = new Vector3 (mapAsset.X, mapAsset.Y, mapAsset.Z);
			Quaternion orientation = Quaternion.Euler(0, mapAsset.Orientation, 0);
			Debug.Log(mapAsset.AssetId + " found at: " + position.ToString());
			Instantiate(GetPrefab(mapAsset.AssetId), position, orientation);

			Toast("Your bear's garden has been found!", 2.0f);

			placeAssetButtons.SetActive(true);
			saveAssetButton.SetActive (true);
			loadedAssets.Add(mapAsset.AssetId);
		};

		//Set up the UI of the scene
		saveAssetButton.SetActive (false);
		placeAssetButtons.SetActive (false);

		if (mapSession.Mode == MapMode.MapModeLocalization) {
			placeAssetButtons.SetActive (false);
		} 

		Toast ("First scan around your area to start!", 10.0f);
	}

	void Update(){
		if (!initialized && focusSquare.SquareState == FocusSquare.FocusState.Found) {
			if (mapSession.Mode == MapMode.MapModeMapping) {
				Toast ("Great job! Now if you were a bear, wouldn't you want a friendly garden? Get planting!", 2.0f);
				placeAssetButtons.SetActive (true);
			} else {
				Toast ("Keep scanning the area until your bear's friendly garden reloads", 20.0f);
			}

			initialized = true;
		}
	}

	// Placing new assets in the scene
	public void PlaceAsset(String assetName) { 
		//Only place asset if focused on a plane
		if (focusSquare.SquareState != FocusSquare.FocusState.Found) {
			Debug.Log ("Focus square hasn't been found yet");
			Toast ("Point to a surface to place plants.", 2.0f);
			return;
		}

		//Instantiate prefab on focus square
		Vector3 position = focusSquare.foundSquare.transform.position;
		GameObject asset = Instantiate(GetPrefab(assetName), position, Quaternion.identity);
		assetsToSave.Add (asset);

		saveAssetButton.SetActive (true);
	}

	//Saving assets using the MapSession
	public void SaveAsset() {
		saveAssetButton.SetActive (false);

		List<MapAsset> mapAssetsToSave = new List<MapAsset> ();

		foreach (GameObject asset in assetsToSave) {
			MapAsset mapAsset = new MapAsset (asset.name, asset.transform.rotation.y, asset.transform.position);
			mapAssetsToSave.Add (mapAsset);
			Debug.Log ("Asset stored at: " + asset.transform.position.ToString ());
		}

		mapSession.StorePlacements (mapAssetsToSave);

		Toast ("Please wait while your bear's garden is planted.", 10.0f);
	}

	public void Back(){
		SceneManager.LoadSceneAsync ("LoginScene");
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
