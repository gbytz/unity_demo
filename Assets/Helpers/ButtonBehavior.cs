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
	public GameObject saveAssetButton;
	public Text notification;

	void Start(){
		bool isMappingMode = PlayerPrefs.GetInt ("IsMappingMode") == 1;
		string mapId = PlayerPrefs.GetString ("MapID");
		string userId = PlayerPrefs.GetString ("UserID");
		string developerKey = @"AKIAIQPSF4LP4V3IV55QxwU2T3GuaWFuneWqSqDIUuQe770dRqVAqUrV8/1u";
		GameObject mapGameObject = GameObject.Find("MapSession");
		MapSession map = mapGameObject.GetComponent<MapSession> ();
		map.Init (isMappingMode ? MapMode.MapModeMapping : MapMode.MapModeLocalization, userId, mapId, developerKey);

		map.AssetLoadedEvent += mapAsset => {
			Vector3 position = new Vector3 (mapAsset.X, mapAsset.Y, mapAsset.Z);
			Quaternion orientation = Quaternion.Euler(0, mapAsset.Orientation, 0);
			Toast(mapAsset.AssetId + " found at: " + position.ToString());
			Debug.Log(mapAsset.AssetId + " found at: " + position.ToString());
			Instantiate(GetPrefab(mapAsset.AssetId), position, orientation);
		};

		map.StatusChangedEvent += mapStatus => {
			Debug.Log ("status updated: " + mapStatus);
			Toast("status updated: " + mapStatus);
		};

		map.AssetStoredEvent += stored => {
			Debug.Log ("Assets stored: " + stored);
			Toast("Assets stored: " + stored);
		};

		saveAssetButton.SetActive (false);

		if (map.Mode == MapMode.MapModeLocalization) {
			//placeAssetButton.SetActive (false);
		} 
	}

	// Use this for initialization
	public void PlaceAsset(String assetName) {
		GameObject focusSquareGO = GameObject.Find("FocusSquare");
		FocusSquare focusSquare = focusSquareGO.GetComponent<FocusSquare> ();
		if (focusSquare.SquareState != FocusSquare.FocusState.Found) {
			Debug.Log ("Focus square hasn't been found yet");
			Toast ("Focus square hasn't been found yet");
			return;
		}

		Vector3 position = focusSquare.foundSquare.transform.position;
		Quaternion orientation = Quaternion.identity;
		orientation.y =  -focusSquare.foundSquare.transform.rotation.y;
		GameObject asset = Instantiate(GetPrefab(assetName), position, orientation);
		assetToSave.Add (asset);

		//placeAssetButton.SetActive (false);
		saveAssetButton.SetActive (true);
	}

	public void SaveAsset() {
		saveAssetButton.SetActive (false);
		List<MapAsset> mapAssetsToSave = new List<MapAsset> ();

		GameObject mapGameObject = GameObject.Find("MapSession");
		MapSession map = mapGameObject.GetComponent<MapSession> ();

		foreach (GameObject asset in assetToSave) {
			MapAsset mapAsset = new MapAsset (asset.name, asset.transform.rotation.y, asset.transform.position);
			mapAssetsToSave.Add (mapAsset);
			Debug.Log ("Asset stored at: " + asset.transform.position.ToString ());
			Toast ("Asset stored at: " + asset.transform.position.ToString ());
		}

		map.StorePlacements (mapAssetsToSave);


	}

	public void Back() {
		Application.LoadLevel("LoginScene");
		//SceneManager.LoadScene ("LoginScene", LoadSceneMode.Single);
	}


	private void Toast(String message) {
		notification.text = message;
		notification.gameObject.SetActive (true);
		Invoke ("ToastOff", 1.0f);
	}

	public void ToastOff(){
		notification.gameObject.SetActive (false);
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

	void OnDisable(){
		Destroy (GameObject.Find("MapSession").GetComponent<MapSession> ());
	}

}
