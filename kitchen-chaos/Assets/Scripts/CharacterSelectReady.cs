using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }
    private Dictionary<ulong, bool> playerReadyDict = new();

    public event EventHandler OnReadyChange;

    private void Awake() {
        Instance = this;
    }

    public void SetPlayerReady() {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        playerReadyDict[serverRpcParams.Receive.SenderClientId] = true;
        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if(playerReadyDict.ContainsKey(clientId) == false || playerReadyDict[clientId] == false) {
                allClientsReady = false;
                break;
            }
        }
        if (allClientsReady) {
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }
    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId) {
        playerReadyDict[clientId] = true;
        OnReadyChange?.Invoke(this, EventArgs.Empty);
    }
    public bool IsPlayerReady(ulong clientId) {
        return playerReadyDict.ContainsKey(clientId) && playerReadyDict[clientId];
    }
}
