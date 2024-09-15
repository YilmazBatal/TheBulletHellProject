using UnityEngine;

public class DetectNPC : MonoBehaviour
{
	#region Variables

	[Header("Collider")]
	[SerializeField] CircleCollider2D playerCollider;
	[SerializeField] Rigidbody2D rb;

	GameObject currentNpcScript;
	GameObject currentNpcCue;

	bool npcAvailable = false;
	bool npcInRange = false;



	#endregion


	private void Update() {
		if (npcAvailable) {
			if (npcInRange && !DialogueManager.GetInstance().dialogueIsPlaying) {

				
				if (Input.GetKeyDown(KeyCode.E) && currentNpcScript.GetComponent<DialogueTrigger>().inkJSON != null) {
					DialogueManager.GetInstance().EnterDialogueMode(currentNpcScript.GetComponent<DialogueTrigger>().inkJSON);
					
				}
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("NPC")) {
			npcAvailable = true;

			currentNpcScript = collision.gameObject.GetComponent<DialogueTrigger>().gameObject; // get dialogue
			currentNpcCue = collision.gameObject.transform.parent.GetChild(2).gameObject; // get visual cue

			npcInRange = true;
			currentNpcCue.SetActive(true);
		}
	}
	private void OnTriggerExit2D(Collider2D collision) {
		if (collision.CompareTag("NPC")) {
			npcAvailable = false;
			npcInRange = false;
			currentNpcScript = null;
			currentNpcCue.SetActive(false);
		}
	}

}
