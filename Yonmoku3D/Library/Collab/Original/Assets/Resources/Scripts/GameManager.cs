using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	static public List<Stone> StoneList = new List<Stone>();

	public GameObject MainCamera;
	public GameObject PlayerStonePrefab;

	private Vector3 lastMousePosition;
	private Vector3 newAngle = new Vector3(0, 0, 0);

	private Vector3 firstPos = Vector3.zero;

	private void Update() {
		Debug.Log("hogehuga");
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
				Debug.Log(i.transform.position);
			}
		}

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
				if(hit.collider.gameObject.layer == 8) {
					Vector3 pos = hit.collider.transform.position;

					if (pos == firstPos) {
						pos.y = 3.75f;
						Instantiate(PlayerStonePrefab, pos, Quaternion.identity);
					}

					firstPos = Vector3.zero;
				}
			}
		}
	}

	private void Judgment(Stone stone) {
		foreach(var i in StoneList) {
			
		}
	}
}
