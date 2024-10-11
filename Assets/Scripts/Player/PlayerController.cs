using Cinemachine;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
	#region Variables

	CinemachineImpulseSource impulse; // For Camera Shake

	[Header("*** Components ***")]
    [SerializeField] Rigidbody2D rb;
	[SerializeField] SpriteRenderer sr;

	[Header("*** Movement Values ***")]
	//[SerializeField] private InputActionReference movement;

    [SerializeField] float playerSpeed = 8f;
    [SerializeField] float currentSpeed;
	[SerializeField] private float smoothFactor = 0.4f;

	private Vector2 movementDirection;
	private Vector2 targetVelocity;
	private Vector2 currentVelocity;

	[Header("*** Dash Values ***")]
	[SerializeField] public float dashSpeed = 50f;
	[SerializeField] public float dashTime = 0.15f;
	[SerializeField] public float dashCooldown = 1f;
	private bool isDashing = false;
	private bool canDash = true;

	// Material
	[Header("Material Settings")]
	[SerializeField] private Material flashMaterial;
	[HideInInspector] private Material defaultMaterial;
	[SerializeField] private float flashDuration = 0.1f;
	bool isFlashing;

	#endregion

	private void Awake() {
		defaultMaterial = sr.material;
		flashDuration = dashTime;
		impulse = Camera.main.GetComponent<CinemachineImpulseSource>();

	}

	void Update() {
		if (DialogueManager.GetInstance().dialogueIsPlaying) {
			rb.velocity = Vector2.zero; // to prevent glitch
			return;
		}
		Movement();
		Dash();
	}

	public void Movement() {
		Vector2 input = new Vector2(
			Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical")
		).normalized;

		if (Input.GetKey(KeyCode.LeftShift)) {
			currentSpeed = playerSpeed / 2;
		}
		else {
			currentSpeed = playerSpeed;
		}

		targetVelocity = input * currentSpeed;
		currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, smoothFactor);

		rb.velocity = currentVelocity;

		if (input.magnitude == 0f) {
			rb.velocity = Vector2.zero;
		}
	}

	void Dash() {
		if (Input.GetKeyDown(KeyCode.Space) && !isDashing && canDash) {
			StartCoroutine(DashRoutine());
			StartCoroutine(Flash());  // Assuming Flash() is for a visual effect
			impulse.GenerateImpulse(); // Shake Camera
		}
	}

	IEnumerator DashRoutine() {
		// Disable dashing during the dash and cooldown period
		canDash = false;

		// Start the dash
		isDashing = true;
		float originalSpeed = playerSpeed;
		playerSpeed = dashSpeed;

		// Dash duration
		yield return new WaitForSeconds(dashTime);

		// End the dash
		playerSpeed = originalSpeed;
		isDashing = false;

		// Cooldown period before allowing another dash
		yield return new WaitForSeconds(dashCooldown);

		// Enable dashing again
		canDash = true;
	}

	public IEnumerator Flash() {
		if (!isFlashing) {
			isFlashing = true;
			sr.material = flashMaterial;
			yield return new WaitForSeconds(flashDuration);
			sr.material = defaultMaterial;
			isFlashing = false;
		}
	}
}