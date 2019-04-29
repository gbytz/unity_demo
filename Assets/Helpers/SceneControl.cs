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

	public Text notification;
    public GameObject addButton;

	private MapSession mapSession;
	private FocusSquare focusSquare;

    public GameObject progressPanel;

	private bool initialized = false;
    private bool found = false;

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
        addButton.SetActive(false);

        if (mapSession.Mode == MapMode.MapModeLocalization)
        {
            addButton.SetActive(false);
        }

        Toast("Look around to start!", 5.0f);
    }

	void Update(){
		if (!initialized && focusSquare.SquareState == FocusSquare.FocusState.Found) {
			if (mapSession.Mode == MapMode.MapModeMapping) {
                Toast ("Great job! Now place some animals in your scene.", 2.0f);
                Invoke("ScanNotification", 4.0f);
				addButton.SetActive (true);
			} else {
				Toast ("Look around your space until the scene is found...", 20.0f);
			}


			initialized = true;
		}
	}

    void ScanNotification(){
        Toast("Look around your space to turn the bar green...", 5.0f);
    }

	// Placing new assets in the scene
	public void PlaceAsset(String assetName) { 
		//Only place asset if focused on a plane
		if (focusSquare.SquareState != FocusSquare.FocusState.Found) {
			Toast ("Find a surface to place animals.", 2.0f);
			return;
		}

		//Instantiate prefab on focus square
		Vector3 position = focusSquare.foundSquare.transform.position;
        Vector3 targetPos = GameObject.Find("Main Camera").transform.position;
		GameObject asset = Instantiate(GetPrefab(assetName), position, Quaternion.identity);
        targetPos.y = asset.transform.position.y;
        asset.transform.LookAt(targetPos);
        asset.name = assetName + "(" + UnityEngine.Random.Range(0, 10000).ToString();
        sceneAssets.Add(asset);
        Animate(asset, "Failure");

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

    private static int assetCounter = 0;

    public void LoadAsset(MapAsset mapAsset) {
        Vector3 position = new Vector3(mapAsset.X, mapAsset.Y, mapAsset.Z);
		Quaternion orientation = Quaternion.Euler(0, mapAsset.Orientation, 0);
		Debug.Log (orientation.eulerAngles);
		Debug.Log ("Asset loaded: " + mapAsset.Orientation);
        GameObject instantiatedAsset = Instantiate(GetPrefab(mapAsset.AssetId), position, orientation);
        instantiatedAsset.name = mapAsset.AssetId + assetCounter;
        sceneAssets.Add(instantiatedAsset);
		Debug.Log ("Asset instantiated: " + instantiatedAsset.transform.rotation.eulerAngles);

        Animate(instantiatedAsset, "Success");

        if(!found){
            found = true;
            Toast("You found the scene!", 2.0f);
        }
    }

    //TODO: shut off in at 5. move to 5 on reload.
    private void ProgressIncrement(int progress){
        if (progress > 5){
            return;
        }

        if(progress > 4 && mapSession.Mode == MapMode.MapModeLocalization && !found){
            progress = 4;
        }
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
        return prefabs[assetCounter++ % prefabs.Count];
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

    private void Animate(GameObject character, string triggerName)
    {
        print(triggerName);
        character.GetComponentInChildren<Animator>().SetTrigger(triggerName);
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
