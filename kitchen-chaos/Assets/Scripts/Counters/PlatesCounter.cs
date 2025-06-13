using System;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;
    [SerializeField] private float spawnPlateTimer;
    [SerializeField] private float spawnPlateTimerMax = 4f;
    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;
    private void Update() {
        if (IsServer == false) return;
        if (GameManager.Instance.isGamePlaying() == false) return;
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax) {
            spawnPlateTimer = 0f;
            if (platesSpawnedAmount < platesSpawnedAmountMax) {
                SpawnPlateServerRpc();
            } 
        }
    }
    public override void Interact(Player player) {
        if (player.HasKitchenObject() == false) {
            if (platesSpawnedAmount > 0) {
                InteractLogicServerRpc();
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
            }
        }else {

        }
    }
    [ServerRpc]
    private void SpawnPlateServerRpc() {
        SpawnPlateClientRpc();
    }
    [ClientRpc]
    private void SpawnPlateClientRpc() {
        platesSpawnedAmount++;
        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }
    [ClientRpc]
    private void InteractLogicClientRpc() {
        platesSpawnedAmount--;
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
    public override void InteractAlternate(Player player) {
        base.InteractAlternate(player);
    }
}
