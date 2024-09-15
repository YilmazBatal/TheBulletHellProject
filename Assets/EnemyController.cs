using Pathfinding;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public Vector2 patrolCenter; 
	private Vector2 patrolPoint;
	public float patrolRadius = 5f;
	public float speed = 2.5f;


	public float minPatrolTime;
	public float maxPatrolTime;
	
	private void Start() {
		// Set the initial patrol point
		//patrolPoint = GetRandomPatrolPoint();
	}

	private void Update() {
		Patrol();
	}

	private void Patrol() {
		float randomWaitTime = Random.Range(minPatrolTime, maxPatrolTime);
		
		LocateAndMove();
	}

	private void LocateAndMove() {
		Vector2 targetPosition = Random.insideUnitCircle * patrolRadius;
		gameObject.transform.position = targetPosition;
	}
}
