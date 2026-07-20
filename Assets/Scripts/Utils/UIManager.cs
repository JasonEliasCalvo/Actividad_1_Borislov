using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

    [Header("UI Elements")]
    public GameObject pausePanel;
    public GameObject interactionPanel;

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
        }
        else
        {
            Debug.Log("Ocultando cursor");
            GameManager.instance.GameStart();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // -------------------------
    // Helpers
    // -------------------------

    public bool IsPanelActive()
    {
        bool isActive =
          (pausePanel != null && pausePanel.activeSelf);

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

    public void ShowPanelOn(GameObject panel) => ShowPanel(panel, true);
    public void ShowPanelOff(GameObject panel) => ShowPanel(panel, false);
    public void ShowAfterDelay(GameObject panel) => ShowPanelAfterDelay(panel);
}
