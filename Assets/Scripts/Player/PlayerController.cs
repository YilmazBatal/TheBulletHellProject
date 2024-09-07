using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	#region Variables

	[Header("*** Components ***")]
	
    [SerializeField] Rigidbody2D rb;

	[Header("*** Movement Values ***")]

    [SerializeField] float maxSpeed = 5f;
	[SerializeField] private float smoothFactor = 0.5f;

	private Vector2 targetVelocity;
	private Vector2 currentVelocity;
	#endregion


	void Update() {
		Movement();
	}

	
	private void Movement() {
		Vector2 input = new Vector2(
			Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical")
		).normalized;

		targetVelocity = input * maxSpeed;
		currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, smoothFactor);

		rb.velocity = currentVelocity;

		if (input.magnitude == 0f) {
			rb.velocity = Vector2.zero;
		}
	}
}
