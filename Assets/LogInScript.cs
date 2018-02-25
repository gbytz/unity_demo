using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogInScript : MonoBehaviour {

	public InputField MapID;
	public InputField UserID;
	public bool IsMappingMode;

	private const string UserIDKey = "UserID";
	private const string MapIDKey = "MapID";

	void Start() {
		if (PlayerPrefs.HasKey (UserIDKey)) {
			UserID.text = PlayerPrefs.GetString (UserIDKey);
		}

		if (PlayerPrefs.HasKey (MapIDKey)) {
			MapID.text = PlayerPrefs.GetString (MapIDKey);
		}
	}

	public void StartNew() {
		if (string.IsNullOrEmpty (MapID.text) || string.IsNullOrEmpty (UserID.text)) {
			return;
		}

		IsMappingMode = true;
		LoadNextScene ();
	}

	public void Reload() {
		if (string.IsNullOrEmpty (MapID.text) || string.IsNullOrEmpty (UserID.text)) {
			return;
		}

		IsMappingMode = false;
		LoadNextScene ();
	}

	private void LoadNextScene() {
		PlayerPrefs.SetInt ("IsMappingMode", IsMappingMode ? 1 : 0);
		PlayerPrefs.SetString (MapIDKey, MapID.text);
		PlayerPrefs.SetString (UserIDKey, UserID.text);

		SceneManager.LoadScene ("FocusSquareScene");
	}
}
