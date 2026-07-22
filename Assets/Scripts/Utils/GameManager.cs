using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool CanMoveCamera { get; private set; } = true;

    public delegate void DelegatedGameStates();
    public DelegatedGameStates InitialGameStart;
    public DelegatedGameStates InitialGameEnd;

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvasGroup;
    public Image fadeImage;
    public float fadeDuration = 1f;
    public Color fadeColor = Color.black;
    [SerializeField] private float timer = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        GamePrepate();
    }

    private void GamePrepate()
    {
        if (fadeCanvasGroup != null)
        {
            fadeImage.color = fadeColor;
            fadeCanvasGroup.alpha = 1f;
            StartFadeIn(() => {
            });
        }

        Invoke(nameof(GameStart), 0.2f);
    }

    public void GameStart() => InitialGameStart?.Invoke();

    public void GamePause()
    {
        if (Time.timeScale == 1f)
        {
            UIManager.instance.ShowPausePanel(true);
            Time.timeScale = 0f;
        }
        else
        {
            UIManager.instance.ShowPausePanel(false);
            Time.timeScale = 1f;
        }
    }

    public void GameResume() { }

    public void GameEnd() => InitialGameEnd?.Invoke();

    public void MovingCamera(bool value)
    {
        CanMoveCamera = value;
    }

    // -------------------------
    // API - Escenas y transiciones
    // -------------------------

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(FadeAndLoad(sceneIndex));
        Time.timeScale = 1;
        UIManager.instance.ShowCursor(true);
    }

    public void StartFadeOut(Action onComplete = null) =>
        StartCoroutine(FadeOutCoroutine(onComplete));

    public void StartFadeIn(Action onComplete = null) =>
        StartCoroutine(FadeInCoroutine(onComplete));

    // -------------------------
    // Helpers privados - Corutinas
    // -------------------------
    private IEnumerator FadeOutCoroutine(Action onComplete)
    {
        if (fadeImage != null)
        {
            fadeImage.color = fadeColor;
        }

        timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1;
        onComplete?.Invoke();
    }

    private IEnumerator FadeInCoroutine(Action onComplete)
    {
        timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0;
        onComplete?.Invoke();
    }

    public IEnumerator FadeAndLoad(int sceneIndex)
    {
        yield return StartCoroutine(FadeOutCoroutine(null));
        SceneManager.LoadScene(sceneIndex);
    }

}
