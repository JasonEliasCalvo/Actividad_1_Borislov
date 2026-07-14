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
    [SerializeField] private QuestionDatabase questionDatabase;

    [SerializeField] private List<DialogueEventEntry> dialogueEvents = new();
    [SerializeField] public List<QuestionEventEntry> questionevents = new();

    private DialogueData currentDialogue;
    private QuestionData currentQuestion;
    private UnityEvent currentChoiceEvent;

    private int currentLine;
    public float typingSpeed = 0.05f;
    public float wordFadeDuration = 0.2f;

    public DialogueState currentDialogueState = DialogueState.None;

    private List<GameObject> choiceButtons = new List<GameObject>();
    private int selectedChoiceIndex = 0;

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
        GameInputManager.OnScrollDialogueChoices += ScrollChoice;
        GameInputManager.OnSkipDialoguePressed += HandleSkipDialogue;
        StandartButton.OnChoiceButtonHoverEnter += OnChoiceButtonHovered;
    }

    private void OnDisable()
    {
        GameInputManager.OnScrollDialogueChoices -= ScrollChoice;
        GameInputManager.OnSkipDialoguePressed -= HandleSkipDialogue;
        StandartButton.OnChoiceButtonHoverEnter -= OnChoiceButtonHovered;
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
        ShowChoicesPanel(false);

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

            case DialogueState.ChoicePresenting:
                ConfirmSelectedChoice(selectedChoiceIndex);
                break;

            case DialogueState.ChoiceFeedbackTyping:
                StopAllCoroutines();
                string feedbackToDisplay = (currentChoiceEvent == GetQuestionEvent(currentQuestion.QuestionID)?.onChoiceCorrectEnd) ?
                                                           currentQuestion.correctFeedback :
                                                           currentQuestion.incorrectFeedback;
                dialogueText.text = feedbackToDisplay;
                ShowContinueIcon(true);
                currentDialogueState = DialogueState.ChoiceFeedbackFinished;
                break;

            case DialogueState.ChoiceFeedbackFinished:
                Endchoice(currentChoiceEvent);
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

    public void ShowChoices(int indexDataQuestion)
    {
        if (currentDialogueState != DialogueState.None) return;

        currentQuestion = questionDatabase.questions.Find
            (x => x != null && x.QuestionID == indexDataQuestion);

        dialogueText.text = string.Empty;
        ShowChoicesPanel(true);
        ShowDialoguePanel(false);
        questionText.text = currentQuestion.questionText;

        ClearChoices();

        selectedChoiceIndex = 0;

        for (int i = 0; i < currentQuestion.options.Length; i++)
        {
            GameObject _buttonTemp = Instantiate(choiceButtonPrefab, choiceContainer);
            if (_buttonTemp != null)
            {
                int choiceIndex = i;
                _buttonTemp.GetComponent<StandartButton>().SetAnswer(currentQuestion.options[choiceIndex], choiceIndex);
            }
            choiceButtons.Add(_buttonTemp);
        }

        UpdateSelectedChoiceUI();

        GameManager.instance.InitialGameEnd();
        GameManager.instance.MovingCamera(false);
        currentDialogueState = DialogueState.ChoicePresenting;
    }

    private void ClearChoices()
    {
        foreach (GameObject button in choiceButtons)
        {
            Destroy(button);
        }
        choiceButtons.Clear();
    }

    private void ConfirmSelectedChoice(int choiceIndex)
    {
        if (currentDialogueState == DialogueState.ChoicePresenting)
        {
            QuestionEventEntry qEvent = GetQuestionEvent(currentQuestion.QuestionID);

            if (choiceIndex == currentQuestion.correctAnswerIndex)
            {
                currentChoiceEvent = qEvent?.onChoiceCorrectEnd;
                StartCoroutine(TypeFeedback(currentQuestion.correctFeedback));
            }
            else
            {
                currentChoiceEvent = qEvent?.onChoiceIncorrectEnd;
                StartCoroutine(TypeFeedback(currentQuestion.incorrectFeedback));
            }
        }
    }

    private QuestionEventEntry GetQuestionEvent(int id)
    {
        return questionevents.Find(x => x.id == id);
    }

    public void ScrollChoice(float scroll)
    {
        if (currentDialogueState == DialogueState.ChoicePresenting && choiceButtons.Count > 0)
        {
            int previousSelected = selectedChoiceIndex;

            if (scroll > 0)
            {
                selectedChoiceIndex--;
                if (selectedChoiceIndex < 0)
                {
                    selectedChoiceIndex = choiceButtons.Count - 1;
                }
            }
            else if (scroll < 0)
            {
                selectedChoiceIndex++;
                if (selectedChoiceIndex >= choiceButtons.Count)
                {
                    selectedChoiceIndex = 0;
                }
            }

            if (selectedChoiceIndex != previousSelected)
            {
                UpdateSelectedChoiceUI();
            }
        }
    }

    private void UpdateSelectedChoiceUI()
    {
        for (int i = 0; i < choiceButtons.Count; i++)
        {
            StandartButton choiceBtn = choiceButtons[i].GetComponent<StandartButton>();
            if (choiceBtn != null)
            {
                choiceBtn.SetSelected(i == selectedChoiceIndex);
            }
        }
    }

    private void OnChoiceButtonHovered(int index)
    {
        if (currentDialogueState == DialogueState.ChoicePresenting)
        {
            selectedChoiceIndex = index;
            UpdateSelectedChoiceUI();
        }
    }

    IEnumerator TypeFeedback(string feedbackText)
    {
        ShowDialoguePanel(true);
        ShowChoicesPanel(false);
        currentDialogueState = DialogueState.ChoiceFeedbackTyping;
        dialogueText.text = string.Empty;
        ShowContinueIcon(false);

        foreach (char ch in feedbackText)
        {
            if (currentDialogueState != DialogueState.ChoiceFeedbackTyping)
            {
                dialogueText.text = feedbackText;
                break;
            }
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (currentDialogueState == DialogueState.ChoiceFeedbackTyping)
        {
            currentDialogueState = DialogueState.ChoiceFeedbackFinished;
            ShowContinueIcon(true);
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

    public void Endchoice(UnityEvent currentEvent)
    {
        currentDialogueState = DialogueState.ChoiceEnding;
        StopAllCoroutines();
        currentLine = 0;
        ShowDialoguePanel(false);

        dialogueText.text = string.Empty;
        GameManager.instance.InitialGameStart();
        GameManager.instance.MovingCamera(true);
        currentDialogueState = DialogueState.None;
        currentChoiceEvent?.Invoke();
        currentQuestion = null;
    }

    public void ShowDialoguePanel(bool state)
    {
        dialoguePanel.SetActive(state);
    }

    public void ShowChoicesPanel(bool state)
    {
        choicesPanel.SetActive(state);
    }

    public void ShowContinueIcon(bool state)
    {
        continueIcon.SetActive(state);
    }
}