using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
    private PlayerControls controls;
    public static event Action OnInteractPressed;
    public static event Action OnInteractStarted;
    public static event Action OnInteractCanceled;

    public static event Action<int> OnSlotKeyPressed;

    public static event Action<float> OnScrollDialogueChoices;
    public static event Action OnSkipDialoguePressed;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.UI.Enable();
        controls.UI.Pause.performed += OnPauseInput;
        controls.UI.ShowCursor.started += ctx => ShowCursor(true);
        controls.UI.ShowCursor.canceled += ctx => ShowCursor(false);
        controls.UI.AdvanceDialogue.performed += OnAdvanceDialogueInput;
        controls.UI.SkipDialogue.performed += OnSkipDialogueInput;
        controls.UI.OnScrollDialogueChoices.performed += OnScrollDialogueChoicesInput;

        controls.Gameplay.Enable();
        controls.Gameplay.Interact.performed += OnInteractInput;
        controls.Gameplay.Interact.started += ctx => OnInteractStarted?.Invoke();
        controls.Gameplay.Interact.canceled += ctx => OnInteractCanceled?.Invoke();
       
    }

    private void OnDisable()
    {
        controls.UI.Pause.performed -= OnPauseInput;
        controls.UI.AdvanceDialogue.performed -= OnAdvanceDialogueInput;
        controls.UI.SkipDialogue.performed -= OnSkipDialogueInput;
        controls.UI.OnScrollDialogueChoices.performed -= OnScrollDialogueChoicesInput;
        controls.UI.Disable();

        controls.Gameplay.Interact.performed -= OnInteractInput;
        controls.Gameplay.Disable();
    }

    private void OnScrollDialogueChoicesInput(InputAction.CallbackContext ctx)
    {
        if (DialogueSystem.instance != null && DialogueSystem.instance.GetCurrentDialogueState() == DialogueState.ChoicePresenting)
        {
            float scroll = ctx.ReadValue<Vector2>().y;
            OnScrollDialogueChoices?.Invoke(scroll);
        }
    }

    private void OnAdvanceDialogueInput(InputAction.CallbackContext ctx)
    {
        if (UIManager.instance.pausePanel.activeSelf) return;
        DialogueSystem.instance?.TryAdvance();
    }

    private void OnSkipDialogueInput(InputAction.CallbackContext ctx)
    {
        if (UIManager.instance.pausePanel.activeSelf) return;

        if (DialogueSystem.instance.GetCurrentDialogueState() == DialogueState.DialogueTyping ||
             DialogueSystem.instance.GetCurrentDialogueState() == DialogueState.ChoiceFeedbackTyping ||
             DialogueSystem.instance.GetCurrentDialogueState() == DialogueState.DialogueLineFinished)
        {
            OnSkipDialoguePressed?.Invoke();

        }
    }

    private void OnPauseInput(InputAction.CallbackContext ctx)
    {
        Debug.Log("Pause Input Detected");

        bool isDialogueActive = DialogueSystem.instance != null && DialogueSystem.instance.dialoguePanel.activeSelf;
        bool isHakingActive = HackingMiniGame.Active != null && HackingMiniGame.Active.isActive;
        bool isTutorialActive = TutorialController.instance != null && TutorialController.instance.TutorialPanel.activeSelf;

        if (isHakingActive) return;

        if (isTutorialActive)
        {
            TutorialController.instance.HideTutorial();
            Debug.Log("Tutorial Panel Active, Hiding Tutorial");
            return;
        }

        if (DialogueSystem.instance.dialoguePanel.activeSelf)
        {
            OnSkipDialoguePressed?.Invoke();
            Debug.Log("Dialogue Panel Active, Skipping Dialogue");
            return;
        }

        if (UIManager.instance != null)
        {
            GameManager.instance?.GamePause();
            return;
        }
    }

    public void ShowCursor(bool show)
    {
        if (GameManager.instance == null || UIManager.instance == null) return;

        if (show)
        {
            GameManager.instance.GameEnd();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            GameManager.instance.GameStart();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnInteractInput(InputAction.CallbackContext ctx)
    {
        OnInteractPressed?.Invoke();
    }
}
