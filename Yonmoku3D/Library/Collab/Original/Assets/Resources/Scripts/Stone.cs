using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Stone : MonoBehaviour {

	Stone Type = new Stone();

	private bool isSaveData = false;
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	private void OnCollisionEnter(Collision col) {
		if (!isSaveData) {
			//this.GetComponent<Rigidbody>().isKinematic = true;
			//Vector3 pos = this.transform.position;
			//pos.y = (float)Math.Round(pos.y, 2);
			//this.transform.position = pos;
			GameManager.StoneList.Add(this);
			isSaveData = true;
		}
	}
}
