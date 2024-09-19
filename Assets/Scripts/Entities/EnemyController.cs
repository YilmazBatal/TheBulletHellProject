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
	bool isMoving = false;

	// Components
	GameObject player;
	Rigidbody2D rb;
	SpriteRenderer sr;
	Animator animator;
	GameObject playerAlert;
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

	private void FixedUpdate() {
		float distanceToPlayerFromCenter = Vector2.Distance(player.transform.position, radiusCenter);

		// this means player in patrol radius
		if (distanceToPlayerFromCenter <= patrolRadius) {
			// chase player
			enemyState = EnemyState.Pursuing;
			ShowPlayerAlert();
		}
		// this means player in pursuing radius
		else if (distanceToPlayerFromCenter <= pursuingRadius) {
			if (enemyState == EnemyState.Pursuing) {
				// chase player till the pursuing border
				enemyState = EnemyState.Pursuing;
			}
			else {
				print("is this getting printed");
				// back to patrol zone and keep patrolling
				enemyState = EnemyState.Idle;
				//print("Patrolling");
			}
		}
		// this means player ran away from the enemy or never been there
		else if (distanceToPlayerFromCenter >= pursuingRadius) {
			// back to patrol zone and keep patrolling
			enemyState = EnemyState.Idle;
			StartCoroutine(Patrolling());
		}
		Debug.Log(enemyState.ToString());
    }

	private void ShowPlayerAlert() {
		playerAlert.SetActive(true);
	}

	IEnumerator Patrolling() {
		while (true) {
			if (isReadyToNextPatrol) {
				isReadyToNextPatrol = false;

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
				}

				// Stop once you reach
				rb.velocity = Vector2.zero;
				isMoving = false;
				animator.SetBool("isMoving", isMoving);

				// patrol timing
				patrollingTime = Random.Range(minPatrol, maxPatrol);
				yield return new WaitForSeconds(patrollingTime);

				isReadyToNextPatrol = true;
			}
			yield return null;			
		}
	}



	private void OnDrawGizmos() {
		Gizmos.color = new Color (0.95f, 0.1f, 0.1f);
		Gizmos.DrawWireSphere(radiusCenter, patrolRadius);

		Gizmos.color = new Color(0.95f, 0.35f, 0.35f);
		Gizmos.DrawWireSphere(radiusCenter, pursuingRadius);
	}
}
