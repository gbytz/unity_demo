﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The LoginScene UI script.
/// </summary>
public class LogInScript : MonoBehaviour {
	public InputField MapID;
	public bool IsMappingMode;

	private const string UserIDKey = "UserID";
	private const string MapIDKey = "MapID";
    private string defaultUserID = "demoUser";

	void Start() {
		if (PlayerPrefs.HasKey (UserIDKey)) {
            defaultUserID = PlayerPrefs.GetString (UserIDKey);
		}

		if (PlayerPrefs.HasKey (MapIDKey)) {
			MapID.text = PlayerPrefs.GetString (MapIDKey);
		}
	}

	public void StartNew() {
		if (string.IsNullOrEmpty (MapID.text)) {
			return;
		}

		IsMappingMode = true;
		LoadNextScene ();
	}

	public void Reload() {
		if (string.IsNullOrEmpty (MapID.text) || string.IsNullOrEmpty (defaultUserID)) {
			return;
		}

		IsMappingMode = false;
		LoadNextScene ();
	}

	private void LoadNextScene() {
		PlayerPrefs.SetInt ("IsMappingMode", IsMappingMode ? 1 : 0);
		PlayerPrefs.SetString (MapIDKey, MapID.text);
        PlayerPrefs.SetString (UserIDKey, defaultUserID);

		SceneManager.LoadSceneAsync ("ARScene");
	}
}
