using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour {

	static public List<Stone> StoneList = new List<Stone>();

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

	private bool isClear = false;

	private void Start() {
		
	}
	private void Update() {
		if (Judgment()) {
			isClear = true;
			Time.timeScale = 0;
			Debug.Log("win");
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

		if (Input.GetKeyDown(KeyCode.D)) {
			foreach (var i in StoneList) {
				Debug.Log((int)i.transform.position.y);
			}
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
			while (true) {
				posX = UnityEngine.Random.Range(0, 4);
				posZ = UnityEngine.Random.Range(0, 4);
				if (puttingStone[posX, posZ] >= 4) continue;
				Stone stone = Instantiate(EnemyStonePrefab, new Vector3(posX, 3.75f, posZ), Quaternion.identity);
				stone.stoneType = StoneType.ENEMY;
				stone.CountData = GameTurnCount;
				TurnTimer = 1f;
				putEnemyStone = true;
				puttingStone[posX, posZ]++;
				MatchTurn();
				break;
			}
		}
	}

	void MatchTurn() {
		for(int i = 0; i < 4; i++) {
			for(int j = 0; j < 4; j++) {
				for(int k = 0; k < 4; k++) {
					if(GameData.stoneDatas[i, j, k].is_stone) {
						if(GameData.stoneDatas[i,j,k].stone_type)
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
			if(!zloop) {
				x++;
				if(x > 3) {
					zloop = true;
					x = 0;
				}
			}
			else { 
				z++;
				if (z > 3) {
					break;
				}
			}

			for(int y = 0; y < 3; y++) {
				if(GameData.stoneDatas[x,y,z].is_stone) {
					int count = 0;
					bool stone_type = GameData.stoneDatas[x, y, z].stone_type;
					if (x == 0) {
						for (int i = 1; i < 4; i++) {
							if (GameData.stoneDatas[i, y, z].stone_type == stone_type && GameData.stoneDatas[i, y, z].is_stone) {
								count++;
							} else {
								count = 0;
								break;
							}
						}
						if (count == 3) return true;
						if (y == 0) {
							for (int i = 1; i < 4; i++) {
								if (GameData.stoneDatas[i, i, z].stone_type == stone_type && GameData.stoneDatas[i, i, z].is_stone) {
									count++;
								} else {
									count = 0;
									break;
								}
							}
							if (count == 3) return true;
						} else if (y == 3) {
							for (int i = 1; i < 4; i++) {
								if (GameData.stoneDatas[i, 3 - i, z].stone_type == stone_type && GameData.stoneDatas[i, 3 - i, z].is_stone) {
									count++;
								} else {
									count = 0;
									break;
								}
							}
							if (count == 3) return true;
						}
					}
					if (z == 0) {
						for (int i = 1; i < 4; i++) {
							if (GameData.stoneDatas[x, y, i].stone_type == stone_type && GameData.stoneDatas[x, y, i].is_stone) {
								count++;
							} else {
								count = 0;
								break;
							}
						}
						if (count == 3) return true;
						if (y == 0) {
							for (int i = 1; i < 4; i++) {
								if (GameData.stoneDatas[x, i, i].stone_type == stone_type && GameData.stoneDatas[x, i, i].is_stone) {
									count++;
								} else {
									count = 0;
									break;
								}
							}
							if (count == 3) return true;
						} else if (y == 3) {
							for (int i = 1; i < 4; i++) {
								if (GameData.stoneDatas[x, 3 - i, i].stone_type == stone_type && GameData.stoneDatas[x, 3 - i, i].is_stone) {
									count++;
								} else {
									count = 0;
									break;
								}
							}
							if (count == 3) return true;
						}
					}
					if(x == 0 && z == 0) {
						if(y == 0) {
							for (int i = 1; i < 4; i++) {
								if (GameData.stoneDatas[i, i, i].stone_type == stone_type && GameData.stoneDatas[i, i, i].is_stone) {
									count++;
								} else {
									count = 0;
									break;
								}
							}
							if (count == 3) return true;
						} else if (y == 3) {
							for (int i = 1; i < 4; i++) {
								if (GameData.stoneDatas[i, 3-i, i].stone_type == stone_type && GameData.stoneDatas[i, 3-i, i].is_stone) {
									count++;
								} else {
									count = 0;
									break;
								}
							}
							if (count == 3) return true;
						}

						for (int i = 1; i < 4; i++) {
							if (GameData.stoneDatas[i, y, i].stone_type == stone_type && GameData.stoneDatas[i, y, i].is_stone) {
								count++;
							} else {
								count = 0;
								break;
							}
						}
						if (count == 3) return true;
					} else if(x == 3 && z == 0) {
						if (y == 0) {
							for (int i = 1; i < 4; i++) {
								if (GameData.stoneDatas[3-i, i, i].stone_type == stone_type && GameData.stoneDatas[3 - i, i, i].is_stone) {
									count++;
								} else {
									count = 0;
									break;
								}
							}
							if (count == 3) return true;
						} else if (y == 3) {
							for (int i = 1; i < 4; i++) {
								if (GameData.stoneDatas[3-i, 3 - i, i].stone_type == stone_type && GameData.stoneDatas[3 - i, 3 - i, i].is_stone) {
									count++;
								} else {
									count = 0;
									break;
								}
							}
							if (count == 3) return true;
						}
						for (int i = 1; i < 4; i++) {
							if (GameData.stoneDatas[3-i, y, i].stone_type == stone_type && GameData.stoneDatas[3-i, y, i].is_stone) {
								count++;
							} else {
								count = 0;
								break;
							}
						}
						if (count == 3) return true;
					}
				}
			}
		}
		for(x = 0; x < 4; x++) {
			for(z = 0; z < 4; z++) {
				if (GameData.stoneDatas[x, 0, z].is_stone) {
					int count = 0;
					bool stone_type = GameData.stoneDatas[x, 0, z].stone_type;
					for (int y = 1; y < 4; y++) {
						if (GameData.stoneDatas[x, y, z].stone_type == stone_type && GameData.stoneDatas[x, y, z].is_stone) {
							count++;
						} else {
							count = 0;
							break;
						}
					}
					if (count == 3) return true;
				}
			}
		}
		return false;
	}
}
