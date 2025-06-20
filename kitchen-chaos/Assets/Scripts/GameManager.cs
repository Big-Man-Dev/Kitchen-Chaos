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
    private bool isLocalGamePaused = false;
    private bool isLocalPlayerReady = false;
    private NetworkVariable<bool> isGamePaused = new(false);

    public event EventHandler OnStateChange;
    public event EventHandler OnLocalGamePause;
    public event EventHandler OnLocalGameUnpause;
    public event EventHandler OnLocalPlayerReadyChanged;
    public event EventHandler OnMultiplayerGamePause;
    public event EventHandler OnMultiplayerGameUnpause;

    private Dictionary<ulong, bool> playerReadyDict = new();
    private Dictionary<ulong, bool> playerPausedDict = new();

    [SerializeField] private GameObject playerPrefab;

    private bool autoTestGamePauseState = false;
    private void Awake() {
        Instance = this;
    }
    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    public override void OnNetworkSpawn() {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;
        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }

    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            GameObject playerObject = Instantiate(playerPrefab);
            playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        Debug.Log("trigger");
        autoTestGamePauseState = true;
    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue) {
        if (isGamePaused.Value) {
            Time.timeScale = 0f;
            OnMultiplayerGamePause?.Invoke(this, EventArgs.Empty);
        }
        else {
            Time.timeScale = 1f;
            OnMultiplayerGameUnpause?.Invoke(this, EventArgs.Empty);
        }
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
    private void LateUpdate() {
        if (IsServer == false) return;

        if (autoTestGamePauseState) {
            autoTestGamePauseState = false;
            TestGamePauseState();
        }
    }
    public bool isGamePlaying() => state.Value == State.GamePlaying;
    public bool IsCountdownToStartActive() => state.Value == State.CountdownToStart;
    public bool IsGameOver() => state.Value == State.GameOver;
    public bool IsLocalPlayerReady() => isLocalPlayerReady;
    public float GetCountDownToStartTimer() => countdownToStart.Value;
    public float GetGamePlayingTimerNormalized() => 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    public bool IsWaitingToStart() => state.Value == State.WaitingToStart;

    public void TogglePauseGame() {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused) {
            PauseGameServerRpc();
            OnLocalGamePause?.Invoke(this, EventArgs.Empty);
        }else {
            UnPauseGameServerRpc();
            OnLocalGameUnpause?.Invoke(this, EventArgs.Empty);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
        playerPausedDict[serverRpcParams.Receive.SenderClientId] = true;
        TestGamePauseState();
    }
    [ServerRpc(RequireOwnership = false)]
    private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
        playerPausedDict[serverRpcParams.Receive.SenderClientId] = false;
        TestGamePauseState();
    }
    private void TestGamePauseState() {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if(playerPausedDict.ContainsKey(clientId) && playerPausedDict[clientId]) {
                isGamePaused.Value = true;
                return;
            }
        }
        isGamePaused.Value = false;
    }


    private enum State {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }
}
