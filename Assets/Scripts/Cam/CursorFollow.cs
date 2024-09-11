using UnityEngine;
using Cinemachine;

public class CursorFollow : MonoBehaviour {
	[Header("*** Vars ***")]
	[SerializeField] Transform cursorFollow;
	[SerializeField] Transform player;
	[SerializeField] float threshold = 5f;

	private void Update() {
		FollowCursor();
	}

	private void FollowCursor() {
		Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		Vector3 targetPosition = (player.position + cursorPosition) / 2f;

		//targetPosition.x =  cursorFollow.position = Vector3.Lerp(cursorFollow.position, targetPosition, followSpeed * Time.deltaTime);

		targetPosition.x = Mathf.Clamp(targetPosition.x, -threshold + player.position.x, threshold + player.position.x);
		targetPosition.y = Mathf.Clamp(targetPosition.y, -threshold + player.position.y, threshold + player.position.y);

		cursorFollow.transform.position = targetPosition;
	}
}