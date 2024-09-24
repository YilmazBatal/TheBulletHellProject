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
    [SerializeField] float currentSpeed;
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

		if (Input.GetKey(KeyCode.LeftShift)) {
			currentSpeed = maxSpeed/2;
		}
		else {
			currentSpeed = maxSpeed;
		}

		targetVelocity = input * currentSpeed;
		currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, smoothFactor);

		rb.velocity = currentVelocity;

		if (input.magnitude == 0f) {
			rb.velocity = Vector2.zero;
		}
		#endregion
	}
}
