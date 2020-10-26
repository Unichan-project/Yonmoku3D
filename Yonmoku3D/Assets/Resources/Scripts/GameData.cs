using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour {
	static public float Gammma = 0.9f;
	static public float alpha = 0.3f;
	static public float epsilon = 0.2f;

	public struct StoneData {
		public bool is_stone;
		public bool stone_type; //true player, false enemy;
	}

	static public StoneData[,,] stoneDatas = new StoneData[4,4,4];
	static public List<float[,]> QValue = new List<float[,]>();
	static public List<StoneData[,,]> NetWorks = new List<StoneData[,,]>();
	static public List<StoneData[,,]> LastNetWorks = new List<StoneData[,,]>();

}
