using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum StoneType {
	PLAYER,
	ENEMY,
}

public class Stone : MonoBehaviour {

	public StoneType stoneType;
	public int CountData;

	private bool isSaveData = false;
	void Start() {

	}

	// Update is called once per frame
	void Update() {
	}

	public void Init(StoneType st) {
		this.stoneType = st;
	}

	private void OnCollisionEnter(Collision col) {
		if (!isSaveData) {
			GameManager.StoneList.Add(this);
			isSaveData = true;
			return;
		}
	}
}
