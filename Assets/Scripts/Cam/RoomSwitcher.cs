using UnityEngine;
using Cinemachine;

public class RoomSwitcher : MonoBehaviour {

	[SerializeField] CinemachineVirtualCamera roomCam;
	[SerializeField] GameObject gridmap;
	CinemachineVirtualCamera[] virtualCameras;

	private void Awake() {
		//HideTilemaps();
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Player")) {
			
			//HideTilemaps();
			HideCams();

			roomCam.gameObject.SetActive(true);
            //gridmap.gameObject.SetActive(true);
		}
	}

	private static void HideCams() {
		foreach (var cam in FindObjectsOfType<CinemachineVirtualCamera>()) {
			cam.gameObject.SetActive(false);
		}
	}

	private static void HideTilemaps() {
		foreach (var tilemap in FindObjectsOfType<Grid>()) {
			tilemap.gameObject.SetActive(false);
		}
	}
}
