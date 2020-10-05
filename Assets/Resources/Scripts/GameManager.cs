using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	private void Update() {
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

				if (Judgment()) {
					isClear = true;
					Time.timeScale = 0;
				}
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
				posX = Random.Range(0, 4);
				posZ = Random.Range(0, 4);
				if (puttingStone[posX, posZ] >= 4) continue;
				Stone stone = Instantiate(EnemyStonePrefab, new Vector3(posX, 3.75f, posZ), Quaternion.identity);
				stone.stoneType = StoneType.ENEMY;
				TurnTimer = 1f;
				putEnemyStone = true;
				puttingStone[posX, posZ]++;
				break;
			}
		}
	}
	private bool Judgment() {
		foreach(var i in StoneList) {
			Vector3 posI = new Vector3((int)i.transform.position.x, (int)i.transform.position.y, (int)i.transform.position.z);
			foreach(var j in StoneList) {
				Vector3 posJ = new Vector3((int)j.transform.position.x, (int)j.transform.position.y, (int)j.transform.position.z);
				foreach (var k in StoneList) {
					Vector3 posK = new Vector3((int)k.transform.position.x, (int)k.transform.position.y, (int)k.transform.position.z);
					foreach (var l in StoneList) {
						Vector3 posL = new Vector3((int)l.transform.position.x, (int)l.transform.position.y, (int)l.transform.position.z);
						if (i.stoneType == j.stoneType && i.stoneType == k.stoneType && i.stoneType == l.stoneType
						&& i.stoneType == StoneType.PLAYER) {
							if(posI == posJ + Vector3.forward * 1
							&& posI == posK + Vector3.forward * 2
							&& posI == posL + Vector3.forward * 3) {
								i.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								j.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								k.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								l.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								return true;
							}
							else if (posI == posJ + Vector3.right * 1
							&& posI == posK + Vector3.right * 2
							&& posI == posL + Vector3.right * 3) {
								i.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								j.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								k.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								l.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								return true;
							}
							else if (posI == posJ + Vector3.up * 1
							&& posI == posK + Vector3.up * 2
							&& posI == posL + Vector3.up * 3) {
								i.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								j.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								k.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								l.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								return true;
							} 
							else if (posI == posJ + (Vector3.forward + Vector3.right) * 1
							&& posI == posK + (Vector3.forward + Vector3.right) * 2
							&& posI == posL + (Vector3.forward + Vector3.right) * 3) {
								i.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								j.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								k.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								l.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								return true;
							}
							else if (posI == posJ + (Vector3.forward + Vector3.left) * 1
							&& posI == posK + (Vector3.forward + Vector3.left) * 2
							&& posI == posL + (Vector3.forward + Vector3.left) * 3) {
								i.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								j.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								k.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								l.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								return true;
							}
							else if (posI == posJ + (Vector3.up + Vector3.forward) * 1
						    && posI == posK + (Vector3.up + Vector3.forward) * 2
							&& posI == posL + (Vector3.up + Vector3.forward) * 3) {
								i.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								j.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								k.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								l.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								return true;
							}
							else if (posI == posJ + (Vector3.up + Vector3.back) * 1
							&& posI == posK + (Vector3.up + Vector3.back) * 2
							&& posI == posL + (Vector3.up + Vector3.back) * 3) {
								i.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								j.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								k.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								l.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								return true;
							} 
							else if (posI == posJ + (Vector3.up + Vector3.right) * 1
							&& posI == posK + (Vector3.up + Vector3.right) * 2
							&& posI == posL + (Vector3.up + Vector3.right) * 3) {
								i.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								j.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								k.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								l.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								return true;
							}
							else if (posI == posJ + (Vector3.up + Vector3.left) * 1
							&& posI == posK + (Vector3.up + Vector3.left) * 2
							&& posI == posL + (Vector3.up + Vector3.left) * 3) {
								i.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								j.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								k.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								l.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								return true;
							}
							else if (posI == posJ + (Vector3.up + Vector3.forward + Vector3.right) * 1
							&& posI == posK + (Vector3.up + Vector3.forward + Vector3.right) * 2
							&& posI == posL + (Vector3.up + Vector3.forward + Vector3.right) * 3) {
								i.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								j.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								k.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								l.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								return true;
							}
							else if (posI == posJ + (Vector3.up + Vector3.forward + Vector3.left) * 1
							&& posI == posK + (Vector3.up + Vector3.forward + Vector3.left) * 2
							&& posI == posL + (Vector3.up + Vector3.forward + Vector3.left) * 3) {
								i.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								j.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								k.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								l.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								return true;
							}
							else if (posI == posJ + (Vector3.up + Vector3.back + Vector3.right) * 1
							&& posI == posK + (Vector3.up + Vector3.back + Vector3.right) * 2
							&& posI == posL + (Vector3.up + Vector3.back + Vector3.right) * 3) {
								i.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								j.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								k.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								l.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								return true;
							}
							else if (posI == posJ + (Vector3.up + Vector3.back + Vector3.left) * 1
							&& posI == posK + (Vector3.up + Vector3.back + Vector3.left) * 2
							&& posI == posL + (Vector3.up + Vector3.back + Vector3.left) * 3) {
								i.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								j.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								k.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								l.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}
}
