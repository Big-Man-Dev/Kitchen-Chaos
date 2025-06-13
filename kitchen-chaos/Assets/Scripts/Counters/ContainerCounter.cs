using System;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabObject;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public override void Interact(Player player) {
        if (HasKitchenObject() == false) {
            if (player.HasKitchenObject() == false) {
                KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
                InteractLogicServerRpc();         
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }
    [ClientRpc]
    private void InteractLogicClientRpc() {
        OnPlayerGrabObject?.Invoke(this, EventArgs.Empty);
    }
}
