using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour {

	#region Variables
	[Header("Params")]
	[SerializeField] private float typingSpeed = 0.05f;
	[Header("Dialogue UI")]
	[SerializeField] private GameObject dialoguePanel;
	[SerializeField] private GameObject continueIcon;
	[SerializeField] private TextMeshProUGUI dialogueText;
	[SerializeField] private TextMeshProUGUI displayNameText;
	[SerializeField] private Animator portraitAnimator;
	[SerializeField] private Animator layoutAnimator;
	[Header("Choices UI")]
	[SerializeField] private GameObject[] choices;
	private TextMeshProUGUI[] choicesText;

	private Story currentStory;

	public bool dialogueIsPlaying { get; private set; }
	public bool canContinueToNextLine = false;
	bool canSkip = false;
	bool submitSkip = false;

	private Coroutine displayLineCoroutine;
	#endregion

	#region Default Instance
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
	#endregion

	#region Story Tags
	private const string SPEAKER_TAG = "speaker";
	private const string PORTRAIT_TAG = "portrait";
	private const string LAYOUT_TAG = "layout";
	#endregion

	
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
		// if dialogue is not playing don't proccess further
		if (!dialogueIsPlaying) {
			return;
		}
		if (canSkip && Input.GetKeyDown(KeyCode.Return)) {
			submitSkip = true;
		}

		// if dialogue is active, can move on the next line and nothing to answer on ENTER key Continue story,
		if (canContinueToNextLine && currentStory.currentChoices.Count == 0 && Input.GetKeyDown(KeyCode.Return)) {
			ContinueStory();
		}
	}

	private IEnumerator CanSkip() {
		canSkip = false; //Making sure the variable is false.
		yield return new WaitForSeconds(0.1f); ;
		canSkip = true;
	}


	#region Dialogue Mode
	/// <summary>
	/// Calls ContinueStory and starts the dialogue mode gets the ink script from singletone
	/// </summary>
	/// <param name="inkJSON"></param>
	public void EnterDialogueMode(TextAsset inkJSON) {
		currentStory = new Story(inkJSON.text);
		dialogueIsPlaying = true;
		dialoguePanel.SetActive(true);

		ContinueStory();
	}

	public void ExitDialogueMode() {
		dialogueIsPlaying = false;
		dialoguePanel.SetActive(false);
		dialogueText.text = string.Empty;
	}
	#endregion
	
	#region ContinueStory (Directs to DisplayLine)
	/// <summary>
	/// Continues the script if more content is available also Handles TAGS
	/// </summary>
	private void ContinueStory() {
		if (currentStory.canContinue) {
			// Set text
			if (displayLineCoroutine != null) {
				StopCoroutine(displayLineCoroutine);
			}
			displayLineCoroutine = StartCoroutine(DisplayLine(currentStory.Continue()));
			
			//Handle Tags
			HandleTags(currentStory.currentTags);
		}
		else {
			ExitDialogueMode();
		}
	}
	#endregion

	#region Display the Story with typing effect
	/// <summary>
	///  ContinueStory passes the string through the InkJSON. With using foreach it slowly types out the string.
	///  If the User Skips the foreach, it breaks out from the loop and Display the Choices
	/// </summary>
	/// <param name="line"></param>
	/// <returns></returns>
	private IEnumerator DisplayLine(string line) {
		dialogueText.text = string.Empty;


		continueIcon.SetActive(false);
		HideChoices();

		submitSkip = false;
		canContinueToNextLine = false;

		StartCoroutine(CanSkip());

		foreach (char letter in line.ToCharArray()) {
			if (canSkip && submitSkip && Input.GetKeyDown(KeyCode.Return)) {
				submitSkip = false;
				dialogueText.text = line;
				break;
			}
			dialogueText.text += letter;
			yield return new WaitForSeconds(typingSpeed);
		}

		continueIcon.SetActive(true);

		// display choices
		DisplayChoices();

		canContinueToNextLine = true;
		canSkip = false;
	}

	#endregion

	#region Tag settings
	/// <summary>
	/// Handle tags in the ink script format : '#key:value'
	/// </summary>
	/// <param name="currentTags"></param>
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
	#endregion
	
	#region Choices
	/// <summary>
	/// Displays the choices while checking the Ink Script, Once done,
	/// it inactivates the remaining choices.
	/// </summary>
	private void DisplayChoices() {
		List<Choice> currentChoices = currentStory.currentChoices;

		if (currentChoices.Count > choices.Length) {
			Debug.Log("More choices were given than the UI can support. Number of choices given: " + currentChoices.Count);
		}

		// Display the choices according to Ink Script
		int index = 0;
        foreach (Choice choice in currentChoices)
        {
			choices[index].gameObject.SetActive(true);
			choicesText[index].text = choice.text;
			index++;
		}

		// Disable the choices if there is more than need
		for (int i = index; i < choices.Length; i++) {
			choices[i].gameObject.SetActive(false);
		}

		StartCoroutine(EnsureFocusOnChoices());

		StartCoroutine(SelectFirstChoice());
    }

	/// <summary>
	/// Inactivates the choices
	/// </summary>
	private void HideChoices() {
		foreach (var choiceButton in choices) {
			choiceButton.SetActive(false);
		}
	}

	/// <summary>
	/// Re-focus on out of focus 
	/// </summary>
	/// <returns></returns>
	private IEnumerator EnsureFocusOnChoices() {
		while (dialogueIsPlaying) {
			// Check if there's no currently selected GameObject and refocus on the first available choice
			if (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.activeInHierarchy) {
				EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
			}
			yield return null; // Wait until the next frame
		}
	}

	/// <summary>
	/// On load select a choice by default
	/// </summary>
	/// <returns></returns>
	private IEnumerator SelectFirstChoice() {
		// Event System requires we clear it first, then wait
		EventSystem.current.SetSelectedGameObject(null);
		yield return new WaitForEndOfFrame();
		EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
	}

	/// <summary>
	/// On Click Method for choices
	/// </summary>
	/// <param name="choiceIndex"></param>
	public void MakeChoice(int choiceIndex) {
		if (canContinueToNextLine) {
			currentStory.ChooseChoiceIndex(choiceIndex);
			//Input.GetKeyDown(KeyCode.Return);
			ContinueStory();
		}
		
	}
	#endregion
}
