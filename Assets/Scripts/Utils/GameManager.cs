using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool CanMoveCamera { get; private set; } = true;

    [SerializeField] private Timer timer;

    [Header("Minijuego")]
    [SerializeField] float initiateTime;

    public delegate void DelegatedGameStates();
    public DelegatedGameStates InitialGameStart;
    public DelegatedGameStates InitialGameEnd;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        GamePrepare();
    }

    private void GamePrepare()
    {
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

    public Timer GetTimer() => timer;

    public void MovingCamera(bool value)
    {
        CanMoveCamera = value;
    }
}
