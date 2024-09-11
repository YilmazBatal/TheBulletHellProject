using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {
	#region Variables
	[SerializeField] Transform player;

	Vector2 cursorPosition;


	bool isFacingRight;
	bool isFlipping;

	#endregion

	private void Start() {
		isFacingRight = (player.localScale.x == 1) ? true : false;
	}

	void Update() {
		cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		// cursor at left of the player
		if (cursorPosition.x <= player.position.x && isFacingRight && !isFlipping) {
			StartFlip(-1);
		}
		//cursor at right of the player
		else if (cursorPosition.x > player.position.x && !isFacingRight && !isFlipping) {
			StartFlip(1);
		}
	}

	private void StartFlip(float targetScaleX) {
		isFlipping = true;

		LeanTween.value(gameObject, FlipPlayer, player.localScale.x, targetScaleX, 0.15f).setEase(LeanTweenType.easeInOutCubic).setOnComplete(() =>
		{
			isFacingRight = (targetScaleX == 1);
			isFlipping = false;
		});
	}

	void FlipPlayer(float val) {
		player.localScale = new Vector3(val, 1, 1);

	}
}
