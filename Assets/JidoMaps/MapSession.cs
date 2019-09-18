using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class MapSession : MonoBehaviour
{
    public string DeveloperKey;

    private UnityMapsyncLibNativeInterface mapsyncInterface = null;
    public MapMode Mode;

    public delegate void StatusDelegate(MapStatus status);
    public StatusDelegate StatusChangedEvent;

    public delegate void AssetDelegate(MapAssets asset);
    public AssetDelegate AssetLoadedEvent;

    public delegate void BoolDelegate(bool value);
    public BoolDelegate AssetStoredEvent;

    public delegate void IntDelegate(int value);
    public IntDelegate ProgressIncrementedEvent;

    public delegate void DetectedObjectDelegate(DetectedObject value);
    public DetectedObjectDelegate ObjectDetectedEvent;

    public void Init(MapMode mapMode, string userId, string mapId)
    {
        if (mapsyncInterface != null)
        {
            Debug.Log("Warning: Mapsync has already been initialized and cannot be initialized again.");
            Application.Quit();
            return;
        }

        this.Mode = mapMode;

        if (string.IsNullOrEmpty(DeveloperKey))
        {
            Debug.Log("Jido Maps Requires a Developer Key. Paste your developer key into the Unity Inspector Window for the MapSession GameObject");
            Application.Quit();
        }

        mapsyncInterface = new UnityMapsyncLibNativeInterface(mapId, userId, DeveloperKey, mapMode == MapMode.MapModeMapping);
    }

	public void Dispose() {
		mapsyncInterface.Dispose ();
	}

    public void StorePlacements(List<MapAsset> assets)
    {
        mapsyncInterface.SaveAssets(assets);
    }

    private void AssetReloaded(string assetJson)
    {
        MapAssets assets = JsonUtility.FromJson<MapAssets>(assetJson);
        AssetLoadedEvent(assets);
    }

    private void ObjectDetected(string objectJson)
    {
        print($"Object detected1 {objectJson}");
        DetectedObjects detectedObjects = JsonUtility.FromJson<DetectedObjects>(objectJson);
        foreach (DetectedObject obj in detectedObjects.Objects)
        {
            ObjectDetectedEvent(obj);
        }
    }

    private void StatusUpdated(string status)
    {
        int asInt = int.Parse(status);
        StatusChangedEvent((MapStatus)asInt);
    }

    private void PlacementStored(string stored)
    {
        AssetStoredEvent(stored == "1");
    }

    private void ProgressIncremented(string progress)
    {
        int p;
        if (int.TryParse(progress, out p)) {
            if (ProgressIncrementedEvent != null)
            {
                ProgressIncrementedEvent(p);
            }
        }
    }
}