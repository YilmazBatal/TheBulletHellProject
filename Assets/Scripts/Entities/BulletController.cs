using UnityEngine;

public class BulletController : MonoBehaviour
{
	private void OnBecameInvisible() {
		Destroy(gameObject);
	}
}
