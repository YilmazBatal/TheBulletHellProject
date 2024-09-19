using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
	#region Variables

	[Header("*** Components ***")]
    [SerializeField] Rigidbody2D rb;

	[Header("*** Movement Values ***")]
	//[SerializeField] private InputActionReference movement;

    [SerializeField] float maxSpeed = 5f;
	[SerializeField] private float smoothFactor = 0.5f;

	private Vector2 movementDirection;
	private Vector2 targetVelocity;
	private Vector2 currentVelocity;

	#endregion


	void FixedUpdate() {
		if (DialogueManager.GetInstance().dialogueIsPlaying) {
			rb.velocity = Vector2.zero; // to prevent glitch
			return;
		}
		Movement();
	}

	public void Movement() {
		#region Old Input System
		Vector2 input = new Vector2(
			Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical")
		).normalized;

		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			maxSpeed = maxSpeed/2;
		}
		else {
			maxSpeed = 5f;
		}

		targetVelocity = input * maxSpeed;
		currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, smoothFactor);

		rb.velocity = currentVelocity;

		if (input.magnitude == 0f) {
			rb.velocity = Vector2.zero;
		}
		#endregion

		#region New Input System

		//movementDirection = movement.action.ReadValue<Vector2>();

		////print(movementDirection);

		//targetVelocity = new Vector2(movementDirection.x * maxSpeed, movementDirection.y * maxSpeed);

		//currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, smoothFactor * Time.deltaTime);

		//rb.velocity = targetVelocity;

		//// Stop movement when there's no input
		//if (movementDirection.magnitude == 0f) {
		//	rb.velocity = Vector2.zero;
		//}

		#endregion

		#region New Input System 2

		//movementDirection = InputManager.GetInstance().GetMoveDirection();

		////print(movementDirection);

		//targetVelocity = movementDirection * maxSpeed;

		////currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, smoothFactor * Time.deltaTime);
		//rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, smoothFactor);
		////rb.velocity = targetVelocity;

		//// Stop movement when there's no input
		//if (movementDirection.magnitude == 0f) {
		//	rb.velocity = Vector2.zero;
		//}

		#endregion
	}
}
