using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour {
	static public float gammma = 0.9f;
	static public float alpha = 0.3f;
	static public float epsilon = 0.2f;

	public struct StoneData {
		public bool is_stone;
		public bool stone_type; //true player, false enemy;
	}

	public struct BoardData {
		public bool[,,] is_stone;
		public bool[,,] stone_type; //true player, false enemy;
		public bool[,] enemyPos;
		public float qValue;
	}

	//static public StoneData[,,] stoneData = new StoneData[4,4,4];
	//static public StoneData[,,] copyStonesData = new StoneData[4, 4, 4];
	static public BoardData boardData;
	static public List<BoardData> NetWorks = new List<BoardData>();
	static public List<BoardData> LastNetWorks = new List<BoardData>();

	/*
	static public void StoneRotation(int degrees) {
		switch(degrees) {
			case 90:
				for(int y = 0; y < 4; y++) {
					for(int x = 0; x < 4; x++) {
						for(int z = 0; z < 4; z++) {
							copyStonesData[x, y, z] = stonesData[3 - z, y, x];
						}
					}
				}
				break;

			case 180:
				for (int y = 0; y < 4; y++) {
					for (int x = 0; x < 4; x++) {
						for (int z = 0; z < 4; z++) {
							copyStonesData[x, y, z] = stonesData[3 - x, y, 3 - z];
						}
					}
				}
				break;

			case 270:
				for (int y = 0; y < 4; y++) {
					for (int x = 0; x < 4; x++) {
						for (int z = 0; z < 4; z++) {
							copyStonesData[x, y, z] = stonesData[z, y, 3 - x];
						}
					}
				}
				break;

		}
	}
	*/
}
