using System;
using Unity.Netcode;
using UnityEngine;

public class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{
    [SerializeField] private Transform counterTopPoint;
    private KitchenObject kitchenObject;
    public static event EventHandler OnAnyObjectPlacedHere;

    public static void ResetStaticData() {
        OnAnyObjectPlacedHere = null;
    }
    public virtual void Interact(Player player) {
        Debug.LogError("BaseCounter.Interact();");
    }
    public virtual void InteractAlternate(Player player) { 
        Debug.LogError("BaseCounter.InteractAlternate();");
    }
    public Transform GetKitchenObjectFollowTransform() => counterTopPoint;
    public KitchenObject GetKitchenObject() => kitchenObject;
    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;

        if(kitchenObject != null) {
            OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
        }
    }
    public void ClearKitchenObject() => kitchenObject = null;
    public bool HasKitchenObject() => kitchenObject != null;
    public NetworkObject GetNetworkObject() {
        return NetworkObject;
    }
}
