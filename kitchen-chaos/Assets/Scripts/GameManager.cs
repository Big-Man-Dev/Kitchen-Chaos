using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    private NetworkVariable<State> state = new(State.WaitingToStart);
    private NetworkVariable<float> countdownToStart = new(3f);
    private NetworkVariable<float> gamePlayingTimer = new(0f);
    private float gamePlayingTimerMax = 300f;
    private bool isGamePaused = false;
    private bool isLocalPlayerReady = false;

    public event EventHandler OnStateChange;
    public event EventHandler OnGamePause;
    public event EventHandler OnGameUnpause;
    public event EventHandler OnLocalPlayerReadyChanged;

    private Dictionary<ulong, bool> playerReadyDict = new();
    private void Awake() {
        Instance = this;
    }
    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    public override void OnNetworkSpawn() {
        state.OnValueChanged += State_OnValueChanged;
    }

    private void State_OnValueChanged(State previousValue, State newValue) {
        OnStateChange?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        if(state.Value == State.WaitingToStart) {
            isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        playerReadyDict[serverRpcParams.Receive.SenderClientId] = true;
        bool allClientsReady = true;
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if(playerReadyDict.ContainsKey(clientId) == false || playerReadyDict[clientId] == false) {
                allClientsReady = false;
                break;
            }
        }
        if (allClientsReady) {
            state.Value = State.CountdownToStart;
        }
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        TogglePauseGame();
    }

    private void Update() {
        if (IsServer == false) return;

        switch (state.Value) {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStart.Value -= Time.deltaTime;
                if(countdownToStart.Value <= 0) {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if(gamePlayingTimer.Value <= 0) {
                    state.Value = State.GameOver;
                }
                break;
            case State.GameOver:
                break;
        }
    }
    public bool isGamePlaying() => state.Value == State.GamePlaying;
    public bool IsCountdownToStartActive() => state.Value == State.CountdownToStart;
    public bool IsGameOver() => state.Value == State.GameOver;
    public bool IsLocalPlayerReady() => isLocalPlayerReady;
    public float GetCountDownToStartTimer() => countdownToStart.Value;
    public float GetGamePlayingTimerNormalized() => 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);

    public void TogglePauseGame() {
        isGamePaused = !isGamePaused;
        if (isGamePaused) {
            Time.timeScale = 0f;
            OnGamePause?.Invoke(this, EventArgs.Empty);
        }else {
            Time.timeScale = 1f;
            OnGameUnpause?.Invoke(this, EventArgs.Empty);
        }
    }

    private enum State {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }
}
