using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	#region Variables

	// Enemy State
	private enum EnemyState {
		Idle,
		Pursuing,
	}
	private EnemyState enemyState;

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
	bool isMoving = false;
	bool didAlertPopUp = false;
	bool backToPatrol = false;
	bool didRunAway = false;

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

	private void Start() {
		DefineComponents();

		radiusCenter = transform.position;
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

	private void Update() {
		float distanceToPlayerFromCenter = Vector2.Distance(player.transform.position, radiusCenter);

		StateModifier(distanceToPlayerFromCenter);

		if (enemyState == EnemyState.Idle) {
			if (pursueCoroutine != null) {
				StopCoroutine(pursueCoroutine);
				pursueCoroutine = null;
			}

			if (patrolCoroutine == null) {
				patrolCoroutine = StartCoroutine(Patrolling()); // Cache and start patrolling
			}
		} else if (enemyState == EnemyState.Pursuing) {
			if (patrolCoroutine != null) {
				StopCoroutine(patrolCoroutine);
				patrolCoroutine = null;
			}

			if (pursueCoroutine == null) {
				pursueCoroutine = StartCoroutine(Pursuing()); // Cache and start pursuing
			}
		}
	}

	private void StateModifier(float distanceToPlayerFromCenter) {
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
		Debug.LogWarning(enemyState.ToString());
	}

	private IEnumerator ShowPlayerAlert() {
		if (!didAlertPopUp) {
			playerAlert.SetActive(true);
			playerAlert.GetComponent<Animator>().Play("EnemyAlert");
			yield return new WaitForSeconds(0.51f); // 0.3s animation
			playerAlert.SetActive(false);

		}
	}


	IEnumerator Patrolling() {
		print("im in patrol");

		while (true) {
			print("HELLOOO");

			if (isReadyToNextPatrol) {
				print("looking for new places to go");

				isReadyToNextPatrol = false;
				float distanceToCenterFromEnemy = Vector2.Distance(radiusCenter, gameObject.transform.position);

				if (distanceToCenterFromEnemy >= patrolRadius) {
					print("im trying to go back	but cant");

					rb.velocity = Vector2.zero;
					yield return new WaitForSeconds(1f);
					sr.flipX = !sr.flipX;
					yield return new WaitForSeconds(0.5f);
					sr.flipX = !sr.flipX;
					yield return new WaitForSeconds(1f);


					// Determine the target pos and direction
					Vector2 targetPos = radiusCenter;
					Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

					// Flip the enemy by cheking target pos
					sr.flipX = targetPos.x < transform.position.x;

					// Walk 'till you reach with offset
					while (Vector2.Distance(transform.position, targetPos) > 0.1f) {
						isMoving = true;
						animator.SetBool("isMoving", isMoving);
						rb.velocity = direction * enemySpeed; // Set the velocity
						yield return null; // Wait until the next frame
						print("HELLOOO 1");

					}

					// Stop once you reach
					rb.velocity = Vector2.zero;
					isMoving = false;
					animator.SetBool("isMoving", isMoving);

					// patrol timing
					patrollingTime = Random.Range(minPatrol, maxPatrol);
					yield return new WaitForSeconds(patrollingTime);
				}
				else {
					print("what am i doing here");

					// Determine the target pos and direction
					Vector2 targetPos = Random.insideUnitCircle * patrolRadius;
					Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

					// Flip the enemy by cheking target pos
					sr.flipX = targetPos.x < transform.position.x;


					// Walk 'till you reach with offset
					while (Vector2.Distance(transform.position, targetPos) > 0.1f) {
						isMoving = true;
						animator.SetBool("isMoving", isMoving);
						rb.velocity = direction * enemySpeed; // Set the velocity
						yield return null; // Wait until the next frame
						print("HELLOOO 2");
					}

					// Stop once you reach
					rb.velocity = Vector2.zero;
					isMoving = false;
					animator.SetBool("isMoving", isMoving);

					// patrol timing
					patrollingTime = Random.Range(minPatrol, maxPatrol);
					yield return new WaitForSeconds(patrollingTime);
				}
					

			}
			isReadyToNextPatrol = true;
			 
			yield return null;
		}
	}

	IEnumerator Pursuing() {
		print("im pursuing");

		while (enemyState == EnemyState.Pursuing) {
			// Determine the target pos and direction
			Vector2 targetPos = player.transform.position;
			Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

			// Flip the enemy by cheking target pos
			sr.flipX = targetPos.x < transform.position.x;

			// Walk 'till you reach with offset
			isMoving = true;
			animator.SetBool("isMoving", isMoving);
			rb.velocity = direction * enemySpeed; // Set the velocity

			//pursueCoroutine = null;
			
			yield return null;
		}

		// Stop once you reach
		rb.velocity = Vector2.zero;
		isMoving = false;
		animator.SetBool("isMoving", isMoving);

	}

	private void OnDrawGizmos() {
		Gizmos.color = new Color(0.95f, 0.1f, 0.1f);
		Gizmos.DrawWireSphere(radiusCenter, patrolRadius);

		Gizmos.color = new Color(0.95f, 0.35f, 0.35f);
		Gizmos.DrawWireSphere(radiusCenter, pursuingRadius);
	}
}