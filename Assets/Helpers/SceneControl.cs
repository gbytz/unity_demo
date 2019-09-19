﻿using System.Collections;
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

    public GameObject boundsPrefab;
    public GameObject bubblePrefab;

    private Dictionary<int, GameObject> objectBounds = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> objectBubbles = new Dictionary<int, GameObject>();


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

            Debug.Log($"Detected {detectedObject.Name} {detectedObject.Confidence}");

            var detectedObjectPosition = detectedObject.Position;


            if (detectedObject.Confidence > 0.6)
            {

                this.boundsPrefab.transform.position = detectedObjectPosition + new Vector3(0f, detectedObject.Height / 2.0f, 0f);
                this.boundsPrefab.transform.transform.localScale = new Vector3(detectedObject.Width, detectedObject.Height, detectedObject.Depth);
                this.boundsPrefab.transform.rotation = Quaternion.Euler(0.0f, (float)(detectedObject.Orientation * 180.0f/Math.PI), 0.0f);

                GameObject resolvedGO;
                if(this.objectBounds.TryGetValue(detectedObject.Id, out resolvedGO))
                {
                    Destroy(resolvedGO);
                }

                var boundingBox = Instantiate(this.boundsPrefab);
                this.objectBounds[detectedObject.Id] = boundingBox;


                var myBubble = Instantiate(bubblePrefab);
                myBubble.GetComponent<BubblePhysics>().Setup(boundingBox);

                myBubble.transform.position = boundingBox.transform.position + new Vector3(0f, boundingBox.transform.localScale.y / 2.0f, 0f);


                if (this.objectBubbles.TryGetValue(detectedObject.Id, out resolvedGO))
                {
                    Destroy(resolvedGO);
                }

                
                this.objectBubbles[detectedObject.Id] = boundingBox;

                Debug.Log("Setting the position!!");
            }
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

    public void LoadAsset(MapAssets mapAssets) {
        foreach(GameObject gameObject in this.sceneAssets)
        {
            Destroy(gameObject);
        }

        this.sceneAssets.Clear();

        foreach (MapAsset mapAsset in mapAssets.Assets)
        {
            Vector3 position = new Vector3(mapAsset.X, mapAsset.Y, mapAsset.Z);
            Quaternion orientation = Quaternion.Euler(0, mapAsset.Orientation, 0);
            Debug.Log(orientation.eulerAngles);
            Debug.Log("Asset loaded: " + mapAsset.Orientation);
            GameObject instantiatedAsset = Instantiate(GetPrefab(mapAsset.AssetId), position, orientation);
            instantiatedAsset.name = mapAsset.AssetId + assetCounter;
            sceneAssets.Add(instantiatedAsset);
            Debug.Log("Asset instantiated: " + instantiatedAsset.transform.rotation.eulerAngles);

            Animate(instantiatedAsset, "Success");

            if (!found)
            {
                found = true;
                Toast("You found the scene!", 2.0f);
            }
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
