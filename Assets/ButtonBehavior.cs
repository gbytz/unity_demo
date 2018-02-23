using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using System;

public class ButtonBehavior : MonoBehaviour {

	public GameObject asset;
	public GameObject placeAssetButton;
	public GameObject saveAssetButton;

	private Vector3 assetPosition;

	void Start(){
		bool isMappingMode = PlayerPrefs.GetInt ("IsMappingMode") == 1;
		string mapId = PlayerPrefs.GetString ("MapId");
		string userId = PlayerPrefs.GetString ("UserId");
		string developerKey = @"AKIAIQPSF4LP4V3IV55QxwU2T3GuaWFuneWqSqDIUuQe770dRqVAqUrV8/1u";
		GameObject mapsyncGO = GameObject.Find("MapsyncSession");
		MapsyncSession mapsync = mapsyncGO.GetComponent<MapsyncSession> ();
		mapsync.Init (isMappingMode ? MapMode.MapModeMapping : MapMode.MapModeLocalization, userId, mapId, developerKey);

		mapsync.assetLoadedEvent += mapAsset => {
			asset.SetActive (true);
			asset.transform.position = new Vector3 (mapAsset.X, mapAsset.Y, mapAsset.Z);
			asset.transform.Rotate (Vector3.up * mapAsset.Orientation);
		};

		mapsync.statusChangedEvent += mapStatus => {
			Debug.Log ("status updated: " + mapStatus);
		};

		mapsync.assetStoredEvent += stored => {
			Debug.Log ("Asset stored: " + stored);
		};

		asset.SetActive (false);
		saveAssetButton.SetActive (false);

		if (mapsync.Mode == MapMode.MapModeLocalization) {
			placeAssetButton.SetActive (false);
		} 
	}

	// Use this for initialization
	public void PlaceAsset() {
		GameObject focusSquareGO = GameObject.Find("FocusSquare");
		FocusSquare focusSquare = focusSquareGO.GetComponent<FocusSquare> ();
		if (focusSquare.SquareState != FocusSquare.FocusState.Found) {
			Debug.Log ("Focus square hasn't been found yet");
			return;
		}

		this.assetPosition = focusSquare.foundSquare.transform.position;
		asset.SetActive (true);
		asset.transform.position = this.assetPosition;

		placeAssetButton.SetActive (false);
		saveAssetButton.SetActive (true);
	}

	public void SaveAsset() {
		saveAssetButton.SetActive (false);

		GameObject mapsyncGO = GameObject.Find("MapsyncSession");
		MapsyncSession mapsync = mapsyncGO.GetComponent<MapsyncSession> ();
		MapAsset asset = new MapAsset ("phonebooth", 0, this.assetPosition);
		mapsync.StorePlacement (asset);
	}
}
