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

	void Update() {
	}

	public void Init(StoneType st) {
		this.stoneType = st;
	}

	private void OnCollisionEnter(Collision col) {
		if (!isSaveData) {
			//GameManager.StoneList.Add(this);
			Vector3 pos = transform.position;

			GameData.boardData.is_stone[(int)pos.x, (int)pos.y, (int)pos.z] = true;
			if (stoneType == StoneType.PLAYER) {
				GameData.boardData.stone_type[(int)pos.x, (int)pos.y, (int)pos.z] = true;
			}
			if (stoneType == StoneType.ENEMY) {
				GameData.boardData.stone_type[(int)pos.x, (int)pos.y, (int)pos.z] = false;
				
				GameData.boardData.enemyPos[(int)pos.x, (int)pos.z] = true;
				GameData.NetWorks.Add(GameData.boardData);

				for (int i = 0; i < 4; i++) {
					for (int j = 0; j < 4; j++) {
						{
							GameData.boardData.enemyPos = new bool[4, 4];
							GameData.boardData.qValue = new float[4, 4];
							GameData.boardData.enemyPos[i, j] = false;
						}
					}
				}
			}
			//ここで同じ盤面がないか検索
			isSaveData = true;

			return;
		}
	}
}
