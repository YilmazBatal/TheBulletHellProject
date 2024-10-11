using Cinemachine;
using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
	// Camera
	CinemachineImpulseSource impulse; // For Camera Shake

	// Player Controller
	PlayerController playerController;
	Rigidbody2D playerRB;

	// Components
	ParticleSystem ps;
	SpriteRenderer sr;
	CircleCollider2D cc;
	Rigidbody2D rb;

	// Bullet Values
	Vector3 initPos;
	[SerializeField] float bulletKnockbackStrength;
	[SerializeField] float bulletKnockbackDuration;

	private void Awake() {
		DefineComponents();

		initPos = transform.position;
	}

	private void DefineComponents() {
		impulse = Camera.main.GetComponent<CinemachineImpulseSource>();
		playerController = GameObject.Find("Player").GetComponent<PlayerController>();
		playerRB = playerController.gameObject.GetComponent<Rigidbody2D>();

		ps = GetComponent<ParticleSystem>();
		sr = GetComponent<SpriteRenderer>();
		cc = GetComponent<CircleCollider2D>();
		rb = GetComponent<Rigidbody2D>();
	}

	private void OnBecameInvisible() {
		//Destroy(gameObject);
	}
	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Player") && collision is BoxCollider2D) {
			HitPlayer();
		}
	}

	private void HitPlayer() {
		impulse.GenerateImpulse(); // Shake
		StartCoroutine(playerController.Flash()); // Flash
		ps.Play(); // Particle
		sr.enabled = false;
		cc.enabled = false;
		rb.velocity = Vector3.zero;
		gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;


		Vector2 bulletDir = -(initPos - playerController.transform.position).normalized;

		StartCoroutine(Knockback(bulletDir, bulletKnockbackStrength, bulletKnockbackDuration));
	}

	private Vector2 combinedKnockback = Vector2.zero;

	IEnumerator Knockback(Vector2 bulletDir, float force, float duration) {
		combinedKnockback += bulletDir * force;


		float elapsedTime = 0f;

		// Apply init force
		playerRB.AddForce(bulletDir * force, ForceMode2D.Impulse);

		// Gradually decrease the velocity
		while (elapsedTime < duration) {
			elapsedTime += Time.deltaTime;

			playerRB.velocity = Vector2.Lerp(combinedKnockback, Vector2.zero, elapsedTime / duration);

			yield return null;
		}

		combinedKnockback = Vector2.zero;
	}
}
