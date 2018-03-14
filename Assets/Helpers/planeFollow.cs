using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeFollow : MonoBehaviour {
	public GameObject toFollow;

	void Update () {
		transform.position = toFollow.transform.position;
	}
}
