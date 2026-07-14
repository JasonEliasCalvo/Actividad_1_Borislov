using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

    [Header("UI Elements")]
    public GameObject pausePanel;
    public GameObject confirmPanel;
    public GameObject warningPanel;
    public GameObject securityPanel;
    public GameObject interactionPanel;
    public CanvasGroup tutorialPanel;
    public bool tutorialShown;
    public TextMeshProUGUI confirmText;
    public TextMeshProUGUI warningText;

    [Header("UI Setting")]
    public bool showCursor = false;

    // -------------------------
    // Ciclo de vida
    // -------------------------

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void LateUpdate()
    {
        securityPanel.SetActive(IsPanelActive());
    }

    // -------------------------
    // API
    // -------------------------

    public void ShowPausePanel(bool state)
    {
        if (state)
        {
            ShowPanel(pausePanel, true);
            GameManager.instance.GameEnd();
        }
        else
        {
            ShowPanel(pausePanel, false);
            GameManager.instance.GameStart();
        }
    }

    public void ShowConfirmPanel(bool state, string message = "")
    {
        confirmText.text = message;
        ShowPanel(confirmPanel, state);
    }

    public void ShowWarningPanel(bool state, string message = "")
    {
        warningText.text = message;
        ShowPanel(warningPanel, state);
    }

    public void ShowInteractPanel(bool state)
    {
        interactionPanel.SetActive(state);
    }

    public void ShowCursor(bool show)
    {
        if (show)
        {
            Debug.Log("Mostrando cursor");
            GameManager.instance.GameEnd();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ShowSlected(false);
        }
        else
        {
            Debug.Log("Ocultando cursor");
            GameManager.instance.GameStart();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ShowSlected(true);
        }
    }

    public void ShowSlected(bool state)
    {
        return; // Deshabilitado temporalmente
    }

    public void ShowTutorial()
    {
        if (tutorialShown == true) return;

        tutorialShown = true; // Asegura que solo pase una vez

        // Secuencia de animación
        tutorialPanel.gameObject.SetActive(true);
        tutorialPanel.alpha = 0; // Empezar invisible

        // Fade In
        tutorialPanel.DOFade(1f, 1f).OnComplete(() => {
            // Esperar 4 segundos visible y luego hacer Fade Out
            tutorialPanel.DOFade(0f, 1.5f)
                .SetDelay(4f)
                .OnComplete(() => tutorialPanel.gameObject.SetActive(false));
        });
    }

    // -------------------------
    // Helpers
    // -------------------------

    public bool IsPanelActive()
    {
        bool isActive =
          (pausePanel != null && pausePanel.activeSelf) ||
      
          (confirmPanel != null && confirmPanel.activeSelf) ||
          (warningPanel != null && warningPanel.activeSelf) ;

        return isActive;
    }

    private IEnumerator ShowPanelAfterDelay(GameObject panel)
    {
        yield return new WaitForSeconds(0.2f);
        ShowPanel(panel, true);
    }

    public void ShowPanel(GameObject panel, bool state)
    {
        if (panel == null) return;
        Transform rt = panel.transform;

        if (DOTween.IsTweening(rt))
        {
            DOTween.Kill(rt);
            StartCoroutine(WaitAndAnimate(panel, rt, state));
        }
        else
        {
            StartCoroutine(Animate(panel, rt, state));
        }
    }

    private IEnumerator WaitAndAnimate(GameObject panel, Transform rt, bool state)
    {
        yield return new WaitForSeconds(0.1f);
        yield return Animate(panel, rt, state);
    }

    private IEnumerator Animate(GameObject panel, Transform rt, bool state)
    {
        if (!originalScales.ContainsKey(panel))
            originalScales[panel] = Vector3.one;

        if (state)
        {
            panel.SetActive(true);
            rt.localScale = Vector3.zero;

            yield return null;
            if (IsPanelActive())
                ShowCursor(true);

            rt.DOScale(originalScales[panel], 0.2f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true)
                .OnComplete(() => {

                    if (IsPanelActive() && Cursor.lockState != CursorLockMode.None && !Cursor.visible)
                        ShowCursor(true);
                });
        }
        else
        {
            rt.DOScale(Vector3.zero, 0.2f)
              .SetEase(Ease.InBack)
              .SetUpdate(true)
              .OnComplete(() =>
              {
                  panel.SetActive(false);
                  rt.localScale = Vector3.zero;

                  if (!IsPanelActive())
                      ShowCursor(false);
              });
        }

        yield return null;
    }

    // -------------------------
    // Wrappers para Inspector
    // -------------------------

    public void ShowConfirmPanelOn(string message) => ShowConfirmPanel(true, message);
    public void ShowConfirmPanelOff() => ShowConfirmPanel(false);
    public void ShowWarningPanelOn(string message) => ShowWarningPanel(true, message);
    public void ShowWarningPanelOff() => ShowWarningPanel(false);

    public void ShowPanelOn(GameObject panel) => ShowPanel(panel, true);
    public void ShowPanelOff(GameObject panel) => ShowPanel(panel, false);
    public void ShowAfterDelay(GameObject panel) => ShowPanelAfterDelay(panel);
    public void ShowTutorial(int tutorialID)
    {
        //TutorialInfo info = tutorialDatabase.GetTutorialByID(tutorialID);

        GameManager.instance.MovingCamera(false);
        GameManager.instance.InitialGameEnd();

        // info = null;
    }

    public void HideTutorial()
    {
        GameManager.instance.MovingCamera(true);
        GameManager.instance.InitialGameStart();

    }
}
