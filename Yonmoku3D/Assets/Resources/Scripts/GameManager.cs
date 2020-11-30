using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour {

	public int[,] puttingStone = new int[4, 4];

	public GameObject MainCamera;
	public Stone PlayerStonePrefab;
	public Stone EnemyStonePrefab;

	private Vector3 lastMousePosition;
	private Vector3 newAngle = new Vector3(0, 0, 0);

	private Vector3 firstPos = Vector3.zero;

	private int GameTurnCount = 0;

	private float clickTimer = 0;
	private float TurnTimer = 0;

	private bool putPlayerStone = false;
	private bool putEnemyStone = false;

	private bool winStoneType;
	private bool isGameover = false;

	private void Awake() {
		GameData.boardData.is_stone = new bool[4, 4, 4];
		GameData.boardData.stone_type = new bool[4, 4, 4];
		GameData.boardData.enemyPos = new bool[4, 4];
		GameData.boardData.qValue = new float[4, 4];
	}

	private void Start() {
		isGameover = false;
		GameData.NetWorks.Clear();
	}
	
	private void QValueUpdate(bool whichWins) {
		//whichWins = true -> playerWin.
		//whichWins = false -> enemyWin.
		float reward = 20;
		if (whichWins) reward = -20;

		foreach (var data in GameData.NetWorks) {
			for (int x = 0; x < 4; x++) {
				for (int y = 0; y < 4; y++) {
					for (int z = 0; z < 4; z++) {
						//data.qValue[x, z] *= GameData.alpha;
						if (data.is_stone[x,y,z] && data.enemyPos[x, z]) {
							float maxQ = 0f;
							for (int xx = 0; xx < 4; xx++) {
								for (int zz = 0; zz < 4; zz++) {
									maxQ = Mathf.Max(maxQ, data.qValue[xx, zz]);
								}
							}
							float q = data.qValue[x, z] + (GameData.alpha * (reward + (GameData.gammma * maxQ) - data.qValue[x, z]));
							data.qValue[x, z] = q;
						} else { 
						}
					}
				}
			}
		}
	}
	
	private void Update() {
	
		if (!isGameover) {
			if (Judgment()) {
				if (winStoneType) {
					Debug.Log("Player win!");
				} else {
					Debug.Log("AI win!");
				}
				foreach (var data in GameData.NetWorks) {
					GameData.AllNetWorks.Add(data);
				}
				QValueUpdate(winStoneType);
				for (int i = 0; i < GameData.AllNetWorks.Count; i++) {
					for (int z = 3; z >= 0; z--) {
						string datastr = "";
						for (int x = 0; x < 4; x++) {
							datastr += GameData.AllNetWorks[i].qValue[x, z] + ", ";
						}
						Debug.Log(datastr);
					}
					Debug.Log("=====================================================================");
				}
				isGameover = true;
				string str = "{\n";
				foreach(var data in GameData.AllNetWorks) {
					//for(data.is_stone)
				}
				StreamWriter sw = new StreamWriter(@"data.json");
				sw.Write(str);
				sw.Close();
				SceneManager.LoadScene("GameScene");
			}
		}
		

		if (Input.GetMouseButtonDown(0)) {
			// マウスクリック開始(マウスダウン)時にカメラの角度を保持(Z軸には回転させないため).
			newAngle = MainCamera.transform.localEulerAngles;
			lastMousePosition = Input.mousePosition;
		} else if (Input.GetMouseButton(0)) {
			// マウスの移動量分カメラを回転させる.
			newAngle.y += (Input.mousePosition.x - lastMousePosition.x) * 0.5f;
			newAngle.x -= (Input.mousePosition.y - lastMousePosition.y) * 0.5f;

			if (newAngle.x > 70) {
				newAngle.x = 70;
			} else if (newAngle.x < 0) {
				newAngle.x = 0;
			}

			MainCamera.gameObject.transform.localEulerAngles = newAngle;

			lastMousePosition = Input.mousePosition;
		}

		if (TurnTimer > 0) {
			TurnTimer -= Time.deltaTime;
			if (TurnTimer <= 0) {
				TurnTimer = 0;
				GameTurnCount++;
				putPlayerStone = false;
				putEnemyStone = false;

			}
		}

		if (GameTurnCount % 2 == 0 && !putPlayerStone) {
			if (Input.GetMouseButtonDown(0)) {
				Ray ray = new Ray();
				RaycastHit hit = new RaycastHit();
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				//マウスクリックした場所からRayを飛ばし、オブジェクトがあればtrue 
				if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity)) {
					if (hit.collider.gameObject.layer == 8) {
						firstPos = hit.collider.transform.position;
					}
				}
			}

			if (Input.GetMouseButtonUp(0)) {
				Ray ray = new Ray();
				RaycastHit hit = new RaycastHit();
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				//マウスクリックした場所からRayを飛ばし、オブジェクトがあればtrue 
				if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity)) {
					if (hit.collider.gameObject.layer == 8) {
						Vector3 pos = hit.collider.transform.position;

						if (pos == firstPos && puttingStone[(int)pos.x, (int)pos.z] < 4) {
							pos.y = 3.75f;
							puttingStone[(int)pos.x, (int)pos.z]++;
							Stone stone = Instantiate(PlayerStonePrefab, pos, Quaternion.identity);
							stone.stoneType = StoneType.PLAYER;
							stone.CountData = GameTurnCount;
							TurnTimer = 1f;
							putPlayerStone = true;
						}

						firstPos = Vector3.zero;
					}
				}
			}
		}

		if(GameTurnCount % 2 == 1 && !putEnemyStone) {
			int posX = 0;
			int posZ = 0;
			int rand = Random.Range(0, 10);
			while (true) {
				float maxQ = 0;
				if (rand >= 3) {
					foreach (var data in GameData.AllNetWorks) {
						if (data.is_stone == GameData.boardData.is_stone
						&& data.stone_type == GameData.boardData.stone_type
						&& data.enemyPos == GameData.boardData.enemyPos) {
							for (int x = 0; x < 4; x++) {
								for (int z = 0; z < 4; z++) {
									if (maxQ < data.qValue[x, z]) {
										if (puttingStone[posX, posZ] >= 4) continue;
										maxQ = data.qValue[x, z];
										posX = x;
										posZ = z;
									}
								}
							}
						}
					}
					if (maxQ == 0) {
						rand = 0;
						continue;
					}
				} else {
					posX = Random.Range(0, 4);
					posZ = Random.Range(0, 4);
					if (puttingStone[posX, posZ] >= 4) continue;
				}

				Stone stone = Instantiate(EnemyStonePrefab, new Vector3(posX, 3.75f, posZ), Quaternion.identity);
				stone.stoneType = StoneType.ENEMY;
				stone.CountData = GameTurnCount;
				TurnTimer = 1f;
				putEnemyStone = true;
				puttingStone[posX, posZ]++;
				break;
			}
		}
	}

	void MatchTurn() {
		for (int i = 0; i < 4; i++) {
			for(int j = 0; j < 4; j++) {
				for(int k = 0; k < 4; k++) {
					if(GameData.boardData.is_stone[i, j, k]) {
						if (GameData.boardData.stone_type[i, j, k])
							Debug.Log(new Vector3(i, j, k) + "  TYPE:PLAYER");
						else
							Debug.Log(new Vector3(i, j, k) + "  TYPE:ENEMY");
					}
				}
			}
		}
	}
	private bool Judgment() {
		int x = 0, z = 0;
		bool zloop = false;
		while(true) {

			if (!zloop) {
				x++;
				if (x > 3) {
					zloop = true;
					x = 0;
				}
			} else {
				z++;
				if (z > 3) {
					break;
				}
			}
			for (int y = 0; y < 3; y++) {
				if(GameData.boardData.is_stone[x, y, z]) {
					int count = 0;
					bool stone_type = GameData.boardData.stone_type[x, y, z];
					if (x == 0) {
						//zの時の横列
						for (int i = 1; i < 4; i++) {
							if (GameData.boardData.stone_type[i, y, z] == stone_type && GameData.boardData.is_stone[i, y, z]) {
								count++;
							} else {
								count = 0;
								break;
							}
						}
						if (count == 3) {
							winStoneType = stone_type;
							return true;
						}
						//zの時の斜めUP
						if (y == 0) {
							for (int i = 1; i < 4; i++) {
								if (GameData.boardData.stone_type[i, i, z] == stone_type && GameData.boardData.is_stone[i, i, z]) {
									count++;
								} else {
									count = 0;
									break;
								}
							}
							if (count == 3) {
								winStoneType = stone_type;
								return true;
							}
							//zの時の斜めdown
						} else if (y == 3) {
							for (int i = 1; i < 4; i++) {
								if (GameData.boardData.stone_type[i, 3 - i, z] == stone_type && GameData.boardData.is_stone[i, 3 - i, z]) {
									count++;
								} else {
									count = 0;
									break;
								}
							}
							if (count == 3) {
								winStoneType = stone_type;
								return true;
							}
						}
					}
					if (z == 0) {
						//ｘの時の縦列
						for (int i = 1; i < 4; i++) {
							if (GameData.boardData.stone_type[x, y, i] == stone_type && GameData.boardData.is_stone[x, y, i]) {
								count++;
							} else {
								count = 0;
								break;
							}
						}
						if (count == 3) {
							winStoneType = stone_type;
							return true;
						}
						//ｘの時の斜めup
						if (y == 0) {
							for (int i = 1; i < 4; i++) {
								if (GameData.boardData.stone_type[x, i, i] == stone_type && GameData.boardData.is_stone[x, i, i]) {
									count++;
								} else {
									count = 0;
									break;
								}
							}
							if (count == 3) {
								winStoneType = stone_type;
								return true;
							}
							//ｘの時の斜めdown
						} else if (y == 3) {
							for (int i = 1; i < 4; i++) {
								if (GameData.boardData.stone_type[x, 3 - i, i] == stone_type && GameData.boardData.is_stone[x, 3 - i, i]) {
									count++;
								} else {
									count = 0;
									break;
								}
							}
							if (count == 3) {
								winStoneType = stone_type;
								return true;
							}
						}
					}
					if(x == 0 && z == 0) {
						if(y == 0) {
							for (int i = 1; i < 4; i++) {
								if (GameData.boardData.stone_type[i, i, i] == stone_type && GameData.boardData.is_stone[i, i, i]) {
									count++;
								} else {
									count = 0;
									break;
								}
							}
							if (count == 3) {
								winStoneType = stone_type;
								return true;
							}
						} else if (y == 3) {
							for (int i = 1; i < 4; i++) {
								if (GameData.boardData.stone_type[i, 3-i, i] == stone_type && GameData.boardData.is_stone[i, 3-i, i]) {
									count++;
								} else {
									count = 0;
									break;
								}
							}
							if (count == 3) {
								winStoneType = stone_type;
								return true;
							}
						}

						for (int i = 1; i < 4; i++) {
							if (GameData.boardData.stone_type[i, y, i] == stone_type && GameData.boardData.is_stone[i, y, i]) {
								count++;
							} else {
								count = 0;
								break;
							}
						}
						if (count == 3) {
							winStoneType = stone_type;
							return true;
						}
					} else if(x == 3 && z == 0) {
						if (y == 0) {
							for (int i = 1; i < 4; i++) {
								if (GameData.boardData.stone_type[3-i, i, i] == stone_type && GameData.boardData.is_stone[3 - i, i, i]) {
									count++;
								} else {
									count = 0;
									break;
								}
							}
							if (count == 3) {
								winStoneType = stone_type;
								return true;
							}
						} else if (y == 3) {
							for (int i = 1; i < 4; i++) {
								if (GameData.boardData.stone_type[3-i, 3 - i, i] == stone_type && GameData.boardData.is_stone[3 - i, 3 - i, i]) {
									count++;
								} else {
									count = 0;
									break;
								}
							}
							if (count == 3) {
								winStoneType = stone_type;
								return true;
							}
						}
						for (int i = 1; i < 4; i++) {
							if (GameData.boardData.stone_type[3-i, y, i] == stone_type && GameData.boardData.is_stone[3-i, y, i]) {
								count++;
							} else {
								count = 0;
								break;
							}
						}
						if (count == 3) {
							winStoneType = stone_type;
							return true;
						}
					}
				}
			}
		}
		for(x = 0; x < 4; x++) {
			for(z = 0; z < 4; z++) {
				if (GameData.boardData.is_stone[x, 0, z]) {
					int count = 0;
					bool stone_type = GameData.boardData.stone_type[x, 0, z];
					for (int y = 1; y < 4; y++) {
						if (GameData.boardData.stone_type[x, y, z] == stone_type && GameData.boardData.is_stone[x, y, z]) {
							count++;
						} else {
							count = 0;
							break;
						}
					}
					if (count == 3) {
						winStoneType = stone_type;
						return true;
					}
				}
			}
		}
		return false;
	}
}
