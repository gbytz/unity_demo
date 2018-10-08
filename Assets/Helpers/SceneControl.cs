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

	private MapSession mapSession;
	private FocusSquare focusSquare;
    private UX_Workflow ux_workflow;
    public string mapID = "";

    public GameObject progressPanel;
    public GameObject progressPanelAnimated;

    private bool initialized = false;
    private bool found = false;

	void Start(){
		InitJido();
	}

    private void InitJido(){

        //Set up references
        mapSession = GameObject.Find("MapSession").GetComponent<MapSession>();
        focusSquare = GameObject.Find("FocusSquare").GetComponent<FocusSquare>();
        ux_workflow = GetComponent<UX_Workflow>();

        //Mapsession initialization
        bool isMappingMode = PlayerPrefs.GetInt("IsMappingMode") == 1;
        mapID = PlayerPrefs.GetString("MapID");
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

    }

	void Update(){
		if (!initialized && focusSquare.SquareState == FocusSquare.FocusState.Found) {
			initialized = true;
		}
	}

    void ScanNotification(){
        ux_workflow.Toast("Look around your space to turn the bar green...", 5.0f);
    }

	// Placing new assets in the scene
	public void PlaceAsset(String assetName) { 
		//Only place asset if focused on a plane
		if (focusSquare.SquareState != FocusSquare.FocusState.Found) {
            ux_workflow.Toast ("Find a surface to place animals.", 2.0f);
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

        ux_workflow.ObjectPlaced();

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
			isLoaded.transform.rotation = Quaternion.Euler(0, mapAsset.Orientation, 0);
            ux_workflow.Toast("Your result was updated.", 3.0f);
            return;
        }

        Vector3 position = new Vector3(mapAsset.X, mapAsset.Y, mapAsset.Z);
		Quaternion orientation = Quaternion.Euler(0, mapAsset.Orientation, 0);
		Debug.Log (orientation.eulerAngles);
		Debug.Log ("Asset loaded: " + mapAsset.Orientation);
        GameObject instantiatedAsset = Instantiate(GetPrefab(mapAsset.AssetId), position, orientation);
        instantiatedAsset.name = mapAsset.AssetId;
        sceneAssets.Add(instantiatedAsset);
		Debug.Log ("Asset instantiated: " + instantiatedAsset.transform.rotation.eulerAngles);

        Animate(instantiatedAsset, "Success");

        if(!found){
            found = true;
            print("scene found");
            ux_workflow.Toast("Your scene has been found! If you are happy with your result press the arrow below, otherwise your result will continue to improve as your bar turns green. ", 5.0f);
            ux_workflow.objectReloaded = true;
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

        progressPanel.GetComponent<ProgressBar>().AddProgress(progress);
        progressPanelAnimated.GetComponent<ProgressBarAnimated>().AddProgress(progress);
        ux_workflow.IncrementProgress(progress);
    }

    public void FakeProgressIncrement (int progressAmount){

        progressPanelAnimated.GetComponent<ProgressBarAnimated>().AddProgress(progressAmount);

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

    private void Animate(GameObject character, string triggerName)
    {
        print(triggerName);
        character.GetComponentInChildren<Animator>().SetTrigger(triggerName);
    }

}
