using UnityEngine;
using Cinemachine;

public class RoomSwitcher : MonoBehaviour {

	[SerializeField] CinemachineVirtualCamera roomCam;
	CinemachineVirtualCamera[] virtualCameras;

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Player")) {
			
			foreach (var cam in FindObjectsOfType<CinemachineVirtualCamera>()) {
				cam.gameObject.SetActive(false);
			}


			roomCam.gameObject.SetActive(true);
		}
	}
}
