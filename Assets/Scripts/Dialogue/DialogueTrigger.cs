using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("VisualCue")]
    [SerializeField] private GameObject visualCue;

	[Header("Ink JSON")]
	[SerializeField] public TextAsset inkJSON;

	private void Awake() {
		visualCue.SetActive(false);
	}

	//private void Update() {
	//	if (playerInRage && !DialogueManager.GetInstance().dialogueIsPlaying) {
			
	//		if (Input.GetKeyDown(KeyCode.E)) {
	//			DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
	//		}
	//	}
	//}
	//private void OnTriggerEnter2D(Collider2D collision) {
 //       if (collision.CompareTag("Player")) {
 //           playerInRage = true;
	//		visualCue.SetActive(true);
 //       }
 //   }
	//private void OnTriggerExit2D(Collider2D collision) {
	//	if (collision.CompareTag("Player")) {
	//		playerInRage = false;
	//		visualCue.SetActive(false);
	//	}
	//}
}
