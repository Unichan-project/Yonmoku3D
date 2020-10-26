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
			Vector3 pos = transform.position;

			GameData.stoneDatas[(int)pos.x, (int)pos.y, (int)pos.z].is_stone = true;
			if(stoneType == StoneType.PLAYER)
				GameData.stoneDatas[(int)pos.x, (int)pos.y, (int)pos.z].stone_type = true;
			if (stoneType == StoneType.ENEMY)
				GameData.stoneDatas[(int)pos.x, (int)pos.y, (int)pos.z].stone_type = false;
			isSaveData = true;

			GameData.NetWorks.Add(GameData.stoneDatas);
			return;
		}
	}
}
