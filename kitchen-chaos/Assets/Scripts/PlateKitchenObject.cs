using System;
using System.Collections.Generic;
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
        kitchenObjectSOList.Add(kitchenObjectSO);
        OnIngredientAdded?.Invoke(this, new() { kitchenObjectSO = kitchenObjectSO });
        return true;
    }
}
