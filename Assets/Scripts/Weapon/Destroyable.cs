using UnityEngine;

public class Destroyable : MonoBehaviour {
	public void DestroyMe() {
		Destroy(gameObject);
	}
}