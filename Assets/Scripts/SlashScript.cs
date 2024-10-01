using Cinemachine;
using System.Collections;
using UnityEngine;

public class SlashScript : MonoBehaviour
{
	CinemachineImpulseSource impulse; // For Camera Shake

	[SerializeField] Material hitMaterial;
	[SerializeField] Material defaultMaterial;
	[SerializeField] public float knockbackStrength = 5f;
	[SerializeField] public bool isTakingDamage = false;
	[HideInInspector] public float attack;
	Transform weapon;

	private void Awake() {
		impulse = Camera.main.GetComponent<CinemachineImpulseSource>();
		weapon = GameObject.Find("Weapon").transform;
	}


	// Call this method when the object gets hit
	public void ApplyKnockback(Vector2 hitPosition, Rigidbody2D rb) {
		// Calculate the knockback direction (away from the hit position)
		Vector2 direction = (rb.position - hitPosition).normalized;

		// Apply force in the direction of the knockback
		rb.AddForce(direction * knockbackStrength, ForceMode2D.Impulse);
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Enemy")) {
			Debug.Log("Dealt : " + attack + " Damage");

			impulse.GenerateImpulse(); // Shake Camera
			ApplyKnockback(weapon.position, collision.gameObject.GetComponent<Rigidbody2D>());
			
		}
	}
}
