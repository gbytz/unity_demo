using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
public class characterControl : MonoBehaviour {

		public ThirdPersonCharacter character;
		private FocusSquare focusSquare;
		private float walkThresh = 0.2f;
		private float yThresh = 0.2f;
		private Vector3 lastLocation;
		private float lastY = 0;
		private bool initialized = false;

	// Use this for initialization
	void Start () {
			character.gameObject.SetActive (false);
			focusSquare = GetComponent<FocusSquare> ();
	}
	
	// Update is called once per frame
	void Update () {
			Vector3 thisLocation = focusSquare.foundSquare.transform.position;
		
			if (focusSquare.squareState == FocusSquare.FocusState.Found) {
				float thisY = thisLocation.y;
				if (Mathf.Abs(thisY - lastY) > yThresh) {
					character.gameObject.SetActive (true);
					character.gameObject.transform.position = thisLocation;
				}
				lastY = thisY;
					
			} else {
				thisLocation = lastLocation;
			}

			Vector3 direction = thisLocation  - character.transform.position;
			bool jump = direction.y > 0;
			if (direction.magnitude > walkThresh) {
				character.Move (direction, false, false);
			}

			lastLocation = thisLocation;

	}
}
}