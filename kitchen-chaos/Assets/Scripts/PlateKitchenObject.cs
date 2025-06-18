using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs {
        public KitchenObjectSO kitchenObjectSO;
    }

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList = new();
    [SerializeField] private List<KitchenObjectSO> kitchenObjectSOList = new();
    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
        if (kitchenObjectSOList.Contains(kitchenObjectSO)) return false;
        if (validKitchenObjectSOList.Contains(kitchenObjectSO) == false) return false;
        AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
        return true;
    }
    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int index) {
        AddIngredientClientRpc(index);
    }
    [ClientRpc]
    private void AddIngredientClientRpc(int index) {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectFromIndex(index);
        kitchenObjectSOList.Add(kitchenObjectSO);
        OnIngredientAdded?.Invoke(this, new() { kitchenObjectSO = kitchenObjectSO });
    }
    public List<KitchenObjectSO> GetKitchenObjectSOList() => kitchenObjectSOList;
}
