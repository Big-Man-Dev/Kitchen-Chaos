using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private State state = State.WaitingToStart;
    private float waitingToStartTimer = 1f;
    private float countdownToStart = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 10f;

    public event EventHandler OnStateChange;
    private void Awake() {
        Instance = this;
    }
    private void Update() {
        switch (state) {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if(waitingToStartTimer <= 0) {
                    state = State.CountdownToStart;
                    OnStateChange?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.CountdownToStart:
                countdownToStart -= Time.deltaTime;
                if(countdownToStart <= 0) {
                    state = State.GamePlaying;
                    gamePlayingTimer = gamePlayingTimerMax;
                    OnStateChange?.Invoke(this, EventArgs.Empty);

                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if(gamePlayingTimer <= 0) {
                    state = State.GameOver;
                    OnStateChange?.Invoke(this, EventArgs.Empty);

                }
                break;
            case State.GameOver:
                break;
        }
        Debug.Log(state);
    }
    public bool isGamePlaying() => state == State.GamePlaying;
    public bool IsCountdownToStartActive() => state == State.CountdownToStart;
    public bool IsGameOver() => state == State.GameOver;
    public float GetCountDownToStartTimer() => countdownToStart;
    public float GetGamePlayingTimerNormalized() => 1 - (gamePlayingTimer / gamePlayingTimerMax);

    private enum State {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }
}
