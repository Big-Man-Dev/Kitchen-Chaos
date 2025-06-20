using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;
    [SerializeField] private List<Color> playerColorList = new();
    private const int MAX_PLAYER_AMOUNT = 2; 
    
    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailToJoinGame;
    public event EventHandler OnPlayerDatasChange;
    public static KitchenGameMultiplayer Instance { get; private set; }

    private NetworkList<PlayerData> playerDatas;
    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        playerDatas = new();
        playerDatas.OnListChanged += PlayerDatas_OnListChanged;
    }

    private void PlayerDatas_OnListChanged(NetworkListEvent<PlayerData> changeEvent) {
        OnPlayerDatasChange?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost() {
        NetworkManager.ConnectionApprovalCallback = NetworkManager_ConnectionAprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId) {
        for (int i = 0; i < playerDatas.Count; i++) {
            PlayerData playerData = playerDatas[i];
            if(clientId == playerData.clientId) {
                playerDatas.RemoveAt(i);
                return;
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId) {
        playerDatas.Add(new PlayerData { 
            clientId = clientId,
            colorId = GetFirstUnusedColorId()
        });
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId) {
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Client_OnClientDisconnectCallback;
        OnFailToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    private void NetworkManager_ConnectionAprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        
        if(SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString()) {
            response.Approved = false;
            response.Reason = "Game has already started";
            Debug.Log(response.Reason);
            return;
        }
        
        if(NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT) {
            response.Approved = false;
            response.Reason = "Game is full";
            Debug.Log(response.Reason);
            return;
        }


        if (GameManager.Instance != null && GameManager.Instance.IsWaitingToStart()) {
            response.Approved = true;
            response.CreatePlayerObject = true;
        }else if(GameManager.Instance == null) {
            response.Approved = true;
        }else {
            response.Approved = false;
        }
    }

    public void StartClient() {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.StartClient();
    }
    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkReference) {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectFromIndex(kitchenObjectSOIndex);
        GameObject kitchenObjectGameobject = Instantiate(kitchenObjectSO.prefab);
        kitchenObjectGameobject.transform.position = new Vector3(2, 2, 2);
        NetworkObject kitchenObjectNetworkObject = kitchenObjectGameobject.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);
        KitchenObject kitchenObject = kitchenObjectGameobject.GetComponent<KitchenObject>();
        kitchenObjectParentNetworkReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        kitchenObject.SetKitchenObjectParent(kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>());
    }
    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO) {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }
    public KitchenObjectSO GetKitchenObjectFromIndex(int index) {
        return kitchenObjectListSO.kitchenObjectSOList[index];
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject) {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }
    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference networkObjectReference) {
        networkObjectReference.TryGet(out NetworkObject networkObject);
        KitchenObject kitchenObject = networkObject.GetComponent<KitchenObject>();
        ClearKitchenObjectOnParentClientRpc(networkObjectReference);
        kitchenObject.DestroySelf();
    }
    [ClientRpc]
    private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference networkObjectReference) {
        networkObjectReference.TryGet(out NetworkObject networkObject);
        KitchenObject kitchenObject = networkObject.GetComponent<KitchenObject>();
        kitchenObject.ClearKitchenObjectOnParent();
    }
    public bool IsPlayerIndexConnected(int index) {
        return index < playerDatas.Count;
    }
    public PlayerData GetPlayerDataFromPlayerIndex(int index) {
        return playerDatas[index];
    }
    public PlayerData GetPlayerData() {
        return GetPlayerDataWithClientId(NetworkManager.Singleton.LocalClientId);
    }
    public PlayerData GetPlayerDataWithClientId(ulong clientId) {
        foreach(PlayerData playerData in playerDatas) {
            if (playerData.clientId == clientId)
                return playerData;
        }
        return default;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId) {
        for (int i = 0; i < playerDatas.Count; i++) {
            if (clientId == playerDatas[i].clientId) return i;
        }
        return -1;
    }
    public Color GetPlayerColor(int colorID) {
        return playerColorList[colorID];
    }

    public void ChangePlayerColor(int colorId) {
        ChangePlayerColorServerRpc(colorId);
    }
    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default) {
        if (IsColorAvailable(colorId) == false) {
            return;
        }
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDatas[playerDataIndex];
        playerData.colorId = colorId;
        playerDatas[playerDataIndex] = playerData;
    }
    private bool IsColorAvailable(int colorId) {
        foreach(PlayerData playerData in playerDatas) {
            if (playerData.colorId == colorId) return false;
        }
        return true;
    }
    private int GetFirstUnusedColorId() {
        for (int i = 0; i < playerColorList.Count; i++) {
            if (IsColorAvailable(i)) return i;
        }
        return -1;
    }
    public void KickPlayer(ulong clientId) {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }
}
