using System;
using UnityEngine;

public class WeaponController : MonoBehaviour {
	#region Variables

	[Header("Player And Weapon Transform")]
	[SerializeField] Transform player;
	[SerializeField] Transform weapon;

	Vector2 cursorPosition;
	bool isFacingRight;
	bool isFlipping;

	[Header("Swing Settings")]
	[SerializeField] GameObject slashPrefab;
	[SerializeField] float swingCooldown = 1f;
	[SerializeField] float slashSpeed = 1f;
	[SerializeField] float slashOffset = 1.5f;
	float currentSwingTime;
	bool canSwing = true;


	#endregion

	private void Start() {
		isFacingRight = (player.localScale.x == 1) ? true : false;
	}

	void Update() {
		cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		ManagePlayerRotation();
		ManageWeaponRotation();
		CheckForSwing();
	}

	private void CheckForSwing() {
		if (Input.GetMouseButton(0) && canSwing) {
			Swing();
			SwingAnimation(); 
			currentSwingTime = 0f;
			canSwing = false;
		}

		currentSwingTime += Time.deltaTime;
		canSwing = currentSwingTime >= swingCooldown; // otherwise false, this is called ternary operator -VERY COOL-
	}

	private void SwingAnimation() {
		
	}

	private void Swing() {
		Vector2 direction = cursorPosition - (Vector2)weapon.position; // Get Direction

		Vector2 forwardDirection = weapon.right;
		Vector2 spawnPosition = (Vector2)weapon.position + forwardDirection * slashOffset; // 1f is the offset

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Get Angle in Degrees
		

		GameObject _slash = Instantiate(slashPrefab, spawnPosition, Quaternion.identity);
		Rigidbody2D _slashRb = _slash.GetComponent<Rigidbody2D>();

		_slashRb.AddForce(direction * Vector2.one * slashSpeed, ForceMode2D.Impulse);
		_slashRb.rotation = angle; // rigidbody

		Destroy(_slash, _slash.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length - 0.1f);
	}

	private void ManagePlayerRotation() {
		// cursor at left of the player
		if (cursorPosition.x <= player.position.x && isFacingRight && !isFlipping) {
			StartFlip(-1);
		}
		//cursor at right of the player
		else if (cursorPosition.x > player.position.x && !isFacingRight && !isFlipping) {
			StartFlip(1);
		}
	}
	private void ManageWeaponRotation() {
		Vector2 direction = cursorPosition - (Vector2)weapon.position;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		weapon.rotation = Quaternion.Slerp(weapon.rotation, rotation, 5f * Time.deltaTime);

		// cursor at left of the player or right
		if (cursorPosition.x <= player.position.x)
			weapon.localScale = new Vector3(1, -1, 1);
		else if (cursorPosition.x > player.position.x)
			weapon.localScale = new Vector3(-1, 1, 1);
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
