using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	#region Variables

	// Enemy State
	private enum EnemyState {
		Idle,
		Pursuing,
	}
	private EnemyState enemyState;

	// Enemy Data
	private MobSO mobData;
	private string mobName = "Slime";
	private float mobAtk = 0f;
	private float mobHp = 0f;

	// Material
	[Header("Material Settings")]
	[SerializeField] private Material flashMaterial;
	[HideInInspector] private Material defaultMaterial;

	// Radiueses
	[Header("Radius settings")]
	[SerializeField] private float patrolRadius = 4f;
	[SerializeField] private float pursuingRadius = 8f;
	private Vector2 radiusCenter;

	// Waiting 'till next Position (Patrolling)
	[Header("Patrol Settings")]
	[SerializeField] private float minPatrol;
	[SerializeField] private float maxPatrol;
	private float patrollingTime;
	private bool isReadyToNextPatrol = true;

	// EnemySettings
	[Header("Enemy Settings")]
	[SerializeField] float enemySpeed = 2.5f;
	[SerializeField] float alertSize = 2f;
	[SerializeField] float alertTime = 0.5f;
	[SerializeField] float attackOffSet = 3f;
	[SerializeField] float bulletPositionOffSet = -1f;
	private bool isMoving = false;
	private bool didAlertPopUp = false;

	// Bullet Settings
	[Header("Bullet Settings")]
	[SerializeField] GameObject bulletPrefab;
	[SerializeField] float bulletSpeed = 10f;
	[SerializeField] float bulletCooldown = 1.3f;
	[SerializeField] float enemyAimOffSet;
	[SerializeField] float recoilStrength = 2;
	private bool readyToShoot = false;

	// Coroutines
	private Coroutine patrolCoroutine;
	private Coroutine pursueCoroutine;

	// Components
	private GameObject player;
	private Rigidbody2D rb;
	private SpriteRenderer sr;
	private Animator animator;
	private GameObject playerAlert;

	#endregion

	private void Awake() {
		radiusCenter = transform.position;

		defaultMaterial = GetComponent<SpriteRenderer>().material;


		DefineData();
	}
	
	private void Start() {
		DefineComponents();

		enemyState = EnemyState.Idle;
		playerAlert.SetActive(false);
	}

	private void DefineComponents() {
		player = GameObject.FindGameObjectWithTag("Player");
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		playerAlert = transform.Find("PlayerAlert").gameObject;
	}
	
	private void DefineData() {
		mobData = Resources.Load<MobSO>("Datas/Mobs/" + mobName);

		mobName = mobData.name;
		mobAtk = mobData.attack;
		mobHp = mobData.hp;
		enemySpeed = mobData.movementSpeed;
	}
	
	private void Update() {
		// Set Enemy State
		StateModifier();

		// Start the Coroutines according to Enemy State
		if (enemyState == EnemyState.Idle) {
			if (pursueCoroutine != null) {
				StopCoroutine(pursueCoroutine);
				pursueCoroutine = null;
			}
			if (patrolCoroutine == null)
				patrolCoroutine = StartCoroutine(Patrolling()); // Cache and start patrolling
		}
		else if (enemyState == EnemyState.Pursuing) {
			if (patrolCoroutine != null) {
				StopCoroutine(patrolCoroutine);
				patrolCoroutine = null;
			}
			if (pursueCoroutine == null)
				pursueCoroutine = StartCoroutine(Pursuing()); // Cache and start pursuing
		}
	}

	bool isTakingDamage = false;

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("PlayerBullet")) {

			//isTakingDamage = false;
			StartCoroutine(Flash());
		}
	}

	private IEnumerator Flash() {
		if (!isTakingDamage) {
			print("Flashing");
			isTakingDamage = true;
			sr.material = flashMaterial;
			yield return new WaitForSeconds(0.05f);
			sr.material = defaultMaterial;
			isTakingDamage = false;
		}
	}
	#region States

	void StateModifier() {
		float distanceToPlayerFromCenter = GetDistanceFromCenter(player.transform);

		if (distanceToPlayerFromCenter <= patrolRadius) {
			enemyState = EnemyState.Pursuing;
		}
		else if (distanceToPlayerFromCenter >= patrolRadius &&
			distanceToPlayerFromCenter <= pursuingRadius &&
			enemyState == EnemyState.Pursuing) {
			enemyState = EnemyState.Pursuing;
		}
		else if (distanceToPlayerFromCenter >= patrolRadius &&
			distanceToPlayerFromCenter <= pursuingRadius &&
			enemyState != EnemyState.Pursuing) {
			enemyState = EnemyState.Idle;
		} else {
			enemyState = EnemyState.Idle;
		}
		//Debug.LogWarning(enemyState.ToString());
	}
	
	IEnumerator ShowPlayerAlert() {
		if (!didAlertPopUp) {
			didAlertPopUp = true;
			playerAlert.SetActive(true);
			playerAlert.GetComponent<Animator>().Play("EnemyAlert");
			yield return new WaitForSeconds(0.51f); // 0.3s animation
			playerAlert.SetActive(false);

		}
	}

	IEnumerator Patrolling() {
		while (true) {
			if (isReadyToNextPatrol) {
				isReadyToNextPatrol = false;

				float distanceToCenterFromEnemy = GetDistanceFromCenter(transform);

				// Back to patrol zone
				if (distanceToCenterFromEnemy >= patrolRadius) {
					rb.velocity = Vector2.zero;
					yield return new WaitForSeconds(1f);
					sr.flipX = !sr.flipX;
					yield return new WaitForSeconds(0.5f);
					sr.flipX = !sr.flipX;
					yield return new WaitForSeconds(1f);


					// Determine the target pos and direction
					Vector2 targetPos = radiusCenter;
					Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

					FlipSprite(targetPos);

					while (Vector2.Distance(transform.position, targetPos) > 0.1f) {
						MoveToTarget(direction); yield return null; // Wait until the next frame
					}

					StopMoving();
					
					mobHp = mobData.hp;

					// patrol timing
					patrollingTime = Random.Range(minPatrol, maxPatrol);
					yield return new WaitForSeconds(patrollingTime);
				}
				// Already in patrol zone
				else { 

					Vector2 targetPos = radiusCenter + (Random.insideUnitCircle * patrolRadius);
					Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
					
					FlipSprite(targetPos);

					while (Vector2.Distance(transform.position, targetPos) > 0.1f) {
						MoveToTarget(direction); yield return null; // Wait until the next frame
					}

					StopMoving();

					// patrol timing
					patrollingTime = Random.Range(minPatrol, maxPatrol);
					yield return new WaitForSeconds(patrollingTime);
				}
			}
			didAlertPopUp = false;
			isReadyToNextPatrol = true;
			yield return null;
		}
	}

	IEnumerator Pursuing() {
		while (enemyState == EnemyState.Pursuing) {

			StartCoroutine(ShowPlayerAlert());

			// Determine the target pos and direction
			Vector2 targetPos = player.transform.position;
			Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

			FlipSprite(targetPos);

			// Check if the enemy is within the attack offset distance
			if (Vector2.Distance(transform.position, targetPos) > attackOffSet) {
				// Continue moving towards the player
				MoveToTarget(direction);
			}
			else {
				readyToShoot = true;

				StopMoving();
				yield return StartCoroutine(ShootBullet()); // Wait for the bullet shooting to finish
			}

			yield return null;
		}

		StopMoving();
	}

	IEnumerator ShootBullet() {
		if (readyToShoot) {
			readyToShoot = false;
			//animator.enabled = false;

			float bulletDirectionOffSet = Random.Range(enemyAimOffSet * -1, enemyAimOffSet); // to give recoil effect
			GameObject bullet = Instantiate(bulletPrefab, transform.position + new Vector3(0, bulletPositionOffSet, 0), Quaternion.identity);
			Vector3 enemyAim = player.transform.position + new Vector3(bulletDirectionOffSet, bulletDirectionOffSet, 0);
			Vector2 bulletDirection = (enemyAim - transform.position + new Vector3(0, bulletPositionOffSet, 0)).normalized;

			bullet.GetComponent<Rigidbody2D>().velocity = bulletDirection * bulletSpeed;
			
			bullet.name = "Bullet";

			LeanTween.value(bullet, (val) => {
				bullet.transform.localScale = val;
			}, new Vector3(0f, 0f, 1f), new Vector3(1f, 1f, 1f), 0.35f).setEase(LeanTweenType.easeOutBack);


			//after shooting make the enemy recoil
			
			rb.AddForce((bulletDirection * -1) * recoilStrength, ForceMode2D.Impulse);
		}

		yield return new WaitForSeconds(bulletCooldown);
		animator.enabled = true;
		readyToShoot = true;
	}

	IEnumerator EnemyRecoil() {
		yield return null;
	}


	#endregion

	#region Functions

	private void FlipSprite(Vector2 targetPos) {
		// Flip the enemy by cheking target pos
		sr.flipX = targetPos.x < transform.position.x;
	}

	private void StopMoving() {
		// Stop once you reach
		rb.velocity = Vector2.zero;
		isMoving = false;
		animator.SetBool("isMoving", isMoving);
	}

	private void MoveToTarget(Vector2 direction) {
		isMoving = true;
		animator.SetBool("isMoving", isMoving);
		rb.velocity = direction * enemySpeed; // Set the velocity
	}

	float GetDistanceFromCenter(Transform entity) {
		return Vector2.Distance(entity.position, radiusCenter);
	}
	
	#endregion

	void OnDrawGizmosSelected() {
		Gizmos.color = new Color(0.9f, 0.2f, 0.2f, 0.5f);
		Gizmos.DrawWireSphere(radiusCenter, patrolRadius);

		Gizmos.color = new Color(0.9f, 0.4f, 0.4f, 0.5f);
		Gizmos.DrawWireSphere(radiusCenter, pursuingRadius);
	}
}