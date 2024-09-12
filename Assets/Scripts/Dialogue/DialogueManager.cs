using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour {
	
	[Header("Dialogue UI")]
	[SerializeField] private GameObject dialoguePanel;
	[SerializeField] private TextMeshProUGUI dialogueText;
	[SerializeField] private TextMeshProUGUI displayNameText;
	[SerializeField] private Animator portraitAnimator;
	[SerializeField] private Animator layoutAnimator;

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

	private const string SPEAKER_TAG = "speaker";
	private const string PORTRAIT_TAG = "portrait";
	private const string LAYOUT_TAG = "layout";

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
			//Handle Tags
			HandleTags(currentStory.currentTags);
		}
		else {
			ExitDialogueMode();
		}
	}

	private void HandleTags(List<string> currentTags) {
        foreach (string tag in currentTags)
        {
			string[] splitTag = tag.Split(':');
			if (splitTag.Length != 2) {
				Debug.LogError("Tag could not be appropriatly parsed: " + tag);
			}

			string tagKey = splitTag[0];
			string tagValue = splitTag[1];

			//handle the tag
			switch (tagKey) {
				case SPEAKER_TAG:
					displayNameText.text = tagValue;
					break;
				case PORTRAIT_TAG:
					portraitAnimator.Play(tagValue);
					break;
				case LAYOUT_TAG:
					layoutAnimator.Play(tagValue);
					break;
				default:
					Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
					break;
			}
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

		StartCoroutine(EnsureFocusOnChoices());

		StartCoroutine(SelectFirstChoice());
    }

	private IEnumerator EnsureFocusOnChoices() {
		while (dialogueIsPlaying) {
			// Check if there's no currently selected GameObject and refocus on the first available choice
			if (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.activeInHierarchy) {
				EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
			}
			yield return null; // Wait until the next frame
		}
	}

	private IEnumerator SelectFirstChoice() {
		// Event System requires we clear it first, then wait
		EventSystem.current.SetSelectedGameObject(null);
		yield return new WaitForEndOfFrame();
		EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
	}

	public void MakeChoice(int choiceIndex) {
		currentStory.ChooseChoiceIndex(choiceIndex);
	}
}
