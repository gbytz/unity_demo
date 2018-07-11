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
    private List<GameObject> sceneAssets = new List<GameObject>();

	public GameObject saveAssetButton;
	public Text notification;
	public GameObject placeAssetButtons;

	private MapSession mapSession;
	private FocusSquare focusSquare;

    public GameObject progressPanel;

	private bool initialized = false;

	void Start(){
		InitJido();
	}

    private void InitJido(){

        //Set up references
        mapSession = GameObject.Find("MapSession").GetComponent<MapSession>();
        focusSquare = GameObject.Find("FocusSquare").GetComponent<FocusSquare>();

        //Mapsession initialization
        bool isMappingMode = PlayerPrefs.GetInt("IsMappingMode") == 1;
        string mapID = PlayerPrefs.GetString("MapID");
        string userID = PlayerPrefs.GetString("UserID");

        mapSession.Init(isMappingMode ? MapMode.MapModeMapping : MapMode.MapModeLocalization, userID, mapID);

        //Set callback to handly MapStatus updates
        mapSession.StatusChangedEvent += mapStatus => {
            Debug.Log("status updated: " + mapStatus);
        };

        //Set callback that confirms when assets are stored
        mapSession.AssetStoredEvent += stored => {
            Debug.Log("Assets stored: " + stored);
        };

        //Set Callback for when assets are reloaded
        mapSession.AssetLoadedEvent += LoadAsset;

        mapSession.ObjectDetectedEvent += detectedObject => {
            Debug.Log("Detected " + detectedObject.Name);
        };

        mapSession.ProgressIncrementedEvent += ProgressIncrement;

        //Set up the UI of the scene
        saveAssetButton.SetActive(false);
        placeAssetButtons.SetActive(false);

        if (mapSession.Mode == MapMode.MapModeLocalization)
        {
            placeAssetButtons.SetActive(false);
        }

        Toast("First scan around your area to start!", 10.0f);
    }

	void Update(){
		if (!initialized && focusSquare.SquareState == FocusSquare.FocusState.Found) {
			if (mapSession.Mode == MapMode.MapModeMapping) {
				Toast ("Great job! Now if you were a bear, wouldn't you want some friends?", 2.0f);
				placeAssetButtons.SetActive (true);
			} else {
				Toast ("Keep scanning the area until your bear's friends appear", 20.0f);
			}


			initialized = true;
		}
	}

	// Placing new assets in the scene
	public void PlaceAsset(String assetName) { 
		//Only place asset if focused on a plane
		if (focusSquare.SquareState != FocusSquare.FocusState.Found) {
			Toast ("Point to a surface to place plants.", 2.0f);
			return;
		}

		//Instantiate prefab on focus square
		Vector3 position = focusSquare.foundSquare.transform.position;
		GameObject asset = Instantiate(GetPrefab(assetName), position, Quaternion.identity);
        asset.name = assetName + "(" + UnityEngine.Random.Range(0, 10000).ToString();
        sceneAssets.Add(asset);

        SaveAssets();

	}

	//Saving assets using the MapSession
    public void SaveAssets() {
        List<MapAsset> mapAssets = new List<MapAsset>();
        foreach(GameObject asset in sceneAssets){
			MapAsset mapAsset = new MapAsset(asset.name, asset.transform.rotation.eulerAngles.y, asset.transform.position);
            mapAssets.Add(mapAsset);
        }

        mapSession.StorePlacements (mapAssets);

	}

    public void LoadAsset(MapAsset mapAsset){
        GameObject isLoaded = IsThisALoadedAsset(mapAsset);
		
        if (isLoaded != null)
        {
            isLoaded.transform.position = mapAsset.Position;
			isLoaded.transform.rotation = Quaternion.Euler(0, mapAsset.OrientationInDegrees, 0);
            return;
        }

        Vector3 position = new Vector3(mapAsset.X, mapAsset.Y, mapAsset.Z);
		Quaternion orientation = Quaternion.Euler(0, mapAsset.OrientationInDegrees, 0);
		Debug.Log (orientation.eulerAngles);
		Debug.Log ("Asset loaded: " + mapAsset.OrientationInDegrees);
        GameObject instantiatedAsset = Instantiate(GetPrefab(mapAsset.AssetId), position, orientation);
        instantiatedAsset.name = mapAsset.AssetId;
        sceneAssets.Add(instantiatedAsset);
		Debug.Log ("Asset instantiated: " + instantiatedAsset.transform.rotation.eulerAngles);

        Toast("Your bear's friends have been found!", 2.0f);

        placeAssetButtons.SetActive(true);
        saveAssetButton.SetActive(true);

    }

    //TODO: shut off in at 5. move to 5 on reload.
    private void ProgressIncrement(int progress){
        Toast("Progress now: " + progress.ToString(), 0.5f);
        if (progress > 5){
            progressPanel.SetActive(false);
            return;
        }
        progressPanel.GetComponent<ProgressBar>().AddProgress(progress);
    }

    public void Back()
    {
		mapSession.Dispose ();
        SceneManager.LoadSceneAsync("LoginScene");
    }

    private GameObject GetPrefab(string prefabName)
    {
        Debug.Log("Looking for a " + prefabName);
        foreach (GameObject prefab in prefabs)
        {
            string[] nameSplit = prefabName.Split('('); //prefabName = Type(Clone)#ID
            if (prefab.name == nameSplit[0])
            {
                return prefab;
            }
        }

        Debug.Log("Could not find correct prefab");
        return prefabs[0];
    }

    private GameObject IsThisALoadedAsset(MapAsset asset)
    {
        foreach (var sceneAsset in sceneAssets)
        {
            if (asset.AssetId == sceneAsset.name)
            {
                return sceneAsset;
            }
        }

        return null;
    }

	private void Toast(string message, float time) {
		notification.text = message;
		notification.gameObject.SetActive (true);
		CancelInvoke ();
		Invoke ("ToastOff", time);
	}

	private void ToastOff(){
		notification.gameObject.SetActive (false);
	}
}
