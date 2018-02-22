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
		asset.SetActive (false);
		saveAssetButton.SetActive (false);
		UnityMapsyncLibNativeInterface instance = UnityMapsyncLibNativeInterface.GetInstance ();
		if (!instance.IsMappingMode ()) {
			placeAssetButton.SetActive (false);
		} 
	}

	// Use this for initialization
	public void PlaceAsset() {
		UnityMapsyncLibNativeInterface instance = UnityMapsyncLibNativeInterface.GetInstance ();
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

		UnityMapsyncLibNativeInterface instance = UnityMapsyncLibNativeInterface.GetInstance ();
		if (instance.IsMappingMode()) {
			instance.SaveAsset (this.assetPosition, "phonebooth", 180);
		}
	}

	public void AssetReloaded(string assetJson) {
		AssetModel assetModel = AssetModel.FromJson (assetJson);

		asset.SetActive (true);

		asset.transform.position = new Vector3 (assetModel.X, assetModel.Y, assetModel.Z);

		asset.transform.Rotate (Vector3.up * assetModel.Orientation);

		Debug.Log ("asset reloaded: " + assetJson);
	}
}
