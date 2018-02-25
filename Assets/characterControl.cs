using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
public class characterControl : MonoBehaviour {

		public ThirdPersonCharacter character;
		private float threshold = 1f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
			Vector3 direction =  gameObject.transform.position - character.transform.position;
			direction.y = 0;
			bool jump = direction.y > 0;
			if (direction.magnitude > threshold) {
				character.Move (direction, false, false);
			}
	}
}
}