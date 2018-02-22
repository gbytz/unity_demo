using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogInScript : MonoBehaviour {

	public InputField MapId;
	public InputField UserId;
	public bool IsMappingMode;

	private const string UserIdKey = "UserId";
	private const string MapIdKey = "MapId";
	private const string AppIdKey = "AppId";

	void Start() {
		if (PlayerPrefs.HasKey (UserIdKey)) {
			UserId.text = PlayerPrefs.GetString (UserIdKey);
		}

		if (PlayerPrefs.HasKey (MapIdKey)) {
			MapId.text = PlayerPrefs.GetString (MapIdKey);
		}
	}

	public void StartNew() {
		if (string.IsNullOrEmpty (MapId.text) || string.IsNullOrEmpty (UserId.text)) {
			return;
		}

		IsMappingMode = true;
		LoadNextScene ();
	}

	public void Reload() {
		if (string.IsNullOrEmpty (MapId.text) || string.IsNullOrEmpty (UserId.text)) {
			return;
		}

		IsMappingMode = false;
		LoadNextScene ();
	}

	private void LoadNextScene() {
		PlayerPrefs.SetInt ("IsMappingMode", IsMappingMode ? 1 : 0);
		PlayerPrefs.SetString (MapIdKey, MapId.text);
		PlayerPrefs.SetString (UserIdKey, UserId.text);
		PlayerPrefs.SetString (AppIdKey, "UnityTestApp");

		SceneManager.LoadScene ("FocusSquareScene");
	}
}
