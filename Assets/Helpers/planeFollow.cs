using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneFollow : MonoBehaviour {
	public GameObject toFollow;

	void Update () {
		transform.position = toFollow.transform.position;
	}
}
