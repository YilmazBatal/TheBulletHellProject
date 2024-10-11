using Cinemachine;
using UnityEngine;

public class SlashScript : MonoBehaviour
{
	CinemachineImpulseSource impulse; // For Camera Shake

	[SerializeField] public float knockbackStrength = 5f;
	[SerializeField] public bool isTakingDamage = false;
	[HideInInspector] public float attack;
	Transform weapon;

	private void Awake() {
		impulse = Camera.main.GetComponent<CinemachineImpulseSource>();
		weapon = GameObject.Find("Weapon").transform;
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Enemy")) {
			Debug.Log("Dealt : " + attack + " Damage");

			impulse.GenerateImpulse(); // Shake Camera
		}
	}
}
