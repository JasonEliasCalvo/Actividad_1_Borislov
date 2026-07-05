using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private Timer timer;

    [Header("Minijuego")]
    [SerializeField] float initiateTime;

    public delegate void DelegatedGameStates();
    public DelegatedGameStates eventGameStart;
    public DelegatedGameStates eventGameEnd;
    public DelegatedGameStates eventHackingMiniGameStart;
    public DelegatedGameStates eventHackingMiniGameReset;
    public DelegatedGameStates eventHackingMiniGameEnd;

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

    public void GameStart() => eventGameStart?.Invoke();
    public void GamePause() { }
    public void GameResume() { }

    public void GameEnd() => eventGameEnd?.Invoke();

    public void HackingMiniGameStart()
    {
        Debug.Log("initiateTime: " + initiateTime);
        timer.eventEndTime += ResetHackingMiniGame;
        timer.Initiate(initiateTime);
        eventHackingMiniGameStart?.Invoke();
    }

    public void ResetHackingMiniGame() => eventHackingMiniGameReset?.Invoke();
    public void HackingMiniGameEnd() => eventHackingMiniGameEnd?.Invoke();
    public Timer GetTimer() => timer;
}
