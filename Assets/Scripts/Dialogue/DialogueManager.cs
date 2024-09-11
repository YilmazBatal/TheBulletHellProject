using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour {
	
	[Header("Dialogue UI")]
	[SerializeField] private GameObject dialoguePanel;
	[SerializeField] private TextMeshProUGUI dialogueText;

	[Header("Choices UI")]

	[SerializeField] private GameObject[] choices;
	private TextMeshProUGUI[] choicesText;


	private Story currentStory;

	public bool dialogueIsPlaying {
		get;
		private set;
	}

	private static DialogueManager instance;

	private void Awake() {

		if (instance != null) {
			Debug.LogWarning("Found more than once Dialogue Manager in the scene");
		}
		instance = this;
	}

	public static DialogueManager GetInstance() {
		return instance;
	}

	private void Start() {
		dialogueIsPlaying = false;
		dialoguePanel.SetActive(false);

		choicesText = new TextMeshProUGUI[choices.Length];
		int index = 0;
		foreach (var choice in choices) {
			choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
			index++;
		}
}

private void Update() {
		if (!dialogueIsPlaying) {
			return;
		}

		if (Input.GetKeyDown(KeyCode.Return)) {
			ContinueStory();
		}
	}

	public void EnterDialogueMode(TextAsset inkJSON) {
		currentStory = new Story(inkJSON.text);
		dialogueIsPlaying = true;
		dialoguePanel.SetActive(true);

		ContinueStory();
	}

	private void ContinueStory() {
		if (currentStory.canContinue) {
			// Set text
			dialogueText.text = currentStory.Continue();
			// display choices
			DisplayChoices();
		}
		else {
			ExitDialogueMode();
		}
	}

	public void ExitDialogueMode() {
		dialogueIsPlaying = false;
		dialoguePanel.SetActive(false);
		dialogueText.text = string.Empty;
	}
	private void DisplayChoices() {
		List<Choice> currentChoices = currentStory.currentChoices;

		if (currentChoices.Count > choices.Length) {
			Debug.Log("More choices were given than the UI can support. Number of choices given: " + currentChoices.Count);
		}

		int index = 0;
        foreach (Choice choice in currentChoices)
        {
			choices[index].gameObject.SetActive(true);
			choicesText[index].text = choice.text;
			index++;
		}
		for (int i = index; i < choices.Length; i++) {
			choices[i].gameObject.SetActive(false);
		}

		StartCoroutine(SelectFirstChoice());
    }

	private IEnumerator SelectFirstChoice() {
		// Event System requires we clear it first, then wait
		EventSystem.current.SetSelectedGameObject(null);
		yield return new WaitForEndOfFrame();
		EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
	}
}
