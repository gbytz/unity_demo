using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
	/// <summary>
	/// Controls the main character avatar.
	/// </summary>
	public class characterControl : MonoBehaviour {
		public ThirdPersonCharacter character;
		private FocusSquare focusSquare;
		private float walkThresh = 0.1f;
		private float yThresh = 0.2f;
		private float lastY = 0;
			 
		void Start () {
			character.gameObject.SetActive (false);
			focusSquare = GetComponent<FocusSquare> ();
		}

		void Update () {
			Vector3 thisLocation = focusSquare.foundSquare.transform.position;
		
			if (focusSquare.squareState == FocusSquare.FocusState.Found) {
				float thisY = thisLocation.y;
				if (Mathf.Abs(thisY - lastY) > yThresh) {
					character.gameObject.SetActive (true);
					//the character follows the focus square
					character.gameObject.transform.position = thisLocation;
				}

				lastY = thisY;
			}

			Vector3 direction = thisLocation  - character.transform.position;
			if (direction.magnitude > walkThresh) {
				character.Move (direction, false, false);
			}
		}
	}
}