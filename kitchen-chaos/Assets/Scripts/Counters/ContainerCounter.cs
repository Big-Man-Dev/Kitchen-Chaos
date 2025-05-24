using System;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabObject;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public override void Interact(Player player) {
        if (HasKitchenObject() == false) {
            if (player.HasKitchenObject() == false) {
                KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
                OnPlayerGrabObject?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
