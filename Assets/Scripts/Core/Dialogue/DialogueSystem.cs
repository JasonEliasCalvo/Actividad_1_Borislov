using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public enum DialogueState
{
    None,
    DialogueTyping,
    DialogueLineFinished,
    ChoicePresenting,
    ChoiceFeedbackTyping,
    ChoiceFeedbackFinished,
    DialogueEnding,
    ChoiceEnding
}

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;

    [SerializeField] private DialogueDatabase dialoguesDatabase;

    [SerializeField] private List<DialogueEventEntry> dialogueEvents = new();

    private DialogueData currentDialogue;

    private int currentLine;
    public float typingSpeed = 0.05f;
    public float wordFadeDuration = 0.2f;

    public DialogueState currentDialogueState = DialogueState.None;

    public GameObject choicesPanel;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI questionText;
    public GameObject continueIcon;
    public GameObject choiceButtonPrefab;
    public Image portraitImage;
    public Transform choiceContainer;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        GameInputManager.OnSkipDialoguePressed += HandleSkipDialogue;
    }

    private void OnDisable()
    {
        GameInputManager.OnSkipDialoguePressed -= HandleSkipDialogue;
    }

    public DialogueState GetCurrentDialogueState()
    {
        return currentDialogueState;
    }

    public void StartDialogue(int indexDialogueData)
    {
        if (currentDialogueState != DialogueState.None)
            return;

        currentDialogue = dialoguesDatabase.dialogues.Find(x =>
            x != null &&
            x.DialogueID == indexDialogueData);

        if (currentDialogue == null)
        {
            Debug.LogError($"No existe un diálogo con ID {indexDialogueData}");
            return;
        }

        currentLine = 0;

        ShowDialoguePanel(true);

        GameManager.instance.MovingCamera(false);
        GameManager.instance.InitialGameEnd();

        currentDialogueState = DialogueState.DialogueTyping;
        StartCoroutine(ShowDialogueLine());
    }

    public void TryAdvance()
    {
        switch (currentDialogueState)
        {
            case DialogueState.DialogueTyping:
                StopAllCoroutines();
                dialogueText.text = currentDialogue.dialogueLines[currentLine].text;
                portraitImage.sprite = currentDialogue.dialogueLines[currentLine].portrait;
                ShowContinueIcon(true);
                currentDialogueState = DialogueState.DialogueLineFinished;
                break;

            case DialogueState.DialogueLineFinished:
                ShowContinueIcon(false);
                NextDialogueLine();
                break;

            default:
                break;
        }
    }

    IEnumerator ShowDialogueLine()
    {
        currentDialogueState = DialogueState.DialogueTyping;
        dialogueText.text = string.Empty;
        ShowContinueIcon(false);

        string line = currentDialogue.dialogueLines[currentLine].text;
        portraitImage.sprite = currentDialogue.dialogueLines[currentLine].portrait;

        foreach (char ch in line)
        {
            if (currentDialogueState != DialogueState.DialogueTyping)
            {
                dialogueText.text = line;
                break;
            }
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (currentDialogueState == DialogueState.DialogueTyping)
        {
            currentDialogueState = DialogueState.DialogueLineFinished;
            ShowContinueIcon(true);
        }
    }

    private void NextDialogueLine()
    {
        currentLine++;
        ShowContinueIcon(false);

        if (currentLine < currentDialogue.dialogueLines.Count)
        {
            StartCoroutine(ShowDialogueLine());
        }
        else
        {
            ShowContinueIcon(false);
            currentDialogueState = DialogueState.DialogueEnding;
            EndDialogue();
        }
    }

    private void HandleSkipDialogue()
    {
        switch (currentDialogueState)
        {
            case DialogueState.DialogueTyping:
            case DialogueState.DialogueLineFinished:
            case DialogueState.ChoiceFeedbackTyping:
                EndDialogue();
                break;
        }
    }

    public void EndDialogue()
    {
        currentDialogueState = DialogueState.DialogueEnding;
        StopAllCoroutines();
        currentLine = 0;
        dialogueText.text = string.Empty;

        ShowDialoguePanel(false);
        GameManager.instance.InitialGameStart();
        GameManager.instance.MovingCamera(true);
        ShowContinueIcon(false);
        currentDialogueState = DialogueState.None;

        DialogueEventEntry entry =
            dialogueEvents.Find(x => x.id == currentDialogue.DialogueID);

        if (entry != null)
            entry.onDialogueFinished?.Invoke();

        currentDialogue = null;
    }

    public void ShowDialoguePanel(bool state)
    {
        dialoguePanel.SetActive(state);
    }

    public void ShowContinueIcon(bool state)
    {
        continueIcon.SetActive(state);
    }
}